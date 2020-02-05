/** @format */

import { Injectable } from '@angular/core';

const K_SUGGESTED_SUMMONERS = 'sc_suggested_summoners';
const K_VERTICAL_CHART = 'sc_vertical_chart';
const K_SERVER = 'sc_server';

@Injectable({
  providedIn: 'root',
})
export class LocalStorageService {
  public set verticalChart(v: boolean) {
    this.set(K_VERTICAL_CHART, v);
  }

  public get verticalChart(): boolean {
    return this.get(K_VERTICAL_CHART);
  }

  public getSuggestedSummoners(): string[] {
    return this.get(K_SUGGESTED_SUMMONERS) || [];
  }

  public addSuggestedSummoner(summonerName: string) {
    let summoners = this.getSuggestedSummoners();
    const i = summoners.indexOf(summonerName);
    if (i > -1) {
      summoners.splice(i, 1);
    }
    summoners = [summonerName].concat(summoners);
    if (summoners.length > 4) {
      summoners.pop();
    }
    this.set(K_SUGGESTED_SUMMONERS, summoners);
  }

  public removeSuggestedSummoner(summonerName: string): string[] {
    const summoners = this.getSuggestedSummoners();
    const i = summoners.indexOf(summonerName);
    if (i === -1) {
      return;
    }
    summoners.splice(i, 1);
    this.set(K_SUGGESTED_SUMMONERS, summoners);
    return summoners;
  }

  public get server(): string {
    return this.get(K_SERVER);
  }

  public set server(v: string) {
    this.set(K_SERVER, v);
  }

  private set<T>(key: string, value: T) {
    window.localStorage.setItem(key, JSON.stringify(value));
  }

  private get<T>(key: string): T {
    const raw = window.localStorage.getItem(key);
    return JSON.parse(raw) as T;
  }
}
