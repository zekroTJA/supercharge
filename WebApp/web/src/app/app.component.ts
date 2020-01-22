/** @format */

import { Component } from '@angular/core';
import { StateService } from './services/api/state/state.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  title = 'WebApp';

  constructor(public state: StateService) {}
}
