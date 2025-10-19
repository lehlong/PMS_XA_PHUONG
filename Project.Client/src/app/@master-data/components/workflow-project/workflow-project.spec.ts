import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkflowProject } from './workflow-project';

describe('WorkflowProject', () => {
  let component: WorkflowProject;
  let fixture: ComponentFixture<WorkflowProject>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WorkflowProject]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WorkflowProject);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
