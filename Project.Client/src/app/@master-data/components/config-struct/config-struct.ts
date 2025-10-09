import { Component, OnInit, ViewChild } from '@angular/core';
import { NzTreeComponent } from 'ng-zorro-antd/tree';
import { Subject, takeUntil } from 'rxjs';
import { ConfigStructDto } from '../../../class/MD/config-struct.class';
import { GlobalService } from '../../../services/common/global.service';
import { TreeUtils } from '../../../services/utilities/tree.ultis';
import { ConfigStructService } from '../../services/config-struct.service';
import { NgModule } from '../../../shared/ng-zorro.module';
import { OrganizeService } from '../../services/organize.service';
import { ProjectStructType } from '../../../shared/statics/project-struct-type.static';

@Component({
  selector: 'app-config-struct',
  imports: [NgModule],
  templateUrl: './config-struct.html',
  styleUrl: './config-struct.scss'
})
export class ConfigStruct implements OnInit {

  @ViewChild('treeCom', { static: false }) treeCom!: NzTreeComponent
  private destroy$ = new Subject<void>();

  visible: boolean = false;
  isEdit: boolean = false;

  searchConfigStruct: string = '';
  ConfigStructTree: any[] = [];
  displayedConfigStructTree: any[] = [];

  dto: ConfigStructDto = new ConfigStructDto();
  filter: ConfigStructDto = new ConfigStructDto();

  lstOrganize : any[] = [];
  projectStructType = ProjectStructType

  constructor(
    private global: GlobalService,
    private service: ConfigStructService,
    private organize : OrganizeService,
  ) {
    this.global.setBreadcrumb([
      {
        name: 'Cấu hình thư mục',
        path: 'master-data/config-struct',
      },
    ]);
  }

  ngOnInit(): void {
    this.getOrganize();
    setTimeout(() => {
      this.onChangeOrg(this.lstOrganize[0].id)
    }, 200)
  }

  getConfigStructs() {
    this.service.search(this.filter).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res: any) => {
        this.ConfigStructTree = this.displayedConfigStructTree = TreeUtils.buildNzConfigStructTree(res.data);
      }
    });
  }

  getOrganize(){
    this.organize.getAll().pipe(takeUntil(this.destroy$)).subscribe({
      next: (res : any) => {
        this.lstOrganize = res;
      }
    })
  }

  nzEvent(event: any): void {

  }

  reset() {
    this.searchConfigStruct = '';
    this.getConfigStructs();
  }

  detail(data: any) {
    this.isEdit = true;
    this.service.detail(data.id).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res: any) => {
        this.dto = res;
        this.visible = true;
      }
    })
  }

  create(data: any, type : any) {
    this.isEdit = false;
    this.dto.pId = data;
    this.dto.orgId = this.filter.orgId
    this.dto.type = type;
    this.visible = true;
  }

  updateOrder() {
    const flatList = this.getCurrentTreeOrderFlat();
    this.service.updateOrder(flatList).subscribe({
      next: (res) => {
        this.globalOrderNumber = 1;
        this.getConfigStructs();
      },
      error: (response) => {
        console.log(response)
      },
    })
  }

  private getCurrentTreeOrderFlat(): any[] {
    const treeNodes = this.treeCom.getTreeNodes();
    const flatList: any[] = [];
    this.flattenNodeWithOrderAndPid(treeNodes, 'STRUCT', flatList);
    return flatList;
  }

  private globalOrderNumber = 1;

  private flattenNodeWithOrderAndPid(nodes: any[], parentId: string, result: any[]): void {
    nodes.forEach((node) => {
      result.push({
        id: node.origin.id,
        pId: parentId,
        code : node.origin.code,
        orgId : node.origin.orgId,
        type : node.origin.type,
        name: node.origin.name,
        orderNumber: this.globalOrderNumber++,
        expanded: node.origin.expanded,
      });

      if (node.children && node.children.length > 0) {
        this.flattenNodeWithOrderAndPid(node.children, node.origin.id, result);
      }
    });
  }

  onSearchChangeConfigStruct() {
    if (!this.searchConfigStruct?.trim()) {
      this.displayedConfigStructTree = this.ConfigStructTree;
    } else {
      this.displayedConfigStructTree = this.filterConfigStructTree(this.ConfigStructTree, this.searchConfigStruct.toLowerCase());
    }
  }

  filterConfigStructTree(ConfigStructTree: any[], keyword: string): any[] {
    const result: any[] = [];

    for (const node of ConfigStructTree) {
      const childrenMatched = node.children ? this.filterConfigStructTree(node.children, keyword) : [];

      const nameMatched = node.name.toLowerCase().includes(keyword);

      if (nameMatched || childrenMatched.length > 0) {
        result.push({
          ...node,
          children: childrenMatched.length > 0 ? childrenMatched : node.children ? [] : null,
          open: childrenMatched.length > 0 || node.open
        });
      }
    }

    return result;
  }

  deleteConfigStruct(id: any) {
    this.service.delete(id).subscribe({
      next: (res) => {
        this.getConfigStructs();
      }
    })
  }

  save() {
    const action = this.isEdit ? 'update' : 'insert';
    this.service[action](this.dto)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          var pId = this.dto.pId;
          var orgId = this.dto.orgId;
          var type = this.dto.type;
          this.getConfigStructs();
          if (!this.isEdit) {
            this.dto = new ConfigStructDto();
            this.dto.pId = pId;
            this.dto.orgId = orgId;
            this.dto.type = type;
          } else {
            this.visible = false;
          }
        }
      })
  }

  close() {
    this.visible = false;
    this.dto = new ConfigStructDto();
  }

  onChangeOrg(e: any){
    this.filter.orgId = e;
    this.getConfigStructs();
  }

  ngOnDestroy(): void {
    this.global.setBreadcrumb([]);
    this.destroy$.next();
    this.destroy$.complete();
    this.ConfigStructTree = [];
    this.displayedConfigStructTree = [];
    this.dto = new ConfigStructDto();
    this.visible = false;
    this.isEdit = false;
    this.searchConfigStruct = '';
    this.treeCom = null as any;
  }

  getNodeLevel(node: any): number {
    let level = 0;
    let current = node;
    while (current.getParentNode()) {
      current = current.getParentNode();
      level++;
    }
    return level;
  }
}

