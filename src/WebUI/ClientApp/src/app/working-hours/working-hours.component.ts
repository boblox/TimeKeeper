import { Component, OnInit } from '@angular/core';
import {
  SwaggerException, WorkingHoursSetDto, WorkingHoursClient,
  PreferredWorkingHoursClient,
  UpdatePreferredWorkingHoursCommand,
  WorkingHoursDto,
  IdentityClient,
  UserDto,
  CreateWorkingHoursCommand,
  UpdateWorkingHoursCommand
} from "../TimeKeeper-api";
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { AuthenticationService } from "../../api-authorization/authentication.service";
import { AlertService } from "../../lib/alert/alert.service";
import { first } from 'rxjs/operators';
import { EditPreferredWHDialogComponent } from "./dialogs/edit-preferred-wh-dialog.component";
import * as moment from 'moment';
import { ConfirmDialogComponent } from "../../lib/confirm-dialog/confirm-dialog.component";
import { Roles, ApplicationPaths } from "../../api-authorization/api-authorization.constants";
import { EditWHDialogComponent, Mode } from "./dialogs/edit-wh-dialog.component";
import { Router, ActivatedRoute } from "@angular/router";

@Component({
  selector: 'app-working-hours',
  templateUrl: './working-hours.component.html',
  styleUrls: ['./working-hours.component.scss']
})
export class WorkingHoursComponent implements OnInit {
  public workingHoursSet: WorkingHoursSetDto;
  public users: UserDto[];
  public selectedUser: string;
  public selectedDateFrom: any;
  public selectedDateTo: any;
  private readonly timeFormat = 'HH:mm:ss';
  private readonly shortTimeFormat = 'HH:mm';
  private readonly dateFormat = 'YYYY-MM-DD';

  editPreferredWHModalRef: BsModalRef;
  deleteWHModalRef: BsModalRef;
  editWHModalRef: BsModalRef;

  constructor(
    private identityClient: IdentityClient,
    private whClient: WorkingHoursClient,
    private preferredWHClient: PreferredWorkingHoursClient,
    private modalService: BsModalService,
    private authService: AuthenticationService,
    private alertService: AlertService,
    private route: ActivatedRoute,
    private router: Router) {
  }

  public get workingHoursList() {
    return this.workingHoursSet && this.workingHoursSet.workingHoursList;
  }

  ngOnInit() {
    this.route.queryParams.subscribe(queryParams => {
      this.selectedUser = queryParams['userName'] || this.authService.getCurrentUserNameFromStorage();
      this.selectedDateFrom = queryParams['start'] ? moment(queryParams['start'], this.dateFormat).toDate() : '';
      this.selectedDateTo = queryParams['end'] ? moment(queryParams['end'], this.dateFormat).toDate() : '';
      this.queryForWorkingHours();
    });

    if (this.isAdmin()) {
      this.identityClient.getUsers().subscribe(result => {
        this.users = result;
      }, (error: SwaggerException) => this.alertService.errorFromException(error));
    }
  }

  queryForWorkingHours() {
    this.alertService.clear();
    this.whClient.getList(this.selectedUser, this.selectedDateFrom, this.selectedDateTo)
      .subscribe(result => {
        this.workingHoursSet = result;
      }, (error: SwaggerException) => this.alertService.errorFromException(error));
  }

  updateQueryParams() {
    var queryParams = {
    }
    if (this.selectedUser && this.selectedUser !== this.authService.getCurrentUserNameFromStorage()) {
      queryParams['userName'] = this.selectedUser;
    }
    if (this.selectedDateFrom) {
      queryParams['start'] = moment(this.selectedDateFrom).format(this.dateFormat);
    }
    if (this.selectedDateTo) {
      queryParams['end'] = moment(this.selectedDateTo).format(this.dateFormat);
    }
    this.router.navigate(['working-hours'],
      {
        queryParams: queryParams
      });
  }

  showEditPreferredWHModal() {
    this.alertService.clear();
    const initialState = {
      duration: moment(this.workingHoursSet.preferredWorkingHoursDuration, this.timeFormat).toDate()
    };
    this.editPreferredWHModalRef = this.modalService.show(EditPreferredWHDialogComponent, { initialState });
    const component = <EditPreferredWHDialogComponent>this.editPreferredWHModalRef.content;

    component.confirm$.pipe(first())
      .subscribe((duration) => {
        var durationStr = moment(duration).format(this.timeFormat);
        this.preferredWHClient.update(new UpdatePreferredWorkingHoursCommand({ userName: this.selectedUser, duration: durationStr }))
          .subscribe(
            () => {
              this.queryForWorkingHours();
              this.editPreferredWHModalRef.hide();
            },
            (error: SwaggerException) => {
              this.alertService.errorFromException(error);
              this.editPreferredWHModalRef.hide();
            });
      });

    component.decline$.pipe(first())
      .subscribe(() => {
        this.editPreferredWHModalRef.hide();
      });
  }

