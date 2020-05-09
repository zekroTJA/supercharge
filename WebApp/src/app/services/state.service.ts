/** @format */

import { Injectable, Inject, EventEmitter } from '@angular/core';
import { IAPIService } from './api/api.interface';
import { VersionModel } from 'src/app/models/version.model';
import { ChampionModel } from 'src/app/models/champion.model';
import { SummonerModel } from 'src/app/models/summoner.model';
import { LocalStorageService } from './local-storage.service';

const DEFAULT_SERVER = 'EUW1';

const VALID_SERVERS: string[] = [
  'RU',
  'KR',
  'BR1',
  'OC1',
  'JP1',
  'NA1',
  'EUN1',
  'EUW1',
  'TR1',
  'LA1',
  'LA2',
];

@Injectable({
  providedIn: 'root',
})
export class StateService {
  public version: VersionModel;
  public champions: ChampionModel[];
  public currentSummoner: SummonerModel;
  public championsMap: { [key: number]: ChampionModel } = {};

  public isInitialized = false;
  public initialized = new EventEmitter<any>();

  private _server = 'EUW1';

  constructor(
    @Inject('APIService') api: IAPIService,
    private localStorage: LocalStorageService
  ) {
    this.server = this.localStorage.server || DEFAULT_SERVER;

    api.getResourcesVersion().subscribe((version) => {
      this.version = version;
    });

    api.getResourcesChampions().subscribe((champs) => {
      this.champions = champs;
      champs.forEach((c) => (this.championsMap[c.key] = c));

      this.isInitialized = true;
      this.initialized.emit();
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
    this.localStorage.server = this._server;
  }

  public get validServers(): string[] {
    return VALID_SERVERS;
  }

  public get defaultServer(): string {
    return DEFAULT_SERVER;
  }

  public isValidServer(server: string): boolean {
    return VALID_SERVERS.includes(server.toUpperCase());
  }
}
