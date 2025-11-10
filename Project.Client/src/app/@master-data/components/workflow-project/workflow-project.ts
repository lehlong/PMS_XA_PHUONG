import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { PaginationResult } from '../../../class/common/pagination-result.class';
import { WorkflowDto } from '../../../class/MD/workflow.class';
import { GlobalService } from '../../../services/common/global.service';
import { WorkflowService } from '../../services/workflow.service';
import { NgModule } from '../../../shared/ng-zorro.module';
import { WorkflowProjectAction } from '../../../shared/statics/workflow-action.static';
import { CapDuAnService } from '../../services/cap-du-an.service';
import { OrganizeService } from '../../services/organize.service';
import { WorkflowType } from '../../../shared/statics/workflow-type.static';
import { ErrorMessage } from '../../../shared/components/error-message/error-message';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-workflow-project',
  imports: [NgModule, ErrorMessage],
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
  lstAction = WorkflowProjectAction.getList();
  lstProjectLevel: any[] = [];
  lstOrganize: any[] = [];
  submitted = false;

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
       this.filter.type = WorkflowType.Project;
    this.service.search(this.filter)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res: any) => {
        
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

  open(data: any, isEdit: boolean) {
    this.isEdit = isEdit;
    if (isEdit) {
      this.service.detail(data.id)
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

  save(form: any) {
    this.submitted = true;
     if (form.invalid) {
      return;
    }
    this.dto.type = WorkflowType.Project;
    const action = this.isEdit ? 'update' : 'insert';
    this.service[action](this.dto)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.search();
          this.visible = false;
          if (!this.isEdit) this.dto = new WorkflowDto();
        }
      })
  }

  reset() {
    this.filter = new WorkflowDto();
    this.search();
  }

  addStep(): void {
    const step = {
      step: this.dto.steps.length + 1,
      id: '',
      workflowId: '',
      name: '',
      hanXuLy: 0,
      action: ''
    };

    this.dto.steps = [...this.dto.steps, step];
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

