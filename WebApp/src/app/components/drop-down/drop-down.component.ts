/** @format */

import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-drop-down',
  templateUrl: './drop-down.component.html',
  styleUrls: ['./drop-down.component.scss'],
})
export class DropDownComponent implements OnInit {
  @Input() public choises: string[] = [];

  @Input() public value: string;
  @Output() public valueChange = new EventEmitter<string>();

  public selectorVisible = false;

  constructor() {}

  public ngOnInit() {
    window.onclick = (ev: MouseEvent) => {
      const target = ev
        .composedPath()
        .find((et: Element) => et.id === ':dropdown');

      if (!target) {
        this.selectorVisible = false;
      }
    };
  }

  public selected(e: string) {
    this.value = e;
    this.valueChange.emit(e);
    this.selectorVisible = false;
  }
}
