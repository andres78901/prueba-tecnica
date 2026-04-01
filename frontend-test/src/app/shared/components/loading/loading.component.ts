import { Component, Input } from '@angular/core';

@Component({
  standalone: true,
  selector: 'app-loading',
  template: `<div class="loading">{{text || 'Loading...'}}</div>`
})
export class LoadingComponent { @Input() text?: string }
