namespace Project.Core.Statics
{
    public static class ProjectStatus
    {
        public const int KhoiTao = 0;
        public const int DaTrinhDuyet = 1;
        public const int DaXacNhan = 2;
        public const int DaPheDuyet = 3;
        public const int TuChoi = 4;

        public static string GetText(int status) => status switch
        {
            KhoiTao => "Khởi tạo",
            DaTrinhDuyet => "Đã trình duyệt",
            DaXacNhan => "Đã xác nhận",
            DaPheDuyet => "Đã phê duyệt",
            TuChoi => "Từ chối",
            _ => string.Empty
        };
    }
}
