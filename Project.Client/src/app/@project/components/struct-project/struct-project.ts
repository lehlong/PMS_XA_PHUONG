import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { NgModule } from '../../../shared/ng-zorro.module';
import { ProjectStructType } from '../../../shared/statics/project-struct-type.static';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { GlobalService } from '../../../services/common/global.service';
import { ProjectStructService } from '../../services/project-struct.service';
import { TreeUtils } from '../../../services/utilities/tree.ultis';
import { ProjectStructDto } from '../../../class/PS/project-struct.class';
import { FileService } from '../../../services/common/file.service';
import { SearchableSelect } from '../../../shared/components/searchable-select/searchable-select';
import { OrganizeService } from '../../../@master-data/services/organize.service';
import { WorkflowDto } from '../../../class/MD/workflow.class';
import { WorkflowType } from '../../../shared/statics/workflow-type.static';
import { WorkflowService } from '../../../@master-data/services/workflow.service';
import { ErrorMessage } from '../../../shared/components/error-message/error-message';
import { ProjectStatus } from '../../../shared/statics/project-status.static';
import { ProjectWorkflowProcessingService } from '../../services/project-workflow-processing.service';
import { WorkflowProjectAction } from '../../../shared/statics/workflow-action.static';

@Component({
  selector: 'app-struct-project',
  imports: [NgModule, SearchableSelect, ErrorMessage],
  templateUrl: './struct-project.html',
  styleUrls: ['../../project.scss']
})
export class StructProject implements OnInit {

  private destroy$ = new Subject<void>();
  projectId: string = '';
  visibleAddCv = false;
  isEdit: boolean = false;
  titleAddCv: string = '';
  projectStructType = ProjectStructType;

  listOfMapDataStruct: any[] = [];
  mapOfExpandedData: { [id: string]: any[] } = {};

  checkedStruct: boolean = false;
  indeterminateStruct: boolean = false;
  setOfCheckedIdStruct = new Set<any>();

  structs: any[] = [];
  lstWorkflow: any[] = [];
  lstSteps: any[] = []; 
  workflowProjectAction = WorkflowProjectAction;

  dto: ProjectStructDto = new ProjectStructDto();
  dataListUser: any = [];
  dataListUserSelected: any = [];
  dataListOrgData: any = [];
  submitted = false;
  codeExistError = false;
  orgId: string = '';
  dataDetailInformation: any;

  constructor(
    private route: ActivatedRoute,
    private global: GlobalService,
    private service: ProjectStructService,
    private _file: FileService,
    private org: OrganizeService,
    private workflowService: WorkflowService,
    private workflowProcessingService: ProjectWorkflowProcessingService
  ) { }

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get('projectId') ?? '';
    this.getProjectStruct();
    this.getDataListOrg();
    this.getWorkflow();
    this.loadProjectEmployeeData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  getProjectStruct() {
    this.service.getProjectStruct(this.projectId).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res: any) => {
        this.structs = res;
        this.listOfMapDataStruct = TreeUtils.buildNzPrjectTree(res);
        this.listOfMapDataStruct.forEach(i => {
          this.mapOfExpandedData[i.id] = this.convertTreeToList(i);
        });
      },
      error: (err: any) => console.error(err),
    })
  }
  getWorkflow() {
    const filter = new WorkflowDto();
     filter.type = WorkflowType.Task;
    filter.isActive = true;
    filter.pageSize = 50; 
    filter.currentPage = 1;

     this.workflowService.search(filter)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
         next: (res: any) => {
           this.lstWorkflow = res.data; 
         }
   })
  }
  getSteps(code: string) {
    if (this.projectId && code) {
      this.workflowProcessingService.getProjectWorkflowStep(this.projectId, code)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (res: any) => {
            this.lstSteps = res;
          }
        })
    }
  }
