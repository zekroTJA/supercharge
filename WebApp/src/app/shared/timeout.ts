/** @format */

export type Timer = ReturnType<typeof setTimeout>;

export class Timeout {
  private timer: Timer;

  constructor(private delayMS: number) {}

  public cancel() {
    if (this.timer) {
      clearTimeout(this.timer);
    }
  }

  public schedule(cb: () => void) {
    this.cancel();
    this.timer = setTimeout(cb, this.delayMS);
  }
}
