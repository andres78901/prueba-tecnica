import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PersonajeResponseDto } from '../models/personaje.dto';
import { ListResponse } from '../models/common.dto';

@Injectable({ providedIn: 'root' })
export class PersonajesApiClient {
  constructor(private http: HttpClient) {}

  importar(body?: { externalIds?: number[]; maxPaginasApi?: number }): Observable<{ importados: number; externalIdsProcesados: number[] }> {
    return this.http.post('/api/personajes/importar', body || {} as any) as Observable<any>;
  }

  listar(params: { page?: number; pageSize?: number; nombre?: string; estado?: string; especie?: string; genero?: string; origen?: string } = {}): Observable<ListResponse<PersonajeResponseDto>> {
    let httpParams = new HttpParams();
    if (params.page) httpParams = httpParams.set('page', String(params.page));
    if (params.pageSize) httpParams = httpParams.set('pageSize', String(params.pageSize));
    if (params.nombre) httpParams = httpParams.set('nombre', params.nombre);
    if (params.estado) httpParams = httpParams.set('estado', params.estado);
    if (params.especie) httpParams = httpParams.set('especie', params.especie);
    if (params.genero) httpParams = httpParams.set('genero', params.genero);
    if (params.origen) httpParams = httpParams.set('origen', params.origen);
    return this.http.get<ListResponse<PersonajeResponseDto>>('/api/personajes', { params: httpParams });
  }

  obtener(id: number): Observable<PersonajeResponseDto> {
    return this.http.get<PersonajeResponseDto>(`/api/personajes/${id}`);
  }
}
