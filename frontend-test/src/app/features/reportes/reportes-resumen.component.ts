import { Component } from '@angular/core';
import { ReportesApiClient } from '../../api/clients/reportes-api.client';
import { CommonModule } from '@angular/common';
import { AlertComponent } from '../../shared/components/alert/alert.component';
import { DataGridColumn, DataGridComponent } from '../../shared/components/data-grid/data-grid.component';
import { SolicitudEstado, SolicitudEstadoLabels } from '../../api/models/common.dto';

@Component({
  standalone: true,
  imports: [CommonModule, AlertComponent, DataGridComponent],
  template: `
  <div>
    <div class="flex items-center justify-between mb-4">
      <h3 class="text-lg font-semibold">Resumen Solicitudes</h3>
      <button class="btn-secondary" (click)="load()" [disabled]="loading">{{ loading ? 'Cargando...' : 'Refrescar' }}</button>
    </div>

    <app-alert *ngIf="error?.message" [message]="error?.message" [errors]="error?.errors"></app-alert>

    <div class="card mb-4">
      <div class="flex items-baseline gap-3">
        <div class="text-sm text-slate-600">Total</div>
        <div class="text-2xl font-semibold">{{ total }}</div>
      </div>
    </div>

    <app-data-grid
      [columns]="columns"
      [rows]="rows"
      [loading]="loading"
      emptyText="No hay datos para mostrar."
      [rowKey]="rowKey"
    ></app-data-grid>
  </div>
  `
})
export class ReportesResumenComponent {
  error: any = null;
  loading = true;
  total = 0;

  rowKey = (r: { estado: string; cantidad: number }) => r.estado;
  columns: DataGridColumn<{ estado: string; cantidad: number }>[] = [
    { header: 'Estado', field: 'estado', sortable: true, widthPx: 180 },
    { header: 'Cantidad', field: 'cantidad', sortable: true, widthPx: 120, align: 'right' }
  ];

  rows: { estado: string; cantidad: number }[] = [];
  estadoLabels = SolicitudEstadoLabels;

  constructor(private api: ReportesApiClient) { this.load(); }

  private toEstadoCode(key: string): SolicitudEstado | null {
    const k = String(key).trim();
    if (k === '0' || k === 'Pendiente') return 0;
    if (k === '1' || k === 'EnProceso' || k === 'En Proceso') return 1;
    if (k === '2' || k === 'Aprobada') return 2;
    if (k === '3' || k === 'Rechazada') return 3;
    return null;
  }

  load() {
    this.loading = true;
    this.error = null;
    this.api.solicitudesResumen().subscribe({
      next: v => {
        const porEstadoRaw = (v as any)?.porEstado ?? {};
        const normalized: Partial<Record<SolicitudEstado, number>> = {};

        for (const [k, val] of Object.entries(porEstadoRaw as Record<string, any>)) {
          const code = this.toEstadoCode(k);
          if (code === null) continue;
          const n = typeof val === 'number' ? val : Number(val);
          normalized[code] = Number.isFinite(n) ? n : 0;
        }

        const estados: SolicitudEstado[] = [0, 1, 2, 3];
        this.rows = estados.map(code => ({
          estado: this.estadoLabels[code],
          cantidad: normalized[code] ?? 0
        }));

        const sum = this.rows.reduce((acc, r) => acc + (r.cantidad || 0), 0);
        this.total = (v as any)?.total ?? sum;
        this.loading = false;
      },
      error: e => { this.error = e; this.loading = false; }
    });
  }
}
