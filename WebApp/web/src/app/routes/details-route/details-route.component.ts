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
import { ActivatedRoute, Router, Params, UrlSerializer } from '@angular/router';
import { SummonerModel } from 'src/app/models/summoner.model';
import { ChartOptions, ChartDataSets } from 'chart.js';
import { Label } from 'ng2-charts';
import { StatsModel } from 'src/app/models/stats.model';
import { ChampionModel } from 'src/app/models/champion.model';
import dateformat from 'dateformat';
import { NotificationService } from 'src/app/services/notification.service';
import { LoadingBarService } from 'src/app/services/loading-bar.service';
import { Location } from '@angular/common';
import { Timeout } from 'src/app/shared/timeout';

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
    elements: {
      line: {
        tension: 0,
      },
    },
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

  private requestTimeout = new Timeout(250);

  constructor(
    @Inject('APIService') private api: IAPIService,
    public state: StateService,
    private router: Router,
    private route: ActivatedRoute,
    private location: Location,
    private urlSerializer: UrlSerializer,
    private notifications: NotificationService,
    private loadingBar: LoadingBarService
  ) {}

  public ngOnInit() {
    this.route.params.subscribe((params) => {
      this.summonerName = params.summonerName;
      this.summoner = this.state.currentSummoner;
      this.state.server = params.server.toUpperCase();

      this.route.queryParams.subscribe((queryParams) => {
        this.fromQueryParams(queryParams, () => {
          this.inputFrom.nativeElement.value = dateformat(
            this.dateFrom,
            'yyyy-mm-dd'
          );

          this.inputTo.nativeElement.value = dateformat(
            this.dateTo,
            'yyyy-mm-dd'
          );

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
      });
    });
  }

  private renderChart() {
    if (!this.selectedChampions || this.selectedChampions.length < 1) {
      this.selectedChampions = this.stats
        .slice(0, 3)
        .map((s) => this.state.championsMap[s.championId]);
    }
    this.setQueryParams();
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
                  .filter((h) => h.championId === cid && !h.predicted)
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

        if (history.find((h) => h.predicted)) {
          this.barChartData = this.barChartData.concat(
            Array.from(champs).map(
              (cid) =>
                ({
                  data: history
                    .filter((h) => h.championId === cid && h.predicted)
                    .map((h) => ({
                      x: new Date(h.timestamp),
                      y: h.championPoints,
                    })),
                  label:
                    this.state.championsMap[cid].name +
                    (this.comparing ? ` (${this.summonerName})` : ''),
                  fill: false,
                  pointRadius: 1,
                  borderWidth: 3,
                  borderDash: [0, 6],
                  borderColor: '#ccc',
                  borderCapStyle: 'round',
                } as ChartDataSets)
            )
          );
        }

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
                    .filter((h) => h.championId === cid && !h.predicted)
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

          if (historyComp.find((h) => h.predicted)) {
            this.barChartData = this.barChartData.concat(
              Array.from(champs).map(
                (cid) =>
                  ({
                    data: historyComp
                      .filter((h) => h.championId === cid && h.predicted)
                      .map((h) => ({
                        x: new Date(h.timestamp),
                        y: h.championPoints,
                      })),
                    label:
                      this.state.championsMap[cid].name +
                      (this.comparing
                        ? ` (${this.summonerComparing.name})`
                        : ''),
                    fill: false,
                    pointRadius: 1,
                    borderWidth: 3,
                    borderDash: [0, 6],
                    borderColor: '#ccc',
                    borderCapStyle: 'round',
                  } as ChartDataSets)
              )
            );
          }

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
    this.requestTimeout.schedule(() => {
      this.renderChart();
    });
  }

  public onDateChange(from: string, to: string) {
    this.dateFrom = new Date(from);
    this.dateTo = new Date(to + ' 23:59:59');
    this.requestTimeout.schedule(() => {
      this.renderChart();
    });
  }

  public onCompareClick() {
    this.comparing = true;
  }

  public onCompareSummonerChange(v: string) {
    if (!v) {
      this.summonerComparing = null;
      return;
    }
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

            console.log('test', this.summonerComparing);
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
    this.requestTimeout.schedule(() => {
      this.renderChart();
    });
  }

  private setQueryParams() {
    const queryParams = {
      from: dateformat(this.dateFrom, 'yyyy-mm-dd'),
      to: dateformat(this.dateTo, 'yyyy-mm-dd'),
      champions: this.selectedChampions.map((c) => c.key).join(','),
      compare: null,
      compareChampions: null,
    };

    if (this.summonerComparing) {
      queryParams.compare = this.summonerComparing.name;
    }

    if (this.selectedChampionsComparage) {
      queryParams.compareChampions = this.selectedChampionsComparage
        .map((c) => c.key)
        .join(',');
    }

    const urlTree = this.router.createUrlTree([], { queryParams });
    this.location.go(this.urlSerializer.serialize(urlTree));
  }

  private fromQueryParams(queryParams: Params, cb: () => void = () => {}) {
    if (!this.state.isInitialized) {
      this.state.initialized.subscribe(() =>
        this.fromQueryParams(queryParams, cb)
      );
      return;
    }

    if (queryParams.from) {
      this.dateFrom = new Date(queryParams.from);
    }

    if (queryParams.to) {
      this.dateTo = new Date(queryParams.to);
    }

    if (queryParams.champions) {
      this.selectedChampions = queryParams.champions
        .split(',')
        .map((id: string) => this.state.championsMap[parseInt(id, 10)]);
    }

    if (queryParams.compareChampions) {
      this.selectedChampionsComparage = queryParams.compareChampions
        .split(',')
        .map((id: string) => this.state.championsMap[parseInt(id, 10)]);
      console.log(this.selectedChampionsComparage);
    }

    if (queryParams.compare) {
      this.comparing = true;
      this.onCompareSummonerChange(queryParams.compare);
    }

    cb();
  }
}
