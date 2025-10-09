import { BaseFilter } from "../common/base-filter.class";

export class LoaiDuAnDto extends BaseFilter {
    code: string = '';
    name: string = '';
    notes : string = '';
}