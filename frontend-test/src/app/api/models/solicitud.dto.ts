import { SolicitudEstado } from './common.dto';

export interface SolicitudCreateDto {
  idExterno?: string;
  personajeId: number;
  solicitante: string;
  evento: string;
  fechaEvento: string;
}

export interface SolicitudResponseDto {
  id: number;
  idExterno?: string;
  personajeId: number;
  personajeNombre?: string;
  solicitante: string;
  evento: string;
  fechaEvento: string;
  estado: SolicitudEstado;
  fechaCreacion: string;
  fechaActualizacion: string;
}
