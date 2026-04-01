import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { SolicitudesApiClient } from './solicitudes-api.client';
import { SolicitudEstado } from '../models/common.dto';

describe('SolicitudesApiClient', () => {
  let client: SolicitudesApiClient;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting(), SolicitudesApiClient]
    });
    client = TestBed.inject(SolicitudesApiClient);
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('crear should POST /api/solicitudes', () => {
    client.crear({ personajeId: 1, solicitante: 'Ana', evento: 'Firma', fechaEvento: '2024-01-01T00:00:00Z' }).subscribe();
    const req = httpMock.expectOne(r => r.method === 'POST' && r.url === '/api/solicitudes');
    expect(req.request.body.personajeId).toBe(1);
    req.flush({ id: 1 });
    httpMock.verify();
  });

  it('listar should GET /api/solicitudes', () => {
    client.listar().subscribe();
    const req = httpMock.expectOne(r => r.method === 'GET' && r.url === '/api/solicitudes');
    req.flush([]);
    httpMock.verify();
  });

  it('obtener should GET /api/solicitudes/{id}', () => {
    client.obtener(10).subscribe();
    const req = httpMock.expectOne(r => r.method === 'GET' && r.url === '/api/solicitudes/10');
    req.flush({ id: 10 });
    httpMock.verify();
  });

  it('cambiarEstado should PATCH /api/solicitudes/{id}/estado', () => {
    const estado: SolicitudEstado = 2;
    client.cambiarEstado(5, { estado }).subscribe();
    const req = httpMock.expectOne(r => r.method === 'PATCH' && r.url === '/api/solicitudes/5/estado');
    expect(req.request.body).toEqual({ estado });
    req.flush({ id: 5 });
    httpMock.verify();
  });
});

