import { BaseFilter } from "../common/base-filter.class";

export class CurrencyDto extends BaseFilter {
    code: string = '';
    name: string = '';
    notes : string = '';
}