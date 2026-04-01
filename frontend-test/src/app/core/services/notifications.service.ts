import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export type ToastType = 'success' | 'error' | 'info';

export interface ToastMessage {
  id: number;
  type: ToastType;
  message: string;
  details?: string[];
  createdAt: number;
}

@Injectable({ providedIn: 'root' })
export class NotificationsService {
  private seq = 1;
  private readonly _toasts = new BehaviorSubject<ToastMessage[]>([]);
  readonly toasts$ = this._toasts.asObservable();

  show(type: ToastType, message: string, details?: string[]) {
    const toast: ToastMessage = {
      id: this.seq++,
      type,
      message,
      details,
      createdAt: Date.now()
    };
    const next = [...this._toasts.value, toast].slice(-3);
    this._toasts.next(next);

    const ttlMs = type === 'error' ? 7000 : 3500;
    window.setTimeout(() => this.dismiss(toast.id), ttlMs);
  }

  dismiss(id: number) {
    this._toasts.next(this._toasts.value.filter(t => t.id !== id));
  }

  clear() {
    this._toasts.next([]);
  }
}

