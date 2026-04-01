import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SolicitudesApiClient } from '../../api/clients/solicitudes-api.client';
import { Router } from '@angular/router';
import { AlertComponent } from '../../shared/components/alert/alert.component';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule, AlertComponent],
  template: `
  <div class="card max-w-2xl mx-auto">
    <div class="flex justify-between items-center mb-4">
      <h3 class="text-lg font-semibold">Crear Solicitud</h3>
      <button class="btn-secondary" (click)="router.navigate(['/solicitudes'])">Volver</button>
    </div>

    <form class="space-y-4" #f="ngForm" (ngSubmit)="create(f)" novalidate>
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div class="md:col-span-2">
          <label class="form-label" for="idExterno">Id Externo (opcional)</label>
          <input id="idExterno" name="idExterno" class="form-input" [(ngModel)]="model.idExterno" autocomplete="off" />
          <small class="form-hint">Útil para trazabilidad con sistemas externos.</small>
        </div>

        <div>
          <label class="form-label" for="personajeId">Personaje Id</label>
          <input
            id="personajeId"
            name="personajeId"
            class="form-input"
            type="number"
            min="1"
            required
            [(ngModel)]="model.personajeId"
            #personajeId="ngModel"
          />
          <small class="form-error" *ngIf="(personajeId.touched || submitted) && personajeId.invalid">
            El Personaje Id es obligatorio y debe ser mayor a 0.
          </small>
        </div>

        <div>
          <label class="form-label" for="solicitante">Solicitante</label>
          <input
            id="solicitante"
            name="solicitante"
            class="form-input"
            required
            minlength="2"
            [(ngModel)]="model.solicitante"
            #solicitante="ngModel"
            autocomplete="name"
          />
          <small class="form-error" *ngIf="(solicitante.touched || submitted) && solicitante.invalid">
            El solicitante es obligatorio (mínimo 2 caracteres).
          </small>
        </div>

        <div class="md:col-span-2">
          <label class="form-label" for="evento">Evento</label>
          <input
            id="evento"
            name="evento"
            class="form-input"
            required
            minlength="2"
            [(ngModel)]="model.evento"
            #evento="ngModel"
            autocomplete="off"
          />
          <small class="form-error" *ngIf="(evento.touched || submitted) && evento.invalid">
            El evento es obligatorio (mínimo 2 caracteres).
          </small>
        </div>

        <div class="md:col-span-2">
          <label class="form-label" for="fechaEventoLocal">Fecha Evento (opcional)</label>
          <input
            id="fechaEventoLocal"
            name="fechaEventoLocal"
            class="form-input"
            type="datetime-local"
            [(ngModel)]="fechaEventoLocal"
          />
          <small class="form-hint">Si la dejas vacía, el backend usará su valor por defecto (si aplica).</small>
        </div>
      </div>

      <div class="flex flex-col sm:flex-row gap-3">
        <button class="btn-primary" type="submit" [disabled]="loading">
          {{ loading ? 'Creando...' : 'Crear' }}
        </button>
        <button class="btn-secondary" type="button" (click)="reset()" [disabled]="loading">Limpiar</button>
      </div>

      <app-alert *ngIf="resultMessage" [message]="resultMessage" [errors]="resultErrors"></app-alert>
    </form>
  </div>
  `
})
export class SolicitudCreateComponent {
  model: any = { idExterno: '', personajeId: null as number | null, solicitante: '', evento: '', fechaEvento: '' };
  resultMessage?: string;
  resultErrors?: string[] | null;
  fechaEventoLocal: string = '';
  loading = false;
  submitted = false;

  constructor(private api: SolicitudesApiClient, public router: Router) {}

  create(form?: any) {
    this.resultMessage = undefined; this.resultErrors = undefined;
    this.submitted = true;
    if (form && form.invalid) return;
    if (!this.model.personajeId || this.model.personajeId < 1) return;
    if (!this.model.solicitante || this.model.solicitante.trim().length < 2) return;
    if (!this.model.evento || this.model.evento.trim().length < 2) return;
    this.loading = true;
    // convert local datetime-local to ISO if provided
    const payload = { ...this.model };
    if (this.fechaEventoLocal) {
      const d = new Date(this.fechaEventoLocal);
      payload.fechaEvento = d.toISOString();
    }
    this.api.crear(payload).subscribe({
      next: r => {
        this.resultMessage = `Solicitud creada correctamente con id ${r.id}`;
        this.loading = false;
        this.router.navigate(['/solicitudes']);
      },
      error: e => {
        this.loading = false;
        this.resultMessage = e.message;
        this.resultErrors = e.errors || undefined;
      }
    });
  }

  reset() {
    this.model = { idExterno: '', personajeId: null, solicitante: '', evento: '', fechaEvento: '' };
    this.fechaEventoLocal = '';
    this.resultMessage = undefined;
    this.resultErrors = undefined;
    this.submitted = false;
  }
}
