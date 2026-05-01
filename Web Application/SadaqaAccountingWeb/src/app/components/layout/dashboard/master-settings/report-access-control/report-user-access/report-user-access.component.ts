import { CommonModule, isPlatformBrowser } from "@angular/common";
import {
  AfterViewInit,
  Component,
  ElementRef,
  Inject,
  OnInit,
  PLATFORM_ID,
  ViewChild
} from "@angular/core";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import {
  ReportRegistryUserAccessGridModel,
  ReportUserAccessCreateModel,
  ReportUserAccessGridModel,
  ReportUserAccessService,
  UserService,
  UserViewModel
} from "../../../../../../../api/base-api";
import { FormsModule } from "@angular/forms";
import { ToastrService } from "ngx-toastr";
import { Router, RouterLink } from "@angular/router";
import TomSelect from "tom-select";
import { SelectModel } from "../../../../../../shared/models/select-model";

@Component({
  selector: "app-report-user-access",
  standalone: true,
  imports: [NgxSpinnerModule, CommonModule, FormsModule, RouterLink],
  templateUrl: "./report-user-access.component.html",
  styleUrl: "./report-user-access.component.scss",
  providers: [ReportUserAccessService, UserService]
})
export class ReportUserAccessComponent implements OnInit, AfterViewInit {
  isBrowser: boolean = false;

  dataTable: any;
  isTableReady: boolean = false;

  @ViewChild("existUserId") existUserId: ElementRef;
  existUserIdTomSelect: any;

  // Select list
  users: SelectModel[] = [];
  reportUserAccessCreateModel: ReportUserAccessCreateModel = new ReportUserAccessCreateModel();
  existingUsers: ReportUserAccessGridModel[] = [];

  reportRegistryUserAccessGridModel: ReportRegistryUserAccessGridModel[] = [];
  selectedReportRegistryUserAccessGridModel: ReportRegistryUserAccessGridModel[] = [];
  selectedReportIds: number[] = [];

