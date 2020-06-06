import { Component, Output, EventEmitter } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ReplaySubject } from 'rxjs';

@Component({
  selector: 'app-edit-preferred-wh-dialog',
  templateUrl: './edit-preferred-wh-dialog.component.html'
})
export class EditPreferredWHDialogComponent {
  title: string = "Set preferred working hours";
  duration: Date = new Date();
  confirmText: string = "Update";
  declineText: string = "Cancel";

  confirm$ = new ReplaySubject<Date>();
  decline$ = new ReplaySubject();

  constructor(public bsModalRef: BsModalRef) { }

  onConfirm(): void {
    this.confirm$.next(this.duration);
  }

  onDecline(): void {
    this.decline$.next();
  }
}
