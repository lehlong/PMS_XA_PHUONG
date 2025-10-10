import { Component, OnDestroy, OnInit } from '@angular/core';
import { NgModule } from '../../../shared/ng-zorro.module';
import { NzMessageService } from 'ng-zorro-antd/message';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import { PaginationResult } from '../../../class/common/pagination-result.class';
import { GlobalService } from '../../../services/common/global.service';
import { ProjectDto } from '../../../class/PS/project.class';
import { ProjectService } from '../../services/project.service';
import { LoaiDuAnService } from '../../../@master-data/services/loai-du-an.service';
import { OrganizeService } from '../../../@master-data/services/organize.service';
import { ConfigStructDto } from '../../../class/MD/config-struct.class';
import { TreeUtils } from '../../../services/utilities/tree.ultis';
import { ConfigStructService } from '../../../@master-data/services/config-struct.service';
import { AccountService } from '../../../@system-manager/services/account.service';
import { CapDuAnService } from '../../../@master-data/services/cap-du-an.service';
import { CustomerService } from '../../../@master-data/services/customer.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-list-project',
  imports: [NgModule],
  templateUrl: './list-project.html',
  styleUrl: './list-project.scss'
})
export class ListProject implements OnInit, OnDestroy {

  private destroy$ = new Subject<void>();
  visible: boolean = false;

  checked: boolean = false;
  indeterminate: boolean = false;
  setOfCheckedId = new Set<any>();

  data: PaginationResult = new PaginationResult();
  dto: ProjectDto = new ProjectDto();
  filter: ProjectDto = new ProjectDto();

  loaiDuAn: any[] = []
  capDuAn: any[] = []
  lstOrganize: any[] = []
  lstAccount: any[] = []
  lstCustomer: any[] = []

  constructor(
    private global: GlobalService,
    private service: ProjectService,
    private message: NzMessageService,
    private _loaiDuAn: LoaiDuAnService,
    private _organize: OrganizeService,
    private _configStruct: ConfigStructService,
    private _account: AccountService,
    private _capDuAn: CapDuAnService,
    private _customer: CustomerService,
    private router: Router,
  ) {
    this.global.setBreadcrumb([
      {
        name: 'Danh sách dự án',
        path: 'project/list-project',
      },
    ]);
  }

  ngOnInit(): void {
    this.search();
    this.getMasterData();
  }

  getMasterData() {
    forkJoin({
      loaiDuAn: this._loaiDuAn.getAll(),
      lstOrganize: this._organize.getAll(),
      lstAccount: this._account.getAll(),
      capDuAn: this._capDuAn.getAll(),
      lstCustomer: this._customer.getAll()
    })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res: any) => {
          this.loaiDuAn = res.loaiDuAn;
          this.lstOrganize = res.lstOrganize;
          this.lstAccount = res.lstAccount;
          this.capDuAn = res.capDuAn;
          this.lstCustomer = res.lstCustomer;
        }
      });
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

  refreshCheckedStatus(): void {
    this.checked = this.data.data.every((i: any) => this.setOfCheckedId.has(i.id));
    this.indeterminate = this.data.data.some((i: any) => this.setOfCheckedId.has(i.id)) && !this.checked;
  }

  updateCheckedSet(id: any, checked: boolean): void {
    if (checked) {
      this.setOfCheckedId.add(id);
    } else {
      this.setOfCheckedId.delete(id);
    }
  }

  onItemChecked(id: any, checked: boolean): void {
    this.updateCheckedSet(id, checked);
    this.refreshCheckedStatus();
  }


  onAllChecked(checked: boolean): void {
    this.data.data
      .forEach((i: any) => this.updateCheckedSet(i.id, checked));
    this.refreshCheckedStatus();
  }

  trackById(index: number, item: any): any {
    return item.id || item.code;
  }

  reset() {
    this.filter = new ProjectDto();
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

  close() {
    this.visible = false;
    this.dto = new ProjectDto();
    this.filterConfigStruct = new ConfigStructDto();
    this.configStructTree = [];
    this.displayedConfigStructTree = [];
  }

  openCreate() {
    this.visible = true;
  }

  save() {
    this.service.insert(this.dto).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res: any) => {
        if (res.status) {
          this.openDetailProject(res.data)
        }
      }
    })
  }

  filterConfigStruct: ConfigStructDto = new ConfigStructDto();
  configStructTree: any[] = [];
  displayedConfigStructTree: any[] = [];

  onChangeOrg(e: any) {
    this.filterConfigStruct.orgId = e;
    this.getConfigStructs();
  }

  getConfigStructs() {
    this._configStruct.search(this.filterConfigStruct).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res: any) => {
        this.configStructTree = this.displayedConfigStructTree = TreeUtils.buildNzConfigStructTree(res.data);
      }
    });
  }

  openDetailProject(projectId: any) {
    this.router.navigate([`/project/${projectId}`]);
  }
}

