export class WorkflowProjectAction {
    static readonly TrinhDuyet = 0;
    static readonly XacNhan = 1;
    static readonly PheDuyet = 2;
    static readonly TuChoi = 3;
    static readonly YeuCauChinhSua = 4;

    static getText(i: number): string {
        switch (i) {
            case WorkflowProjectAction.TrinhDuyet:
                return 'Trình duyệt';
            case WorkflowProjectAction.XacNhan:
                return 'Xác nhận';
            case WorkflowProjectAction.PheDuyet:
                return 'Phê duyệt';
            case WorkflowProjectAction.TuChoi:
                return 'Từ chối';
            case WorkflowProjectAction.YeuCauChinhSua:
                return 'Yêu cầu chỉnh sửa';
            default:
                return '';
        }
    }

    static getList(): Array<{ value: number; text: string }> {
        return [
            { value: this.TrinhDuyet, text: this.getText(this.TrinhDuyet) },
            { value: this.XacNhan, text: this.getText(this.XacNhan) },
            { value: this.PheDuyet, text: this.getText(this.PheDuyet) },
            { value: this.TuChoi, text: this.getText(this.TuChoi) },
            { value: this.YeuCauChinhSua, text: this.getText(this.YeuCauChinhSua) }
        ];
    }
}