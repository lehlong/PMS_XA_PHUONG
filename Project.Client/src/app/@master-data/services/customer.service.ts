import { Injectable } from '@angular/core';
import { CommonService } from '../../services/common/common.service';
import { CustomerDto } from '../../class/MD/customer.class';

@Injectable({
    providedIn: 'root',
})
export class CustomerService {
    constructor(private common: CommonService) { }

    search(params: CustomerDto) { return this.common.get('Customer/Search', params, false) }

    getAll() { return this.common.get('Customer/GetAll', {}, false) }

    getAllActive() { return this.common.get('Customer/GetAllActive', {}, false) }

    insert(data: CustomerDto) { return this.common.post('Customer/Insert', data, false) }

    update(data: CustomerDto) { return this.common.put('Customer/Update', data, false) }

    detail(data: string) { return this.common.get(`Customer/Detail/${data}`, {}, false) }

    delete(data: string) { return this.common.delete(`Customer/Delete/${data}`, {}, false) }

    updateOrder(data: any) { return this.common.put(`Customer/UpdateOrder`, data, false) }
}
