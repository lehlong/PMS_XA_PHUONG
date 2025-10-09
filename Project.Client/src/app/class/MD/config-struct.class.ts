import { BaseFilter } from "../common/base-filter.class";

export class ConfigStructDto extends BaseFilter {
    id: string = '';
    code: string = '';
    name: string = '';
    pId: string = '';
    orderNumber: number = 0;
    expanded : boolean = true;
    orgId : string = '';
    type : string = '';
}