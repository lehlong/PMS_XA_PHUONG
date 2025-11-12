import { Component, OnInit } from '@angular/core';
import { NgModule } from '../../../shared/ng-zorro.module';
import { ProjectStructType } from '../../../shared/statics/project-struct-type.static';
import { Subject, takeUntil } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { GlobalService } from '../../../services/common/global.service';
import { ProjectStructService } from '../../services/project-struct.service';
import { TreeUtils } from '../../../services/utilities/tree.ultis';
import { ProjectStructDto } from '../../../class/PS/project-struct.class';
import { FileService } from '../../../services/common/file.service';
import { WorkflowDto } from '../../../class/MD/workflow.class';
import { WorkflowType } from '../../../shared/statics/workflow-type.static';
import { WorkflowService } from '../../../@master-data/services/workflow.service';
@Component({
  selector: 'app-struct-project',
  imports: [NgModule],
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

  constructor(
    private route: ActivatedRoute,
    private global: GlobalService,
    private service: ProjectStructService,
    private workflowService: WorkflowService,
    private _file: FileService
  ) { }

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get('projectId') ?? '';
    this.getProjectStruct();
    this.getWorkflow();
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

  addCv() {
    this.service.insert(this.dto).pipe(takeUntil(this.destroy$)).subscribe({
      next :(res) => {
        this.closeAddCv();
        this.getProjectStruct();
      }
    })
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
}
