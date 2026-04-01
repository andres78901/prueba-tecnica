import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  imports: [CommonModule],
  selector: 'app-simple-table',
  template: `
    <table class="table">
      <thead><tr><th *ngFor="let h of headers">{{h}}</th></tr></thead>
      <tbody>
        <tr *ngFor="let row of rows" (click)="onRowClick(row)" style="cursor:pointer">
          <td *ngFor="let key of keys">{{formatCell(key, row)}}</td>
        </tr>
      </tbody>
    </table>
  `
})
export class SimpleTableComponent {
  @Input() headers: string[] = [];
  @Input() rows: Record<string, any>[] = [];
  @Input() keys: string[] = [];
  @Input() formatters: Record<string, (value: any, row: Record<string, any>) => string> = {};
  @Input() rowClick?: (row: Record<string, any>) => void;
  onRowClick(row: Record<string, any>) { if (this.rowClick) this.rowClick(row); }

  formatCell(key: string, row: Record<string, any>): string {
    const raw = row[key];
    const formatter = this.formatters?.[key];
    if (formatter) return formatter(raw, row);
    if (raw === null || raw === undefined) return '';
    return String(raw);
  }
}
