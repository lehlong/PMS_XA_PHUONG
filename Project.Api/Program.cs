using Hangfire;
using Microsoft.EntityFrameworkCore;
using Minio;
using NLog;
using NLog.Web;
using Project.Core;
using Project.Service;
using Project.Service.Common;
using Project.Service.Dtos.CM;
using Project.Service.Services.AD;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    logger.Info("Application starting...");

    var frontendFolder = "fe";

    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddDIServices(builder.Configuration);
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    );

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration["Redis:ConnectionString"];
        options.InstanceName = builder.Configuration["Redis:InstanceName"];
    });

    builder.Services.Configure<MinioConfigDto>(builder.Configuration.GetSection("Minio"));

    builder.Services.AddSingleton(sp =>
    {
        var config = builder.Configuration.GetSection("Minio").Get<MinioConfigDto>();

        return new MinioClient()
            .WithEndpoint(config.Endpoint, config.Port)
            .WithCredentials(config.AccessKey, config.SecretKey)
            .WithSSL(config.UseSSL)
            .Build();
    });

    builder.Services.AddHangfire(config =>
    {
        config.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"));
    });
    builder.Services.AddHangfireServer();

    builder.Services.AddControllers();
    builder.Services.AddSpaStaticFiles(options => options.RootPath = frontendFolder);
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddCors(options => options.AddPolicy("CorsPolicy", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .WithExposedHeaders("Accept-Ranges", "Content-Range", "Content-Length", "Content-Disposition");
    }));

    var app = builder.Build();
    ServiceProviderAccessor.Instance = app.Services;

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors("CorsPolicy");
    app.UseHangfireDashboard("/hangfire");
    app.UseHttpsRedirection();
    app.UseAuthorization();

    app.UseStaticFiles();

    app.Use(async (context, next) =>
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";

        if (path.StartsWith("/api/"))
        {
            await next();

            if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\":\"API endpoint not found\",\"path\":\"" + context.Request.Path + "\"}");
                return;
            }
        }
        else
        {
            await next();
        }
    });

    app.MapControllers();

    if (Directory.Exists(Path.Combine(app.Environment.ContentRootPath, frontendFolder)))
    {
        app.UseSpaStaticFiles();
        app.UseSpa(spa =>
        {
            spa.Options.SourcePath = frontendFolder;

            spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    var requestPath = ctx.Context.Request.Path.Value?.ToLower() ?? "";

                    if (requestPath.StartsWith("/api/") ||
                        requestPath.StartsWith("/swagger") ||
                        requestPath.StartsWith("/health") ||
                        requestPath.StartsWith("/hangfire"))
                    {
                        ctx.Context.Response.StatusCode = 404;
                        ctx.Context.Response.ContentLength = 0;
                        ctx.Context.Response.Body = Stream.Null;
                    }
                }
            };
        });
    }
    else
    {
        logger.Warn($"Frontend folder '{frontendFolder}' not found. Running in API-only mode.");
    }

    logger.Info("Application configuration completed successfully");

    using (var scope = app.Services.CreateScope())
    {
        var messageCache = scope.ServiceProvider.GetRequiredService<IMessageCacheService>();
        logger.Info("Starting message cache synchronization...");
        await messageCache.SyncFromDatabaseAsync(CancellationToken.None);
        logger.Info("Message cache synchronized successfully");

        RecurringJob.AddOrUpdate("Sync Message Cache", () => messageCache.SyncFromDatabaseAsync(CancellationToken.None), Cron.MinuteInterval(30));
        logger.Info("Recurring job for message cache sync configured");
    }

    logger.Info("Application started successfully");
    app.Run();
}
catch (Exception ex)
{
    logger.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    logger.Info("Application shutting down...");
    LogManager.Shutdown();
}