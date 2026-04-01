import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { PersonajesApiClient } from './personajes-api.client';

describe('PersonajesApiClient', () => {
  let client: PersonajesApiClient;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting(), PersonajesApiClient] });
    client = TestBed.inject(PersonajesApiClient);
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('listar should call /api/personajes with params', () => {
    client.listar({ page: 2, pageSize: 10, nombre: 'rick' }).subscribe();
    const req = httpMock.expectOne(r =>
      r.url === '/api/personajes' &&
      r.params.get('page') === '2' &&
      r.params.get('pageSize') === '10' &&
      r.params.get('nombre') === 'rick'
    );
    expect(req).toBeTruthy();
    req.flush({ items: [], page:2, pageSize:10, totalCount:0, totalPages:0, hasPreviousPage:false, hasNextPage:false });
    httpMock.verify();
  });
});
