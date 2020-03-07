/** @format */

import {
  Component,
  OnInit,
  Inject,
  ViewChild,
  ElementRef,
} from '@angular/core';
import { IAPIService } from 'src/app/services/api/api.interface';
import { StateService } from 'src/app/services/state.service';
import { ActivatedRoute } from '@angular/router';
import { SummonerModel } from 'src/app/models/summoner.model';
import { ChartOptions, ChartDataSets } from 'chart.js';
import { Label } from 'ng2-charts';
import { StatsModel } from 'src/app/models/stats.model';
import { ChampionModel } from 'src/app/models/champion.model';
import dateformat from 'dateformat';
import { NotificationService } from 'src/app/services/notification.service';
import { LoadingBarService } from 'src/app/services/loading-bar.service';

@Component({
  selector: 'app-details-route',
  templateUrl: './details-route.component.html',
  styleUrls: ['./details-route.component.scss'],
})
export class DetailsRouteComponent implements OnInit {
  @ViewChild('inputFrom', { static: true }) public inputFrom: ElementRef;
  @ViewChild('inputTo', { static: true }) public inputTo: ElementRef;

  public summonerName: string;
  public summoner: SummonerModel;
  public stats: StatsModel[];
  public selectedChampions: ChampionModel[];

  public comparing = false;
  public selectedChampionsComparage: ChampionModel[];
  public summonerComparing: SummonerModel;

  public dateFrom: Date = new Date(new Date().getTime() - 30 * 24 * 3600_000);
  public dateTo: Date = new Date();

  public barChartOptions: ChartOptions = {
    responsive: true,
    legend: {
      display: true,
    },
    scales: {
      xAxes: [
        {
          type: 'time',
          time: {
            unit: 'day',
          },
        },
      ],
    },
  };
  public barChartLabels: Label[] = [];
  public barChartData: ChartDataSets[] = [{ data: [] }];

  public lastUpdated: Date;
  public isData: boolean;

  constructor(
    @Inject('APIService') private api: IAPIService,
    public state: StateService,
    private route: ActivatedRoute,
    private notifications: NotificationService,
    private loadingBar: LoadingBarService
  ) {}

  public ngOnInit() {
    this.route.params.subscribe((params) => {
      this.summonerName = params.summonerName;
      this.summoner = this.state.currentSummoner;

      this.inputFrom.nativeElement.value = dateformat(
        this.dateFrom,
        'yyyy-mm-dd'
      );
      this.inputTo.nativeElement.value = dateformat(this.dateTo, 'yyyy-mm-dd');

      this.loadingBar.activate();
      this.api
        .getSummonerStats(this.state.server, this.summonerName)
        .subscribe((stats) => {
          this.stats = stats;
          this.renderChart();
          this.loadingBar.deactivate();
        });

      if (!this.summoner || this.summoner.name !== this.summonerName) {
        this.loadingBar.activate();
        this.api
          .getSummoner(this.state.server, params.summonerName)
          .subscribe((summoner) => {
            this.summoner = summoner;
            this.state.currentSummoner = summoner;
            this.loadingBar.deactivate();
          });
      }
    });
  }

  private renderChart() {
    if (!this.selectedChampions || this.selectedChampions.length < 1) {
      this.selectedChampions = this.stats
        .slice(0, 3)
        .map((s) => this.state.championsMap[s.championId]);
    }
    this.fetchHistory();
  }

  private fetchHistory() {
    this.barChartData = [];
    this.loadingBar.activate();
    this.api
      .getSummonerHistory(
        this.state.server,
        this.summonerName,
        this.selectedChampions.map((c) => c.id),
        this.dateFrom,
        this.dateTo
      )
      .subscribe((history) => {
        const champs = new Set<number>();
        history.forEach((h) => champs.add(h.championId));

        this.barChartData = this.barChartData.concat(
          Array.from(champs).map(
            (cid) =>
              ({
                data: history
                  .filter((h) => h.championId === cid)
                  .map((h) => ({
                    x: new Date(h.timestamp),
                    y: h.championPoints,
                  })),
                label:
                  this.state.championsMap[cid].name +
                  (this.comparing ? ` (${this.summonerName})` : ''),
              } as ChartDataSets)
          )
        );

        this.loadingBar.deactivate();
      });

    if (
      this.comparing &&
      this.summonerComparing &&
      this.selectedChampionsComparage.length > 0
    ) {
      this.loadingBar.activate();

      this.api
        .getSummonerHistory(
          this.state.server,
          this.summonerComparing.name,
          this.selectedChampionsComparage.map((c) => c.id),
          this.dateFrom,
          this.dateTo
        )
        .subscribe((historyComp) => {
          const champs = new Set<number>();
          historyComp.forEach((h) => champs.add(h.championId));
          this.barChartData = this.barChartData.concat(
            Array.from(champs).map(
              (cid) =>
                ({
                  data: historyComp
                    .filter((h) => h.championId === cid)
                    .map((h) => ({
                      x: new Date(h.timestamp),
                      y: h.championPoints,
                    })),
                  label:
                    this.state.championsMap[cid].name +
                    (this.comparing ? ` (${this.summonerComparing.name})` : ''),
                } as ChartDataSets)
            )
          );

          this.loadingBar.deactivate();
        });
    }
  }

  public inputFilter(v: ChampionModel, input: string): boolean {
    input = input.toLowerCase();
    return (
      v.key.toLowerCase().includes(input) ||
      v.name.toLocaleLowerCase().includes(input)
    );
  }

  public inputFormatter(v: ChampionModel): string {
    return v.name;
  }

  public onSelectedChange() {
    this.renderChart();
  }

  public onDateChange(from: Date, to: Date) {
    this.dateFrom = new Date(from);
    this.dateTo = new Date(to);
    this.renderChart();
  }

  public onCompareClick() {
    this.comparing = true;
  }

  public onCompareSummonerChange(v: string) {
    if (!v) {
      this.summonerComparing = null;
      return;
    }
    console.log('TEST');
    this.api
      .getSummoner(this.state.server, v)
      .toPromise()
      .then((summoner) => {
        if (!summoner.registered) {
          this.notifications.show('Summoner is not registered.', 'error');
          return;
        }
        if (summoner.accountId === this.state.currentSummoner.accountId) {
          this.notifications.show(
            'You can not compare you with yourself.',
            'error'
          );
          return;
        }

        this.summonerComparing = summoner;

        this.api
          .getSummonerStats(this.state.server, summoner.name)
          .subscribe((stats) => {
            if (stats.length <= 0) {
              return;
            }

            this.selectedChampionsComparage = [
              this.state.championsMap[stats[0].championId],
            ];

            this.renderChart();
          });
      })
      .catch((err) => {
        if (err.status === 404) {
          this.notifications.show('Summoner could not be found.', 'error');
        }
      });
  }

  public onSelectedComparisonChange() {
    this.renderChart();
  }
}
