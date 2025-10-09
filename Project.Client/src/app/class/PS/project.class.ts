import { BaseFilter } from "../common/base-filter.class";

export class ProjectDto extends BaseFilter {
    id: string = '';
    code: string = '';
    name: string = '';
    donViPhuTrach: string = '';
    lanhDaoPhuTrach: string = '';
    loaiDuAn: string = '';
    pmDuAn: string = '';
    capDuAn: string = '';
    phuTrachDuAn: string = '';
    quanLyHopDong: string = '';
    khachHang: string = '';
    startDate : any;
    endDate: any;
    khuVuc: string ='';
    diaDiem: string = '';
    trangThai: string = '';
    giaiDoan: string = '';
    notes: string = '';
    refrenceFileId: string = '';
}