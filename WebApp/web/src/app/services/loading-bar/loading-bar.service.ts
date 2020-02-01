/** @format */

import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LoadingBarService {
  private _active = false;
  private _activations = 0;

  constructor() {}

  public get active(): boolean {
    return this._active;
  }

  public activate() {
    this.activations++;
  }

  public deactivate() {
    this.activations--;
  }

  private get activations(): number {
    return this._activations;
  }

  private set activations(v: number) {
    if (v < 0) {
      return;
    }

    this._activations = v;

    console.log(this.activations);
    this._active = this.activations > 0;
  }
}
