import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { PaginationResult } from '../../../class/common/pagination-result.class';
import { GlobalService } from '../../../services/common/global.service';
import { NgModule } from '../../../shared/ng-zorro.module';
import { UnitService } from '../../services/unit.service';
import { UnitDto } from '../../../class/MD/unit.class';

@Component({
  selector: 'app-unit',
  imports: [NgModule],
  templateUrl: './unit.html',
})
export class Unit implements OnInit, OnDestroy {

  private destroy$ = new Subject<void>();
  visible: boolean = false;
  isEdit: boolean = false;
  data: PaginationResult = new PaginationResult();
  dto: UnitDto = new UnitDto();
  filter: UnitDto = new UnitDto();

  constructor(private global: GlobalService, private service: UnitService) {
    this.global.setBreadcrumb([
      {
        name: 'Đơn vị tính',
        path: 'master-data/unit',
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
      this.dto = new UnitDto();
    }
  }

  close() {
    this.visible = false;
    this.dto = new UnitDto();
  }

  save() {
    const action = this.isEdit ? 'update' : 'insert';
    this.service[action](this.dto)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.search();
          if (!this.isEdit) this.dto = new UnitDto();
        }
      })
  }

  reset() {
    this.filter = new UnitDto();
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

