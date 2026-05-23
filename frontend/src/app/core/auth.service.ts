import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { ApiService } from './api.service';
import { AuthResponse, User } from './models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly storageKey = 'gastos-control-session';
  private readonly sessionSubject = new BehaviorSubject<AuthResponse | null>(this.readSession());

  readonly session$ = this.sessionSubject.asObservable();
  readonly user$ = new BehaviorSubject<User | null>(this.sessionSubject.value?.user ?? null);

  constructor(private readonly api: ApiService, private readonly router: Router) {}

  get token(): string | null {
    return this.sessionSubject.value?.accessToken ?? null;
  }

  isAuthenticated(): boolean {
    const session = this.sessionSubject.value;
    if (!session) {
      return false;
    }

    return new Date(session.expiresAt).getTime() > Date.now();
  }

  login(email: string, password: string): Observable<AuthResponse> {
    return this.api.login({ email, password }).pipe(tap((session) => this.setSession(session)));
  }

  register(name: string, email: string, password: string): Observable<AuthResponse> {
    return this.api.register({ name, email, password }).pipe(tap((session) => this.setSession(session)));
  }

  logout(): void {
    localStorage.removeItem(this.storageKey);
    this.sessionSubject.next(null);
    this.user$.next(null);
    this.router.navigateByUrl('/login');
  }

  private setSession(session: AuthResponse): void {
    localStorage.setItem(this.storageKey, JSON.stringify(session));
    this.sessionSubject.next(session);
    this.user$.next(session.user);
  }

  private readSession(): AuthResponse | null {
    const rawSession = localStorage.getItem(this.storageKey);
    if (!rawSession) {
      return null;
    }

    try {
      const session = JSON.parse(rawSession) as AuthResponse;
      if (new Date(session.expiresAt).getTime() <= Date.now()) {
        localStorage.removeItem(this.storageKey);
        return null;
      }

      return session;
    } catch {
      localStorage.removeItem(this.storageKey);
      return null;
    }
  }
}
