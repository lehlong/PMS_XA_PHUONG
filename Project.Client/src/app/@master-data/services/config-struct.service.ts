import { Injectable } from '@angular/core';
import { CommonService } from '../../services/common/common.service';
import { ConfigStructDto } from '../../class/MD/config-struct.class';

@Injectable({
    providedIn: 'root',
})
export class ConfigStructService {
    constructor(private common: CommonService) { }

    search(params: ConfigStructDto) { return this.common.get('ConfigStruct/Search', params, false) }

    getAll() { return this.common.get('ConfigStruct/GetAll', {}, false) }

    getAllActive() { return this.common.get('ConfigStruct/GetAllActive', {}, false) }

    insert(data: ConfigStructDto) { return this.common.post('ConfigStruct/Insert', data, false) }

    update(data: ConfigStructDto) { return this.common.put('ConfigStruct/Update', data, false) }

    detail(data: string) { return this.common.get(`ConfigStruct/Detail/${data}`, {}, false) }

    delete(data: string) { return this.common.delete(`ConfigStruct/Delete/${data}`, {}, false) }

    updateOrder(data: any) { return this.common.put(`ConfigStruct/UpdateOrder`, data, false) }
}
