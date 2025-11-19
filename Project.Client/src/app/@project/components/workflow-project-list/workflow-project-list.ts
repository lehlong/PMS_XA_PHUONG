import { Component, EventEmitter, Output } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { PaginationResult } from '../../../class/common/pagination-result.class';
import { WorkflowDto } from '../../../class/MD/workflow.class';
import { WorkflowProjectAction } from '../../../shared/statics/workflow-action.static';
import { GlobalService } from '../../../services/common/global.service';
import { CapDuAnService } from '../../../@master-data/services/cap-du-an.service';
import { OrganizeService } from '../../../@master-data/services/organize.service';
import { WorkflowType } from '../../../shared/statics/workflow-type.static';
import { NgModule } from '../../../shared/ng-zorro.module';
import { ProjectWorkflowProcessingService } from '../../services/project-workflow-processing.service';
import { ActivatedRoute } from '@angular/router';

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
  data: any = [];
  originalData: any[] = [];  // dữ liệu gốc từ API (không thay đổi)
  filter: WorkflowDto = new WorkflowDto();
  lstAction = WorkflowProjectAction.getList();
  lstProjectLevel: any[] = [];
  lstOrganize: any[] = [];
  projectId: string = '';
  pageIndex = 1;
  pageSize = 10;
  total = 0;// tổng bản ghi sau khi filter

  constructor(
    private global: GlobalService,
    private projectLevelService: CapDuAnService,
    private organizeService: OrganizeService,
    private projectWorkflowProcessingService: ProjectWorkflowProcessingService,
    private route: ActivatedRoute,
  ) {
    
  }

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get('projectId') ?? '';
    this.search();
    this.getProjectLevel();
    this.getOrganize();
    this.workflowProjectData();
  }

  search() {
    this.filter.type = WorkflowType.Task;
    this.projectWorkflowProcessingService.getProjectWorkFlow(this.projectId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res: any) => {
          this.originalData = res
          this.applyFilterAndPaging();
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
    // this.filter = new WorkflowDto();
    // this.search();
    this.filter = new WorkflowDto();
    this.pageIndex = 1;
    this.applyFilterAndPaging();
  }

  pageIndexChange(e: any) {
    // this.filter.currentPage = e;
    // this.search();
    this.pageIndex = e;
    this.applyFilterAndPaging();
  }

  pageSizeChange(e: any) {
    // this.filter.pageSize = e;
    // this.search();
    this.pageSize = e;
    this.pageIndex = 1; // reset về trang đầu
    this.applyFilterAndPaging()
  }

  ngOnDestroy(): void {
    this.global.setBreadcrumb([]);
    this.destroy$.next();
    this.destroy$.complete();
  }

  onShowDetailWorkflow(value: boolean): void{
    this.showDetailWorkFolow.emit(value)
  }

  private workflowProjectData(): void{
    this.projectWorkflowProcessingService.getProjectFlowData(this.projectId)
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: (res: any) => {
        console.log(res);
      }
    })
  }

  private applyFilterAndPaging() {
    let filtered = [...this.originalData];

    // === SEARCH ===
    if (this.filter.keyWord && this.filter.keyWord.trim() !== '') {
      const keyword = this.filter.keyWord.toLowerCase();
      filtered = filtered.filter(item =>
        (item.code?.toLowerCase().includes(keyword)) ||
        (item.name?.toLowerCase().includes(keyword)) ||
        (item.notes?.toLowerCase().includes(keyword))
      );
    }

    // Tổng sau khi search
    this.total = filtered.length;

    // === PAGINATION ===
    const start = (this.pageIndex - 1) * this.pageSize;
    const end = this.pageIndex * this.pageSize;

    this.data = filtered.slice(start, end);
  }
}
