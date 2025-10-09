import { Injectable } from '@angular/core';
import { CommonService } from '../../services/common/common.service';
import { UnitDto } from '../../class/MD/unit.class';

@Injectable({
    providedIn: 'root',
})
export class UnitService {
    constructor(private common: CommonService) { }

    search(params: UnitDto) { return this.common.get('Unit/Search', params, false) }

    getAll() { return this.common.get('Unit/GetAll', {}, false) }

    getAllActive() { return this.common.get('Unit/GetAllActive', {}, false) }

    insert(data: UnitDto) { return this.common.post('Unit/Insert', data, false) }

    update(data: UnitDto) { return this.common.put('Unit/Update', data, false) }

    detail(data: string) { return this.common.get(`Unit/Detail/${data}`, {}, false) }

    delete(data: string) { return this.common.delete(`Unit/Delete/${data}`, {}, false) }

    updateOrder(data: any) { return this.common.put(`Unit/UpdateOrder`, data, false) }
}
