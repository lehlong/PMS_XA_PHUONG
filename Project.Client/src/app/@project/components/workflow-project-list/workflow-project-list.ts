import { Component, EventEmitter, Output } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { PaginationResult } from '../../../class/common/pagination-result.class';
import { WorkflowDto } from '../../../class/MD/workflow.class';
import { WorkflowProjectAction } from '../../../shared/statics/workflow-action.static';
import { GlobalService } from '../../../services/common/global.service';
import { WorkflowService } from '../../../@master-data/services/workflow.service';
import { CapDuAnService } from '../../../@master-data/services/cap-du-an.service';
import { OrganizeService } from '../../../@master-data/services/organize.service';
import { WorkflowType } from '../../../shared/statics/workflow-type.static';
import { NgModule } from '../../../shared/ng-zorro.module';

@Component({
  selector: 'app-workflow-project-list',
  imports: [NgModule],
  templateUrl: './workflow-project-list.html',
  styleUrl: './workflow-project-list.scss',
  standalone: true
})
export class WorkflowProjectList {
  @Output() showDetailWorkFolow = new EventEmitter<boolean>(false)
  private destroy$ = new Subject<void>();
  visible: boolean = false;
  isEdit: boolean = false;
  data: PaginationResult = new PaginationResult();
  dto: WorkflowDto = new WorkflowDto();
  filter: WorkflowDto = new WorkflowDto();
  lstAction = WorkflowProjectAction.getList();
  lstProjectLevel: any[] = [];
  lstOrganize: any[] = [];

  constructor(
    private global: GlobalService,
    private service: WorkflowService,
    private projectLevelService: CapDuAnService,
    private organizeService: OrganizeService
  ) {
    this.global.setBreadcrumb([
      {
        name: 'Workflow dự án',
        path: 'master-data/workflow-project',
      },
    ]);
  }

  ngOnInit(): void {
    this.search();
    this.getProjectLevel();
    this.getOrganize();
  }

  search() {
    this.filter.type = WorkflowType.Task;
    this.service.search(this.filter)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res: any) => {
          console.log(res);
          this.data = res
        }
      })
  }

  getProjectLevel() {
    this.projectLevelService.getAll().pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res: any) => {
          this.lstProjectLevel = res
        }
      })
  }

  getOrganize() {
    this.organizeService.getAll().pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res: any) => {
          this.lstOrganize = res
        }
      })
  }

  trackById(index: number, item: any): any {
    return item.id || item.code;
  }

  reset() {
    this.filter = new WorkflowDto();
    this.search();
  }

  pageIndexChange(e: any) {
    this.filter.currentPage = e;
    this.search();
  }

  pageSizeChange(e: any) {
    this.filter.pageSize = e;
    this.search();
  }

  ngOnDestroy(): void {
    this.global.setBreadcrumb([]);
    this.destroy$.next();
    this.destroy$.complete();
  }

  onShowDetailWorkflow(value: boolean): void{
    this.showDetailWorkFolow.emit(value)
  }
}
