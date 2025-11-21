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

    update(data: ProjectStructDto) { return this.common.put('ProjectStruct/Update', data, false) }

    getProjectPerson(projectId: string) { return this.common.get(`ProjectPerson/GetProjectPerson/${projectId}`, {}, false) }

    // Gán người thực hiện
    assignPersonToTask(data: any) { return this.common.post('ProjectStruct/InsertTaskPerson', data) }

    updateAssignPersonToTask(data: any) { return this.common.put('ProjectStruct/UpdateTaskPerson', data) }

    // Lấy chi tiết thông tin task
    getTaskDetail(taskId: string) { return this.common.get(`ProjectStruct/GetTask/${taskId}`) }

    getCurrentStep(projectId: string,code:string) { return this.common.get(`ProjectStruct/GetCurrentStep/${projectId}/${code}`, {}, false) }

    trinhDuyet(data: string) { return this.common.put(`ProjectStruct/TrinhDuyet/${data}`, {}, false) }

    xacNhan(data: string) { return this.common.put(`ProjectStruct/XacNhan/${data}`, {}, false) }

    pheDuyet(data: string) { return this.common.put(`ProjectStruct/PheDuyet/${data}`, {}, false) }
    
    tuChoi(data: string) { return this.common.put(`ProjectStruct/TuChoi/${data}`, {}, false) }

    yeuCauChinhSua(data: string) { return this.common.put(`ProjectStruct/YeuCauChinhSua/${data}`, {}, false) }
}
