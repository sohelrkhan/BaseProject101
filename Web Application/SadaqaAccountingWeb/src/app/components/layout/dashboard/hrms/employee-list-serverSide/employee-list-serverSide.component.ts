import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { Component, OnInit, ViewChild, TemplateRef, Inject, PLATFORM_ID, ChangeDetectorRef } from "@angular/core";
import { ReactiveFormsModule, FormsModule } from "@angular/forms";
import { RouterLink } from "@angular/router";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import { EmployeeService, FeatureActionMappingService, UserAccessMappingService, FeatureService,
  ActionService,
  UserService,
  EmployeeGridModel,
  SelectModel,
  UserCreateFromEmployeeModel,
  UserAccessMappingCreateModel,
  FeatureActionMappingGridModel,
  FeatureActionMappingCreateModel,
  EmployeeViewModel,
  PaginationRequest,
  AdditionalFilter
} from "../../../../../../api/base-api";
import { environment } from "../../../../../../environments/environment";
import { CheckPermissionDirective } from "../../../../../../identity/directive/check-permission.directive";
import { AccessControlService } from "../../../../../../identity/services/access-control.service";
import { PaginationComponent } from "../../../../../shared/DisplayComponents/pagination/pagination.component";
import {
  TableComponent,
  TableColumn,
  ServerTableRequest
} from "../../../../../shared/DisplayComponents/table/table.component";
import { OrdinalDatePipe } from "../../../../../shared/pipe/ordinaldate.pipe";
import { CustomTosterServiceService } from "../../../../../shared/Toster/CustomTosterService.service";

@Component({
  selector: "app-employee-list-serverSide",
  templateUrl: "./employee-list-serverSide.component.html",
  styleUrls: ["./employee-list-serverSide.component.css"],
  standalone: true,
  imports: [
    OrdinalDatePipe,
    NgxSpinnerModule,
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule,
    CommonModule,
    RouterLink,
    CheckPermissionDirective,
    TableComponent,
    PaginationComponent
  ],
  providers: [
    EmployeeService,
    FeatureActionMappingService,
    UserAccessMappingService,
    FeatureService,
    ActionService,
    UserService
  ]
})
export class EmployeeListServerSideComponent implements OnInit {
  employees: EmployeeGridModel[] = [];

  companyId: number = -1;
  statusId: number = -1;

  private _employeeId: number = 0;
  private _employeeDeleteId: string | undefined;
  statuses: SelectModel[] = [];

  // user create model
  userCreateFromEmployee: UserCreateFromEmployeeModel = new UserCreateFromEmployeeModel();

  // Default user access Mapping id
  public mapId: number = 0;

  public employeeMapName: any = null;

  // Application base url
  applicationBaseUrl: string | undefined;

  // Feature create model
  userAccessMappingCreateModel: UserAccessMappingCreateModel = new UserAccessMappingCreateModel();

  // Select list
  users: SelectModel[] = [];
  actions: SelectModel[] = [];
  featureActionsMappings: FeatureActionMappingGridModel[] = [];
  featureActionsMappingsCreate: FeatureActionMappingCreateModel[] = [];
  selectedFeatureActionMappings: FeatureActionMappingGridModel[] = [];
  filters: any[];

  /** Table view pagination */
  currentTablePage: number = 1;
  tablePageSize: number = 5;

  /** Grid view pagination */
  currentGridPage: number = 1;
  gridPageSize: number = 5;

  /** Total number of records from server */
  totalRecords: number = 0;
  /** Loading state indicator */
  loading: boolean = false;
  /** Table column definitions */
  tableColumns: TableColumn[] = [];
  /** Template reference for action buttons */
  @ViewChild("actionsTemplate", { static: true }) actionsTemplate!: TemplateRef<any>;
  /** Current search term applied to server query */
  currentSearchTerm: string = "";

  /** Current sort field name */
  currentSortField: string = "";

  /** Current sort direction */
  currentSortOrder: "asc" | "desc" | null = null;

  /** Current active view mode */
  currentView: "table" | "grid" = "table";

  constructor(
    private employeeService: EmployeeService,
    private spinnerService: NgxSpinnerService,
    private toastrService: CustomTosterServiceService,
    private userService: UserService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.userAccessMappingCreateModel.userId = "-1";
    this.getEmployeeById("-1");
    this.initializeTableColumns();
    this.loadTableData();
    // Get application base url
    this.applicationBaseUrl = environment.coreBaseUrl;
    this.injectWidthClasses();
  }

  private injectWidthClasses() {
    if (document.getElementById("width-classes")) return;

    const style = document.createElement("style");
    style.id = "width-classes";

    let css = "";
    for (let i = 0; i <= 100; i++) {
      css += `.w-${i} { width: ${i}% !important; }\n`;
    }

    style.textContent = css;
    document.head.appendChild(style);
  }

