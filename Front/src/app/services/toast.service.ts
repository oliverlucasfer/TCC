import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface Toast {
  message: string;
  type: 'success' | 'danger' | 'warning' | 'info';
  id: number;
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private toasts = new BehaviorSubject<Toast[]>([]);
  public toasts$ = this.toasts.asObservable();
  private counter = 0;

  private push(message: string, type: Toast['type']): void {
    const id = ++this.counter;
    this.toasts.next([...this.toasts.value, { message, type, id }]);
    setTimeout(() => this.dismiss(id), 4000);
  }

  dismiss(id: number): void {
    this.toasts.next(this.toasts.value.filter((t) => t.id !== id));
  }

  success(message: string): void {
    this.push(message, 'success');
  }

  error(message: string): void {
    this.push(message, 'danger');
  }

  warning(message: string): void {
    this.push(message, 'warning');
  }

  info(message: string): void {
    this.push(message, 'info');
  }
}