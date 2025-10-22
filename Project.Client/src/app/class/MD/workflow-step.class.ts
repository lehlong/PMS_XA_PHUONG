import { BaseFilter } from "../common/base-filter.class";

export class WorkflowStepDto extends BaseFilter {
    id: string = '';
    workflowId: string = '';
    step : number = 0;
    name: string = '';
    hanXuLy : number = 0;
    action : string = '';
}