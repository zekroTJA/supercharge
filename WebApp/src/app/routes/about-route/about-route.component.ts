/** @format */

import { Component, OnInit, Inject } from '@angular/core';
import {
  StatusCountsModel,
  StatusVersionsModel,
} from 'src/app/models/status.model';
import { IAPIService } from 'src/app/services/api/api.interface';

@Component({
  selector: 'app-about-route',
  templateUrl: './about-route.component.html',
  styleUrls: ['./about-route.component.scss'],
})
export class AboutRouteComponent implements OnInit {
  public counts: StatusCountsModel;
  public versions: StatusVersionsModel;

  constructor(@Inject('APIService') private api: IAPIService) {}

  ngOnInit() {
    this.api.getStatusCounts().subscribe((counts) => {
      this.counts = counts;
    });

    this.api.getStatusVersions().subscribe((versions) => {
      this.versions = versions;
    });
  }
}
