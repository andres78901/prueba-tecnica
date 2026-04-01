import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { ToastComponent } from './shared/components/toast/toast.component';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, ToastComponent],
  template: `
    <div class="app-shell">
      <div class="header">
        <h1>App - Personajes y Solicitudes</h1>
        <nav class="nav">
          <a routerLink="/personajes" routerLinkActive="active" [routerLinkActiveOptions]="{ exact: true }">Personajes</a>
          <a routerLink="/solicitudes" routerLinkActive="active" [routerLinkActiveOptions]="{ exact: true }">Solicitudes</a>
          <a routerLink="/reportes" routerLinkActive="active" [routerLinkActiveOptions]="{ exact: true }">Reportes</a>
        </nav>
      </div>
      <app-toast></app-toast>
      <router-outlet></router-outlet>
    </div>
  `,
  styles: [``]
})
export class App {}
