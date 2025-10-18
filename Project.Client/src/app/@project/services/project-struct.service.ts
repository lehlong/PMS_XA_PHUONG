import { Injectable } from '@angular/core';
import { CommonService } from '../../services/common/common.service';
import { ProjectDto } from '../../class/PS/project.class';

@Injectable({
    providedIn: 'root',
})
export class ProjectStructService {
    constructor(private common: CommonService) { }

    getProjectStruct(projectId: string) { return this.common.get(`ProjectStruct/GetProjectStruct/${projectId}`, {}, false) }
}
