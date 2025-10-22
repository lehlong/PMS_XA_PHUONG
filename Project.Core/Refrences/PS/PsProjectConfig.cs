using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Core.Entities.PS;

namespace Project.Core.Refrences.PS
{
    public class PsProjectConfig : IEntityTypeConfiguration<PsProject>
    {
        public void Configure(EntityTypeBuilder<PsProject> builder)
        {
            builder.HasOne(x => x.DonViPhuTrachRef)
                   .WithMany()
                   .HasForeignKey(x => x.DonViPhuTrach)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(p => p.Files)
                   .WithOne(f => f.Project)
                   .HasForeignKey(f => f.RefrenceFileId)
                   .HasPrincipalKey(p => p.RefrenceFileId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(p => p.Structs)
                   .WithOne(s => s.Project)
                   .HasForeignKey(s => s.ProjectId)
                   .HasPrincipalKey(p => p.Id)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
