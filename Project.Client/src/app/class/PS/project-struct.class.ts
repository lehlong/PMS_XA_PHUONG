import { BaseFilter } from "../common/base-filter.class";

export class ProjectStructDto extends BaseFilter {
    id: string = '';
    projectId: string = '';
    code: string = '';
    name: string = '';
    pId: string = '';
    orderNumber: number = 0;
    expanded: boolean = true;
    orgId: string = '';
    type: number = 0;
    startDate: any;
    endDate: any;
}