import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class UiService {
  private menuAberto = new BehaviorSubject<boolean>(false);
  public menuAberto$ = this.menuAberto.asObservable();

  toggleMenu(): void {
    this.menuAberto.next(!this.menuAberto.value);
  }

  fecharMenu(): void {
    this.menuAberto.next(false);
  }
}