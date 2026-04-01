import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  imports: [CommonModule],
  selector: 'app-pager',
  template: `
    <div class="pager" *ngIf="totalPages > 1">
      <button class="btn-secondary" type="button" (click)="goTo(page - 1)" [disabled]="page <= 1 || !hasPreviousPage">
        Prev
      </button>

      <div class="pager-pages" role="navigation" aria-label="Paginación">
        <button
          *ngFor="let p of pages"
          type="button"
          class="pager-page"
          [class.pager-page-active]="p === page"
          (click)="goTo(p)"
          [disabled]="p === page"
          [attr.aria-current]="p === page ? 'page' : null"
        >
          {{ p }}
        </button>
      </div>

      <div class="pager-meta">Página {{page}} / {{totalPages}}</div>

      <button class="btn-primary" type="button" (click)="goTo(page + 1)" [disabled]="page >= totalPages || !hasNextPage">
        Next
      </button>
    </div>
  `
})
export class PagerComponent {
  @Input() page = 1;
  @Input() totalPages = 1;
  @Input() hasPreviousPage = false;
  @Input() hasNextPage = false;
  @Output() pageChange = new EventEmitter<number>();

  get pages(): number[] {
    const total = Math.max(1, this.totalPages || 1);
    const current = Math.min(Math.max(1, this.page || 1), total);
    const windowSize = 7;
    const half = Math.floor(windowSize / 2);
    let start = Math.max(1, current - half);
    let end = Math.min(total, start + windowSize - 1);
    start = Math.max(1, end - windowSize + 1);
    const arr: number[] = [];
    for (let i = start; i <= end; i++) arr.push(i);
    return arr;
  }

  goTo(p: number) {
    const total = Math.max(1, this.totalPages || 1);
    const next = Math.min(Math.max(1, p), total);
    if (next === this.page) return;
    this.pageChange.emit(next);
  }
}
