import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subject } from 'rxjs';
import { GlobalService } from '../../../services/common/global.service';

@Component({
  selector: 'app-dashboard-project',
  imports: [],
  templateUrl: './dashboard-project.html',
  styleUrls: ['../../project.scss']
})
export class DashboardProject implements OnInit {
  private destroy$ = new Subject<void>();
  projectId: string = '';

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
