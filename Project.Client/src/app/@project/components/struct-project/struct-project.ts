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
  titleAddCv: string = '';
  projectStructType = ProjectStructType;

  listOfMapDataStruct: any[] = [];
  mapOfExpandedData: { [id: string]: any[] } = {};

  checkedStruct: boolean = false;
  indeterminateStruct: boolean = false;
  setOfCheckedIdStruct = new Set<any>();

  structs: any[] = [];
  lstWorkflow: any[] = [];

  dto: ProjectStructDto = new ProjectStructDto();
  dataListUser: any = [];
  dataListUserSelected: any = [];
  dataListOrgData: any = [];
  submitted = false;
  codeExistError = false;
  orgId: string = '';

  constructor(
    private route: ActivatedRoute,
    private global: GlobalService,
    private service: ProjectStructService,
    private _file: FileService,
    private org: OrganizeService,
    private workflowService: WorkflowService,
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
        console.log(this.lstWorkflow)
         }
   })
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
  }

  openAddCv(data: any) {
    this.titleAddCv = data?.name;
    this.dto.projectId = this.projectId;
    this.dto.type = this.projectStructType.CongViec;
    this.dto.pId = data.id;
    this.visibleAddCv = true;
  }

  addCv(form: any) {
    this.submitted = true;
    if (form.invalid || this.codeExistError) {
      return;
    }
    this.service.insert(this.dto).pipe(takeUntil(this.destroy$)).subscribe({
      next :(res) => {
        console.log(res)
        // this.closeAddCv();
        // this.getProjectStruct();
      }
    })
    let dataRequestAssignPerson = this.dataListUserSelected.map((item: any) => {
      let dataTaskRole = [];
      if(item.isChuTri) dataTaskRole.push(1);
      if(item.isPhoiHop) dataTaskRole.push(2);
      if(item.isNhanDeBiet) dataTaskRole.push(3);
      if(dataTaskRole.length === 0) return;
      return {
        taskId: '',
        projectId: this.projectId,
        userName: item.person.userName,
        taskRoles: item.dataTaskRole,
        projectRoleCode: item.projectRoleCode ? item.projectRoleCode : '',
        taskPersonDetails: item.dataTask,
      }
    })
    console.log(dataRequestAssignPerson);
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
    // Nếu item chưa được chọn → push vào mảng
    const isSelected = item.isChuTri || item.isPhoiHop || item.isNhanDeBiet;
    
    if (isSelected) {
      // Nếu bất kỳ checkbox nào ON → add vào selected (nếu chưa có)
      const exists = this.dataListUserSelected.some((x: any) => x.id === item.id);
      if (!exists) {
        this.dataListUserSelected = [
          ...this.dataListUserSelected,
          {
            ...item,
            dataTask: [
              {workItem: '', note: ''}
            ]
          }
        ];
      }
    } else {
      // Nếu cả 3 checkbox đều OFF → remove
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
    console.log(item)
    console.log(this.dataListUserSelected)
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
          const saved = this.dataListUserSelected.find((x: any) => x.id === emp.id);

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