import { Component, ElementRef, inject, ViewChild } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterOutlet } from '@angular/router';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NgModule } from '../../shared/ng-zorro.module';
import { AuthService } from '../services/auth.service';
import { LocalStorageUtils } from '../../services/utilities/local-storage.ultis';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [NgModule],
    templateUrl: './login.html',
    styleUrl: './login.scss'
})
export class Login {
    @ViewChild('loginFaceVideo', { static: false }) videoElement!: ElementRef<HTMLVideoElement>;

    stream: MediaStream | null = null;

    constructor(private service: AuthService, private router: Router) { }

    isPasswordVisible = false;
    activeTabIndex: number = 0;

    private fb = inject(NonNullableFormBuilder);

    validateForm = this.fb.group({
        username: this.fb.control('', [Validators.required]),
        password: this.fb.control('', [Validators.required])
    });

    togglePasswordVisibility(): void {
        this.isPasswordVisible = !this.isPasswordVisible;
    }

    async onTabChange(index: number) {
        this.activeTabIndex = index;

        if (index === 1) {
            await this.startCamera();
        } else {
            this.stopCamera();
        }
    }

    async startCamera() {
        try {
            if (!this.stream) {
                this.stream = await navigator.mediaDevices.getUserMedia({ video: true });
                if (this.videoElement?.nativeElement) {
                    this.videoElement.nativeElement.srcObject = this.stream;
                }
            }
        } catch (error) {
            console.error('Không thể truy cập webcam:', error);
        }
    }

    stopCamera() {
        if (this.stream) {
            this.stream.getTracks().forEach(track => track.stop());
            this.stream = null;
        }
    }

    submitForm(): void {
        if (this.validateForm.valid) {
            this.service.login(this.validateForm.value).subscribe({
                next: (res: any) => {
                    if (res.status) {
                        localStorage.setItem('accessToken', res.data.accessToken)
                        LocalStorageUtils.setItem('accountInfo', res.data.accountInfo);
                        this.router.navigate(['/'])
                    }
                },
                error: (err) => {
                    console.error(err)
                }
            })
        } else {
            Object.values(this.validateForm.controls).forEach(control => {
                if (control.invalid) {
                    control.markAsDirty();
                    control.updateValueAndValidity({ onlySelf: true });
                }
            });
        }
    }

    ngOnDestroy(): void {
        this.stopCamera();
    }
}
