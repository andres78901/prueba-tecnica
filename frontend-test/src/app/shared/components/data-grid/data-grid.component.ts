import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

export type DataGridAlign = 'left' | 'center' | 'right';

export interface DataGridColumn<T extends Record<string, any>> {
  header: string;
  field: keyof T | string;
  sortable?: boolean;
  widthPx?: number;
  align?: DataGridAlign;
  formatter?: (value: any, row: T) => string;
}

@Component({
  standalone: true,
  selector: 'app-data-grid',
  imports: [CommonModule],
  template: `
    <div class="dg">
      <div class="dg-scroll">
        <table class="dg-table" [attr.aria-busy]="loading ? 'true' : 'false'">
          <thead>
            <tr>
              <th
                *ngFor="let c of columns; trackBy: trackCol"
                [style.width.px]="c.widthPx || null"
                [class.dg-sortable]="!!c.sortable"
                [class.dg-align-center]="(c.align || 'left') === 'center'"
                [class.dg-align-right]="(c.align || 'left') === 'right'"
                (click)="onHeaderClick(c)"
              >
                <span class="dg-th">
                  <span>{{ c.header }}</span>
                  <span class="dg-sort" *ngIf="c.sortable">
                    <span *ngIf="sortField !== colField(c)">↕</span>
                    <span *ngIf="sortField === colField(c) && sortDir === 'asc'">↑</span>
                    <span *ngIf="sortField === colField(c) && sortDir === 'desc'">↓</span>
                  </span>
                </span>
              </th>
            </tr>
          </thead>

          <tbody>
            <tr *ngIf="loading">
              <td class="dg-muted" [attr.colspan]="columns.length">Cargando...</td>
            </tr>

            <tr *ngIf="!loading && (!rows || rows.length === 0)">
              <td class="dg-muted" [attr.colspan]="columns.length">{{ emptyText }}</td>
            </tr>

            <tr
              *ngFor="let row of sortedRows(); trackBy: trackRow"
              class="dg-row"
              [class.dg-clickable]="!!rowClick"
              [attr.tabindex]="rowClick ? 0 : null"
              [attr.role]="rowClick ? 'button' : null"
              (click)="onRowClick(row)"
              (keydown.enter)="onRowClick(row)"
              (keydown.space)="onRowClick(row)"
            >
              <td
                *ngFor="let c of columns; trackBy: trackCol"
                [class.dg-align-center]="(c.align || 'left') === 'center'"
                [class.dg-align-right]="(c.align || 'left') === 'right'"
              >
                {{ formatCell(c, row) }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class DataGridComponent<T extends Record<string, any>> {
  @Input() columns: DataGridColumn<T>[] = [];
  @Input() rows: T[] = [];
  @Input() emptyText = 'No hay resultados.';
  @Input() loading = false;
  @Input() rowKey?: (row: T) => string | number;
  @Input() rowClick?: (row: T) => void;

  @Output() sortChange = new EventEmitter<{ field: string; dir: 'asc' | 'desc' } | null>();

  sortField: string | null = null;
  sortDir: 'asc' | 'desc' = 'asc';

  trackCol = (_: number, c: DataGridColumn<T>) => `${c.header}::${this.colField(c)}`;
  trackRow = (_: number, row: T) => (this.rowKey ? this.rowKey(row) : row);

  colField(c: DataGridColumn<T>): string {
    return String(c.field);
  }

  onHeaderClick(c: DataGridColumn<T>) {
    if (!c.sortable) return;
    const field = this.colField(c);
    if (this.sortField !== field) {
      this.sortField = field;
      this.sortDir = 'asc';
      this.sortChange.emit({ field, dir: this.sortDir });
      return;
    }
    this.sortDir = this.sortDir === 'asc' ? 'desc' : 'asc';
    this.sortChange.emit({ field, dir: this.sortDir });
  }

  onRowClick(row: T) {
    if (this.rowClick) this.rowClick(row);
  }

  formatCell(c: DataGridColumn<T>, row: T): string {
    const field = this.colField(c);
    const raw = (row as any)?.[field];
    if (c.formatter) return c.formatter(raw, row);
    if (raw === null || raw === undefined) return '';
    return String(raw);
  }

  sortedRows(): T[] {
    if (!this.rows) return [];
    if (!this.sortField) return this.rows;
    const field = this.sortField;
    const dir = this.sortDir;
    // Copia para no mutar input
    const copy = [...this.rows];
    copy.sort((a: any, b: any) => {
      const av = a?.[field];
      const bv = b?.[field];
      if (av === bv) return 0;
      if (av === null || av === undefined) return 1;
      if (bv === null || bv === undefined) return -1;
      // números
      if (typeof av === 'number' && typeof bv === 'number') return dir === 'asc' ? av - bv : bv - av;
      // fechas ISO
      const ad = typeof av === 'string' ? Date.parse(av) : NaN;
      const bd = typeof bv === 'string' ? Date.parse(bv) : NaN;
      if (!isNaN(ad) && !isNaN(bd)) return dir === 'asc' ? ad - bd : bd - ad;
      // string fallback
      const as = String(av).toLowerCase();
      const bs = String(bv).toLowerCase();
      return dir === 'asc' ? as.localeCompare(bs) : bs.localeCompare(as);
    });
    return copy;
  }
}

