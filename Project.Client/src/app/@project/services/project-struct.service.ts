import { Injectable } from '@angular/core';
import { CommonService } from '../../services/common/common.service';
import { ProjectStructDto } from '../../class/PS/project-struct.class';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class ProjectStructService {
    constructor(private common: CommonService) { }

    getProjectStruct(projectId: string) { return this.common.get(`ProjectStruct/GetProjectStruct/${projectId}`, {}, false) }

    insert(data: ProjectStructDto) { return this.common.post('ProjectStruct/Insert', data, false) }

    getProjectPerson(projectId: string) { return this.common.get(`ProjectPerson/GetProjectPerson/${projectId}`, {}, false) }

    // Gán người thực hiện
    assignPersonToTask(data: any) { return this.common.post('ProjectStruct/InsertTaskPerson', data) }

    // Lấy chi tiết thông tin task
    getTaskDetail(taskId: string) { return this.common.get(`ProjectStruct/GetTask/${taskId}`) }
}
