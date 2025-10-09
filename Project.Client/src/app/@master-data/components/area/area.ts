import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { PaginationResult } from '../../../class/common/pagination-result.class';
import { AreaDto } from '../../../class/MD/area.class';
import { GlobalService } from '../../../services/common/global.service';
import { AreaService } from '../../services/area.service';
import { NgModule } from '../../../shared/ng-zorro.module';

@Component({
  selector: 'app-area',
  imports: [NgModule],
  templateUrl: './area.html',
})
export class Area implements OnInit, OnDestroy {

  private destroy$ = new Subject<void>();
  visible: boolean = false;
  isEdit: boolean = false;
  data: PaginationResult = new PaginationResult();
  dto: AreaDto = new AreaDto();
  filter: AreaDto = new AreaDto();

  constructor(private global: GlobalService, private service: AreaService) {
    this.global.setBreadcrumb([
      {
        name: 'Cấp dự án',
        path: 'master-data/project-level',
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
      this.dto = new AreaDto();
    }
  }

  close() {
    this.visible = false;
    this.dto = new AreaDto();
  }

  save() {
    const action = this.isEdit ? 'update' : 'insert';
    this.service[action](this.dto)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.search();
          if (!this.isEdit) this.dto = new AreaDto();
        }
      })
  }

  reset() {
    this.filter = new AreaDto();
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

