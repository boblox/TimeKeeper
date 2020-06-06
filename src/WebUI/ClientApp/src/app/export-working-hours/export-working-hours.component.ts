import { Component } from '@angular/core';
import { SwaggerException, ExportWorkingHoursDto, WorkingHoursClient } from "../TimeKeeper-api";
import { AlertService } from "../../lib/alert/alert.service";
import { ActivatedRoute } from "@angular/router";
import * as moment from 'moment';
import { PrintService } from "../../lib/print/print.service";

@Component({
  selector: 'app-export-working-hours',
  templateUrl: './export-working-hours.component.html',
  styleUrls: ['./export-working-hours.component.scss']
})
export class ExportWorkingHoursComponent {
  public exportedWH: ExportWorkingHoursDto[];
  private readonly timeFormat = 'HH:mm:ss';
  private readonly shortTimeFormat = 'HH:mm';
  private readonly dateFormat = 'YYYY-MM-DD';

  constructor(private route: ActivatedRoute, private client: WorkingHoursClient, private alertService: AlertService, private printService: PrintService) {
    var queryParams = this.route.snapshot.queryParams;
    var startDate = queryParams['start'] ? moment(queryParams['start'], this.dateFormat).toDate() : '';
    var endDate = queryParams['end'] ? moment(queryParams['end'], this.dateFormat).toDate() : '';
    var userName = queryParams['userName'];
    this.printService.setPrintMode(true);

    this.client.export(userName, <any>startDate, <any>endDate).subscribe(result => {
      this.exportedWH = result;
    }, (error: SwaggerException) => this.alertService.errorFromException(error));
  }

  getShortTime(time: string) {
    return moment(time, this.timeFormat).format(this.shortTimeFormat);
  }
}
