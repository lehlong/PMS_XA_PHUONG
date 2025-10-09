import { Injectable } from '@angular/core';
import { CommonService } from '../../services/common/common.service';
import { AreaDto } from '../../class/MD/area.class';

@Injectable({
    providedIn: 'root',
})
export class AreaService {
    constructor(private common: CommonService) { }

    search(params: AreaDto) { return this.common.get('Area/Search', params, false) }

    getAll() { return this.common.get('Area/GetAll', {}, false) }

    getAllActive() { return this.common.get('Area/GetAllActive', {}, false) }

    insert(data: AreaDto) { return this.common.post('Area/Insert', data, false) }

    update(data: AreaDto) { return this.common.put('Area/Update', data, false) }

    detail(data: string) { return this.common.get(`Area/Detail/${data}`, {}, false) }

    delete(data: string) { return this.common.delete(`Area/Delete/${data}`, {}, false) }

    updateOrder(data: any) { return this.common.put(`Area/UpdateOrder`, data, false) }
}
