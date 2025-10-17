import { Component, OnInit } from '@angular/core';
import { ProjectDto } from '../../../class/PS/project.class';
import { ActivatedRoute, Router } from '@angular/router';
import { ProjectService } from '../../services/project.service';
import { GlobalService } from '../../../services/common/global.service';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import { NgModule } from '../../../shared/ng-zorro.module';
import { CapDuAnService } from '../../../@master-data/services/cap-du-an.service';
import { ConfigStructService } from '../../../@master-data/services/config-struct.service';
import { CustomerService } from '../../../@master-data/services/customer.service';
import { LoaiDuAnService } from '../../../@master-data/services/loai-du-an.service';
import { OrganizeService } from '../../../@master-data/services/organize.service';
import { AccountService } from '../../../@system-manager/services/account.service';
import { TreeUtils } from '../../../services/utilities/tree.ultis';
import { ProjectStructType } from '../../../shared/statics/project-struct-type.static';
import { FileService } from '../../../services/common/file.service';

@Component({
  selector: 'app-info-project',
  imports: [NgModule],
  templateUrl: './info-project.html',
  styleUrl: './info-project.scss'
})
export class InfoProject implements OnInit {

  private destroy$ = new Subject<void>();
  indexTabProject: number = 1
  projectId: string = '';
  project: ProjectDto = new ProjectDto();
  giaiDoanHienTai: any = {}

  loaiDuAn: any[] = []
  capDuAn: any[] = []
  lstOrganize: any[] = []
  lstAccount: any[] = []
  lstCustomer: any[] = []

  projectStructType = ProjectStructType

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private service: ProjectService,
    private global: GlobalService,
    private _loaiDuAn: LoaiDuAnService,
    private _organize: OrganizeService,
    private _configStruct: ConfigStructService,
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
        this.listOfMapDataStruct = TreeUtils.buildNzPrjectTree(this.project.struct);
        this.listOfMapDataStruct.forEach(i => {
          this.mapOfExpandedData[i.id] = this.convertTreeToList(i);
        });
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

  // Xử lý cây cấu trúc
  listOfMapDataStruct: any[] = [];
  mapOfExpandedData: { [id: string]: any[] } = {};

  checkedStruct: boolean = false;
  indeterminateStruct: boolean = false;
  setOfCheckedIdStruct = new Set<any>();

  refreshCheckedStatusStruct(): void {
    this.checkedStruct = this.project.struct.every((i: any) => this.setOfCheckedIdStruct.has(i.id));
    this.indeterminateStruct = this.project.struct.some((i: any) => this.setOfCheckedIdStruct.has(i.id)) && !this.checkedStruct;
  }

  updateCheckedSetStruct(id: any, checked: boolean): void {
    if (checked) {
      this.setOfCheckedIdStruct.add(id);
    } else {
      this.setOfCheckedIdStruct.delete(id);
    }
  }

  onItemCheckedStruct(id: any, checked: boolean): void {
    this.updateCheckedSetStruct(id, checked);
    this.refreshCheckedStatusStruct();
  }


  onAllCheckedStruct(checked: boolean): void {
    this.project.struct
      .forEach((i: any) => this.updateCheckedSetStruct(i.id, checked));
    this.refreshCheckedStatusStruct();
  }

  collapse(array: any[], data: any, $event: boolean): void {
    if (!$event) {
      if (data.children) {
        data.children.forEach((d: any) => {
          const target = array.find(a => a.id === d.id)!;
          target.expand = false;
          this.collapse(array, target, false);
        });
      } else {
        return;
      }
    }
  }

  convertTreeToList(root: any): any[] {
    const stack: any[] = [];
    const array: any[] = [];
    const hashMap = {};
    stack.push({ ...root, level: 0, expand: false });

    while (stack.length !== 0) {
      const node = stack.pop()!;
      this.visitNode(node, hashMap, array);
      if (node.children) {
        for (let i = node.children.length - 1; i >= 0; i--) {
          stack.push({ ...node.children[i], level: node.level! + 1, expand: false, parent: node });
        }
      }
    }

    return array;
  }

  visitNode(node: any, hashMap: { [id: string]: boolean }, array: any[]): void {
    if (!hashMap[node.id]) {
      hashMap[node.id] = true;
      array.push(node);
    }
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

  visibleHangMuc: boolean = false;
  visibleCongViec: boolean = false;

  titleHangMuc: string = 'TẠO MỚI CÔNG VIỆC'
  titleCongViec: string = 'THÔNG TIN CÔNG VIỆC'

  closeDrawerHangMuc() {
    this.visibleHangMuc = false;
  }

  closeDrawerCongViec() {
    this.visibleCongViec = false;
  }

  openDrawerHangMuc(data: any) {
    this.visibleHangMuc = true;
  }

  openDrawerCongViec(data: any) {
    this.titleCongViec = 'CÔNG VIỆC: ' + data.name;
    this.visibleCongViec = true;
  }

}
