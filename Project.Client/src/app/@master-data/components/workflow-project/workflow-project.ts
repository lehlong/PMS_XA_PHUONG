import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { PaginationResult } from '../../../class/common/pagination-result.class';
import { WorkflowDto } from '../../../class/MD/workflow.class';
import { GlobalService } from '../../../services/common/global.service';
import { WorkflowService } from '../../services/workflow.service';
import { NgModule } from '../../../shared/ng-zorro.module';

@Component({
  selector: 'app-workflow-project',
  imports: [NgModule],
  templateUrl: './workflow-project.html',
  styleUrl: './workflow-project.scss'
})
export class WorkflowProject implements OnInit, OnDestroy {

  private destroy$ = new Subject<void>();
  visible: boolean = false;
  isEdit: boolean = false;
  data: PaginationResult = new PaginationResult();
  dto: WorkflowDto = new WorkflowDto();
  filter: WorkflowDto = new WorkflowDto();

  constructor(private global: GlobalService, private service: WorkflowService) {
    this.global.setBreadcrumb([
      {
        name: 'Workflow dự án',
        path: 'master-data/workflow-project',
      },
    ]);
  }

  ngOnInit(): void {
    this.search();
  }

  search() {
    this.service.search(this.filter)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res: any) => {
          this.data = res
        }
      })
  }

  trackById(index: number, item: any): any {
    return item.id || item.code;
  }

  open(data: any, isEdit: boolean) {
    debugger
    this.isEdit = isEdit;
    if (isEdit) {
      this.service.detail(data.code)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (res: any) => {
            this.dto = res
            this.visible = true;
          }
        })
    }
    else {
      this.visible = true;
      this.dto = new WorkflowDto();
    }
  }

  close() {
    this.visible = false;
    this.dto = new WorkflowDto();
  }

  save() {
    const action = this.isEdit ? 'update' : 'insert';
    this.service[action](this.dto)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.search();
          if (!this.isEdit) this.dto = new WorkflowDto();
        }
      })
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
}

