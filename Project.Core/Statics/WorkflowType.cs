namespace Project.Core.Statics
{
    public static class WorkflowType
    {
        public const int Project = 0;
        public const int Task = 1;

        public static string GetText(int i) => i switch
        {
            Project => "Workflow dự án",
            Task => "Workflow công việc",
            _ => string.Empty
        };
    }
}
