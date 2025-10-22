namespace Project.Core.Statics
{
    public static class ProjectWorkflowStatus
    {
        public const int ChuaBatDau = 0;
        public const int DangXuLy = 1;
        public const int HoanThanh = 2;
        public const int KhongHoatDong = 3;

        public static string GetText(int i) => i switch
        {
            ChuaBatDau => "Chưa bắt đầu",
            DangXuLy => "Đang xử lý",
            HoanThanh => "Hoàn thành",
            KhongHoatDong => "Không hoạt động",
            _ => string.Empty
        };
    }
}
