import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SolicitudCreateDto, SolicitudResponseDto } from '../models/solicitud.dto';
import { SolicitudEstado } from '../models/common.dto';

@Injectable({ providedIn: 'root' })
export class SolicitudesApiClient {
  constructor(private http: HttpClient) {}

  crear(body: SolicitudCreateDto): Observable<SolicitudResponseDto> {
    return this.http.post<SolicitudResponseDto>('/api/solicitudes', body);
  }

  listar(): Observable<SolicitudResponseDto[]> {
    return this.http.get<SolicitudResponseDto[]>('/api/solicitudes');
  }

  obtener(id: number): Observable<SolicitudResponseDto> {
    return this.http.get<SolicitudResponseDto>(`/api/solicitudes/${id}`);
  }

  cambiarEstado(id: number, body: { estado: SolicitudEstado }): Observable<SolicitudResponseDto> {
    return this.http.patch<SolicitudResponseDto>(`/api/solicitudes/${id}/estado`, body);
  }
}
