import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from "../authentication.service";
import { ApplicationPaths } from "../api-authorization.constants";
import { AlertService } from "../../lib/alert/alert.service";
import { SwaggerException } from "../../app/TimeKeeper-api";
import { controlValuesMustMatch } from "../../lib/control-values-must-match";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  loading = false;
  submitted = false;
  returnUrl: string;

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private authenticationService: AuthenticationService,
    private alertService: AlertService
  ) { }

  async ngOnInit() {
    this.registerForm = this.formBuilder.group({
      //firstName: ['', Validators.required],
      //lastName: ['', Validators.required],
      username: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['']
    }, { validator: controlValuesMustMatch('password', 'confirmPassword') });

    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    if (this.authenticationService.isAuthenticatedFromStorage()) {
      this.router.navigate([this.returnUrl]);
    }
  }

  get f() { return this.registerForm.controls; }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.registerForm.invalid) {
      return;
    }

    this.loading = true;
    this.authenticationService.register(this.f["username"].value, this.f["password"].value/*, this.f["firstName"].value, this.f["lastName"].value*/)
      .subscribe(
        data => {
          this.loading = false;
          this.alertService.success("User was successfully registered!", true);
          this.router.navigate(ApplicationPaths.LoginPathComponents);
        },
        (error: SwaggerException) => {
          this.loading = false;
          this.alertService.errorFromException(error);
        });
  }
}
