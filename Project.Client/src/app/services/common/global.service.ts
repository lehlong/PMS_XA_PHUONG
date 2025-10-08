import { Injectable } from "@angular/core";
import { Subject } from "rxjs";

@Injectable({ providedIn: 'root' })
export class GlobalService {
    breadcrumbSubject: Subject<boolean> = new Subject<boolean>()
    breadcrumb: any = []

    constructor() {
        this.breadcrumbSubject.subscribe((value) => {
            this.breadcrumb = value
        })
    }

    setBreadcrumb(value: any) {
        localStorage.setItem('breadcrumb', JSON.stringify(value))
        this.breadcrumbSubject.next(value)
    }

    getBreadcrumb() {
        try {
            if (this.breadcrumb && this.breadcrumb?.length > 0) {
                return this.breadcrumb
            }
            const breadcrumb = localStorage.getItem('breadcrumb')
            return breadcrumb ? JSON.parse(breadcrumb) : null
        } catch (e) {
            return null
        }
    }

}