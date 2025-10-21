export class ProjectStatus {
    static readonly KhoiTao = 0;
    static readonly DaTrinhDuyet = 1;
    static readonly DaXacNhan = 2;
    static readonly DaPheDuyet = 3;
    static readonly TuChoi = 4;

    static getText(i: number): string {
        switch (i) {
            case ProjectStatus.KhoiTao:
                return 'Khởi tạo';
            case ProjectStatus.DaTrinhDuyet:
                return 'Đã trình duyệt';
            case ProjectStatus.DaXacNhan:
                return 'Đã xác nhận';
            case ProjectStatus.DaPheDuyet:
                return 'Đã phê duyệt';
            case ProjectStatus.TuChoi:
                return 'Từ chối';
            default:
                return '';
        }
    }

    static getList(): Array<{ value: number; text: string }> {
        return [
            { value: this.KhoiTao, text: this.getText(this.KhoiTao) },
            { value: this.DaTrinhDuyet, text: this.getText(this.DaTrinhDuyet) },
            { value: this.DaXacNhan, text: this.getText(this.DaXacNhan) },
            { value: this.DaPheDuyet, text: this.getText(this.DaPheDuyet) },
            { value: this.TuChoi, text: this.getText(this.TuChoi) }
        ];
    }
}