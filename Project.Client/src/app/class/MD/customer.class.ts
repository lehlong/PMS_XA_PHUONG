import { BaseFilter } from "../common/base-filter.class";

export class CustomerDto extends BaseFilter {
    code: string = '';
    name: string = '';
    shortName: string = '';
    maSoThue: string = '';
    phone: string = '';
    email: string = '';
    address: string = '';
    notes: string = '';
}