import { Injectable } from '@angular/core';
import { CommonService } from '../../services/common/common.service';
import { CurrencyDto } from '../../class/MD/currency.class';

@Injectable({
    providedIn: 'root',
})
export class CurrencyService {
    constructor(private common: CommonService) { }

    search(params: CurrencyDto) { return this.common.get('Currency/Search', params, false) }

    getAll() { return this.common.get('Currency/GetAll', {}, false) }

    getAllActive() { return this.common.get('Currency/GetAllActive', {}, false) }

    insert(data: CurrencyDto) { return this.common.post('Currency/Insert', data, false) }

    update(data: CurrencyDto) { return this.common.put('Currency/Update', data, false) }

    detail(data: string) { return this.common.get(`Currency/Detail/${data}`, {}, false) }

    delete(data: string) { return this.common.delete(`Currency/Delete/${data}`, {}, false) }

    updateOrder(data: any) { return this.common.put(`Currency/UpdateOrder`, data, false) }
}
