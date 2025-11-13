import { CommonModule } from '@angular/common';
import { Component, ElementRef, EventEmitter, HostListener, Input, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-searchable-select',
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './searchable-select.html',
  styleUrl: './searchable-select.scss'
})
export class SearchableSelect implements OnInit {
  @Input() items: any[] = [];                 // mảng object
  @Input() labelKey = 'label';                // tên property hiển thị
  @Input() valueKey = 'value';                // tên property value
  @Input() placeholder = 'Chọn...';
  @Input() disabled = false;

  @Output() selectionChange = new EventEmitter<any>();

  @ViewChild('searchInput') searchInput!: ElementRef<HTMLInputElement>;

  open = false;
  filter = '';
  filtered: any[] = [];
  highlighted = 0;
  selectedItem: any = null;

  private filterTimeout: any = null;

  constructor(private host: ElementRef) {}

  ngOnInit(): void {
    this.filtered = [...this.items];
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['items']) {
      this.applyFilterSync();
    }
  }

  toggleDropdown(open?: boolean) {
    if (this.disabled) return;
    this.open = typeof open === 'boolean' ? open : !this.open;

    if (this.open) {
      // refresh filter & focus
      this.applyFilterSync();
      setTimeout(() => {
        this.searchInput?.nativeElement?.focus();
      }, 0);

      // nếu đã có selectedItem -> tìm index trong filtered để highlight
      if (this.selectedItem) {
        const idx = this.filtered.findIndex(it => this._isSameValue(it, this.selectedItem));
        this.highlighted = Math.max(0, idx === -1 ? 0 : idx);
        // scroll to highlighted after render
        setTimeout(() => this.scrollToHighlighted(), 0);
      } else {
        this.highlighted = 0;
      }
    } else {
      // đóng -> reset filter (tuỳ bạn)
      this.filter = '';
      this.applyFilterSync();
    }
  }

  onFilter() {
    clearTimeout(this.filterTimeout);
    this.filterTimeout = setTimeout(() => {
      this.applyFilterSync();
    }, 150);
  }

  applyFilterSync() {
    const q = (this.filter || '').toString().trim().toLowerCase();
    if (!q) {
      this.filtered = [...this.items];
    } else {
      this.filtered = this.items.filter(it => {
        const val = (it[this.labelKey] ?? '').toString().toLowerCase();
        return val.includes(q);
      });
    }

    // nếu selectedItem tồn tại, cố gắng giữ highlighted trên item đã chọn (nếu còn trong filtered)
    if (this.selectedItem) {
      const idx = this.filtered.findIndex(it => this._isSameValue(it, this.selectedItem));
      this.highlighted = Math.max(0, idx === -1 ? 0 : idx);
    } else {
      this.highlighted = 0;
    }
  }

  select(item: any) {
    this.selectedItem = item;
    this.highlighted = this.filtered.findIndex(it => this._isSameValue(it, item));
    this.selectionChange.emit(item);
    this.toggleDropdown(false);
  }

  // keyboard handling
  onKeyDown(ev: KeyboardEvent) {
    if (!this.open && (ev.key === 'ArrowDown' || ev.key === 'Enter')) {
      this.toggleDropdown(true);
      ev.preventDefault();
      return;
    }
    if (!this.open) return;

    if (ev.key === 'ArrowDown') {
      this.highlighted = Math.min(this.highlighted + 1, this.filtered.length - 1);
      this.scrollToHighlighted();
      ev.preventDefault();
    } else if (ev.key === 'ArrowUp') {
      this.highlighted = Math.max(this.highlighted - 1, 0);
      this.scrollToHighlighted();
      ev.preventDefault();
    } else if (ev.key === 'Enter') {
      if (this.filtered[this.highlighted]) {
        this.select(this.filtered[this.highlighted]);
        ev.preventDefault();
      }
    } else if (ev.key === 'Escape') {
      this.toggleDropdown(false);
      ev.preventDefault();
    }
  }

  @HostListener('document:click', ['$event'])
  onDocClick(ev: MouseEvent) {
    const target = ev.target as HTMLElement;
    if (!this.host.nativeElement.contains(target)) {
      this.open = false;
      this.filter = '';
      this.applyFilterSync();
    }
  }

  private scrollToHighlighted() {
    const ul = this.host.nativeElement.querySelector('.options-list') as HTMLElement;
    const li = ul?.children?.[this.highlighted] as HTMLElement;
    if (li && ul) {
      const top = li.offsetTop;
      const bottom = top + li.offsetHeight;
      if (top < ul.scrollTop) ul.scrollTop = top;
      else if (bottom > ul.scrollTop + ul.clientHeight) ul.scrollTop = bottom - ul.clientHeight;
    }
  }

  // helper so sánh valueKey nếu object
  protected _isSameValue(a: any, b: any) {
    if (!a || !b) return false;
    // nếu có valueKey, so sánh theo key, còn không so sánh theo object reference
    if (this.valueKey && a[this.valueKey] !== undefined && b[this.valueKey] !== undefined) {
      return a[this.valueKey] === b[this.valueKey];
    }
    return a === b;
  }

  // optional trackBy
  trackByFn(index: number, item: any) {
    if (this.valueKey && item && item[this.valueKey] !== undefined) return item[this.valueKey];
    return index;
  }
}
