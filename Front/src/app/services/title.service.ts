import { Injectable } from '@angular/core';
import { Title } from '@angular/platform-browser';

@Injectable({
  providedIn: 'root',
})
export class TitleService {
  constructor(private title: Title) {}
  setTitle(t: string) {
    this.title.setTitle(t + ' - ProDocs');
  }
}
