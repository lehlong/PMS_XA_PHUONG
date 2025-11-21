import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { GlobalService } from '../../../services/common/global.service';
import { ProjectWorkflowProcessingService } from '../../services/project-workflow-processing.service';
import { NgModule } from '../../../shared/ng-zorro.module';
import { WorkflowProjectAction } from '../../../shared/statics/workflow-action.static';
import { ProjectPersonService } from '../../services/project-person.service';
import { ProjectStructService } from '../../services/project-struct.service';
import { ProjectStructDto } from '../../../class/PS/project-struct.class';

@Component({
  selector: 'app-workflow-project',
  imports: [NgModule],
  templateUrl: './workflow-project.html',
  styleUrls: ['../../project.scss']
})
export class WorkflowProject implements OnInit {
  @Output() showDetailWorkFolow = new EventEmitter<boolean>(true)
  private destroy$ = new Subject<void>();
  projectId: string = '';
  code: string = '';
  lstSteps: any[] = [];
  currentStep: any = {}
  workflowProjectAction = WorkflowProjectAction;
  projectStruct: ProjectStructDto = new ProjectStructDto();
  lstAction = WorkflowProjectAction.getList();
  lstProjectLevel: any[] = [];
  lstOrganize: any[] = [];
  personnel: any[] = [];
  stepId: string = '';

  constructor(
    private route: ActivatedRoute,
    private global: GlobalService,
    private service: ProjectWorkflowProcessingService,
    private projectPersonService: ProjectPersonService,
    private projectStructService : ProjectStructService,
  ) { }

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get('projectId') ?? '';
    this.code = this.service.getCurrentProcessingCode() ?? ''; 
    this.getSteps();
    this.getCurrentStep(this.projectId,this.code);
    this.getProjectPerson();
  }

  
 getSteps() {
  if (this.projectId && this.code) { 
    this.service.getProjectWorkflowStep(this.projectId, this.code)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res: any) => {
          this.lstSteps = res
          console.log(res)
        }
      })
  }
}
  getCurrentStep(projectId: string,code:string) {
    this.projectStructService.getCurrentStep(this.projectId,this.code).subscribe({
      next: (res) => {
        this.currentStep = res
        console.log(res)
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
    this.service.startWorkflow(this.projectId,this.code).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res: any) => {
        this.getSteps();
      }
    })
  }
  startTaskWorkflow() {
    this.service.startTaskWorkflow(this.projectId,this.code).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res: any) => {
        this.getSteps();
        this.getCurrentStep(this.projectId, this.code);
      }
    })
  }
  checkShowStartButton(): boolean {
  if (!this.lstSteps || this.lstSteps.length === 0) {
    return true; 
  }

  const hasStarted = this.lstSteps.some(step => step.isProcessing || step.isDone);
  return !hasStarted;
}
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();

    this.showDetailWorkFolow.emit(false);
  }

  goBack(): void{
    this.showDetailWorkFolow.emit(false);
  }
  
   trinhDuyet() {
    this.projectStructService.trinhDuyet(this.code).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res) => {
        this.ngOnInit();
      }
    })
  }

  xacNhan() {
    this.projectStructService.xacNhan(this.code).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res) => {
        this.ngOnInit();
      }
    })
  }

  pheDuyet() {
    this.projectStructService.pheDuyet(this.code).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res) => {
        this.ngOnInit();
      }
    })
  }

  tuChoi() {
    this.projectStructService.tuChoi(this.code).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res) => {
        this.ngOnInit();
      }
    })
  }

  yeuCauChinhSua() {
    this.projectStructService.yeuCauChinhSua(this.code).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res) => {
        this.ngOnInit();
      }
    })
  }
    checkActionCurrent(action: number): boolean {
    return !!this.currentStep && this.currentStep.listActions?.includes(action);
  }
}
