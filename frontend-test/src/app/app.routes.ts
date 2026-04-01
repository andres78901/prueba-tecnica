import { Routes } from '@angular/router';
import { PersonajesListComponent } from './features/personajes/personajes-list.component';
import { PersonajeDetailComponent } from './features/personajes/personaje-detail.component';
import { PersonajesImportComponent } from './features/personajes/personajes-import.component';
import { SolicitudesListComponent } from './features/solicitudes/solicitudes-list.component';
import { SolicitudCreateComponent } from './features/solicitudes/solicitud-create.component';
import { SolicitudDetailComponent } from './features/solicitudes/solicitud-detail.component';
import { ReportesResumenComponent } from './features/reportes/reportes-resumen.component';

export const routes: Routes = [
	{ path: '', redirectTo: 'personajes', pathMatch: 'full' },
	{ path: 'personajes', component: PersonajesListComponent },
	{ path: 'personajes/importar', component: PersonajesImportComponent },
	{ path: 'personajes/:id', component: PersonajeDetailComponent },
	{ path: 'solicitudes', component: SolicitudesListComponent },
	{ path: 'solicitudes/nueva', component: SolicitudCreateComponent },
	{ path: 'solicitudes/:id', component: SolicitudDetailComponent },
	{ path: 'reportes', component: ReportesResumenComponent }
];
