import { BaseFilter } from "../common/base-filter.class";

export class AreaDto extends BaseFilter {
    code: string = '';
    name: string = '';
    notes : string = '';
}