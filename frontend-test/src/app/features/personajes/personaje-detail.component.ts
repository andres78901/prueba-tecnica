import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { PersonajesApiClient } from '../../api/clients/personajes-api.client';
import { BogotaDatePipe } from '../../core/pipes/bogota-date.pipe';
import { AlertComponent } from '../../shared/components/alert/alert.component';
import { LoadingComponent } from '../../shared/components/loading/loading.component';

@Component({
  standalone: true,
  imports: [CommonModule, BogotaDatePipe, AlertComponent, LoadingComponent],
  template: `
  <div>
    <button class="btn secondary" (click)="router.navigate(['/personajes'])">Volver</button>
    <app-alert *ngIf="error?.message" [message]="error?.message" [errors]="error?.errors"></app-alert>
    <app-loading *ngIf="loading"></app-loading>

    <div *ngIf="item">
      <h2>{{item.nombre}}</h2>
      <img *ngIf="item.imagenUrl" [src]="item.imagenUrl" alt="" style="max-width:180px;border-radius:8px" />
      <p><strong>Estado:</strong> {{item.estado}}</p>
      <p><strong>Especie:</strong> {{item.especie}}</p>
      <p><strong>Origen:</strong> {{item.origen}}</p>
      <p><strong>Fecha import:</strong> {{item.fechaImport | bogotaDate}}</p>
    </div>
  </div>
  `
})
export class PersonajeDetailComponent implements OnInit {
  item: any | null = null;
  loading = false;
  error: any = null;
  constructor(private route: ActivatedRoute, private api: PersonajesApiClient, public router: Router) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loading = true;
    this.api.obtener(id).subscribe({
      next: v => { this.item = v; this.loading = false; },
      error: e => { this.error = e; this.loading = false; }
    });
  }
}
