/** @format */

import { Component, OnInit, Inject } from '@angular/core';
import { IAPIService } from 'src/app/services/api/api.interface';
import { ActivatedRoute } from '@angular/router';
import { SummonerModel } from 'src/app/models/summoner.model';
import { StateService } from 'src/app/services/state/state.service';
import { ChartOptions, ChartType, ChartDataSets } from 'chart.js';
import { Label } from 'ng2-charts';

@Component({
  selector: 'app-summoner-route',
  templateUrl: './summoner-route.component.html',
  styleUrls: ['./summoner-route.component.scss'],
})
export class SummonerRouteComponent implements OnInit {
  public summoner: SummonerModel;

  public barChartOptions: ChartOptions = {
    responsive: true,
  };

  public barChartLabels: string[] = [];

  public barChartData: ChartDataSets[] = [{ data: [], label: 'Champion' }];

  constructor(
    @Inject('APIService') private api: IAPIService,
    private state: StateService,
    private route: ActivatedRoute
  ) {}

  public ngOnInit() {
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

      if (!this.state.initialized) {
        this.state.initialized.subscribe(() => {
          this.fetchChampData(summonerName);
        });
      } else {
        this.fetchChampData(summonerName);
      }
    });
  }

  public fetchChampData(summonerName: string) {
    this.api
      .getSummonerStats(this.state.server, summonerName)
      .subscribe((res) => {
        console.log(this.state.championsMap);
        this.barChartLabels = res.map(
          (r) => this.state.championsMap[r.championId].name
        );

        this.barChartData[0].data = res.map((r) => r.championPoints);
      });
  }
}
