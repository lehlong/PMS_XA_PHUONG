namespace Project.Core.Statics
{
    public static class WorkflowProjectAction
    {
        public const int TrinhDuyet = 0;
        public const int XacNhan = 1;
        public const int PheDuyet = 2;
        public const int TuChoi = 3;
        public const int YeuCauChinhSua = 4;

        public static string GetText(int i) => i switch
        {
            TrinhDuyet => "Trình duyệt",
            XacNhan => "Xác nhận",
            PheDuyet => "Phê duyệt",
            TuChoi => "Từ chối",
            YeuCauChinhSua => "Yêu cầu chỉnh sửa",
            _ => string.Empty
        };
    }
}
