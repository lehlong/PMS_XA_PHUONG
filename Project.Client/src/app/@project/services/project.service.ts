import { Injectable } from '@angular/core';
import { CommonService } from '../../services/common/common.service';
import { ProjectDto } from '../../class/PS/project.class';

@Injectable({
    providedIn: 'root',
})
export class ProjectService {
    constructor(private common: CommonService) { }

    search(params: ProjectDto) { return this.common.get('Project/Search', params, false) }

    getAll() { return this.common.get('Project/GetAll', {}, false) }

    getAllActive() { return this.common.get('Project/GetAllActive', {}, false) }

    insert(data: ProjectDto) { return this.common.post('Project/Insert', data, false) }

    update(data: ProjectDto) { return this.common.put('Project/Update', data, false) }

    detail(data: string) { return this.common.get(`Project/Detail/${data}`, {}, false) }

    delete(data: string) { return this.common.delete(`Project/Delete/${data}`, {}, false) }

    updateOrder(data: any) { return this.common.put(`Project/UpdateOrder`, data, false) }
}
