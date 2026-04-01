import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { PersonajesApiClient } from '../../api/clients/personajes-api.client';
import { DataGridComponent, DataGridColumn } from '../../shared/components/data-grid/data-grid.component';
import { PagerComponent } from '../../shared/components/pager/pager.component';
import { Router } from '@angular/router';
import { AlertComponent } from '../../shared/components/alert/alert.component';
import { LoadingComponent } from '../../shared/components/loading/loading.component';
import { formatBogotaDate } from '../../core/util/format-bogota-date';
import { PersonajeResponseDto } from '../../api/models/personaje.dto';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule, DataGridComponent, PagerComponent, AlertComponent, LoadingComponent],
  template: `
  <div>
    <div class="card mb-4">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-3 items-end">
        <div>
          <label class="form-label" for="nombre">Nombre</label>
          <input id="nombre" name="nombre" class="form-input" placeholder="Ej. Rick" [(ngModel)]="filters.nombre" />
        </div>
        <div>
          <label class="form-label" for="estado">Estado</label>
          <select id="estado" name="estado" class="form-input" [(ngModel)]="filters.estado">
        <option value="">Todos estados</option>
        <option>Alive</option><option>Dead</option><option>Unknown</option>
          </select>
        </div>
        <div>
          <label class="form-label" for="especie">Especie</label>
          <input id="especie" name="especie" class="form-input" placeholder="Ej. Human" [(ngModel)]="filters.especie" />
        </div>

        <div class="flex flex-col sm:flex-row gap-2">
          <button class="btn-primary" (click)="onBuscar()" [disabled]="loading">
            {{ loading ? 'Buscando...' : 'Buscar' }}
          </button>
          <button class="btn-secondary" (click)="router.navigate(['/personajes/importar'])" [disabled]="loading">Importar</button>
          <button class="btn-secondary" type="button" (click)="onLimpiar()" [disabled]="loading">Limpiar</button>
        </div>
      </div>
    </div>

    <app-alert *ngIf="error?.message" [message]="error?.message" [errors]="error?.errors"></app-alert>
    <app-loading *ngIf="loading"></app-loading>

    <app-data-grid
      [columns]="columns"
      [rows]="items"
      [loading]="loading"
      emptyText="No hay personajes para mostrar."
      [rowKey]="rowKey"
      [rowClick]="onRowClick"
    ></app-data-grid>

    <app-pager [page]="page" [totalPages]="totalPages" [hasPreviousPage]="hasPrevious" [hasNextPage]="hasNext" (pageChange)="onPageChange($event)"></app-pager>
  </div>
  `
})
export class PersonajesListComponent implements OnInit {
  items: PersonajeResponseDto[] = [];
  page = 1;
  pageSize = 20;
  totalPages = 1;
  hasPrevious = false;
  hasNext = false;
  loading = true;
  error: any = null;
  filters: any = { nombre: '', estado: '', especie: '', genero: '', origen: '' };
  rowKey = (r: PersonajeResponseDto) => r.id;
  columns: DataGridColumn<PersonajeResponseDto>[] = [
    { header: 'ID', field: 'id', sortable: true, widthPx: 70, align: 'right' },
    { header: 'External', field: 'externalId', sortable: true, widthPx: 90, align: 'right' },
    { header: 'Nombre', field: 'nombre', sortable: true, widthPx: 220 },
    { header: 'Estado', field: 'estado', sortable: true, widthPx: 110 },
    { header: 'Especie', field: 'especie', sortable: true, widthPx: 140 },
    { header: 'Genero', field: 'genero', sortable: true, widthPx: 120 },
    { header: 'Origen', field: 'origen', sortable: true, widthPx: 180 },
    { header: 'Fecha Import', field: 'fechaImport', sortable: true, widthPx: 170, formatter: (v) => formatBogotaDate(v) }
  ];

  constructor(private api: PersonajesApiClient, public router: Router) {}

  ngOnInit(): void {
    this.load();
  }

  load() {
    // Importante: loading inicia en true para evitar NG0100 (disabled false→true) en el primer check.
    this.loading = true;
    this.error = null;
    this.api.listar({ page: this.page, pageSize: this.pageSize, ...this.filters }).subscribe({
      next: res => { this.items = res.items; this.page = res.page; this.pageSize = res.pageSize; this.totalPages = res.totalPages; this.hasPrevious = res.hasPreviousPage; this.hasNext = res.hasNextPage; this.loading = false; },
      error: (err) => { this.error = err; this.loading = false; }
    });
  }

  onBuscar() {
    this.page = 1;
    this.load();
  }

  onLimpiar() {
    this.filters = { nombre: '', estado: '', especie: '', genero: '', origen: '' };
    this.page = 1;
    this.load();
  }

  onPageChange(newPage: number) { this.page = newPage; this.load(); }
  onRowClick = (row: PersonajeResponseDto) => {
    this.router.navigate(['/personajes', row.id]);
  };
}
