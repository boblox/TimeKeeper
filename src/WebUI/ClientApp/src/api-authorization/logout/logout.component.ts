import { Component, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { ApplicationPaths } from '../api-authorization.constants';
import { AuthenticationService } from "../authentication.service";

@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html',
  styleUrls: ['./logout.component.css']
})
export class LogoutComponent implements OnInit {
  public message = new BehaviorSubject<string>(null);

  constructor(
    private authenticationService: AuthenticationService,
    private activatedRoute: ActivatedRoute,
    private router: Router) { }

  async ngOnInit() {
    var returnUrl = this.activatedRoute.snapshot.queryParams['returnUrl'] || ApplicationPaths.LoginPathComponents;
    this.authenticationService.logout();
    this.router.navigate(returnUrl);
  }
}
