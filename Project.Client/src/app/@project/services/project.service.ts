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

    getGiaiDoan(data: string) { return this.common.get(`Project/GetGiaiDoan/${data}`, {}, false) }

    delete(data: string) { return this.common.delete(`Project/Delete/${data}`, {}, false) }

    updateOrder(data: any) { return this.common.put(`Project/UpdateOrder`, data, false) }

    trinhDuyet(data: string) { return this.common.put(`Project/TrinhDuyet/${data}`, {}, false) }

    xacNhan(data: string) { return this.common.put(`Project/XacNhan/${data}`, {}, false) }

    pheDuyet(data: string) { return this.common.put(`Project/PheDuyet/${data}`, {}, false) }
    
    tuChoi(data: string) { return this.common.put(`Project/TuChoi/${data}`, {}, false) }

    yeuCauChinhSua(data: string) { return this.common.put(`Project/YeuCauChinhSua/${data}`, {}, false) }

    getCurrentStep(data: string) { return this.common.get(`Project/GetCurrentStep/${data}`, {}, false) }
}
