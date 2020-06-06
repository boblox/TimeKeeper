import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { ApplicationPaths, QueryParameterNames } from './api-authorization.constants';
import { AuthenticationService } from "./authentication.service";
import { PrintService } from "../lib/print/print.service";

@Injectable({
  providedIn: 'root'
})
export class AuthorizeGuard implements CanActivate {
  constructor(private authenticationService: AuthenticationService,
    private printService: PrintService,
    private router: Router) {
  }
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    const expectedRoles = route.data.expectedRoles;
    const currentRole = this.authenticationService.getCurrentUserRoleFromStorage();
    const isAuthenticated = this.authenticationService.isAuthenticatedFromStorage();

    this.printService.setPrintMode(false);

    if (!isAuthenticated) {
      this.router.navigate(ApplicationPaths.LoginPathComponents, {
        queryParams: {
          [QueryParameterNames.ReturnUrl]: state.url
        }
      });
      console.log(`Can't navigate to ${state.url}. Unathenticated`);
      return false;
    } else if (expectedRoles && expectedRoles.findIndex(role => role.toLowerCase() === currentRole.toLowerCase()) === -1) {
      console.error(`Can't navigate to ${state.url}. Unathorized`);
      return false;
    }
    return true;
  }
}
