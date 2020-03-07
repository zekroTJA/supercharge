/** @format */

import { Component, OnInit } from '@angular/core';
import { StateService } from 'src/app/services/state.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent implements OnInit {
  constructor(public state: StateService, private router: Router) {}

  public ngOnInit() {}

  public onHeadingClick() {
    this.router.navigate(['/']);
  }
}
