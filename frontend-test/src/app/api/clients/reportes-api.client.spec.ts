import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ReportesApiClient } from './reportes-api.client';

describe('ReportesApiClient', () => {
  let client: ReportesApiClient;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting(), ReportesApiClient]
    });
    client = TestBed.inject(ReportesApiClient);
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('solicitudesResumen should GET /api/reportes/solicitudes-resumen', () => {
    client.solicitudesResumen().subscribe();
    const req = httpMock.expectOne(r => r.method === 'GET' && r.url === '/api/reportes/solicitudes-resumen');
    req.flush({ total: 0, porEstado: { '0': 0, '1': 0, '2': 0, '3': 0 } });
    httpMock.verify();
  });
});

