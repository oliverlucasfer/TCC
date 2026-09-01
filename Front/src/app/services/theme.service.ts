import { Injectable } from '@angular/core';

const THEME_KEY = 'prodocs-theme';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private dark = false;

  constructor() {
    const saved = localStorage.getItem(THEME_KEY);
    this.dark = saved === 'dark';
    this.apply();
  }

  public isDark(): boolean {
    return this.dark;
  }

  public toggle(): void {
    this.dark = !this.dark;
    localStorage.setItem(THEME_KEY, this.dark ? 'dark' : 'light');
    this.apply();
  }

  private apply(): void {
    document.body.setAttribute('data-bs-theme', this.dark ? 'dark' : 'light');
  }
}