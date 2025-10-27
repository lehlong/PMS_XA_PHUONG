import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { GlobalService } from '../../../services/common/global.service';
import { ProjectWorkflowProcessingService } from '../../services/project-workflow-processing.service';
import { NgModule } from '../../../shared/ng-zorro.module';
import { WorkflowProjectAction } from '../../../shared/statics/workflow-action.static';
import { ProjectPersonService } from '../../services/project-person.service';

@Component({
  selector: 'app-workflow-project',
  imports: [NgModule],
  templateUrl: './workflow-project.html',
  styleUrls: ['../../project.scss']
})
export class WorkflowProject implements OnInit {
  private destroy$ = new Subject<void>();
  projectId: string = '';
  lstSteps: any[] = [];
  workflowProjectAction = WorkflowProjectAction;
  lstAction = WorkflowProjectAction.getList();
  lstProjectLevel: any[] = [];
  lstOrganize: any[] = [];
  personnel: any[] = [];

  constructor(
    private route: ActivatedRoute,
    private global: GlobalService,
    private service: ProjectWorkflowProcessingService,
    private projectPersonService: ProjectPersonService,
  ) { }

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get('projectId') ?? '';
    this.getSteps();
    this.getProjectPerson();
  }

  getSteps() {
    this.service.getProjectWorkflowStep(this.projectId).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res: any) => {
        this.lstSteps = res
      }
    })
  }

  getProjectPerson(): void {
    this.projectPersonService.getProjectPerson(this.projectId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res: any) => {
          this.personnel = res;
        }
      });
  }

  update() {
    this.service.updateWorkflowProject(this.lstSteps).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res: any) => {
        this.getSteps();
      }
    })
  }

  startWorkflow() {
    this.service.startWorkflow(this.projectId).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res: any) => {
        this.getSteps();
      }
    })
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
