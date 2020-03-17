/** @format */

import { Component, OnInit, Inject } from '@angular/core';
import { IAPIService } from 'src/app/services/api/api.interface';
import { StateService } from 'src/app/services/state.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CodeModel } from 'src/app/models/code.model';
import { NotificationService } from 'src/app/services/notification.service';

@Component({
  selector: 'app-confirm-route',
  templateUrl: './confirm-route.component.html',
  styleUrls: ['./confirm-route.component.scss'],
})
export class ConfirmRouteComponent implements OnInit {
  private summonerName: string;
  private action: string;
  public code: CodeModel;

  constructor(
    @Inject('APIService') private api: IAPIService,
    private state: StateService,
    private route: ActivatedRoute,
    private router: Router,
    private notifications: NotificationService
  ) {}

  public ngOnInit() {
    this.route.params.subscribe((params) => {
      this.summonerName = params.summonerName;
      this.state.server = params.server.toUpperCase();

      this.api
        .getRegistrationCode(this.state.server, this.summonerName)
        .subscribe((code) => {
          this.code = code;
        });
    });

    this.route.queryParams.subscribe((query) => {
      this.action = query.action;
    });
  }

  public onConfirmClick() {
    console.log(this.action);
    switch (this.action) {
      case 'watch':
        this.actionWatch();
        break;

      case 'unwatch':
        this.actionUnwatch();
        break;
    }
  }

  private actionWatch() {
    this.api
      .postRegistrationWatch(this.state.server, this.summonerName)
      .then(() => {
        this.notifications.show('Successfully set to watching.', 'success');
        this.state.currentSummoner.watch = true;
        this.router.navigate([
          this.state.server.toLowerCase(),
          this.summonerName,
        ]);
      })
      .catch((err) => {
        this.notifications.show(`Code is invalid (${err.status}).`, 'error');
      });
  }

  private actionUnwatch() {
    this.api
      .postRegistrationUnWatch(this.state.server, this.summonerName)
      .then(() => {
        this.notifications.show('Successfully set to not watching.', 'success');
        this.state.currentSummoner.watch = false;
        this.router.navigate([
          this.state.server.toLowerCase(),
          this.summonerName,
        ]);
      })
      .catch((err) => {
        this.notifications.show(`Code is invalid (${err.status}).`, 'error');
      });
  }
}
