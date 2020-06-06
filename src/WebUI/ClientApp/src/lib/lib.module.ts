import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AlertComponent } from "./alert/alert.component";
import { ConfirmDialogComponent } from "./confirm-dialog/confirm-dialog.component";
import { PageNotFoundComponent } from "./page-not-found/page-not-found.component";

@NgModule({
  declarations: [
    AlertComponent,
    ConfirmDialogComponent,
    PageNotFoundComponent
  ],
  imports: [CommonModule],
  exports: [
    AlertComponent,
    ConfirmDialogComponent,
    PageNotFoundComponent
  ]
})
export class LibModule { }
