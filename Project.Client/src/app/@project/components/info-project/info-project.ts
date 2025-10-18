import { Component, OnInit } from '@angular/core';
import { ProjectDto } from '../../../class/PS/project.class';
import { ActivatedRoute, Router } from '@angular/router';
import { ProjectService } from '../../services/project.service';
import { GlobalService } from '../../../services/common/global.service';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import { NgModule } from '../../../shared/ng-zorro.module';
import { CapDuAnService } from '../../../@master-data/services/cap-du-an.service';
import { CustomerService } from '../../../@master-data/services/customer.service';
import { LoaiDuAnService } from '../../../@master-data/services/loai-du-an.service';
import { OrganizeService } from '../../../@master-data/services/organize.service';
import { AccountService } from '../../../@system-manager/services/account.service';
import { FileService } from '../../../services/common/file.service';

@Component({
  selector: 'app-info-project',
  imports: [NgModule],
  templateUrl: './info-project.html',
  styleUrls: ['../../project.scss']
})
export class InfoProject implements OnInit {
  private destroy$ = new Subject<void>();
  projectId: string = '';
  project: ProjectDto = new ProjectDto();
  giaiDoanHienTai: any = {}

  loaiDuAn: any[] = []
  capDuAn: any[] = []
  lstOrganize: any[] = []
  lstAccount: any[] = []
  lstCustomer: any[] = []

  constructor(
    private route: ActivatedRoute,
    private service: ProjectService,
    private global: GlobalService,
    private _loaiDuAn: LoaiDuAnService,
    private _organize: OrganizeService,
    private _account: AccountService,
    private _capDuAn: CapDuAnService,
    private _customer: CustomerService,
    private _file: FileService
  ) { }

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get('projectId') ?? '';

    this.service.detail(this.projectId).subscribe({
      next: (res: any) => {
        this.project = res;
        this.giaiDoanHienTai = this.project.listGiaiDoan[this.project.giaiDoan];
        this.global.setBreadcrumb([
          { name: 'Danh sách dự án', path: 'project/list-project' },
          { name: this.project.name, path: 'project/' + this.project.id },
        ]);
      }
    });
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

  chuyenTiepGiaiDoan() {
    this.project.giaiDoan += 1;
    this.service.update(this.project).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res) => {
        this.ngOnInit();
      }
    })
  }

  ngOnDestroy(): void {
    this.global.setBreadcrumb([]);
    this.destroy$.next();
    this.destroy$.complete();
  }

  upload(e: any) {
    const input = e.target as HTMLInputElement;
    const files = input.files;
    if (!files?.length) return;
    const formData = new FormData();
    if (files?.length) {
      Array.from(files).forEach((file) => formData.append('files', file));
    }
    this._file.upload(formData).subscribe({
      next: (res: any) => {
        this.project.files = [...this.project.files, ...res.data]
      }
    })
  }
}
