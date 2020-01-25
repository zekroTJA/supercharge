/** @format */

import { Pipe, PipeTransform } from '@angular/core';
import dateFormat from 'dateformat';

const DEF_DATE_FORMAT = 'yyyy-mm-dd hh:MM:ss Z';

@Pipe({
  name: 'dateTime',
})
export class DateTimePipe implements PipeTransform {
  public transform(value: Date, ...args: any[]): any {
    return dateFormat(value, args[0] || DEF_DATE_FORMAT);
  }
}
