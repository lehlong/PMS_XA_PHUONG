using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Core.Entities.PS;

namespace Project.Core.Refrences.PS
{
    public class PsProjectConfig : IEntityTypeConfiguration<PsProject>
    {
        public void Configure(EntityTypeBuilder<PsProject> builder)
        {
            // === 1. Quan hệ PsProject → MdOrganize (Đơn vị phụ trách)
            builder.HasOne(x => x.DonViPhuTrachRef)
                   .WithMany()
                   .HasForeignKey(x => x.DonViPhuTrach)
                   .OnDelete(DeleteBehavior.NoAction);

            // === 2. Quan hệ PsProject → CmFile (theo RefrenceFileId)
            builder.HasMany(p => p.Files)
                   .WithOne(f => f.Project)
                   .HasForeignKey(f => f.RefrenceFileId)
                   .HasPrincipalKey(p => p.RefrenceFileId)
                   .OnDelete(DeleteBehavior.NoAction);

            // === 3. Quan hệ PsProject → PsProjectStruct (theo ProjectId)
            builder.HasMany(p => p.Structs)
                   .WithOne(s => s.Project)
                   .HasForeignKey(s => s.ProjectId)
                   .HasPrincipalKey(p => p.Id)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
