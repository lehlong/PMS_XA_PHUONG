import { Injectable } from '@angular/core';
import { CommonService } from '../../services/common/common.service';

@Injectable({
    providedIn: 'root',
})
export class ProjectWorkflowProcessingService {

    constructor(private common: CommonService) { }

    getProjectWorkflowStep(projectId: string) { return this.common.get(`ProjectWorkflowProcessing/GetProjectWorkflowStep/${projectId}`, {}, false) }

    updateWorkflowProject(data: any) { return this.common.put(`ProjectWorkflowProcessing/UpdateWorkflowProject`, data, false) }

    startWorkflow(projectId: string) { return this.common.put(`ProjectWorkflowProcessing/StartWorkflow/${projectId}`, {}, false) }

    // danh sách các workflow công việc được gán vào dự án
    getProjectWorkFlow(projectId: string){
        return this.common.get('ProjectStruct/GetTaskWorkflow/'+projectId)
    }

    // danh sách các workflow dự án
    getProjectFlowData(projectId: string){
        return this.common.get('ProjectWorkflowProcessing/GetProjectWorkflow/'+projectId)
    }
}
