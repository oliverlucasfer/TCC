import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../models/identity/User';
import { UserUpdate } from '../models/identity/UserUpdate';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private currentUserSource = new ReplaySubject<User>(1);
  public currentUser$ = this.currentUserSource.asObservable();

  baseUrl = environment.apiURL + 'api/account/';

  constructor(private http: HttpClient) {}

  public login(model: any): Observable<void> {
    return this.http.post<User>(this.baseUrl + 'login', model).pipe(
      take(1),
      map((response: User) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  public register(model: any): Observable<void> {
    return this.http.post<User>(this.baseUrl + 'register', model).pipe(
      take(1),
      map((response: User) => {
        const user = response;
        if (user && user.token) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  public logout(): void {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }

  public getToken(): string | null {
    const stored = localStorage.getItem('user');
    if (!stored) return null;
    try {
      const user = JSON.parse(stored) as User;
      return user.token ?? null;
    } catch {
      return null;
    }
  }

  public isLoggedIn(): boolean {
    const user = this.currentUserValue();
    return !!user && this.isTokenValid(user.token);
  }

  public currentUserValue(): User | null {
    const stored = localStorage.getItem('user');
    if (!stored) return null;
    try {
      return JSON.parse(stored) as User;
    } catch {
      return null;
    }
  }

  private isTokenValid(token?: string): boolean {
    if (!token) return false;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const exp = payload.exp;
      if (!exp) return false;
      return exp * 1000 > Date.now();
    } catch {
      return false;
    }
  }

  public setCurrentUser(user: User): void {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  getUser() {
    return this.http.get<UserUpdate[]>(this.baseUrl + 'GetUser', ).pipe(take(1));
  }

  updateUser(model: UserUpdate): Observable<void> {
    return this.http.post<UserUpdate>(this.baseUrl + 'UpdateUser', model).pipe(
      take(1),
      map((user: UserUpdate) => {
        this.setCurrentUser(user);
      })
    );
  }
}
