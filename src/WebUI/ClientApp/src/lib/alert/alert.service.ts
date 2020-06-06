import { Injectable } from '@angular/core';
import { Router, NavigationStart } from '@angular/router';
import { Observable, Subject } from 'rxjs';
import { SwaggerException } from "../../app/TimeKeeper-api";

@Injectable({ providedIn: 'root' })
export class AlertService {
  private subject = new Subject<any>();
  private keepAfterRouteChange = false;

  constructor(private router: Router) {
    // clear alert messages on route change unless 'keepAfterRouteChange' flag is true
    this.router.events.subscribe(event => {
      if (event instanceof NavigationStart) {
        if (this.keepAfterRouteChange) {
          // only keep for a single route change
          this.keepAfterRouteChange = false;
        } else {
          // clear alert message
          this.clear();
        }
      }
    });
  }

  getAlert(): Observable<any> {
    return this.subject.asObservable();
  }

  success(message: string, keepAfterRouteChange = false) {
    this.keepAfterRouteChange = keepAfterRouteChange;
    this.subject.next({ type: 'success', text: message });
  }

  error(message: string, keepAfterRouteChange = false) {
    this.keepAfterRouteChange = keepAfterRouteChange;
    this.subject.next({ type: 'error', text: message });
  }

  errorFromException(exception: SwaggerException, keepAfterRouteChange = false) {
    this.keepAfterRouteChange = keepAfterRouteChange;
    let message = "";
    if (exception.status === 400) {
      var errors = <IException400>(JSON.parse(exception.response));
      for (let key in errors) {
        if (errors.hasOwnProperty(key)) {
          for (let error of errors[key]) {
            message += error + "\n";
          }
        }
      }
    }
    else if (exception.status === 500) {
      var errorDetails = <IException500>(JSON.parse(exception.response));
      message = errorDetails.title;
    }
    else {
      message = exception.message;
    }
    this.subject.next({ type: 'error', text: message });
  }

  clear() {
    // clear by calling subject.next() without parameters
    this.subject.next();
  }
}

interface IException400 {
  [id: string]: string[];
}

interface IException500 {
  status: any;
  title: string;
  type: any;
}
