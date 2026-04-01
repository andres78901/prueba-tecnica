import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  imports: [CommonModule],
  selector: 'app-alert',
  template: `
    <div class="alert" *ngIf="message">
      <strong>{{message}}</strong>
      <ul *ngIf="errors?.length">
        <li *ngFor="let e of errors">{{e}}</li>
      </ul>
    </div>
  `
})
export class AlertComponent {
  @Input() message?: string;
  @Input() errors?: string[] | null;
}
