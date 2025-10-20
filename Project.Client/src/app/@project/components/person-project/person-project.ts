import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgModule } from '../../../shared/ng-zorro.module';
import { Subject, takeUntil } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { GlobalService } from '../../../services/common/global.service';
import { AccountService } from '../../../@system-manager/services/account.service';
import { ProjectPersonService } from '../../services/project-person.service';
import { ProjectRoleService } from '../../../@master-data/services/project-role.service';
import { NzMessageService } from 'ng-zorro-antd/message';

@Component({
  selector: 'app-person-project',
  imports: [NgModule],
  templateUrl: './person-project.html',
  styleUrls: ['../../project.scss']
})
export class PersonProject implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  projectId = '';
  visible = false;
  personnel: any[] = [];
  lstAccount: any[] = [];
  lstProjectRole: any[] = [];

  checked = false;
  indeterminate = false;
  setOfCheckedId = new Set<any>();

  checkedUpdate = false;
  indeterminateUpdate = false;
  setOfCheckedIdUpdate = new Set<any>();

  tempCheckedUser = new Set<any>();

  constructor(
    private route: ActivatedRoute,
    private message: NzMessageService,
    private global: GlobalService,
    private service: ProjectPersonService,
    private accountService: AccountService,
    private projectRoleService: ProjectRoleService
  ) { }

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get('projectId') ?? '';
    this.getProjectRole();
    this.getProjectPerson();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  getProjectPerson(): void {
    this.tempCheckedUser.clear();
    this.service.getProjectPerson(this.projectId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res: any) => {
          this.personnel = res;
          res.forEach((i: any) => this.tempCheckedUser.add(i.userName));
        }
      });
  }

  getAllAccount(): void {
    this.accountService.getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({ next: (res: any) => this.lstAccount = res });
  }

  getProjectRole(): void {
    this.projectRoleService.getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({ next: (res: any) => this.lstProjectRole = res });
  }

  open(): void {
    this.getAllAccount();
    this.visible = true;
  }

  close(): void {
    this.lstAccount = [];
    this.setOfCheckedIdUpdate.clear();
    this.visible = false;
  }

  update(): void {
    this.service.updateInfoProjectPerson(this.personnel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({ next: () => this.getProjectPerson() });
  }

  add(): void {
    const adds = Array.from(this.setOfCheckedIdUpdate)
      .filter(x => !this.tempCheckedUser.has(x));
    
    if (adds.length === 0) return;

    const request = adds.map(userName => ({ 
      userName, 
      projectId: this.projectId 
    }));

    this.service.updateProjectPerson(request)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.getProjectPerson();
          this.close();
        }
      });
  }

  delete(): void {
    if (this.setOfCheckedId.size === 0) {
      this.message.error('Vui lòng chọn ít nhất 1 bản ghi!');
      return;
    }

    this.service.deleteProjectPerson([...this.setOfCheckedId])
      .pipe(takeUntil(this.destroy$))
      .subscribe({ next: () => this.getProjectPerson() });
  }

  private refreshCheckedStatus(
    list: any[], 
    checkedSet: Set<any>, 
    key: string = 'id'
  ): { checked: boolean; indeterminate: boolean } {
    const checked = list.every(i => checkedSet.has(i[key]));
    const indeterminate = list.some(i => checkedSet.has(i[key])) && !checked;
    return { checked, indeterminate };
  }

  private updateCheckedSet(set: Set<any>, id: any, checked: boolean): void {
    checked ? set.add(id) : set.delete(id);
  }

  onItemChecked(id: any, checked: boolean): void {
    this.updateCheckedSet(this.setOfCheckedId, id, checked);
    const status = this.refreshCheckedStatus(this.personnel, this.setOfCheckedId);
    this.checked = status.checked;
    this.indeterminate = status.indeterminate;
  }

  onAllChecked(checked: boolean): void {
    this.personnel.forEach(i => this.updateCheckedSet(this.setOfCheckedId, i.id, checked));
    const status = this.refreshCheckedStatus(this.personnel, this.setOfCheckedId);
    this.checked = status.checked;
    this.indeterminate = status.indeterminate;
  }

  onItemCheckedUpdate(userName: any, checked: boolean): void {
    this.updateCheckedSet(this.setOfCheckedIdUpdate, userName, checked);
    const status = this.refreshCheckedStatus(this.lstAccount, this.setOfCheckedIdUpdate, 'userName');
    this.checkedUpdate = status.checked;
    this.indeterminateUpdate = status.indeterminate;
  }

  onAllCheckedUpdate(checked: boolean): void {
    this.lstAccount.forEach(i => this.updateCheckedSet(this.setOfCheckedIdUpdate, i.userName, checked));
    const status = this.refreshCheckedStatus(this.lstAccount, this.setOfCheckedIdUpdate, 'userName');
    this.checkedUpdate = status.checked;
    this.indeterminateUpdate = status.indeterminate;
  }
}