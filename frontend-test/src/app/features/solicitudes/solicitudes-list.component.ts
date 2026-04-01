import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SolicitudesApiClient } from '../../api/clients/solicitudes-api.client';
import { Router } from '@angular/router';
import { AlertComponent } from '../../shared/components/alert/alert.component';
import { SimpleTableComponent } from '../../shared/components/simple-table/simple-table.component';
import { BogotaDatePipe } from '../../core/pipes/bogota-date.pipe';
import { formatBogotaDate } from '../../core/util/format-bogota-date';
import { DataGridColumn, DataGridComponent } from '../../shared/components/data-grid/data-grid.component';
import { SolicitudResponseDto } from '../../api/models/solicitud.dto';
import { SolicitudEstadoLabels } from '../../api/models/common.dto';

@Component({
  standalone: true,
  imports: [CommonModule, DataGridComponent, AlertComponent],
  template: `
  <div>
    <div class="form-row">
      <button class="btn" (click)="router.navigate(['/solicitudes/nueva'])">Crear</button>
    </div>

    <app-alert *ngIf="error?.message" [message]="error?.message" [errors]="error?.errors"></app-alert>

    <app-data-grid
      [columns]="columns"
      [rows]="items"
      [loading]="loading"
      emptyText="No hay solicitudes para mostrar."
      [rowKey]="rowKey"
      [rowClick]="onRowClick"
    ></app-data-grid>
  </div>
  `
})
export class SolicitudesListComponent {
  items: SolicitudResponseDto[] = [];
  error: any = null;
  loading = true;
  rowKey = (r: SolicitudResponseDto) => r.id;
  columns: DataGridColumn<SolicitudResponseDto>[] = [
    { header: 'ID', field: 'id', sortable: true, widthPx: 70, align: 'right' },
    { header: 'IdExterno', field: 'idExterno', sortable: true, widthPx: 140 },
    { header: 'Personaje', field: 'personajeNombre', sortable: true, widthPx: 220 },
    { header: 'Solicitante', field: 'solicitante', sortable: true, widthPx: 200 },
    { header: 'Evento', field: 'evento', sortable: true, widthPx: 220 },
    { header: 'Fecha Evento', field: 'fechaEvento', sortable: true, widthPx: 170, formatter: (v) => formatBogotaDate(v) },
    { header: 'Estado', field: 'estado', sortable: true, widthPx: 140, formatter: (v) => SolicitudEstadoLabels[v as 0 | 1 | 2 | 3] ?? String(v) }
  ];
  constructor(private api: SolicitudesApiClient, public router: Router) { this.load(); }
  load() {
    this.loading = true;
    this.api.listar().subscribe({
      next: v => { this.items = v; this.loading = false; },
      error: e => { this.error = e; this.loading = false; }
    });
  }
  onRowClick = (row: SolicitudResponseDto) => {
    this.router.navigate(['/solicitudes', row.id]);
  };
}
