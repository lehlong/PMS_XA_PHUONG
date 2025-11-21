import { Injectable } from '@angular/core';
import { CommonService } from '../../services/common/common.service';
import { BehaviorSubject, Observable } from 'rxjs';
@Injectable({
    providedIn: 'root',
})
export class ProjectWorkflowProcessingService {
private selectedCode = new BehaviorSubject<string | null>(null);
    constructor(private common: CommonService) { }
    setProcessingCode(code: string): void {
    this.selectedCode.next(code);
  }

  getProcessingCode(): Observable<string | null> {
    return this.selectedCode.asObservable();
  }

  getCurrentProcessingCode(): string | null {
    return this.selectedCode.value;
  }

    getProjectWorkflowStep(projectId: string,code: string) { return this.common.get(`ProjectWorkflowProcessing/GetProjectWorkflowStep/${projectId}/${code}`, {}, false) }

    updateWorkflowProject(data: any) { return this.common.put(`ProjectWorkflowProcessing/UpdateWorkflowProject`, data, false) }

    startWorkflow(projectId: string,code: string) { return this.common.put(`ProjectWorkflowProcessing/StartWorkflow/${projectId}/${code}`, {}, false) }
    startTaskWorkflow(projectId: string,code: string) { return this.common.put(`ProjectWorkflowProcessing/StartTaskWorkflow/${projectId}/${code}`, {}, false) }
    // danh sách các workflow công việc được gán vào dự án
    getProjectWorkFlow(projectId: string){
        return this.common.get('ProjectStruct/GetTaskWorkflow/'+projectId)
    }

    // danh sách các workflow dự án
    getProjectFlowData(projectId: string){
        return this.common.get('ProjectWorkflowProcessing/GetProjectWorkflow/'+projectId)
    }
}
