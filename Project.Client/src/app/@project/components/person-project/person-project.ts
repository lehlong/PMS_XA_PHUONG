import { Component, OnInit } from '@angular/core';
import { NgModule } from '../../../shared/ng-zorro.module';
import { Subject } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { GlobalService } from '../../../services/common/global.service';

@Component({
  selector: 'app-person-project',
  imports: [NgModule],
  templateUrl: './person-project.html',
  styleUrls: ['../../project.scss']
})
export class PersonProject implements OnInit {
  private destroy$ = new Subject<void>();
  projectId: string = '';
  personnel: any[] = [];

  constructor(
    private route: ActivatedRoute,
    private global: GlobalService
  ) { }

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get('projectId') ?? '';
  }

  ngOnDestroy(): void {
    this.global.setBreadcrumb([]);
    this.destroy$.next();
    this.destroy$.complete();
  }
}