getStatusText(status: number | null | undefined): string {
    if (status === null || status === undefined) return '';
    return ProjectStatus.getText(status);
}

  // 3. Viết hàm lấy Màu (để hiển thị trên nz-tag)
  // Bạn có thể tùy chỉnh màu theo ý thích
  getStatusColor(status: number): string {
    switch (status) {
        case ProjectStatus.KhoiTao: return 'default';         // Màu xám
        case ProjectStatus.DaTrinhDuyet: return 'processing'; // Màu xanh dương nhạt
        case ProjectStatus.DaXacNhan: return 'geekblue';      // Màu xanh đậm
        case ProjectStatus.DaPheDuyet: return 'success';      // Màu xanh lá
        case ProjectStatus.TuChoi: return 'error';            // Màu đỏ
        case ProjectStatus.YeuCauChinhSua: return 'warning';  // Màu cam
        default: return 'default';
    }
  }

  refreshCheckedStatusStruct(): void {
    this.checkedStruct = this.structs.every((i: any) => this.setOfCheckedIdStruct.has(i.id));
    this.indeterminateStruct = this.structs.some((i: any) => this.setOfCheckedIdStruct.has(i.id)) && !this.checkedStruct;
  }

  updateCheckedSetStruct(id: any, checked: boolean): void {
    if (checked) {
      this.setOfCheckedIdStruct.add(id);
    } else {
      this.setOfCheckedIdStruct.delete(id);
    }
  }

  onItemCheckedStruct(id: any, checked: boolean): void {
    this.updateCheckedSetStruct(id, checked);
    this.refreshCheckedStatusStruct();
  }


  onAllCheckedStruct(checked: boolean): void {
    this.structs
      .forEach((i: any) => this.updateCheckedSetStruct(i.id, checked));
    this.refreshCheckedStatusStruct();
  }

  collapse(array: any[], data: any, $event: boolean): void {
  if (!$event) {
    if (data.children) {
      data.children.forEach((d: any) => {
        const target = array.find(a => a.id === d.id);
        if (target) {
          target.expand = false;
          target.expanded = false;
          this.collapse(array, target, false);
        }
      });
    }
  }
}

  convertTreeToList(root: any): any[] {
    const stack: any[] = [];
    const array: any[] = [];
    const hashMap = {};
    stack.push({ ...root, level: 0, expand: false });

    while (stack.length !== 0) {
      const node = stack.pop()!;
      this.visitNode(node, hashMap, array);
      if (node.children) {
        for (let i = node.children.length - 1; i >= 0; i--) {
          stack.push({ ...node.children[i], level: node.level! + 1, expand: false, parent: node });
        }
      }
    }

    return array;
  }

  visitNode(node: any, hashMap: { [id: string]: boolean }, array: any[]): void {
    if (!hashMap[node.id]) {
      hashMap[node.id] = true;
      array.push(node);
    }
  }

  closeAddCv() {
    this.titleAddCv = '';
    this.visibleAddCv = false;
    this.dto = new ProjectStructDto();
    this.lstSteps = [];
  }

  openAddCv(data: any, isCallDetailData: boolean = false) {
    this.isEdit = false;
    this.titleAddCv = data?.name;
    this.dto.projectId = this.projectId;
    this.dto.type = this.projectStructType.CongViec;
    this.dto.pId = data.id;
    this.visibleAddCv = true;

    if(isCallDetailData){
      this.getDataTaskDetail(data.id);
    }else{
      this.dataListUserSelected = [];
      this.loadProjectEmployeeData();
    }
  }

  addCv(form: any) {
    this.submitted = true;
    if (form.invalid || this.codeExistError) {
      return;
    }
    this.service.insert(this.dto).pipe(takeUntil(this.destroy$)).subscribe({
      next :(res: any) => {
        if(res?.data?.id){
          let dataRequest = this.prepareApiAssignPerson(res.data.id);
          this.callApiAssignPerson(dataRequest);
        }else{
          this.closeAddCv();
          this.getProjectStruct();
        }
      }
    })
  }
  openUpdateCv(data: any) {
    // 1. Đặt cờ hiệu là đang sửa
    this.isEdit = true;
    
    // 2. Gán tiêu đề drawer (để hiển thị ở HTML như bước trước ta đã làm)
    this.titleAddCv = data.name;
    
    // 3. Reset DTO và gán ID dự án
    this.dto = new ProjectStructDto();
    this.dto.projectId = this.projectId;
    this.dto.id = data.id; // Quan trọng: Phải có ID thì nút Lưu mới hiểu là Update

    // 4. Mở Drawer
    this.visibleAddCv = true;

    // 5. Gọi API lấy chi tiết để fill dữ liệu vào Form (Ngày tháng, người thực hiện...)
    this.getDataTaskDetail(data.id);
    this.getSteps(data.code);
  }
updateCv(form: any) {
  this.submitted = true;

  if (form.invalid || this.codeExistError) {
    return;
  }

  this.service.update(this.dto)
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: (res: any) => {
        // Lấy ID từ response hoặc dto (ưu tiên response nếu backend trả về id mới)
        const taskId = res.data ? res.data.id : this.dto.id; 
        
        let dataRequest = this.prepareApiUpdateAssignPerson(taskId);

        // GỌI API KỂ CẢ KHI MẢNG RỖNG (Để backend biết mà xóa hết)
        // Hoặc tùy logic backend của bạn có chặn mảng rỗng không
        if (dataRequest) {
           // Backend của bạn đang yêu cầu mảng có ít nhất 1 phần tử để lấy TaskId
           // Nếu mảng rỗng, ta không gọi API UpdateTaskPerson được theo cách hiện tại
           // TRỪ KHI bạn viết riêng 1 API DeleteAllTaskPerson.
           
           // Nếu dataRequest có dữ liệu -> Gọi Update
           if (dataRequest.length > 0) {
              this.callApiUpdateAssignPerson(dataRequest);
           } else {
              // Nếu rỗng -> Coi như xong việc (Hoặc gọi API xóa sạch nếu có)
              this.closeAddCv();
              this.getProjectStruct();
           }
        }
      },
      error: (err) => {
        console.error("Lỗi update công việc:", err);
      }
    });
}

