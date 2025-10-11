import { BaseFilter } from "../common/base-filter.class";

export class TitleDto extends BaseFilter {
    code: string = '';
    name: string = '';
    notes : string = '';
}