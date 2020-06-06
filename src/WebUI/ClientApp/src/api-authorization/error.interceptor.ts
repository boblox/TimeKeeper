import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthenticationService } from "./authentication.service";
import { Router } from '@angular/router';
import { ApplicationPaths, QueryParameterNames } from './api-authorization.constants';
import { HttpErrorResponse } from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private router: Router,
    private authenticationService: AuthenticationService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(catchError((err: HttpErrorResponse) => {
      if (err.status === 401 || err.status === 403) {
        // auto logout if 401 or 403 response returned from api
        this.authenticationService.logout();
        this.router.navigate(ApplicationPaths.LoginPathComponents, {
          queryParams: {
            [QueryParameterNames.ReturnUrl]: this.router.url
          }
        });
        console.error(`Server returned 400 or 403 - you've been logged out.`);
      }

      return throwError(err);
    }));
  }
}
