import { CommonModule } from '@angular/common';
import { Component, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { NotificationsService, ToastMessage } from '../../../core/services/notifications.service';

@Component({
  standalone: true,
  selector: 'app-toast',
  imports: [CommonModule],
  template: `
    <div class="toast-stack" aria-live="polite" aria-relevant="additions removals">
      <div *ngFor="let t of toasts; trackBy: trackToast" class="toast" [class.toast-success]="t.type==='success'" [class.toast-error]="t.type==='error'">
        <div class="toast-head">
          <strong class="toast-title">{{ title(t) }}</strong>
          <button class="toast-x" type="button" (click)="dismiss(t.id)" aria-label="Cerrar">×</button>
        </div>
        <div class="toast-msg">{{ t.message }}</div>
        <ul class="toast-details" *ngIf="t.details?.length">
          <li *ngFor="let d of t.details">{{ d }}</li>
        </ul>
      </div>
    </div>
  `
})
export class ToastComponent implements OnDestroy {
  toasts: ToastMessage[] = [];
  private sub: Subscription;

  constructor(private notifications: NotificationsService) {
    this.sub = this.notifications.toasts$.subscribe(v => (this.toasts = v));
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  trackToast = (_: number, t: ToastMessage) => t.id;

  dismiss(id: number) {
    this.notifications.dismiss(id);
  }

  title(t: ToastMessage): string {
    if (t.type === 'error') return 'Error';
    if (t.type === 'success') return 'OK';
    return 'Info';
  }
}

