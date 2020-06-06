import { Component, Output, EventEmitter } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ReplaySubject } from 'rxjs';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import * as moment from 'moment';

export enum Mode {
  Create,
  Update
}

export interface IWHData {
  date: Date;
  duration: string;
  description: string;
}

@Component({
  selector: 'app-edit-wh-dialog',
  templateUrl: './edit-wh-dialog.component.html',
  styleUrls: ['./edit-wh-dialog.component.scss']
})
export class EditWHDialogComponent {
  formGrp: FormGroup;
  dateCtrl: FormControl;
  durationCtrl: FormControl;
  descriptionCtrl: FormControl;

  mode = Mode.Create;
  loading = false;
  submitted = false;

  readonly title: string = "Set working hours";
  readonly declineText: string = "Cancel";
  timeFormat: string;

  confirm$ = new ReplaySubject<IWHData>();
  decline$ = new ReplaySubject();

  constructor(public bsModalRef: BsModalRef, private fb: FormBuilder) {
    this.dateCtrl = this.fb.control(moment().toDate(), Validators.required);
    this.durationCtrl = this.fb.control(moment().toDate(), Validators.required);
    this.descriptionCtrl = this.fb.control("", Validators.required);
    this.formGrp = this.fb.group({
      date: this.dateCtrl,
      duration: this.durationCtrl,
      description: this.descriptionCtrl,
    });
  }

  get confirmText() {
    return this.mode === Mode.Create ? "Create" : "Update";
  }

  get f() { return this.formGrp.controls; }

  setData(data: IWHData) {
    this.dateCtrl.setValue(data.date);
    this.durationCtrl.setValue(moment(data.duration, this.timeFormat).toDate());
    this.descriptionCtrl.setValue(data.description);
  }

  onDecline(): void {
    this.decline$.next();
  }

  onSubmit() {
    this.submitted = true;
    if (this.formGrp.invalid) {
      return;
    }

    this.loading = true;
    var data = {
      date: <Date>this.dateCtrl.value,
      duration: moment(this.durationCtrl.value).format(this.timeFormat),
      description: <string>this.descriptionCtrl.value
    }
    this.confirm$.next(data);
  }

  reset() {
    this.loading = false;
  }
}
