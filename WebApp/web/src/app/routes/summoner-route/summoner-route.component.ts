/** @format */

import { Component, OnInit, Inject } from '@angular/core';
import { IAPIService } from 'src/app/services/api/api.interface';
import { ActivatedRoute } from '@angular/router';
import { SummonerModel } from 'src/app/models/summoner.model';
import { StateService } from 'src/app/services/api/state/state.service';

@Component({
  selector: 'app-summoner-route',
  templateUrl: './summoner-route.component.html',
  styleUrls: ['./summoner-route.component.scss'],
})
export class SummonerRouteComponent implements OnInit {
  public summoner: SummonerModel;

  constructor(
    @Inject('APIService') private api: IAPIService,
    private state: StateService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.route.params.subscribe((params) => {
      const summonerName = params.summonerName;
      this.summoner = this.state.currentSummoner;

      if (!this.summoner || this.summoner.name !== summonerName) {
        this.api
          .getSummoner(this.state.server, params.summonerName)
          .subscribe((summoner) => {
            this.summoner = summoner;
          });
      }
    });
  }
}
