import { PersonajeEstado } from './common.dto';

export interface PersonajeResponseDto {
  id: number;
  externalId: number;
  nombre: string;
  estado: PersonajeEstado;
  especie: string;
  genero: string;
  origen: string;
  imagenUrl: string;
  fechaImport: string;
}
