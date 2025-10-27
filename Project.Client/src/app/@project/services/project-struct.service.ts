import { Injectable } from '@angular/core';
import { CommonService } from '../../services/common/common.service';
import { ProjectStructDto } from '../../class/PS/project-struct.class';

@Injectable({
    providedIn: 'root',
})
export class ProjectStructService {
    constructor(private common: CommonService) { }

    getProjectStruct(projectId: string) { return this.common.get(`ProjectStruct/GetProjectStruct/${projectId}`, {}, true) }

    insert(data: ProjectStructDto) { return this.common.post('ProjectStruct/Insert', data, true) }
}
