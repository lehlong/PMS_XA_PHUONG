using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Core.Entities.PS;

namespace Project.Core.Refrences.PS
{

    public class PsProjectWorkflowProcessingConfig : IEntityTypeConfiguration<PsProjectWorkflowProcessing>
    {
        public void Configure(EntityTypeBuilder<PsProjectWorkflowProcessing> builder)
        {
            builder.HasOne(x => x.Person)
                   .WithMany()
                   .HasForeignKey(x => x.UserName)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.NoAction);

        }
    }

}
