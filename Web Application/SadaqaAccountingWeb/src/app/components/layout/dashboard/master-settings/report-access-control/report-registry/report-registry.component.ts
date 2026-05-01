import { Component, OnInit } from "@angular/core";
import { ReportRegistryListComponent } from "./report-registry-list/report-registry-list.component";
import { ReportRegistryCreateComponent } from "./report-registry-create/report-registry-create.component";
import { NgxSpinnerModule } from "ngx-spinner";
import { CheckPermissionDirective } from "../../../../../../../identity/directive/check-permission.directive";
import { AccessControlService } from "../../../../../../../identity/services/access-control.service";
import { ReportRegistryService } from "../../../../../../../api/base-api";
import { CommonModule } from "@angular/common";

@Component({
  selector: "app-report-registry",
  standalone: true,
  imports: [
    ReportRegistryListComponent,
    ReportRegistryCreateComponent,
    NgxSpinnerModule,
    CheckPermissionDirective,
    CommonModule
  ],
  templateUrl: "./report-registry.component.html",
  styleUrl: "./report-registry.component.scss",
  providers: [ReportRegistryService]
})
export class ReportRegistryComponent implements OnInit {
  //List Component Reload
  showListComponent: boolean = true;

  constructor(private accessControlService: AccessControlService) {}

  ngOnInit() {
    this.accessControlService.setPermissions();
  }

  onReportRegistryCreated() {
    this.showListComponent = false;
    setTimeout(() => {
      this.showListComponent = true; // re-initialize list component
    });
  }
}
