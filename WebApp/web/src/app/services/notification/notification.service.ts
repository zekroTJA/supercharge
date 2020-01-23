/** @format */

import { Injectable } from '@angular/core';

const COLORS = {
  error: '#f44336',
  info: '#03A9F4',
  warning: '#FF9800',
  success: '#8BC34A',
};

@Injectable({ providedIn: 'root' })
export class NotificationService {
  constructor() {}

  public show(content: string, type: string = 'info', duration: number = 5000) {
    const div = document.createElement('div');
    div.classList.add('notification-bar');
    div.style.backgroundColor = COLORS[type] || COLORS.info;

    const p = document.createElement('p');
    p.innerText = content;
    div.appendChild(p);

    document.body.appendChild(div);

    setTimeout(() => div.classList.add('active'), 100);
    setTimeout(() => div.classList.remove('active'), duration);
    setTimeout(() => document.body.removeChild(div), duration + 250);
  }
}
