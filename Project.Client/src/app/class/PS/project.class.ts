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
    startDate: any;
    endDate: any;
    khuVuc: string = '';
    diaDiem: string = '';
    trangThai: number = 0;
    giaiDoan: number = 0;
    notes: string = '';
    refrenceFileId: string = '';
    structs: any[] = []
    listGiaiDoan: any[] = []
    files: any[] = []
    personnel: any[] = []
}