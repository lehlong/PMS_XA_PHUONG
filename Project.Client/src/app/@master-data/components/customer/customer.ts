import { Component, OnDestroy, OnInit } from '@angular/core';
import { NgModule } from '../../../shared/ng-zorro.module';
import { Subject, takeUntil } from 'rxjs';
import { PaginationResult } from '../../../class/common/pagination-result.class';
import { CustomerDto } from '../../../class/MD/customer.class';
import { GlobalService } from '../../../services/common/global.service';
import { CustomerService } from '../../services/customer.service';

@Component({
  selector: 'app-customer',
  imports: [NgModule],
  templateUrl: './customer.html',
})
export class Customer implements OnInit, OnDestroy {

  private destroy$ = new Subject<void>();
  visible: boolean = false;
  isEdit: boolean = false;
  data: PaginationResult = new PaginationResult();
  dto: CustomerDto = new CustomerDto();
  filter: CustomerDto = new CustomerDto();

  constructor(private global: GlobalService, private service: CustomerService) {
    this.global.setBreadcrumb([
      {
        name: 'Khách hàng',
        path: 'master-data/customer',
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
      this.dto = new CustomerDto();
    }
  }

  close() {
    this.visible = false;
    this.dto = new CustomerDto();
  }

  save() {
    const action = this.isEdit ? 'update' : 'insert';
    this.service[action](this.dto)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.search();
          if (!this.isEdit) this.dto = new CustomerDto();
        }
      })
  }

  reset() {
    this.filter = new CustomerDto();
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


