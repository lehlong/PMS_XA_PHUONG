import { BaseFilter } from "../common/base-filter.class";

export class WorkflowDto extends BaseFilter {
    id: string = '';
    code: string = '';
    name: string = '';
    projectLevelCode: string = '';
    orgId: string = '';
    type: string = '';
    notes: string = '';
}