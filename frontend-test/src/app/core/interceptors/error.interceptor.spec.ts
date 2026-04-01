import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { HTTP_INTERCEPTORS, HttpClient } from '@angular/common/http';
import { ErrorInterceptor } from './error.interceptor';
import { UiError } from '../models/ui-error.model';

describe('ErrorInterceptor', () => {
  let http: HttpClient;
  let httpMock: HttpTestingController;
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [{ provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }]
    });
    http = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('should map backend {message, errors} to UiError', async () => {
    const result = new Promise<UiError>((resolve, reject) => {
      http.get('/api/test').subscribe({
        next: () => reject(new Error('Expected request to error')),
        error: (err: UiError) => resolve(err)
      });
      const req = httpMock.expectOne('/api/test');
      req.flush({ message: 'Bad things', errors: ['a', 'b'] }, { status: 400, statusText: 'Bad Request' });
    });

    const err = await result;
    expect(err.status).toBe(400);
    expect(err.message).toBe('Bad things');
    expect(err.errors).toEqual(['a', 'b']);
  });
});
