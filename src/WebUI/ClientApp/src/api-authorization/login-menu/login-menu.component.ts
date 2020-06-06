import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthenticationService } from "../authentication.service";

@Component({
  selector: 'app-login-menu',
  templateUrl: './login-menu.component.html',
  styleUrls: ['./login-menu.component.scss']
})
export class LoginMenuComponent implements OnInit {
  public isAuthenticated: Observable<boolean>;
  public userName: Observable<string>;

  constructor(private authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.isAuthenticated = this.authenticationService.isAuthenticated();
    this.userName = this.authenticationService.getCurrentUser();
  }
}
