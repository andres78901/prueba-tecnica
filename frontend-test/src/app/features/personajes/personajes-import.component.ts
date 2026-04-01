import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PersonajesApiClient } from '../../api/clients/personajes-api.client';
import { Router } from '@angular/router';
import { AlertComponent } from '../../shared/components/alert/alert.component';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule, AlertComponent],
  template: `
  <div class="card max-w-2xl mx-auto">
    <div class="flex justify-between items-center mb-4">
      <h3 class="text-lg font-semibold">Importar Personajes</h3>
      <button class="btn-secondary" (click)="router.navigate(['/personajes'])">Volver</button>
    </div>

    <div class="space-y-4">
      <div>
        <label class="form-label">External IDs (coma separados)</label>
        <input class="form-input" placeholder="e.g. 1,2,3" [(ngModel)]="externalIdsText" />
      </div>

      <div>
        <label class="form-label">Max páginas API</label>
        <input class="form-input" placeholder="Max páginas" type="number" [(ngModel)]="maxPaginasApi" />
      </div>

      <div class="flex gap-3">
        <button class="btn-primary" (click)="import()">Importar</button>
        <button class="btn-secondary" (click)="externalIdsText=''; maxPaginasApi=null">Limpiar</button>
      </div>

      <app-alert *ngIf="resultMessage" [message]="resultMessage" [errors]="resultErrors"></app-alert>
    </div>
  </div>
  `
})
export class PersonajesImportComponent {
  externalIdsText = '';
  maxPaginasApi: number | null = null;
  resultMessage?: string;
  resultErrors?: string[] | null;
  constructor(private api: PersonajesApiClient, public router: Router) {}

  import() {
    this.resultMessage = undefined; this.resultErrors = undefined;
    const ids = this.externalIdsText ? this.externalIdsText.split(',').map(s => Number(s.trim())).filter(Boolean) : undefined;
    this.api.importar({ externalIds: ids, maxPaginasApi: this.maxPaginasApi || undefined }).subscribe({ next: r => this.resultMessage = `Importados: ${r.importados}`, error: (e) => { this.resultMessage = e.message; this.resultErrors = e.errors || undefined; } });
  }
}
