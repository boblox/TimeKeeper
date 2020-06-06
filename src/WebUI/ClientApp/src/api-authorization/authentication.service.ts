import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { IdentityClient, CreateUserCommand, GetTokenCommand } from "../app/TimeKeeper-api";
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
  private currentUserSubject: BehaviorSubject<string>;
  private tokenSubject: BehaviorSubject<string>;
  private accessTokenKey = "access_token";
  private jwtHelper = new JwtHelperService();

  constructor(private identityService: IdentityClient) {
    this.currentUserSubject = new BehaviorSubject<string>(this.getCurrentUserNameFromStorage());
    this.tokenSubject = new BehaviorSubject<string>(this.getTokenFromStorage());
  }

  public register(userName: string, password: string/*, firstName: string, lastName: string*/) {
    return this.identityService.createUser(new CreateUserCommand({ userName, password /*, firstName, lastName  */ }));
  }

  public login(userName: string, password: string) {
    return this.identityService.getUserToken(new GetTokenCommand({ userName, password }))
      .pipe(
        map(token => {
          this.setToken(token);
          this.setUser(this.getCurrentUserNameFromStorage());
        }));
  }

  public logout() {
    this.setUser(null);
    this.setToken(null);
  }

  private setToken(token: string) {
    if (token) {
      localStorage.setItem(this.accessTokenKey, token);
    } else {
      localStorage.removeItem(this.accessTokenKey);
    }
    this.tokenSubject.next(token);
  }

  private setUser(user: string) {
    this.currentUserSubject.next(user);
  }

  private getDecodedToken(): any {
    try {
      return this.jwtHelper.decodeToken(this.getTokenFromStorage());
    }
    catch (err) {
      return null;
    }
  }

  public getCurrentUserNameFromStorage(): string {
    var decodedToken = this.getDecodedToken();
    return decodedToken ? decodedToken.unique_name : null;
  }

  public getCurrentUserRoleFromStorage(): string {
    var decodedToken = this.getDecodedToken();
    return decodedToken ? decodedToken.role : null;
  }

  public getCurrentUser(): Observable<string> {
    return this.currentUserSubject.asObservable();
  }

  public getTokenFromStorage(): string {
    return localStorage.getItem(this.accessTokenKey);
  }

  public getToken(): Observable<string> {
    return this.tokenSubject.asObservable();
  }

  public isAuthenticatedFromStorage(): boolean {
    return !this.jwtHelper.isTokenExpired(this.getTokenFromStorage());
  }

  public isAuthenticated(): Observable<boolean> {
    return this.tokenSubject.asObservable().pipe(map(token => this.isAuthenticatedFromStorage()));
  }
}
