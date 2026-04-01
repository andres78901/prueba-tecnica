export interface ListResponse<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export type PersonajeEstado = 'Alive' | 'Dead' | 'Unknown';
/**
 * Mapeo requerido:
 * Pendiente = 0, EnProceso = 1, Aprobada = 2, Rechazada = 3
 */
export type SolicitudEstado = 0 | 1 | 2 | 3;

export const SolicitudEstadoLabels: Record<SolicitudEstado, string> = {
  0: 'Pendiente',
  1: 'EnProceso',
  2: 'Aprobada',
  3: 'Rechazada'
};

export interface ErrorResponse {
  message: string;
  errors?: string[] | null;
}
