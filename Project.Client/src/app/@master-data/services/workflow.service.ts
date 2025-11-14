import { Injectable } from '@angular/core';
import { CommonService } from '../../services/common/common.service';
import { WorkflowDto } from '../../class/MD/workflow.class';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class WorkflowService {
    constructor(private common: CommonService) { }

    search(params: WorkflowDto) { return this.common.get('Workflow/Search', params, false) }

    getAll() { return this.common.get('Workflow/GetAll', {}, false) }

    getAllActive() { return this.common.get('Workflow/GetAllActive', {}, false) }

    insert(data: WorkflowDto) { return this.common.post('Workflow/Insert', data, false) }

    update(data: WorkflowDto) { return this.common.put('Workflow/Update', data, false) }

    detail(data: string) { return this.common.get(`Workflow/Detail/${data}`, {}, true) }

    delete(data: string) { return this.common.delete(`Workflow/Delete/${data}`, {}, false) }

    updateOrder(data: any) { return this.common.put(`Workflow/UpdateOrder`, data, false) }

    checkCodeExits(code: string): Observable<void>{
        return this.common.get('ProjectStruct/CheckCodeExists', { code: code }, false);
    }
}
