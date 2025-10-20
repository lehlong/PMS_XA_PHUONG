import { Routes } from "@angular/router";
import { ListProject } from "./components/list-project/list-project";
import { DetailProject } from "./components/detail-project/detail-project";
import { TaskProject } from "./components/task-project/task-project"
import { TaskPersonal } from "./components/task-personal/task-personal"

export const projectRoutes: Routes = [
    { path: 'list-project', component: ListProject },
    { path: 'detail/:projectId', component: DetailProject },
    { path: 'task-project', component: TaskProject },
    { path: 'task-personal', component: TaskPersonal },
]