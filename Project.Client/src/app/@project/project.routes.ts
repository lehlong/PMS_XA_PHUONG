import { Routes } from "@angular/router";
import { ListProject } from "./components/list-project/list-project";
import { InfoProject } from "./components/info-project/info-project";

export const projectRoutes: Routes = [
    { path: 'list-project', component: ListProject },
    { path: ':projectId', component: InfoProject }
]