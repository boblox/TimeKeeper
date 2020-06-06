import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from "../authentication.service";
import { ApplicationPaths } from "../api-authorization.constants";
import { AlertService } from "../../lib/alert/alert.service";
import { SwaggerException, CreateWorkingHoursCommand, IdentityClient, ChangePasswordForCurrentUserCommand } from "../../app/TimeKeeper-api";
import { ChangePasswordDialogComponent } from "../change-password/change-password-dialog.component";
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  registerForm: FormGroup;
  loading = false;
  submitted = false;
  returnUrl: string;
  changePasswordModalRef: BsModalRef;

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private authenticationService: AuthenticationService,
    private identityClient: IdentityClient,
    private alertService: AlertService,
    private modalService: BsModalService,
  ) { }

  async ngOnInit() {
    this.registerForm = this.formBuilder.group({
      //firstName: ['', Validators.required],
      //lastName: ['', Validators.required],
      username: [{ value: this.authenticationService.getCurrentUserNameFromStorage(), disabled: true }, Validators.required],
    });
  }

  get f() { return this.registerForm.controls; }

  showUpdatePasswordModal() {
    this.alertService.clear();
    this.changePasswordModalRef = this.modalService.show(ChangePasswordDialogComponent, { ignoreBackdropClick: true });
    const component = <ChangePasswordDialogComponent>this.changePasswordModalRef.content;

    component.confirm$
      .subscribe((data) => {
        var command = new ChangePasswordForCurrentUserCommand(
          {
            oldPassword: data.oldPassword,
            newPassword: data.newPassword
          });
        this.identityClient.changePasswordForCurrentUser(command)
          .subscribe(
            () => {
              this.changePasswordModalRef.hide();
              this.alertService.success("Password was successfully changed!");
            },
            (error: SwaggerException) => {
              this.alertService.errorFromException(error);
              component.reset();
            });
      });

    component.decline$
      .subscribe(() => {
        this.changePasswordModalRef.hide();
      });
  }
}
