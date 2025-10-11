import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { PaginationResult } from '../../../class/common/pagination-result.class';
import { ProjectRoleDto } from '../../../class/MD/project-role.class';
import { GlobalService } from '../../../services/common/global.service';
import { ProjectRoleService } from '../../services/project-role.service';
import { NgModule } from '../../../shared/ng-zorro.module';

@Component({
  selector: 'app-project-role',
  imports: [NgModule],
  templateUrl: './project-role.html'
})
export class ProjectRole implements OnInit, OnDestroy {

  private destroy$ = new Subject<void>();
  visible: boolean = false;
  isEdit: boolean = false;
  data: PaginationResult = new PaginationResult();
  dto: ProjectRoleDto = new ProjectRoleDto();
  filter: ProjectRoleDto = new ProjectRoleDto();

  constructor(private global: GlobalService, private service: ProjectRoleService) {
    this.global.setBreadcrumb([
      {
        name: 'Vai trò dự án',
        path: 'master-data/project-role',
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
      this.dto = new ProjectRoleDto();
    }
  }

  close() {
    this.visible = false;
    this.dto = new ProjectRoleDto();
  }

  save() {
    const action = this.isEdit ? 'update' : 'insert';
    this.service[action](this.dto)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.search();
          if (!this.isEdit) this.dto = new ProjectRoleDto();
        }
      })
  }

  reset() {
    this.filter = new ProjectRoleDto();
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

