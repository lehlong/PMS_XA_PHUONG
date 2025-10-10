import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { Router } from '@angular/router';
import { GlobalService } from '../../services/common/global.service';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { DeepSeekService } from '../../services/chatbot-ai/deep-seek.service';
import { NgModule } from '../../shared/ng-zorro.module';
import { MainLayoutService } from '../../services/common/main-layout.service';
import { LocalStorageUtils } from '../../services/utilities/local-storage.ultis';
import { StringUtils } from '../../services/utilities/string.ultis';
import { TreeUtils } from '../../services/utilities/tree.ultis';

@Component({
  selector: 'app-main-layout',
  imports: [NgModule],
  standalone: true,
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.scss'
})
export class MainLayout implements OnInit, OnDestroy {
  isCollapsed: boolean = false;
  isVisibleAI: boolean = false;
  isVisibleChangePass: boolean = false;
  language: string = 'vi';
  breadcrumbs: any = [];

  chatAi: any[] = [];
  inputChatbot: string = '';

  modelChangePass: any = {
    userName: '',
    currentPassword: '',
    newPassword: '',
    confirmNewPassword: ''
  };

  searchMenu: string = '';
  menuTree: any[] = [];
  displayedMenuTree: any[] = [];

  accountInfo: any;

  private lastCheckedToken = 0;
  private checkIntervalToken = 60 * 1000;
  private destroy$ = new Subject<void>();

  constructor(
    private global: GlobalService,
    private service: MainLayoutService,
    private notification: NzNotificationService,
    private router: Router,
    private deepSeek: DeepSeekService
  ) {
    this.accountInfo = LocalStorageUtils.getItem('accountInfo')
  }

  fullscreenListener = () => {
    this.isCollapsed = !!document.fullscreenElement;
  };

  ngOnInit(): void {
    this.breadcrumbs = this.global.breadcrumb || [];

    this.global.breadcrumbSubject
      .pipe(takeUntil(this.destroy$))
      .subscribe(value => {
        this.breadcrumbs = value;
      });

    this.getAllMenu();
    this.listenToVisibilityChange();

    document.addEventListener('fullscreenchange', this.fullscreenListener);
  }

  getAllMenu() {
    this.service.getAllMenu()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res: any) => {
          this.menuTree = this.displayedMenuTree = TreeUtils.buildNzMenuTree(res);
        }
      });
  }

  onOpenChange(changedMenu: any): void {
    if (changedMenu.level !== 1) return;

    this.menuTree.forEach(menu => {
      if (menu !== changedMenu && menu.level === 1) {
        menu.open = false;
      }
    });

    changedMenu.open = true;
  }

  onSearchChangeMenu() {
    if (!this.searchMenu?.trim()) {
      this.displayedMenuTree = this.menuTree;
    } else {
      this.displayedMenuTree = this.filterMenuTree(this.menuTree, this.searchMenu.toLowerCase());
    }
  }

  filterMenuTree(menuTree: any[], keyword: string): any[] {
    const result: any[] = [];

    for (const node of menuTree) {
      const childrenMatched = node.children ? this.filterMenuTree(node.children, keyword) : [];

      const nameMatched = node.name.toLowerCase().includes(keyword);

      if (nameMatched || childrenMatched.length > 0) {
        result.push({
          ...node,
          children: childrenMatched.length > 0 ? childrenMatched : node.children ? [] : null,
          open: childrenMatched.length > 0 || node.open
        });
      }
    }

    return result;
  }

  openAI() {
    this.isVisibleAI = true;
  }

  closeAI() {
    this.isVisibleAI = false;
  }

  resetAI() {
    this.chatAi = [];
  }

  onAskChatbot() {
    this.chatAi.push({
      role: 'User',
      content: this.inputChatbot,
    });

    let aiMessage = {
      role: 'DeepSeek',
      content: ''
    };
    this.chatAi.push(aiMessage);

    this.deepSeek.sendMessage(this.inputChatbot)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (chunk) => {
          const formattedText = StringUtils.removeEmoji(chunk)
            .replaceAll('"', '')
            .replaceAll('[', '')
            .replaceAll(']', '')
            .replaceAll(',', '')
            .replace(/\\n/g, '<br>');

          aiMessage.content += formattedText;
        },
        error: (err) => {
          console.error('API error:', err);
        }
      });

    this.inputChatbot = '';
  }

  changeLanguage(language: string) {
    this.language = language;
  }

  navigateRoute(route: any) {
    this.router.navigate([route]);
  }

  changePassOpen() {
    this.resetModelChangePass();
    this.isVisibleChangePass = true;
  }

  changePassOk() {
    const { currentPassword, newPassword, confirmNewPassword } = this.modelChangePass;
    if (!currentPassword || !newPassword || !confirmNewPassword) {
      this.notification.error('Lỗi', 'Vui lòng nhập đầy đủ thông tin!');
      return;
    }

    if (newPassword !== confirmNewPassword) {
      this.notification.error('Lỗi', 'Mật khẩu xác nhận không giống với mật khẩu mới!');
      return;
    }

    let countdown = 5;
    const notiKey = 'countdown';

    this.notification.success(
      'Đổi mật khẩu thành công!',
      `Hệ thống sẽ khởi động lại sau ${countdown}s!`,
      {
        nzKey: notiKey,
        nzDuration: 0
      }
    );

    const timer = setInterval(() => {
      countdown--;
      this.notification.success(
        'Đổi mật khẩu thành công!',
        `Hệ thống sẽ khởi động lại sau ${countdown}s!`,
        {
          nzKey: notiKey,
          nzDuration: 0
        }
      );

      if (countdown === 0) {
        clearInterval(timer);
        this.notification.remove(notiKey);
        this.logOut();
      }
    }, 1000);
  }

  changePassCancel() {
    this.resetModelChangePass();
    this.isVisibleChangePass = false;
  }

  resetModelChangePass() {
    this.modelChangePass = {
      userName: '',
      currentPassword: '',
      newPassword: '',
      confirmNewPassword: ''
    };
  }

  logOut() {
    LocalStorageUtils.clear();
    this.navigateRoute('login');
  }

  @HostListener('document:mousemove')
  @HostListener('document:keydown')
  @HostListener('document:click')
  onUserInteraction(): void {
    const now = Date.now();
    if (now - this.lastCheckedToken > this.checkIntervalToken) {
      this.checkToken();
      this.lastCheckedToken = now;
    }
  }

  private listenToVisibilityChange(): void {
    document.addEventListener('visibilitychange', this.onVisibilityChange);
  }

  private onVisibilityChange = () => {
    if (document.visibilityState === 'visible') {
      this.checkToken();
    }
  };

  private checkToken(): void {
    const token = localStorage.getItem('accessToken');
    if (!token || this.isTokenExpired(token)) {
      LocalStorageUtils.clear();
      this.router.navigate(['/un-authen']);
    }
  }

  private isTokenExpired(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return Date.now() > payload.exp * 1000;
    } catch (e) {
      return true;
    }
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();

    document.removeEventListener('fullscreenchange', this.fullscreenListener);
    document.removeEventListener('visibilitychange', this.onVisibilityChange);
  }
}
