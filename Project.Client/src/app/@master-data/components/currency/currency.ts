import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { PaginationResult } from '../../../class/common/pagination-result.class';
import { GlobalService } from '../../../services/common/global.service';
import { NgModule } from '../../../shared/ng-zorro.module';
import { CurrencyService } from '../../services/currency.service';
import { CurrencyDto } from '../../../class/MD/currency.class';

@Component({
  selector: 'app-currency',
  imports: [NgModule],
  templateUrl: './currency.html',
})
export class Currency implements OnInit, OnDestroy {

  private destroy$ = new Subject<void>();
  visible: boolean = false;
  isEdit: boolean = false;
  data: PaginationResult = new PaginationResult();
  dto: CurrencyDto = new CurrencyDto();
  filter: CurrencyDto = new CurrencyDto();

  constructor(private global: GlobalService, private service: CurrencyService) {
    this.global.setBreadcrumb([
      {
        name: 'Tiền tệ',
        path: 'master-data/currency',
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
      this.dto = new CurrencyDto();
    }
  }

  close() {
    this.visible = false;
    this.dto = new CurrencyDto();
  }

  save() {
    const action = this.isEdit ? 'update' : 'insert';
    this.service[action](this.dto)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.search();
          if (!this.isEdit) this.dto = new CurrencyDto();
        }
      })
  }

  reset() {
    this.filter = new CurrencyDto();
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

