import { BaseFilter } from "../common/base-filter.class";

export class WorkflowDto extends BaseFilter {
    id: string = '';
    code: string = '';
    name: string = '';
    projectLevelCode: number| null = null;
    orgId: string = '';
    type: number = 0;
    notes: string = '';
    steps: any[] = [];
}