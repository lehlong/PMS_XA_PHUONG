import { Injectable } from '@angular/core';
import { CommonService } from '../../services/common/common.service';
import { LoaiDuAnDto } from '../../class/MD/loai-du-an.class';

@Injectable({
    providedIn: 'root',
})
export class LoaiDuAnService {
    constructor(private common: CommonService) { }

    search(params: LoaiDuAnDto) { return this.common.get('LoaiDuAn/Search', params, false) }

    getAll() { return this.common.get('LoaiDuAn/GetAll', {}, false) }

    getAllActive() { return this.common.get('LoaiDuAn/GetAllActive', {}, false) }

    insert(data: LoaiDuAnDto) { return this.common.post('LoaiDuAn/Insert', data, false) }

    update(data: LoaiDuAnDto) { return this.common.put('LoaiDuAn/Update', data, false) }

    detail(data: string) { return this.common.get(`LoaiDuAn/Detail/${data}`, {}, false) }

    delete(data: string) { return this.common.delete(`LoaiDuAn/Delete/${data}`, {}, false) }

    updateOrder(data: any) { return this.common.put(`LoaiDuAn/UpdateOrder`, data, false) }
}
