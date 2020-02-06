/** @format */

import { CodeModel } from 'src/app/models/code.model';
import { Observable } from 'rxjs';
import { VersionModel } from 'src/app/models/version.model';
import { ChampionModel } from 'src/app/models/champion.model';
import { SummonerModel } from 'src/app/models/summoner.model';
import { StatsModel } from 'src/app/models/stats.model';
import { EventEmitter } from '@angular/core';
import { HistoryModel } from 'src/app/models/history.model';
import {
  StatusCountsModel,
  StatusVersionsModel,
} from 'src/app/models/status.model';

export interface IAPIService {
  error: EventEmitter<any>;

  getRegistrationCode(
    server: string,
    summonerName: string
  ): Observable<CodeModel>;
  postRegistrationWatch(server: string, summonerName: string): Promise<any>;
  postRegistrationUnWatch(server: string, summonerName: string): Promise<any>;

  getResourcesVersion(): Observable<VersionModel>;
  getResourcesChampions(): Observable<ChampionModel[]>;

  getSummoner(server: string, summonerName: string): Observable<SummonerModel>;
  getSummonerStats(
    server: string,
    summonerName: string,
    championNames?: string[]
  ): Observable<StatsModel[]>;
  getSummonerHistory(
    server: string,
    summonerName: string,
    championNames?: string[],
    from?: Date,
    to?: Date
  ): Observable<HistoryModel[]>;

  getStatusCounts(): Observable<StatusCountsModel>;
  getStatusVersions(): Observable<StatusVersionsModel>;
}
