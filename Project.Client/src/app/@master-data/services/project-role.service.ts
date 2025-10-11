import { Injectable } from '@angular/core';
import { CommonService } from '../../services/common/common.service';
import { ProjectRoleDto } from '../../class/MD/project-role.class';

@Injectable({
    providedIn: 'root',
})
export class ProjectRoleService {
    constructor(private common: CommonService) { }

    search(params: ProjectRoleDto) { return this.common.get('ProjectRole/Search', params, false) }

    getAll() { return this.common.get('ProjectRole/GetAll', {}, false) }

    getAllActive() { return this.common.get('ProjectRole/GetAllActive', {}, false) }

    insert(data: ProjectRoleDto) { return this.common.post('ProjectRole/Insert', data, false) }

    update(data: ProjectRoleDto) { return this.common.put('ProjectRole/Update', data, false) }

    detail(data: string) { return this.common.get(`ProjectRole/Detail/${data}`, {}, false) }

    delete(data: string) { return this.common.delete(`ProjectRole/Delete/${data}`, {}, false) }

    updateOrder(data: any) { return this.common.put(`ProjectRole/UpdateOrder`, data, false) }
}
