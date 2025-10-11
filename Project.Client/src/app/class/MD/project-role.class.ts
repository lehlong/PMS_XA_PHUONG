import { BaseFilter } from "../common/base-filter.class";

export class ProjectRoleDto extends BaseFilter {
    code: string = '';
    name: string = '';
    notes : string = '';
}