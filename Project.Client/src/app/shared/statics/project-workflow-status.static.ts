export class ProjectWorkflowStatus {
    static readonly ChuaBatDau = 0;
    static readonly DangXuLy = 1;
    static readonly HoanThanh = 2;
    static readonly KhongHoatDong = 3;

    static getText(i: number): string {
        switch (i) {
            case ProjectWorkflowStatus.ChuaBatDau:
                return 'Chưa bắt đầu';
            case ProjectWorkflowStatus.DangXuLy:
                return 'Đang xử lý';
            case ProjectWorkflowStatus.HoanThanh:
                return 'Hoàn thành';
            case ProjectWorkflowStatus.KhongHoatDong:
                return 'Không hoạt động';
            default:
                return '';
        }
    }

    static getList(): Array<{ value: number; text: string }> {
        return [
            { value: this.ChuaBatDau, text: this.getText(this.ChuaBatDau) },
            { value: this.DangXuLy, text: this.getText(this.DangXuLy) },
            { value: this.HoanThanh, text: this.getText(this.HoanThanh) },
            { value: this.KhongHoatDong, text: this.getText(this.KhongHoatDong) }
        ];
    }
}