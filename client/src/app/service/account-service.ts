import { inject, Injectable, signal } from '@angular/core';
import { tap } from 'rxjs';
import {AccountClient, LoginDto, RegisterDto, UserDto} from './api-client';
import {User} from '../shared/models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private accountService = inject(AccountClient);
  currentUser = signal<User | null>(null);

  register(creds: RegisterDto) {
    return this.accountService.register(creds).pipe(
      tap(user => {
        if (user) {
          this.setCurrentUser(user);
          this.startTokenRefreshInterval();
        }
      })
    )
  }

  login(creds: LoginDto) {
    return this.accountService.login(creds).pipe(
      tap(user => {
        if (user)
          this.setCurrentUser(user);
          this.startTokenRefreshInterval();
      })
    );
  }

  startTokenRefreshInterval() {
    setInterval(() => {
      this.accountService.refreshToken().subscribe({
        next: (user ) => {
          this.setCurrentUser(user);
        },
        error: (err) => {
          this.logout();
        }
      });
    }, 14 * 24 * 60 * 60 * 1000) // 14 days
  }

  logout() {
    this.accountService.logout().subscribe({
      next: () => {
        this.currentUser.set(null);
      }
    });
  }

  setCurrentUser(user: UserDto) {
    user.roles = this.getRolesFromToken(user);
    console.log('logedUser:', user);
    this.currentUser.set(user);
  }

  private getRolesFromToken(user: UserDto): string[] {
    const payload = user.token?.split('.')[1];
    const decoded = payload !== undefined ? atob(payload) : '';
    const jsonPayload = JSON.parse(decoded);
    return Array.isArray(jsonPayload.role) ? jsonPayload.role : [jsonPayload.role]
  }
}
