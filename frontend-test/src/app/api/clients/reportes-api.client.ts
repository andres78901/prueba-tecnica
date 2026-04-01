import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SolicitudEstado } from '../models/common.dto';

@Injectable({ providedIn: 'root' })
export class ReportesApiClient {
  constructor(private http: HttpClient) {}

  solicitudesResumen(): Observable<{ total: number; porEstado: Record<string, number> }> {
    // Nota: cuando las keys son estados numéricos, en JSON llegan como strings ("0","1"...).
    return this.http.get<{ total: number; porEstado: Record<string, number> }>('/api/reportes/solicitudes-resumen');
  }
}
