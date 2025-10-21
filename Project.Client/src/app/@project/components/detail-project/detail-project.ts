import { Component, OnInit } from '@angular/core';
import { NgModule } from '../../../shared/ng-zorro.module';
import { DashboardProject } from '../dashboard-project/dashboard-project';
import { InfoProject } from '../info-project/info-project';
import { PersonProject } from '../person-project/person-project';
import { WorkflowProject } from '../workflow-project/workflow-project';
import { StructProject } from '../struct-project/struct-project';
import { Subject } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { GlobalService } from '../../../services/common/global.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-detail-project',
  imports: [DashboardProject, InfoProject, PersonProject, WorkflowProject, StructProject, NgModule, CommonModule],
  templateUrl: './detail-project.html',
  styleUrls: ['../../project.scss']
})
export class DetailProject implements OnInit {
  private destroy$ = new Subject<void>();
  projectId: string = '';
  indexTabProject: number = 1;

  constructor(
    private route: ActivatedRoute,
    private global: GlobalService
  ) { }

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get('projectId') ?? '';
  }

  onTabChange(index: number): void {
    this.indexTabProject = index;
  }

  ngOnDestroy(): void {
    this.global.setBreadcrumb([]);
    this.destroy$.next();
    this.destroy$.complete();
  }
}