// Hàm gọi API update assign person riêng để code gọn hơn
private callApiUpdateAssignPerson(dataRequest: any): void {
  this.service.updateAssignPersonToTask(dataRequest)
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: (res) => {
        this.closeAddCv();
        this.getProjectStruct();
      },
      error: (err) => {
        console.error("Lỗi update người thực hiện:", err);
      }
    });
}
saveCv(form: any) {
    // Nếu dto có id => Đang là sửa => Gọi Update
    if (this.dto.id) {
      this.updateCv(form);
    } else {
      // Ngược lại => Thêm mới => Gọi Add
      this.addCv(form);
    }
  }
  upload(e: any) {
    const input = e.target as HTMLInputElement;
    const files = input.files;

    if (!files?.length) return;

    const formData = new FormData();
    if (files?.length) {
      Array.from(files).forEach((file) => formData.append('files', file));
    }

    this._file.upload(formData).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res: any) => {
        this.dto.files = [...this.dto.files, ...res.data]
      }
    })
  }

  deleteFile(f: any) {
    this.dto.files = this.dto.files.filter(x => x.id != f.id)
  }

  // Khi click checkbox
  onChangeCheckbox(item: any, field: 'isChuTri' | 'isPhoiHop' | 'isNhanDeBiet') {
    const isSelected =
    item.isChuTri ||
    item.isPhoiHop ||
    item.isNhanDeBiet;

    const index = this.dataListUserSelected.findIndex((x: any) => x.userName === item.person.userName);

    if (isSelected) {
      // Nếu chưa có → push
      if (index === -1) {
        this.dataListUserSelected = [
          ...this.dataListUserSelected,
          {
            ...item,
            dataTask: [
              { workItem: '', note: '' }
            ]
          }
        ];
      } else {
        // Nếu đã có → cập nhật lại 3 checkbox
        this.dataListUserSelected[index] = {
          ...this.dataListUserSelected[index],
          isChuTri: item.isChuTri,
          isPhoiHop: item.isPhoiHop,
          isNhanDeBiet: item.isNhanDeBiet
        };

        // tạo array mới để Angular detect thay đổi
        this.dataListUserSelected = [...this.dataListUserSelected];
      }
    } 
    else {
      // Nếu cả 3 đều false → remove
      this.dataListUserSelected = this.dataListUserSelected.filter((x: any) => x.id !== item.id);
    }
  }

  onSearchOrgId(event: any): void{
    this.orgId = event.value
    this.loadProjectEmployeeData(this.orgId);
  }

  addNewSelectedRow(item: any) {
    item.dataTask.push(
      {workItem: '', note: ''}
    )
  }

  removeSelectedRow(item: any, index: number) {

    if(item.dataTask.length > 1){
      item.dataTask.splice(index, 1);
      return;
    }

    const target = this.dataListUser.find((x: any) => x.id === item.id);
    if (target) {
      target.isChuTri = false;
      target.isPhoiHop = false;
      target.isNhanDeBiet = false;
    }

    this.dataListUserSelected = this.dataListUserSelected.filter((x: any) => x !== item);
  }

  changeDataWorkName(event: any, item: any): void{
    item.workItem = event.target.value; 
  }

  changeDataWorkNote(event: any, item: any): void{
    item.note = event.target.value;
  }

  getDataTaskDetail(taskId: string): void{
    this.service.getTaskDetail(taskId)
    .pipe(takeUntil(this.destroy$))
    .subscribe((res: any) => {
      if(res && res.length > 0){
        this.dataDetailInformation = res;
        this.dataListUserSelected = res[0].taskPerson.map((item: any) => {
          const roles = item.taskRoles || []
          return {
            ...item,
            isChuTri: roles.includes(1),      
            isPhoiHop: roles.includes(2),    
            isNhanDeBiet: roles.includes(3),
            person: {
              fullName: item.userName,
              userName: item.userName
            },
            dataTask: item.taskPersonDetails.map((detail: any) => {
              return {
                id: detail.id,
                taskPersonId: detail.taskPersonId,
                workItem: detail.task,
                note: detail.note
              }
            })
          }
        });
        this.dto.id = res[0].id;
        this.dto.code = res[0].code;
        this.dto.name = res[0].name;
        this.dto.endDate = res[0].endDate;
        this.dto.notes = res[0].notes;
        this.dto.workflowId = res[0].workflowId;
        this.loadProjectEmployeeData(this.orgId);
        }
    })
  }

  private callApiAssignPerson(dataRequest: any): void{
    this.service.assignPersonToTask(dataRequest)
    .pipe(takeUntil(this.destroy$))
    .subscribe((res) => {
      this.closeAddCv();
      this.getProjectStruct();
    })
  }

  private prepareApiAssignPerson(taskId: string): any{
    return this.dataListUserSelected.map((item: any) => {
      let dataTaskRole = [];
      if(item.isChuTri) dataTaskRole.push(1);
      if(item.isPhoiHop) dataTaskRole.push(2);
      if(item.isNhanDeBiet) dataTaskRole.push(3);

      item.dataTask = item.dataTask.map((detail: any) => ({
        ...detail,
        task: detail.workItem
      }));

      if(dataTaskRole.length === 0) return;
      return {
        taskId: taskId,
        projectId: this.projectId,
        userName: item.person.userName,
        taskRoles: dataTaskRole,
        projectRoleCode: item.projectRoleCode ? item.projectRoleCode : '',
        taskPersonDetails: item.dataTask,
      }
    })
  }
  private prepareApiUpdateAssignPerson(taskId: string): any {
  const result = this.dataListUserSelected.map((item: any) => {
    let dataTaskRole = [];
    if (item.isChuTri) dataTaskRole.push(1);
    if (item.isPhoiHop) dataTaskRole.push(2);
    if (item.isNhanDeBiet) dataTaskRole.push(3);

    // Map lại cấu trúc detail cho khớp DTO backend
    const details = item.dataTask ? item.dataTask.map((detail: any) => ({
      id: detail.id ? detail.id : null, 
      taskPersonId: detail.taskPersonId ? detail.taskPersonId : null,
      userName: detail.userName,
      task: detail.workItem,
      note: detail.note
    })) : [];

    // Nếu không chọn vai trò nào thì bỏ qua người này
    if (dataTaskRole.length === 0) return undefined;

    return {
      id: item.id ? item.id : null, // QUAN TRỌNG: Gửi ID của TaskPerson cũ nếu có (để Backend biết là Sửa)
      taskId: taskId,               // BẮT BUỘC CÓ
      projectId: this.projectId,    // BẮT BUỘC CÓ
      userName: item.userName || item.person?.userName,
      taskRoles: dataTaskRole,      // Backend nhận mảng [1,2]
      projectRoleCode: item.projectRoleCode ? item.projectRoleCode : '',
      taskPersonDetails: details,
    };
  });

  // LỌC BỎ GIÁ TRỊ NULL/UNDEFINED
  return result.filter((item: any) => item !== undefined && item !== null);
}
  
  private getDataListOrg(): void{
    this.org.getAll().pipe(takeUntil(this.destroy$)).subscribe({
      next: (res: any) => {
        this.dataListOrgData = [];
        this.dataListOrgData = res
        .filter((item: any) => item.pId === 'ORG')
        .map((item: any) => ({
          label: item.name,
          value: item.id
        }));
        this.dataListOrgData.unshift({ label: 'Tất cả', value: '' });
      }
    });
  }

  checkExistCode() {
    const code = this.dto.code?.trim();
    if (!code) return;

    this.workflowService.checkCodeExits(code).subscribe({
      next: (res: any) => {
        this.codeExistError = res
      },
      error: (err) => {
        console.error('Lỗi khi kiểm tra code:', err);
      },
    });
  }

  private loadProjectEmployeeData(orgId?: string): void{
    forkJoin({
      roles: this.workflowService.getProjectRoles(),
      employees: this.workflowService.getProjectEmployee(this.projectId, orgId ? orgId : '')
    })
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: (res: any) => {
        const roles = res.roles;
        const employees = res.employees;

        // Map employees để gán roleName
        const employeesWithRoleName = employees.map((emp: any) => {
          const matchedRole = roles.find((role: any) => role.code === emp.projectRoleCode);

          // tìm trong selected (dữ liệu đã lưu)
          const saved = this.dataListUserSelected.find((x: any) => x.userName === emp.userName);

          return {
            ...emp,
            roleName: matchedRole ? matchedRole.name : null,
            isChuTri: saved ? saved.isChuTri : false,
            isPhoiHop: saved ? saved.isPhoiHop : false,
            isNhanDeBiet: saved ? saved.isNhanDeBiet : false
          };
        });

        if(employeesWithRoleName.length > 0){
          this.dataListUser = employeesWithRoleName;
        }
      },
      error: (err) => {
        console.error(err);
      }
    });
  }
}