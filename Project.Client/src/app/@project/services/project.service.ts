import { Injectable } from '@angular/core';
import { CommonService } from '../../services/common/common.service';
import { ProjectDto } from '../../class/PS/project.class';

@Injectable({
    providedIn: 'root',
})
export class ProjectService {
    constructor(private common: CommonService) { }

    search(params: ProjectDto) { return this.common.get('Project/Search', params, true) }

    getAll() { return this.common.get('Project/GetAll', {}, true) }

    getAllActive() { return this.common.get('Project/GetAllActive', {}, true) }

    insert(data: ProjectDto) { return this.common.post('Project/Insert', data, true) }

    update(data: ProjectDto) { return this.common.put('Project/Update', data, true) }

    detail(data: string) { return this.common.get(`Project/Detail/${data}`, {}, true) }

    getGiaiDoan(data: string) { return this.common.get(`Project/GetGiaiDoan/${data}`, {}, true) }

    delete(data: string) { return this.common.delete(`Project/Delete/${data}`, {}, true) }

    updateOrder(data: any) { return this.common.put(`Project/UpdateOrder`, data, true) }

    trinhDuyet(data: string) { return this.common.put(`Project/TrinhDuyet/${data}`, {}, true) }

    xacNhan(data: string) { return this.common.put(`Project/XacNhan/${data}`, {}, true) }

    pheDuyet(data: string) { return this.common.put(`Project/PheDuyet/${data}`, {}, true) }
    
    tuChoi(data: string) { return this.common.put(`Project/TuChoi/${data}`, {}, true) }

    yeuCauChinhSua(data: string) { return this.common.put(`Project/YeuCauChinhSua/${data}`, {}, true) }

    getCurrentStep(data: string) { return this.common.get(`Project/GetCurrentStep/${data}`, {}, true) }
}
