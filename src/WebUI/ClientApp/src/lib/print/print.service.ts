import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PrintService {
  private printMode = false;
  private subject = new BehaviorSubject<boolean>(this.printMode);

  constructor() {
  }

  isPrintMode(): Observable<boolean> {
    return this.subject.asObservable();
  }

  setPrintMode(isPrintMode: boolean) {
    this.subject.next(isPrintMode);
  }
}
