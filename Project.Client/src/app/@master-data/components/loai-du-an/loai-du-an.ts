import { Component, OnDestroy, OnInit } from '@angular/core';
import { NgModule } from '../../../shared/ng-zorro.module';
import { Subject, takeUntil } from 'rxjs';
import { PaginationResult } from '../../../class/common/pagination-result.class';
import { LoaiDuAnDto } from '../../../class/MD/loai-du-an.class';
import { GlobalService } from '../../../services/common/global.service';
import { LoaiDuAnService } from '../../services/loai-du-an.service';

@Component({
  selector: 'app-loai-du-an',
  imports: [NgModule],
  templateUrl: './loai-du-an.html',
})
export class LoaiDuAn implements OnInit, OnDestroy {

  private destroy$ = new Subject<void>();
  visible: boolean = false;
  isEdit: boolean = false;
  data: PaginationResult = new PaginationResult();
  dto: LoaiDuAnDto = new LoaiDuAnDto();
  filter: LoaiDuAnDto = new LoaiDuAnDto();

  constructor(private global: GlobalService, private service: LoaiDuAnService) {
    this.global.setBreadcrumb([
      {
        name: 'Loại dự án',
        path: 'master-data/project-type',
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
      this.dto = new LoaiDuAnDto();
    }
  }

  close() {
    this.visible = false;
    this.dto = new LoaiDuAnDto();
  }

  save() {
    const action = this.isEdit ? 'update' : 'insert';
    this.service[action](this.dto)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.search();
          if (!this.isEdit) this.dto = new LoaiDuAnDto();
        }
      })
  }

  reset() {
    this.filter = new LoaiDuAnDto();
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

