import { Component, TemplateRef } from '@angular/core';
import { UserDto, IdentityClient, UpdateRoleCommand, SwaggerException } from "../TimeKeeper-api";
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { Roles, AllRoles } from "../../api-authorization/api-authorization.constants";
import { AuthenticationService } from "../../api-authorization/authentication.service";
import { AlertService } from "../../lib/alert/alert.service";
import { ConfirmDialogComponent } from "../../lib/confirm-dialog/confirm-dialog.component";
import { first } from 'rxjs/operators';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent {
  public users: UserDto[];
  editRoleModalRef: BsModalRef;
  deleteUserModalRef: BsModalRef;

  selectedUser: UserDto;
  availableRoles: string[];

  editRoleEditor: any = {};

  constructor(private client: IdentityClient, private modalService: BsModalService, private authService: AuthenticationService, private alertService: AlertService) {
    this.client.getUsers().subscribe(result => {
      this.users = result;
    }, (error: SwaggerException) => this.alertService.errorFromException(error));
  }

  showEditRoleModal(user: UserDto, template: TemplateRef<any>): void {
    this.selectedUser = user;

    var isSignedInUserOfRoleUserManager = this.authService.getCurrentUserRoleFromStorage() === Roles.UserManager;

    this.editRoleEditor.role = this.selectedUser.role;
    this.editRoleEditor.availableRoles = AllRoles.filter(role => !(role === Roles.Admin && isSignedInUserOfRoleUserManager));

    this.editRoleModalRef = this.modalService.show(template);
  }

  editRole() {
    this.client.updateUserRole(this.selectedUser.userName, new UpdateRoleCommand({ desiredRole: this.editRoleEditor.role }))
      .subscribe(
        () => {
          this.selectedUser.role = this.editRoleEditor.role;
          this.editRoleModalRef.hide();
          this.editRoleEditor = {};
        },
        (error: SwaggerException) => {
          this.alertService.errorFromException(error);
          this.editRoleModalRef.hide();
        });
  }

  showDeleteUserModal(user: UserDto) {
    const initialState = {
      title: 'Delete user',
      content: 'Are you sure you want to delete this user?'
    };
    this.deleteUserModalRef = this.modalService.show(ConfirmDialogComponent, { initialState });
    const component = <ConfirmDialogComponent>this.deleteUserModalRef.content;

    component.confirm$.pipe(first())
      .subscribe(() => {
        this.client.deleteUser(user.userName)
          .subscribe(
            () => {
              this.users.splice(this.users.indexOf(user), 1);
              this.deleteUserModalRef.hide();
            },
            (error: SwaggerException) => {
              this.alertService.errorFromException(error);
              this.deleteUserModalRef.hide();
            });
      });

    component.decline$.pipe(first())
      .subscribe(() => {
        this.deleteUserModalRef.hide();
      });
  }

  canSetRoleFor(user: UserDto) {
    return !(this.authService.getCurrentUserRoleFromStorage() === Roles.UserManager && user.role === Roles.Admin) &&
      !(this.authService.getCurrentUserNameFromStorage() === user.userName);
  }

  canDelete(user: UserDto) {
    return this.canSetRoleFor(user);
  }
}
