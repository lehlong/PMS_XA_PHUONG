export class ProjectStructType {
    static readonly Project = 0;
    static readonly GiaiDoan = 1;
    static readonly HangMuc = 2;
    static readonly CongViec = 3;

    static getText(i: number): string {
        switch (i) {
            case ProjectStructType.Project:
                return 'Dự án';
            case ProjectStructType.GiaiDoan:
                return 'Giai đoạn';
            case ProjectStructType.HangMuc:
                return 'Hạng mục';
            case ProjectStructType.CongViec:
                return 'Công việc';
            default:
                return '';
        }
    }

    static getList(): Array<{ value: number; text: string }> {
        return [
            { value: this.Project, text: this.getText(this.Project) },
            { value: this.GiaiDoan, text: this.getText(this.GiaiDoan) },
            { value: this.HangMuc, text: this.getText(this.HangMuc) },
            { value: this.CongViec, text: this.getText(this.CongViec) }
        ];
    }
}