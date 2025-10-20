using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Core.Entities.PS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Refrences.PS
{
    public class PsProjectStructConfig : IEntityTypeConfiguration<PsProjectStruct>
    {
        public void Configure(EntityTypeBuilder<PsProjectStruct> builder)
        {
            builder.HasMany(p => p.Files)
                   .WithOne(f => f.ProjectStruct)
                   .HasForeignKey(f => f.RefrenceFileId)
                   .HasPrincipalKey(p => p.RefrenceFileId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
