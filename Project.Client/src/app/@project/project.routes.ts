import { Routes } from "@angular/router";
import { ListProject } from "./components/list-project/list-project";
import { DetailProject } from "./components/detail-project/detail-project";

export const projectRoutes: Routes = [
    { path: 'list-project', component: ListProject },
    { path: ':projectId', component: DetailProject }
]