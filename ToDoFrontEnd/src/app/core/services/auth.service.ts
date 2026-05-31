// auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private api = 'http://localhost:5261/api';

  constructor(private http: HttpClient) {}

  login(data: { email: string; password: string }) {
    return this.http.post<{ token: string }>(
      `${this.api}/auth/login`,
      data
    ).pipe(
      tap(res => {
        localStorage.setItem('token', res.token);
      })
    );
  }

  register(data: { email: string; password: string }) {
    return this.http.post(`${this.api}/auth/register`, data);
  }

  logout() {
    localStorage.removeItem('token');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isAuth(): boolean {
    return !!this.getToken();
  }
}