/** @format */

import { IAPIService } from './api.interface';
import { CodeModel } from 'src/app/models/code.model';
import { Observable, throwError } from 'rxjs';
import { VersionModel } from 'src/app/models/version.model';
import { ChampionModel } from 'src/app/models/champion.model';
import { SummonerModel } from 'src/app/models/summoner.model';
import { StatsModel } from 'src/app/models/stats.model';
import { EventEmitter, Inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { catchError } from 'rxjs/operators';
import { HistoryModel } from 'src/app/models/history.model';
import { NotificationService } from './notification/notification.service';

const ROOT_URL = environment.production ? '/api' : 'https://localhost:5001';

@Injectable({ providedIn: 'root' })
export class RestAPIService implements IAPIService {
  public error = new EventEmitter<any>();

  private readonly errorCatcher = (err) => {
    this.error.emit(err);
    console.error(err);
    this.notifications.show(`API Error: ${err.status}`, 'error');
    return throwError(err);
  };

  constructor(
    private http: HttpClient,
    private notifications: NotificationService
  ) {}

  public getRegistrationCode(
    server: string,
    summonerName: string
  ): Observable<CodeModel> {
    return this.http
      .get<CodeModel>(`${ROOT_URL}/registration/code/${server}/${summonerName}`)
      .pipe(catchError(this.errorCatcher));
  }

  public postRegistrationWatch(
    server: string,
    summonerName: string
  ): Promise<any> {
    return this.http
      .post(`${ROOT_URL}/registration/watch/${server}/${summonerName}`, {})
      .pipe(catchError(this.errorCatcher))
      .toPromise();
  }

  public postRegistrationUnWatch(
    server: string,
    summonerName: string
  ): Promise<any> {
    return this.http
      .post(`${ROOT_URL}/registration/unwatch/${server}/${summonerName}`, {})
      .pipe(catchError(this.errorCatcher))
      .toPromise();
  }

  public getResourcesVersion(): Observable<VersionModel> {
    return this.http
      .get<VersionModel>(`${ROOT_URL}/resources/version`)
      .pipe(catchError(this.errorCatcher));
  }

  public getResourcesChampions(): Observable<ChampionModel[]> {
    return this.http
      .get<ChampionModel[]>(`${ROOT_URL}/resources/champions`)
      .pipe(catchError(this.errorCatcher));
  }

  public getSummoner(
    server: string,
    summonerName: string
  ): Observable<SummonerModel> {
    return this.http
      .get<SummonerModel>(`${ROOT_URL}/summoner/${server}/${summonerName}`)
      .pipe(catchError(this.errorCatcher));
  }

  public getSummonerStats(
    server: string,
    summonerName: string,
    championNames?: string[]
  ): Observable<StatsModel[]> {
    const params = new HttpParams();
    championNames.forEach((cn) => params.append('championNames', cn));
    return this.http
      .get<StatsModel[]>(
        `${ROOT_URL}/summoner/${server}/${summonerName}/stats`,
        {
          params,
        }
      )
      .pipe(catchError(this.errorCatcher));
  }

  public getSummonerHistory(
    server: string,
    summonerName: string,
    championNames?: string[],
    from?: Date,
    to?: Date
  ): Observable<HistoryModel[]> {
    const params = new HttpParams()
      .set('from', from.toString())
      .set('to', to.toString());
    championNames.forEach((cn) => params.append('championNames', cn));
    return this.http
      .get<HistoryModel[]>(
        `${ROOT_URL}/summoner/${server}/${summonerName}/history`,
        {
          params,
        }
      )
      .pipe(catchError(this.errorCatcher));
  }
}
