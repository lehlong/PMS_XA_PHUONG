import { Routes } from "@angular/router";
import { Organize } from "./components/organize/organize";
import { LoaiDuAn } from "./components/loai-du-an/loai-du-an";
import { CapDuAn } from "./components/cap-du-an/cap-du-an";
import { Area } from "./components/area/area";
import { Customer } from "./components/customer/customer";
import { Unit } from "./components/unit/unit";
import { Currency } from "./components/currency/currency";
import { ConfigStruct } from "./components/config-struct/config-struct";
import { Title } from "./components/title/title";
import { ProjectRole } from "./components/project-role/project-role";

export const masterDataRoutes: Routes = [
  { path: 'organize', component: Organize },
  { path: 'project-type', component: LoaiDuAn },
  { path: 'project-level', component: CapDuAn },
  { path: 'area', component: Area },
  { path: 'customer', component: Customer },
  { path: 'unit', component: Unit },
  { path: 'currency', component: Currency },
  { path: 'config-struct', component: ConfigStruct },
  { path: 'title', component: Title },
  { path: 'project-role', component: ProjectRole },
]