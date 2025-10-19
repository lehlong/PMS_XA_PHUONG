using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Core.Entities.PS;

namespace Project.Core.Refrences.PS
{
    public class PsProjectPersonConfig : IEntityTypeConfiguration<PsProjectPerson>
    {
        public void Configure(EntityTypeBuilder<PsProjectPerson> builder)
        {
            builder.HasOne(x => x.Person)
                   .WithMany()
                   .HasForeignKey(x => x.UserName)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
