/** @format */

import { Component, Inject, OnInit } from '@angular/core';
import { StateService } from 'src/app/services/state.service';
import { IAPIService } from 'src/app/services/api/api.interface';
import { NotificationService } from 'src/app/services/notification.service';
import { Router, ActivatedRoute } from '@angular/router';
import { LoadingBarService } from 'src/app/services/loading-bar.service';
import { LocalStorageService } from 'src/app/services/local-storage.service';
import { changeServer } from 'src/app/shared/server-router';

@Component({
  selector: 'app-main-route',
  templateUrl: './main-route.component.html',
  styleUrls: ['./main-route.component.scss'],
})
export class MainRouteComponent implements OnInit {
  public displaySuggestedSummoners = false;
  public suggestedSummonerNames: string[] = [];

  constructor(
    public state: StateService,
    @Inject('APIService') private api: IAPIService,
    private notifications: NotificationService,
    private router: Router,
    private route: ActivatedRoute,
    private loadingBar: LoadingBarService,
    private localStorage: LocalStorageService
  ) {}

  public ngOnInit() {
    this.route.params.subscribe((params) => {
      if (!this.state.isValidServer(params.server)) {
        this.notifications.show(
          `'${params.server}' is not a valid server! Switched to default server '${this.state.defaultServer}'.`,
          'error'
        );
        changeServer(this.router, this.state.defaultServer);
      } else {
        this.state.server = params.server.toUpperCase();
      }
    });

    this.suggestedSummonerNames = this.localStorage.getSuggestedSummoners();

    window.onclick = (ev: MouseEvent) => {
      const target = ev
        .composedPath()
        .find((et: Element) => et.id === ':summoner-search');

      if (!target) {
        this.displaySuggestedSummoners = false;
      }
    };
  }

  public onSearch(value: string) {
    if (!value) {
      return;
    }

    this.loadingBar.activate();
    this.api
      .getSummoner(this.state.server, value)
      .toPromise()
      .then((summoner) => {
        this.state.currentSummoner = summoner;
        this.localStorage.addSuggestedSummoner(summoner.name);
        this.router.navigate([this.state.server.toLowerCase(), summoner.name]);
      })
      .catch((err) => {
        if (err.status === 404) {
          this.notifications.show(
            `This summoner could not be found on ${this.state.server}!`,
            'error'
          );
        }
      })
      .finally(() => {
        this.loadingBar.deactivate();
      });
  }

  public onSuggestionClick(s: string) {
    this.displaySuggestedSummoners = false;
    this.onSearch(s);
  }

  public onSuggestionRemoveClick(s, e: MouseEvent) {
    e.stopPropagation();
    this.suggestedSummonerNames = this.localStorage.removeSuggestedSummoner(s);
  }
}
