import { Component, Output, EventEmitter } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ReplaySubject } from 'rxjs';

@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html'
})
export class ConfirmDialogComponent {
  title: string = "Action";
  content: string = "Are you sure you want to make this action?";
  confirmText: string = "Yes";
  declineText: string = "No";
  confirm$ = new ReplaySubject();
  decline$ = new ReplaySubject();

  constructor(public bsModalRef: BsModalRef) { }

  onConfirm(): void {
    this.confirm$.next();
  }

  onDecline(): void {
    this.decline$.next();
  }

  hide() {
    this.bsModalRef.hide();
  }
}
