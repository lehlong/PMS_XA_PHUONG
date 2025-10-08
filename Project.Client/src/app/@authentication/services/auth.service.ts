import { Injectable } from '@angular/core';
import { CommonService } from '../../services/common/common.service';

@Injectable({
    providedIn: 'root',
})
export class AuthService {
    constructor(private common: CommonService) { }

    login(data: any) { return this.common.post('Auth/Login', data) }
}
