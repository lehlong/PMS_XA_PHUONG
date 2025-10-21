namespace Project.Core.Statics
{
    public static class ProjectStructType
    {
        public const int Project = 0;
        public const int GiaiDoan = 1;
        public const int HangMuc = 2;
        public const int CongViec = 3;

        public static string GetText(int i) => i switch
        {
            Project => "Dự án",
            GiaiDoan => "Giai đoạn",
            HangMuc => "Hạng mục",
            CongViec => "Công việc",
            _ => string.Empty
        };
    }
}
