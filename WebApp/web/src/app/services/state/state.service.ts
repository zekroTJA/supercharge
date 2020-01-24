/** @format */

import { Injectable, Inject } from '@angular/core';
import { IAPIService } from '../api/api.interface';
import { VersionModel } from 'src/app/models/version.model';
import { ChampionModel } from 'src/app/models/champion.model';
import { SummonerModel } from 'src/app/models/summoner.model';

const DEFAULT_SERVER = 'EUW1';

@Injectable({
  providedIn: 'root',
})
export class StateService {
  public version: VersionModel;
  public champions: ChampionModel[];
  public currentSummoner: SummonerModel;

  private _server = 'EUW1';

  constructor(@Inject('APIService') api: IAPIService) {
    this.server = localStorage.getItem('server') || DEFAULT_SERVER;

    api.getResourcesVersion().subscribe((version) => {
      this.version = version;
    });

    api.getResourcesChampions().subscribe((champs) => {
      this.champions = champs;
    });
  }

  public get server() {
    return this._server;
  }

  public set server(v: string) {
    if (v === this._server) {
      return;
    }

    this._server = v;
    window.localStorage.setItem('server', v);
  }
}