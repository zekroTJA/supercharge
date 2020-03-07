/** @format */

import { Component, OnInit, Inject } from '@angular/core';
import { IAPIService } from 'src/app/services/api/api.interface';
import { ActivatedRoute, Router } from '@angular/router';
import { SummonerModel } from 'src/app/models/summoner.model';
import { StateService } from 'src/app/services/state.service';
import { ChartOptions, ChartDataSets } from 'chart.js';
import { Label } from 'ng2-charts';
import { StatsModel } from 'src/app/models/stats.model';
import { GRAPH_COLORS } from 'src/app/const/const';
import { LoadingBarService } from 'src/app/services/loading-bar.service';
import { LocalStorageService } from 'src/app/services/local-storage.service';

@Component({
  selector: 'app-summoner-route',
  templateUrl: './summoner-route.component.html',
  styleUrls: ['./summoner-route.component.scss'],
})
export class SummonerRouteComponent implements OnInit {
  public summoner: SummonerModel;

  public barChartOptions: ChartOptions = {
    // responsive: true,
    // maintainAspectRatio: false,
    legend: {
      display: false,
    },
    scales: {
      xAxes: [
        {
          ticks: {
            autoSkip: false,
          },
        },
      ],
      yAxes: [
        {
          ticks: {
            autoSkip: false,
          },
        },
      ],
    },
  };
  public barChartLabels: Label[] = [];
  public barChartData: ChartDataSets[] = [{ data: [], label: 'Champion' }];

  public lastUpdated: Date;
  public isData: boolean;

  private _verticalChart = false;

  constructor(
    @Inject('APIService') private api: IAPIService,
    private state: StateService,
    private route: ActivatedRoute,
    private router: Router,
    private loadingBar: LoadingBarService,
    private localStorage: LocalStorageService
  ) {}

  public ngOnInit() {
    this._verticalChart = this.localStorage.verticalChart;

    this.route.params.subscribe((params) => {
      const summonerName = params.summonerName;
      this.summoner = this.state.currentSummoner;

      if (!this.summoner || this.summoner.name !== summonerName) {
        this.loadingBar.activate();
        this.api
          .getSummoner(this.state.server, params.summonerName)
          .subscribe((summoner) => {
            this.summoner = summoner;
            this.loadingBar.deactivate();
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
    this.loadingBar.activate();
    this.api
      .getSummonerStats(this.state.server, summonerName)
      .toPromise()
      .then((res) => {
        if (!res || res.length < 1) {
          this.isData = false;
          return;
        }

        this.barChartLabels = res
          .map((r) => this.state.championsMap[r.championId].name)
          .slice(0, 30);

        this.barChartData = [];
        this.barChartData.push({
          data: res.map((r) => r.championPoints),
          backgroundColor: res.map((r) => this.mapbackgroundColor(r, 200)),
          hoverBackgroundColor: res.map((r) => this.mapbackgroundColor(r)),
        });

        this.lastUpdated = res[0].updated;

        this.isData = true;
      })
      .catch(() => {
        this.isData = false;
      })
      .finally(() => this.loadingBar.deactivate());
  }

  public onWatchClick() {
    this.router.navigate(['summoner', this.summoner.name, 'confirm'], {
      queryParams: { action: 'watch' },
    });
  }

  public onUnwatchClick() {
    this.router.navigate(['summoner', this.summoner.name, 'confirm'], {
      queryParams: { action: 'unwatch' },
    });
  }

  public onDetailedClick() {
    this.router.navigate(['summoner', this.summoner.name, 'details']);
  }

  public get verticalChart(): boolean {
    return this._verticalChart;
  }

  public set verticalChart(v: boolean) {
    console.log(v, this._verticalChart);
    if (v === this._verticalChart) {
      return;
    }

    this._verticalChart = v;
    this.localStorage.verticalChart = v;
  }

  private mapbackgroundColor(r: StatsModel, opacity: number = 255) {
    const op = opacity.toString(16);

    switch (r.championLevel) {
      case 7:
        return GRAPH_COLORS.RED + op;
      case 6:
        return GRAPH_COLORS.PINK + op;
      case 5:
        return GRAPH_COLORS.PURPLE + op;
    }
    return GRAPH_COLORS.DEEP_PURPLE + op;
  }
}
