/** @format */

import {
  Component,
  OnInit,
  Input,
  Output,
  EventEmitter,
  ViewChild,
  ElementRef,
} from '@angular/core';
import { ChampionModel } from 'src/app/models/champion.model';

@Component({
  selector: 'app-tag-select',
  templateUrl: './tag-select.component.html',
  styleUrls: ['./tag-select.component.scss'],
})
export class TagSelectComponent implements OnInit {
  @ViewChild('input', { static: false }) inputChild: ElementRef;

  @Input() values: any[] = [];

  @Input() selected: any[] = [];
  @Output() selectedChage = new EventEmitter<any[]>();

  public suggestions: any[] = [];

  @Input() formatter: (v: any) => string = (v) => v.toString();
  @Input() filter: (v: any, input: string) => boolean = (v, input) =>
    v.toString().toLowerCase() === input.toLowerCase();

  constructor() {}

  public ngOnInit() {}

  public onRemoveClick(i: number) {
    this.selected.splice(i, 1);
    this.selectedChage.emit(this.selected);
  }

  public onInput(v: string) {
    if (!v) {
      this.suggestions = [];
      return;
    }

    this.suggestions = this.values.filter(
      (val) => !this.selected.includes(val) && this.filter(val, v)
    );
  }

  public onSuggestionClick(s: any) {
    this.selected.push(s);
    this.selectedChage.emit(this.selected);
    this.inputChild.nativeElement.value = '';
    this.suggestions = [];
  }
}