  private initializeTableColumns(): void {
    this.tableColumns = [
      {
        field: "fullName",
        header: "Name",
        sortable: true,
        width: "150px",
        format: (value: any, row: any) => {
          let imageSrc = row.imageThumbnail
            ? this.applicationBaseUrl + row.imageThumbnail
            : "assets/img/profiles/avatar-12.jpg";
          let html = `<div class="d-flex align-items-center file-name-icon">
                    <a href="javascript:void(0);" class="avatar avatar-md border avatar-rounded">
                      <img src="${imageSrc}"
                        class="img-fluid avatar-img w-100 h-100 rounded-circle object-fit-cover"
                        alt="img" />
                    </a>
                    <div class="ms-2">
                      <h6 class="fw-medium">
                        <a target="_blank" href="/app/employee-details/${row.encryptedId}" class="text-primary fw-bold">
                          ${row.fullName}
                        </a>
                      </h6>
                    </div>
                  </div>`;
          return html;
        }
      },
      {
        field: "companyName",
        header: "Company",
        sortable: true,
        width: "120px",
        format: (value: any, row: any) => {
          // Build HTML
          let html = `
            <div class="date-group">
              <div class="text-primary"><strong><i class="ti ti-mail"></i></strong> ${row.companyName || "N/A"}</div>
          `;
          html += `</div>`;

          return html;
        }
      },
      {
        field: "phoneNumber",
        header: "Phone",
        sortable: true,
        width: "120px",
        format: (value: any, row: any) => {
          // Build HTML
          let html = `
            <div class="date-group">
              <div class="text-primary"><strong><i class="ti ti-device-landline-phone"></i></strong> ${row.phoneNumber || "N/A"}</div>
          `;
          html += `</div>`;

          return html;
        }
      },
      {
        field: "email",
        header: "Email",
        sortable: true,
        width: "120px",
        format: (value: any, row: any) => {
          // Build HTML
          let html = `
            <div class="date-group">
              <div class="text-primary"><strong><i class="ti ti-mail"></i></strong> ${row.officeEmail || "N/A"}</div>
              <div class="text-secondary"><strong><i class="ti ti-mail"></i></strong> ${row.personalEmail || "N/A"}</div>
          `;
          html += `</div>`;

          return html;
        }
      },
      {
        field: "statusName",
        header: "Status",
        sortable: false,
        width: "100px",
        align: "center",
        format: (value: any, row: any) => this.getStatusBadge(row)
      },
      {
        field: "actions",
        header: "Actions",
        sortable: false,
        width: "150px",
        align: "center"
      }
    ];
  }

  private applyProgressBarWidths() {
    const progressBars = document.querySelectorAll(".progress-bar[data-width]");
    progressBars.forEach((bar: HTMLElement) => {
      const width = bar.getAttribute("data-width");
      if (width) {
        bar.style.width = `${width}%`;
      }
    });
  }

  getCompletionClass(row: any): string {
    const percentage = row.profileCompletion.completionPercentage;

    if (percentage >= 80) {
      return "bg-success";
    } else if (percentage >= 50) {
      return "bg-warning";
    } else {
      return "bg-danger";
    }
  }

  getFormatedDate(value: any): string {
    if (!value) return "";

    const date = new Date(value);

    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const day = String(date.getDate()).padStart(2, "0");

    return `${day}-${month}-${year}`;
  }

  getStatusBadge(employee: EmployeeGridModel): string {
    if (!employee) return '<span class="badge bg-secondary">Unknown</span>';

    if (employee.statusId === 1) {
      return '<span class="badge bg-success">' + employee.statusName + "</span>";
    } else if (employee.statusId === 2) {
      return '<span class="badge bg-danger">' + employee.statusName + "</span>";
    }
    return '<span class="badge bg-secondary">Unknown</span>';
  }

  // Get employee by id
  private getEmployeeById(employeeId: string): void {
    this.spinnerService.show();
    this.employeeService.getById(employeeId).subscribe((result: EmployeeViewModel) => {
      this.statuses = result.optionsDataSources.StatusSelectList

      // Reset pagination and reload data
      this.currentTablePage = 1;
      this.currentSearchTerm = "";
      this.currentSortField = "";
      this.currentSortOrder = null;

      this.loadTableData();
      this.cdr.detectChanges();
      this.spinnerService.hide();
    });
  }

  // Open delete employee modal
  onClickOpenDeleteEmployeeModal(employeeId: string): void {
    this._employeeDeleteId = employeeId;
  }
  // Open create user modal
  onClickCreateUserModal(employeeId: number): void {
    this._employeeId = employeeId;
  }
  // Delete employee
  onClickDeleteEmployee(): void {
    this.spinnerService.show();
    this.employeeService.delete(this._employeeDeleteId).subscribe(
      (result: boolean) => {
        this.spinnerService.hide();
        this.toastrService.success("Delete successful.", "Success");
        //this.getEmployees();
        this.closeDeleteEmployeeModal();
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error("Employee cannot be deleted! Please, try again.", "Error.");
      }
    );
  }

