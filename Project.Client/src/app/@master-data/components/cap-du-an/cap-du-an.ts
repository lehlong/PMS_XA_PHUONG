import { Component, OnDestroy, OnInit } from '@angular/core';
import { NgModule } from '../../../shared/ng-zorro.module';
import { Subject, takeUntil } from 'rxjs';
import { PaginationResult } from '../../../class/common/pagination-result.class';
import { CapDuAnDto } from '../../../class/MD/cap-du-an.class';
import { GlobalService } from '../../../services/common/global.service';
import { CapDuAnService } from '../../services/cap-du-an.service';

@Component({
  selector: 'app-cap-du-an',
  imports: [NgModule],
  templateUrl: './cap-du-an.html',
})
export class CapDuAn implements OnInit, OnDestroy {

  private destroy$ = new Subject<void>();
  visible: boolean = false;
  isEdit: boolean = false;
  data: PaginationResult = new PaginationResult();
  dto: CapDuAnDto = new CapDuAnDto();
  filter: CapDuAnDto = new CapDuAnDto();

  constructor(private global: GlobalService, private service: CapDuAnService) {
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
      this.dto = new CapDuAnDto();
    }
  }

  close() {
    this.visible = false;
    this.dto = new CapDuAnDto();
  }

  save() {
    const action = this.isEdit ? 'update' : 'insert';
    this.service[action](this.dto)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.search();
          if (!this.isEdit) this.dto = new CapDuAnDto();
        }
      })
  }

  reset() {
    this.filter = new CapDuAnDto();
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