  showDeleteWHModal(workingHours: WorkingHoursDto) {
    this.alertService.clear();
    const initialState = {
      title: 'Delete record',
      content: 'Are you sure you want to delete this record?'
    };
    this.deleteWHModalRef = this.modalService.show(ConfirmDialogComponent, { initialState });
    const component = <ConfirmDialogComponent>this.deleteWHModalRef.content;

    component.confirm$.pipe(first())
      .subscribe(() => {
        this.whClient.delete(workingHours.id)
          .subscribe(
            () => {
              this.queryForWorkingHours();
              this.deleteWHModalRef.hide();
            },
            (error: SwaggerException) => {
              this.alertService.errorFromException(error);
              this.deleteWHModalRef.hide();
            });
      });

    component.decline$.pipe(first())
      .subscribe(() => {
        this.deleteWHModalRef.hide();
      });
  }

  showCreateWHModal() {
    this.alertService.clear();
    const initialState = {
      mode: Mode.Create,
      timeFormat: this.timeFormat
    };
    this.editWHModalRef = this.modalService.show(EditWHDialogComponent, { initialState, ignoreBackdropClick: true });
    const component = <EditWHDialogComponent>this.editWHModalRef.content;
    component.setData({
      date: moment().hour(10).toDate(),
      duration: moment("10:00:00", this.timeFormat).format(this.timeFormat),
      description: ''
    });

    component.confirm$
      .subscribe((data) => {
        var command = new CreateWorkingHoursCommand(
          {
            userName: this.selectedUser,
            duration: data.duration,
            date: data.date,
            description: data.description
          });
        this.whClient.create(command)
          .subscribe(
            () => {
              this.queryForWorkingHours();
              this.editWHModalRef.hide();
            },
            (error: SwaggerException) => {
              this.alertService.errorFromException(error);
              component.reset();
            });
      });

    component.decline$.pipe(first())
      .subscribe(() => {
        this.editWHModalRef.hide();
      });
  }

  showUpdateWHModal(workingHours: WorkingHoursDto) {
    this.alertService.clear();
    const initialState = {
      mode: Mode.Update,
      timeFormat: this.timeFormat
    };
    this.editWHModalRef = this.modalService.show(EditWHDialogComponent, { initialState, ignoreBackdropClick: true });
    const component = <EditWHDialogComponent>this.editWHModalRef.content;
    component.setData({
      date: moment(workingHours.date).hour(10).toDate(),
      duration: workingHours.duration,
      description: workingHours.description
    });

    component.confirm$
      .subscribe((data) => {
        var command = new UpdateWorkingHoursCommand(
          {
            duration: data.duration,
            date: data.date,
            description: data.description
          });
        this.whClient.update(workingHours.id, command)
          .subscribe(
            () => {
              this.queryForWorkingHours();
              this.editWHModalRef.hide();
            },
            (error: SwaggerException) => {
              this.alertService.errorFromException(error);
              component.reset();
            });
      });

    component.decline$.pipe(first())
      .subscribe(() => {
        this.editWHModalRef.hide();
      });
  }

  isAdmin() {
    return this.authService.getCurrentUserRoleFromStorage() === Roles.Admin;
  }

  onDateFromChange(date: Date) {
    this.selectedDateFrom = date;
    this.updateQueryParams();
  }

  onDateToChange(date: Date) {
    this.selectedDateTo = date;
    this.updateQueryParams();
  }

  onUserSelected(user: string) {
    this.selectedUser = user;
    this.updateQueryParams();
  }

  exportRecords() {
    this.router.navigate(ApplicationPaths.ExportWorkingHoursPathComponents,
      {
        queryParams: {
          'start': this.selectedDateFrom ? moment(this.selectedDateFrom).format(this.dateFormat) : '',
          'end': this.selectedDateTo ? moment(this.selectedDateTo).format(this.dateFormat) : '',
          'userName': this.selectedUser,
        }
      });
  }

  getShortTime(time: string) {
    return moment(time, this.timeFormat).format(this.shortTimeFormat);
  }

  getTableRowClass(workingHour: WorkingHoursDto) {
    return workingHour.isUnderPreferredWorkingHoursDuration ? 'table-danger' : 'table-success';
  }
}
