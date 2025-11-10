import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { AbstractControl, NgModel } from '@angular/forms';

@Component({
  selector: 'app-error-message',
  imports: [
    CommonModule
  ],
  templateUrl: './error-message.html',
  styleUrl: './error-message.scss'
})
export class ErrorMessage implements OnInit {
  @Input() control?: AbstractControl | NgModel | null;
  @Input() messages: { [key: string]: string } = {};
  @Input() submitted = false; // truyền từ cha khi click submit
  
  /**
   * 1. các loại message default đi kèm các loại validator 
   * 2. Cách dùng:
   * 2.1 Nếu dùng ReactiveFormModules => truyền validator bên ts và custom message theo cặp [{key: value}, {key: value}] 
   * 2.2 Nếu dùng ngModels thuần => truyền validators các key bên dưới cho vào bên input html => **hạn chế dùng binding dữ liệu thuần**
   */
  defaultMessages: { [key: string]: (error: any) => string } = {
    required: () => 'Trường này là bắt buộc',
    minlength: (e) => `Tối thiểu ${e.requiredLength} ký tự`,
    maxlength: (e) => `Tối đa ${e.requiredLength} ký tự`,
    min: (e) => `Giá trị nhỏ nhất là ${e.min}`,
    max: (e) => `Giá trị lớn nhất là ${e.max}`,
    email: () => 'Email không hợp lệ',
    pattern: () => 'Giá trị không đúng định dạng',
  };

  ngOnInit() {
    if (!this.control) {}
  }

  get showError(): boolean {
    if (!this.control) return false;
    const invalid = this.control.invalid;
    const touched = (this.control as any).touched;
    return invalid && (touched || this.submitted);
  }

  get firstErrorMessage(): string | null {
     if (!this.control?.errors) return null;
      const firstKey = Object.keys(this.control.errors)[0];
      const errorValue = this.control.errors[firstKey];

      // Ưu tiên message custom
      if (this.messages[firstKey]) return this.messages[firstKey];

      // Dùng message mặc định
      const defaultMsgFn = this.defaultMessages[firstKey];
      return defaultMsgFn ? defaultMsgFn(errorValue) : 'Giá trị không hợp lệ';
  }
}

/**
 * Cách gọi hàm (ngModel form #f="ngForm")
 *  <form #f="ngForm" (ngSubmit)="onSubmit(f)">
  *   <app-error-message
        [control]="email"
        [messages]="{
          required: 'Email không được để trống',
          email: 'Email không hợp lệ'
        }"
        [submitted]="submitted">
      </app-error-message>
    </form>
 */

  /**
   * <app-error-message
      [control]="form.get('email')"
      [messages]="{
        required: 'Email không được để trống',
        email: 'Email không hợp lệ'
      }"
      [submitted]="submitted">
    </app-error-message>
   */