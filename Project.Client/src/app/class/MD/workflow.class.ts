import { BaseFilter } from "../common/base-filter.class";

export class WorkflowDto extends BaseFilter {
    id: string = '';
    code: string = '';
    name: string = '';
    projectLevelCode: number = 0;
    orgId: string = '';
    type: number = 0;
    notes: string = '';
    steps: any[] = [];
}