import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SolicitudesApiClient } from '../../api/clients/solicitudes-api.client';
import { AlertComponent } from '../../shared/components/alert/alert.component';
import { LoadingComponent } from '../../shared/components/loading/loading.component';
import { BogotaDatePipe } from '../../core/pipes/bogota-date.pipe';
import { SolicitudEstado, SolicitudEstadoLabels } from '../../api/models/common.dto';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule, AlertComponent, LoadingComponent, BogotaDatePipe],
  template: `
  <div class="card max-w-3xl mx-auto">
    <div class="flex justify-between items-center mb-4">
      <h3 class="text-lg font-semibold">Solicitud</h3>
      <button class="btn-secondary" (click)="router.navigate(['/solicitudes'])">Volver</button>
    </div>

    <app-alert *ngIf="error?.message" [message]="error?.message" [errors]="error?.errors"></app-alert>
    <app-loading *ngIf="loading"></app-loading>

    <div *ngIf="item" class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <div class="md:col-span-2">
        <h4 class="text-md font-medium">#{{item.id}} - {{item.personajeNombre}}</h4>
        <p class="mt-2"><span class="font-semibold">Solicitante:</span> {{item.solicitante}}</p>
        <p class="mt-1"><span class="font-semibold">Evento:</span> {{item.evento}}</p>
        <p class="mt-1"><span class="font-semibold">Fecha Evento:</span> {{item.fechaEvento | bogotaDate}}</p>
        <p class="mt-1"><span class="font-semibold">Estado actual:</span> {{item.estado}}</p>
        <p class="mt-1"><span class="font-semibold">Fecha creación:</span> {{item.fechaCreacion | bogotaDate}}</p>
        <p class="mt-1"><span class="font-semibold">Última actualización:</span> {{item.fechaActualizacion | bogotaDate}}</p>
      </div>
      <div class="md:col-span-1">
        <form (ngSubmit)="cambiarEstado()" #estadoForm="ngForm" novalidate>
          <label class="form-label" for="nuevoEstado">Nuevo estado</label>
          <select
            id="nuevoEstado"
            name="nuevoEstado"
            class="form-input"
            [(ngModel)]="newEstado"
            required
            [disabled]="loading"
          >
            <option value="">-- Seleccionar --</option>
            <option [ngValue]="0">{{ estadoLabels[0] }}</option>
            <option [ngValue]="1">{{ estadoLabels[1] }}</option>
            <option [ngValue]="2">{{ estadoLabels[2] }}</option>
            <option [ngValue]="3">{{ estadoLabels[3] }}</option>
          </select>
          <small class="form-hint">El cambio se aplica inmediatamente.</small>

          <div class="mt-4">
            <button class="btn-primary w-full" type="submit" [disabled]="loading || !estadoForm.valid">
              {{ loading ? 'Aplicando...' : 'Aplicar' }}
            </button>
          </div>

          <div *ngIf="successMessage" class="mt-3 text-sm text-green-600">{{successMessage}}</div>
        </form>
      </div>
    </div>

    <div *ngIf="!loading && !item && !error" class="text-sm text-slate-600">
      No se encontró información para esta solicitud.
    </div>
  </div>
  `
})
export class SolicitudDetailComponent implements OnInit {
  item: any | null = null;
  error: any = null;
  newEstado: SolicitudEstado | '' = '';
  estadoLabels = SolicitudEstadoLabels;
  successMessage?: string;
  loading = false;
  constructor(private route: ActivatedRoute, private api: SolicitudesApiClient, public router: Router) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!Number.isFinite(id) || id <= 0) {
      this.error = { status: 400, message: 'Id de solicitud inválido.' };
      return;
    }
    this.loading = true;
    this.api.obtener(id).subscribe({
      next: v => { this.item = v; this.loading = false; },
      error: e => { this.error = e; this.item = null; this.loading = false; }
    });
  }

  cambiarEstado() {
    if (!this.item || !this.newEstado) return;
    this.loading = true;
    this.successMessage = undefined;
    this.api.cambiarEstado(this.item.id, { estado: this.newEstado }).subscribe({
      next: v => {
        this.item = v;
        this.error = null;
        this.loading = false;
        this.successMessage = `Estado actualizado a ${this.estadoLabels[this.newEstado as SolicitudEstado]}.`;
      },
      error: e => { this.error = e; this.loading = false; }
    });
  }
}
