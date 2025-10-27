import { Injectable } from '@angular/core';
import { CommonService } from '../../services/common/common.service';

@Injectable({
    providedIn: 'root',
})
export class ProjectWorkflowProcessingService {

    constructor(private common: CommonService) { }

    getProjectWorkflowStep(projectId: string) { return this.common.get(`ProjectWorkflowProcessing/GetProjectWorkflowStep/${projectId}`, {}, true) }

    updateWorkflowProject(data: any) { return this.common.put(`ProjectWorkflowProcessing/UpdateWorkflowProject`, data, true) }

    startWorkflow(projectId: string) { return this.common.put(`ProjectWorkflowProcessing/StartWorkflow/${projectId}`, {}, true) }
}
