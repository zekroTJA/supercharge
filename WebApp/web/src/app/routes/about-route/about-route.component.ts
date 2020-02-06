/** @format */

import { Component, OnInit, Inject } from '@angular/core';
import { StatusModel } from 'src/app/models/status.model';
import { IAPIService } from 'src/app/services/api/api.interface';

@Component({
  selector: 'app-about-route',
  templateUrl: './about-route.component.html',
  styleUrls: ['./about-route.component.scss'],
})
export class AboutRouteComponent implements OnInit {
  public status: StatusModel;

  constructor(@Inject('APIService') private api: IAPIService) {}

  ngOnInit() {
    this.api.getStatus().subscribe((status) => {
      this.status = status;
    });
  }
}
