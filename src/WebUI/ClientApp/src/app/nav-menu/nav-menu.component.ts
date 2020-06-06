import { Component } from '@angular/core';
import { AuthenticationService } from "../../api-authorization/authentication.service";
import { Roles } from "../../api-authorization/api-authorization.constants";
import { PrintService } from "../../lib/print/print.service";
import { Observable } from 'rxjs';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent {
  isExpanded = false;
  isPrintMode: Observable<boolean>;

  constructor(private authService: AuthenticationService, private printService: PrintService) {
    this.isPrintMode = this.printService.isPrintMode();
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  canSeeUsersTab(): boolean {
    var currentUserRole = this.authService.getCurrentUserRoleFromStorage();
    return currentUserRole === Roles.Admin || currentUserRole === Roles.UserManager;
  }
}