  constructor(
    private reportUserAccessService: ReportUserAccessService,
    private userService: UserService,
    private spinnerService: NgxSpinnerService,
    private toastrService: ToastrService,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: object
  ) {}

  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);
    this.reportUserAccessCreateModel.userId = "-1";
    this.getSelectList();
    this.getReportRegistryList();
    this.getAllReportUserAccessGroupByUserId();
  }

  ngAfterViewInit() {
    if (this.isBrowser) {
      setTimeout(() => {
        ($(".select2") as any).select2({
          placeholder: "Choose...",
          width: "100%" // Optional, ensures it stretches full width
        });

        $(".select2").on("change", (e) => {
          const name = (e.target as HTMLSelectElement).name;
          const event = (e.target as HTMLSelectElement).value;
          if (event != "") {
            this.spinnerService.show();
            this.reportUserAccessCreateModel.userId = event;
            this.getReportRegistryList();
            this.reportUserAccessService.getReportUserAccessesByUser(event).subscribe(
              (result: ReportRegistryUserAccessGridModel[]) => {
                this.selectedReportRegistryUserAccessGridModel = result || [];
                if (this.selectedReportRegistryUserAccessGridModel.length > 0) {
                  this.reportRegistryUserAccessGridModel =
                    this.reportRegistryUserAccessGridModel.map((item) => {
                      const isSelected = this.selectedReportRegistryUserAccessGridModel.some(
                        (selected) => selected.reportId === item.reportId
                      );

                      return {
                        ...item,
                        isChecked: isSelected
                      } as ReportRegistryUserAccessGridModel;
                    });

                  this.getSelectedReportIds();
                } else {
                  // Usage:
                  this.getReportRegistryList();
                }
                this.spinnerService.hide();
              },
              (error: any) => {
                this.spinnerService.hide();
              }
            );
          } else {
            this.getReportRegistryList();
          }
        });
      }, 0);
    }
  }

  private getAllReportUserAccessGroupByUserId() {
    this.spinnerService.show();
    this.reportUserAccessService.getAllReportUserAccessGroupByUserId().subscribe(
      (result: ReportUserAccessGridModel[]) => {
        this.existingUsers = result;

        setTimeout(() => {
          this.initializeTomSelectForExistUser();
        }, 0);
        this.spinnerService.hide();
      },
      (error: any) => {
        this.spinnerService.hide();
      }
    );
  }

  private initializeTomSelectForExistUser() {
    this.existUserIdTomSelect = new TomSelect(this.existUserId.nativeElement, {
      placeholder: "Choose an user",
      plugins: ["remove_button"],
      onChange: (value: string) => {
        if (!value) {
          this.clearUser();
        } else {
          this.ChangeUser();
        }
      }
    });

    this.existUserIdTomSelect.setValue(null);
  }

  clearUser() {
    this.reportRegistryUserAccessGridModel.forEach((report) => {
      report.isChecked = false;
    });
  }

  ChangeUser() {
    const selectedId: string = this.existUserIdTomSelect.getValue();
    this.spinnerService.show();

    const copiedUser = this.existingUsers.find((x) => x.userId === selectedId);

    if (!copiedUser) {
      this.toastrService.warning("Selected user has no access mappings to copy.");
      this.spinnerService.hide();
      return;
    }

    this.getReportRegistryList();
    this.reportUserAccessService.getReportUserAccessesByUser(copiedUser.userId).subscribe(
      (result: ReportRegistryUserAccessGridModel[]) => {
        this.selectedReportRegistryUserAccessGridModel = result || [];
        if (this.selectedReportRegistryUserAccessGridModel.length > 0) {
          this.reportRegistryUserAccessGridModel = this.reportRegistryUserAccessGridModel.map(
            (item) => {
              const isSelected = this.selectedReportRegistryUserAccessGridModel.some(
                (selected) => selected.reportId === item.reportId
              );

              return {
                ...item,
                isChecked: isSelected
              } as ReportRegistryUserAccessGridModel;
            }
          );

          this.getSelectedReportIds();
        } else {
          // Usage:
          this.getReportRegistryList();
        }
        this.spinnerService.hide();
      },
      (error: any) => {
        this.spinnerService.hide();
      }
    );

    this.spinnerService.hide();
  }

  private getSelectedReportIds(): void {
    this.selectedReportIds = this.reportRegistryUserAccessGridModel
      .filter((item) => item.isChecked)
      .map((item) => item.reportId);
  }

  private getSelectList(): void {
    this.spinnerService.show();
    this.userService.getSelectListUser().subscribe(
      (result: UserViewModel) => {
        this.users = result.optionsDataSources.UserSelectList;
        this.spinnerService.hide();
      },
      (error: any) => {
        this.spinnerService.hide();
      }
    );
  }

  onClickAddAll(event: any): void {
    this.reportRegistryUserAccessGridModel.forEach((item) => {
      item.isChecked = event.target.checked;
      this.onClickAddReport(event, item.reportId); // Check/uncheck based on the parent checkbox
    });
  }

  onClickAddReport(event: any, reportId: number): void {
    if (event.target.checked) {
      this.selectedReportIds.push(reportId);
    } else {
      this.selectedReportIds = this.selectedReportIds.filter((id) => id !== reportId);
    }
  }

  onClickCreateReportUserAccess(): void {
    this.spinnerService.show();
    this.reportUserAccessCreateModel.reportIds = this.selectedReportIds;
    if (this.reportUserAccessCreateModel.userId !== "-1") {
      this.reportUserAccessService.create(this.reportUserAccessCreateModel).subscribe(
        (result: ReportUserAccessCreateModel) => {
          this.spinnerService.hide();
          this.toastrService.success("Report User Access Mapped successful.", "Success");
          this.users = [];
          this.reportUserAccessCreateModel.userId = "-1";
          // Usage:
          this.getSelectList();
          this.getReportRegistryList();
          this.initializeTomSelectForExistUser();
        },
        (error: any) => {
          this.toastrService.error("Report User Access Not Mapped.", "Error");
          this.spinnerService.hide();
        }
      );
    } else {
      this.toastrService.error("Please select a user.", "Error");
      this.spinnerService.hide();
    }
  }

  private getReportRegistryList(): void {
    this.reportUserAccessService.getAllReportRegistry().subscribe(
      (result: ReportRegistryUserAccessGridModel[]) => {
        this.reportRegistryUserAccessGridModel = result;
        this.spinnerService.hide();

        this.isTableReady = false;
        setTimeout(() => {
          this.isTableReady = true;
          setTimeout(() => {
            this.refreshDataTable();
          }, 0);
        });
      },
      (error: any) => {
        this.spinnerService.hide();
      }
    );
  }

  private refreshDataTable(): void {
    const tableElement = $("#reportUserAccessTable");

    if (!tableElement.length) {
      return;
    }

    if (this.dataTable) {
      this.dataTable.destroy(true);
      this.dataTable = null;
    }

    this.dataTable = tableElement.DataTable({
      responsive: true,
      pageLength: 50, // Change your value
      lengthMenu: [
        [10, 25, 50, 100],
        [10, 25, 50, 100]
      ]
    });
  }
}
