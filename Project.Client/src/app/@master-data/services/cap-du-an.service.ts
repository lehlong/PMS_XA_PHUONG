import { Injectable } from '@angular/core';
import { CommonService } from '../../services/common/common.service';
import { CapDuAnDto } from '../../class/MD/cap-du-an.class';

@Injectable({
    providedIn: 'root',
})
export class CapDuAnService {
    constructor(private common: CommonService) { }

    search(params: CapDuAnDto) { return this.common.get('CapDuAn/Search', params, false) }

    getAll() { return this.common.get('CapDuAn/GetAll', {}, false) }

    getAllActive() { return this.common.get('CapDuAn/GetAllActive', {}, false) }

    insert(data: CapDuAnDto) { return this.common.post('CapDuAn/Insert', data, false) }

    update(data: CapDuAnDto) { return this.common.put('CapDuAn/Update', data, false) }

    detail(data: string) { return this.common.get(`CapDuAn/Detail/${data}`, {}, false) }

    delete(data: string) { return this.common.delete(`CapDuAn/Delete/${data}`, {}, false) }

    updateOrder(data: any) { return this.common.put(`CapDuAn/UpdateOrder`, data, false) }
}
