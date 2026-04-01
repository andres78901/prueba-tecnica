import { Injectable } from '@angular/core';
import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { NotificationsService } from '../services/notifications.service';

@Injectable()
export class RequestNotifyInterceptor implements HttpInterceptor {
  constructor(private notifications: NotificationsService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Evita ruido en assets o requests internos.
    if (req.url.includes('/assets/') || req.url.includes('hot-update')) {
      return next.handle(req);
    }

    const method = req.method.toUpperCase();
    const path = req.url.replace(/^https?:\/\/[^/]+/i, '');

    return next.handle(req).pipe(
      tap({
        next: (evt) => {
          if (!(evt instanceof HttpResponse)) return;
          // Solo notificar cuando hay respuesta final.
          const msg = this.successMessage(method, path, evt.status);
          if (msg) this.notifications.show('success', msg);
        },
        error: (err: unknown) => {
          // El ErrorInterceptor ya mapea a UiError para consumers, pero acá garantizamos un toast.
          if (err instanceof HttpErrorResponse) {
            const body = err.error as any;
            const msg = body?.message ?? err.statusText ?? 'Ocurrió un error';
            const details = body?.errors ?? undefined;
            this.notifications.show('error', msg, Array.isArray(details) ? details : undefined);
            return;
          }
          this.notifications.show('error', 'Ocurrió un error al completar la solicitud.');
        }
      })
    );
  }

  private successMessage(method: string, path: string, status: number): string | null {
    // Mensajes genéricos (sin inventar payload).
    if (path.startsWith('/health')) return null;

    if (method === 'GET') return 'Datos cargados correctamente.';
    if (method === 'POST' && status === 201) return 'Registro creado correctamente.';
    if (method === 'POST') return 'Solicitud completada correctamente.';
    if (method === 'PATCH' || method === 'PUT') return 'Actualización aplicada correctamente.';
    return 'Solicitud completada correctamente.';
  }
}

