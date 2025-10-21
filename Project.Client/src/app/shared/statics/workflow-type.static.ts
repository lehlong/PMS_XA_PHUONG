export class WorkflowType {
    static readonly Project = 0;
    static readonly Task = 1;

    static getText(i: number): string {
        switch (i) {
            case WorkflowType.Project:
                return 'Workflow dự án';
            case WorkflowType.Task:
                return 'Workflow công việc';
            default:
                return '';
        }
    }

    static getList(): Array<{ value: number; text: string }> {
        return [
            { value: this.Project, text: this.getText(this.Project) },
            { value: this.Task, text: this.getText(this.Task) }
        ];
    }
}