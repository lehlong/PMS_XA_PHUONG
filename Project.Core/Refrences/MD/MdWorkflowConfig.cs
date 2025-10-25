using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Core.Entities.MD;

namespace Project.Core.Refrences.MD
{
    public class MdWorkflowConfig : IEntityTypeConfiguration<MdWorkflow>
    {
        public void Configure(EntityTypeBuilder<MdWorkflow> builder)
        {
            builder.HasMany(p => p.Steps)
                   .WithOne(f => f.Workflow)
                   .HasForeignKey(f => f.WorkflowId)
                   .HasPrincipalKey(p => p.Id)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
