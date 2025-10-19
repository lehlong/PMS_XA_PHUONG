import { Injectable } from '@angular/core';
import { CommonService } from '../../services/common/common.service';

@Injectable({
    providedIn: 'root',
})
export class ProjectPersonService {
    constructor(private common: CommonService) { }

    getProjectPerson(projectId: string) { return this.common.get(`ProjectPerson/GetProjectPerson/${projectId}`, {}, false) }

    updateProjectPerson(request: any) { return this.common.put(`ProjectPerson/UpdateProjectPerson`, request, false) }

    updateInfoProjectPerson(request: any) { return this.common.put(`ProjectPerson/UpdateInfoProjectPerson`, request, false) }

    deleteProjectPerson(ids: any) { return this.common.post(`ProjectPerson/DeleteProjectPerson`, ids, false) }
}
