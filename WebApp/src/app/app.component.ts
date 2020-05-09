/** @format */

import { Component, Inject, ContentChild, TemplateRef } from '@angular/core';
import { StateService } from './services/state.service';
import { IAPIService } from './services/api/api.interface';
import { SummonerModel } from './models/summoner.model';
import { NotificationService } from './services/notification.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  public summoner: SummonerModel;
  public loading = false;

  constructor(
    public state: StateService,
    @Inject('APIService') private api: IAPIService,
    private notifications: NotificationService
  ) {}

  public onSearch(value: string) {
    if (!value) {
      return;
    }

    this.loading = true;
    this.api
      .getSummoner(this.state.server, value)
      .toPromise()
      .then((summoner) => {
        this.summoner = summoner;
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
        this.loading = false;
      });
  }
}