  // Close delete employee modal
  private closeDeleteEmployeeModal(): void {
    // Manually close the modal
    const deleteModal = document.getElementById("delete_employee_modal");
    if (deleteModal) {
      deleteModal.classList.remove("show"); // Hide modal visually
      deleteModal.setAttribute("aria-hidden", "true");
      deleteModal.style.display = "none"; // Hide modal

      // Remove modal backdrop
      const modalBackdrop = document.querySelector(".modal-backdrop");
      if (modalBackdrop) {
        modalBackdrop.remove();
      }

      // Restore body scroll (important!)
      document.body.classList.remove("modal-open");
      document.body.style.overflow = "";
      document.body.style.paddingRight = "";
    }
  }



  // on click create user
  onClickCreateUser(): void {
    this.userCreateFromEmployee.employeeId = this._employeeId;

    this.spinnerService.show();
    this.userService
      .userCreateFromEmployee(this.userCreateFromEmployee)
      .subscribe((result: UserCreateFromEmployeeModel) => {
        this.toastrService.success("User create successfully.", "Success");
        this.closeCreateUserFromEmployeeModal();
        //this.getEmployees();
        this.spinnerService.hide();
      });
  }
  // Close create user from employee modal
  private closeCreateUserFromEmployeeModal(): void {
    // Manually close the modal
    const deleteModal = document.getElementById("add_employee_user_modal");
    if (deleteModal) {
      deleteModal.classList.remove("show"); // Hide modal visually
      deleteModal.setAttribute("aria-hidden", "true");
      deleteModal.style.display = "none"; // Hide modal

      // Remove modal backdrop
      const modalBackdrop = document.querySelector(".modal-backdrop");
      if (modalBackdrop) {
        modalBackdrop.remove();
      }

      // Restore body scroll (important!)
      document.body.classList.remove("modal-open");
      document.body.style.overflow = "";
      document.body.style.paddingRight = "";
    }
  }
  onTableDataRequest(request: ServerTableRequest): void {
    console.log("📋 Table data request:", request);

    this.currentTablePage = request.page;
    this.tablePageSize = request.pageSize;
    this.currentSearchTerm = request.searchTerm || "";
    this.currentSortField = request.sortField || "";
    this.currentSortOrder = request.sortOrder || null;

    this.loadTableData();
  }
  private loadTableData(): void {
    const request = this.buildPaginationRequest(this.currentTablePage, this.tablePageSize);
    this.loadServerData(request);
  }
  private buildPaginationRequest(page: number, pageSize: number): PaginationRequest {
    const request = new PaginationRequest();
    request.page = page;
    request.pageSize = pageSize;
    request.sortField = this.currentSortField || undefined;
    request.sortOrder = this.currentSortOrder || undefined;
    request.searchTerm = this.currentSearchTerm || undefined;
    request.additionalFilters = [];

    // Add company filter
    if (this.companyId != -1) {
      const additionalFilter = new AdditionalFilter();
      additionalFilter.filterPropertyName = "CompanyId";
      additionalFilter.filterValue = this.companyId;
      additionalFilter.operator = "Equal";
      request.additionalFilters.push(additionalFilter);
    }
    // Add status filter
    if (this.statusId != -1) {
      const additionalFilter = new AdditionalFilter();
      additionalFilter.filterPropertyName = "StatusId";
      additionalFilter.filterValue = this.statusId;
      additionalFilter.operator = "Equal";
      request.additionalFilters.push(additionalFilter);
    }

    return request;
  }
  private loadServerData(request: PaginationRequest): void {
    this.loading = true;
    this.spinnerService.show();

    this.employeeService.getPaginated(request).subscribe({
      next: (response: any) => {
        // Extract and validate data
        this.employees = Array.isArray(response.data) ? response.data : [];
        this.totalRecords = response.totalRecords || 0;

        this.loading = false;
        this.spinnerService.hide();

        // Force change detection
        this.cdr.detectChanges();
      }
    });
  }
  onTableRowAction(event: { action: string; row: any }): void {
    this.rowActionHandler(event.action, event.row);
  }
  onTablePaginationChange(event: { currentPage: number; pageSize: number }): void {
    console.log("📊 Table pagination changed:", event);

    const pageChanged = this.currentTablePage !== event.currentPage;
    const pageSizeChanged = this.tablePageSize !== event.pageSize;

    this.currentTablePage = event.currentPage;
    this.tablePageSize = event.pageSize;

    console.log("📊 New state - Page:", this.currentTablePage, "PageSize:", this.tablePageSize);

    if (pageChanged || pageSizeChanged) {
      this.loadTableData();
    } else {
      console.log("⏭️ Skipping reload - no changes detected");
    }
  }
  rowActionHandler = (action: string, row: any) => {
    if (!row) {
      console.error("❌ Row is undefined in rowActionHandler");
      return;
    }

    console.log("🎬 Row action:", action, row);

    switch (action) {
      case "edit":
        //this.onEditInvoice(row);
        break;
      case "view":
        //this.onViewInvoice(row);
        break;
      case "delete":
        //this.onDeleteInvoice(row);
        break;
      default:
        console.warn("⚠️ Unknown action:", action);
    }
  };
}
