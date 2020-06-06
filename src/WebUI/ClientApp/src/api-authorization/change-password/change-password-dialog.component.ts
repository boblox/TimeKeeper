import { Component, } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ReplaySubject } from 'rxjs';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { controlValuesMustMatch } from "../../lib/control-values-must-match";

export interface IPasswordData {
  oldPassword: string;
  newPassword: string;
}

@Component({
  selector: 'app-change-password-dialog',
  templateUrl: './change-password-dialog.component.html'
})
export class ChangePasswordDialogComponent {
  formGrp: FormGroup;
  oldPasswordCtrl: FormControl;
  newPasswordCtrl: FormControl;
  confirmNewPasswordCtrl: FormControl;

  loading = false;
  submitted = false;

  readonly title: string = "Update password";
  readonly confirmText: string = "Update";
  readonly declineText: string = "Cancel";

  confirm$ = new ReplaySubject<IPasswordData>();
  decline$ = new ReplaySubject();

  constructor(public bsModalRef: BsModalRef, private fb: FormBuilder) {
    this.oldPasswordCtrl = this.fb.control('', Validators.required);
    this.newPasswordCtrl = this.fb.control('', [Validators.required, Validators.minLength(6)]);
    this.confirmNewPasswordCtrl = this.fb.control("");
    this.formGrp = this.fb.group({
      oldPassword: this.oldPasswordCtrl,
      newPassword: this.newPasswordCtrl,
      confirmNewPassword: this.confirmNewPasswordCtrl,
    }, { validator: controlValuesMustMatch('newPassword', 'confirmNewPassword') });
  }

  get f() { return this.formGrp.controls; }

  onDecline(): void {
    this.decline$.next();
  }

  onSubmit() {
    this.submitted = true;
    if (this.formGrp.invalid) {
      return;
    }

    this.loading = true;
    var data: IPasswordData = {
      oldPassword: this.oldPasswordCtrl.value,
      newPassword: this.newPasswordCtrl.value
    }
    this.confirm$.next(data);
  }

  reset() {
    this.loading = false;
  }
}
