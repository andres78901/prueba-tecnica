import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { UiError } from '../models/ui-error.model';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((err: unknown) => {
        if (err instanceof HttpErrorResponse) {
          const body = err.error as any;
          const ui: UiError = {
            status: err.status,
            message: err.status === 0
              ? 'No fue posible conectar con el API (revisa CORS, certificado HTTPS, o que el backend esté ejecutándose).'
              : (body?.message ?? err.statusText ?? 'Unexpected error'),
            errors: body?.errors ?? undefined
          };
          return throwError(() => ui);
        }
        return throwError(() => ({ status: 0, message: 'No fue posible conectar con el API.' } as UiError));
      })
    );
  }
}
