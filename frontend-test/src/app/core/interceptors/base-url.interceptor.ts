import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable()
export class BaseUrlInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const baseUrl = (environment.apiBaseUrl || '').trim();
    const url = req.url.startsWith('http') || !baseUrl ? req.url : `${baseUrl}${req.url}`;
    return next.handle(req.clone({ url }));
  }
}
