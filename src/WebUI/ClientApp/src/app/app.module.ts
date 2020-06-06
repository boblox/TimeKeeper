import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms'; //TODO: remove this!
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { ApiAuthorizationModule } from 'src/api-authorization/api-authorization.module';
import { AuthorizeGuard } from 'src/api-authorization/authorize.guard';
import { AuthorizeInterceptor } from 'src/api-authorization/authorize.interceptor';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ModalModule } from 'ngx-bootstrap/modal';
import { TimepickerModule } from 'ngx-bootstrap/timepicker';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { ErrorInterceptor } from "../api-authorization/error.interceptor";
import { LibModule } from "../lib/lib.module";
import { UsersComponent } from "./users/users.component";
import { Roles, ApplicationPaths } from "../api-authorization/api-authorization.constants";
import { WorkingHoursComponent } from "./working-hours/working-hours.component";
import { EditPreferredWHDialogComponent } from "./working-hours/dialogs/edit-preferred-wh-dialog.component";
import { EditWHDialogComponent } from "./working-hours/dialogs/edit-wh-dialog.component";
import { ExportWorkingHoursComponent } from "./export-working-hours/export-working-hours.component";
import { PageNotFoundComponent } from "../lib/page-not-found/page-not-found.component";

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    UsersComponent,
    WorkingHoursComponent,
    ExportWorkingHoursComponent,
    EditPreferredWHDialogComponent,
    EditWHDialogComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    FontAwesomeModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    ApiAuthorizationModule,
    LibModule,
    RouterModule.forRoot([
      //TODO: move them to ApplicationPathes
      { path: '', component: HomeComponent, pathMatch: 'full' },
      {
        path: 'users', component: UsersComponent, canActivate: [AuthorizeGuard],
        data: {
          expectedRoles: [Roles.Admin, Roles.UserManager] 
        } 
      },
      { path: 'working-hours', component: WorkingHoursComponent, canActivate: [AuthorizeGuard] },
      { path: ApplicationPaths.ExportWorkingHours, component: ExportWorkingHoursComponent, canActivate: [AuthorizeGuard] },
      { path: '**', component: PageNotFoundComponent }
    ]),
    BrowserAnimationsModule,
    ModalModule.forRoot(),
    TimepickerModule.forRoot(),
    BsDatepickerModule.forRoot()
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthorizeInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
