import { AfterViewInit, Component, Inject, OnDestroy, OnInit, PLATFORM_ID } from "@angular/core";
import { RouterLink } from "@angular/router";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import { ToastrService } from "ngx-toastr";
import { CommonModule, isPlatformBrowser } from "@angular/common";
import { CompanyGridModel, CompanyService } from "../../../../../../../api/base-api";
import { CheckPermissionDirective } from "../../../../../../../identity/directive/check-permission.directive";
import { AccessControlService } from "../../../../../../../identity/services/access-control.service";
import { environment } from "../../../../../../../environments/environment";
declare var $: any;

@Component({
  selector: "app-company-list",
  templateUrl: "./company-list.component.html",
  styleUrls: ["./company-list.component.css"],
  standalone: true,
  imports: [RouterLink, NgxSpinnerModule, CommonModule, CheckPermissionDirective],
  providers: [CompanyService]
})

export class CompanyListComponent implements OnInit, AfterViewInit, OnDestroy {

  // Data table related property
  isBrowser: boolean = false;
  isTableReady: boolean = false;
  show: boolean = false;
  dataTable: any;

  // Delete company id
  deleteCompanyId: string | undefined;

  // Company data source
  companies: CompanyGridModel[] = [];

  // Application base url
  applicationBaseUrl: string | undefined;

  constructor(private companyService: CompanyService, private spinnerService: NgxSpinnerService, private toastrService: ToastrService, 
    @Inject(PLATFORM_ID) private platformId: object, private accessControlService: AccessControlService) { }

  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);

    if (this.isBrowser) {

      // Get application base url
      this.applicationBaseUrl = environment.coreBaseUrl;

      this.accessControlService.setPermissions();
      this.getCompanies();
    }
  }

  ngAfterViewInit() { }

  ngOnDestroy(): void {
    if (this.dataTable) {
      this.dataTable.destroy();
    }
  }
  
  // Get all companies
  private getCompanies(): void {
    this.spinnerService.show();
    this.companyService.getAll().subscribe((result: CompanyGridModel[]) => {
      this.companies = result || [];
  
      this.isTableReady = false;
      setTimeout(() => {
        this.isTableReady = true;

        setTimeout(() => {
          this.refreshDataTable();
        }, 0);
      });

      this.spinnerService.hide();
      return;
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Companies cannot be loaded! Please, try again.", "Error.", { closeButton: true });
    });
  }

  onClickOpenDeleteCompanyModal(companyId: string): void {
    this.deleteCompanyId = companyId;
  }

  // On click delete company
  onClickDeleteCompany(): void {
    this.spinnerService.show();
    this.companyService.delete(this.deleteCompanyId).subscribe((result: boolean) => {
      this.spinnerService.hide();
      this.toastrService.success("Delete successful.", "Success", { closeButton: true });
      this.getCompanies();
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Company cannot be deleted! Please, try again.", "Error.", { closeButton: true });
    });
  }

  // Refresh data table
  private refreshDataTable(): void {
    const tableElement = ($("#example") as any);

    if (!tableElement.length) {
      return;
    }

    if (this.dataTable) {
      this.dataTable.destroy(true);
      this.dataTable = null;
    }

    this.dataTable = tableElement.DataTable({
      responsive: true,
      pageLength: 50
    });
  }
}