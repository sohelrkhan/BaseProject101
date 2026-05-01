import { Component, Inject, OnInit, PLATFORM_ID } from "@angular/core";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import { EmployeeGridModel, EmployeeService, PaginatedResponseOfEmployeeGridModel, PaginationRequest } from "../../../../../../../api/base-api";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { HttpClientModule } from "@angular/common/http";
import { CommonModule, isPlatformBrowser } from "@angular/common";
import { RouterLink } from "@angular/router";
import { CheckPermissionDirective } from "../../../../../../../identity/directive/check-permission.directive";
import { AccessControlService } from "../../../../../../../identity/services/access-control.service";
import { ToastrService } from "ngx-toastr";

declare var $: any;

@Component({
  selector: "app-employee-list",
  templateUrl: "./employee-list.component.html",
  styleUrls: ["./employee-list.component.css"],
  standalone: true,
  imports: [NgxSpinnerModule, ReactiveFormsModule, FormsModule, HttpClientModule, CommonModule, RouterLink, CheckPermissionDirective],
  providers: [EmployeeService]
})

export class EmployeeListComponent implements OnInit {

  // Pagination request model
  pagination: PaginationRequest = new PaginationRequest();

  // Data table related property
  isBrowser: boolean = false;
  isTableReady: boolean = false;
  show: boolean = false;
  dataTable: any;
  totalRecords: number = 0;
  totalPages: number = 0;

  // UI state
  loading = false;

  // Debounce for search
  private searchTimer: any;

  employees: EmployeeGridModel[] = [];
  employeeId: number | null = null;
 
  constructor(private employeeService: EmployeeService, private spinnerService: NgxSpinnerService, private toastrService: ToastrService,
    @Inject(PLATFORM_ID) private platformId: object, private accessControlService: AccessControlService) { }

  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);

    if(this.isBrowser) {      
      this.accessControlService.setPermissions();
      this.setInitialPagination();
      this.getEmployees();
    }    
  }

  // Set initial pagination
  private setInitialPagination(): void {
    this.pagination.page = 0;
    this.pagination.pageSize = 10;
    this.pagination.sortField = "fullName";
    this.pagination.sortOrder = "async";
    this.pagination.searchTerm = "";
  }

  // Get employees
  private getEmployees(): void {
    this.loading = true;
    this.spinnerService.show();
    
    this.employeeService.getPaginated(this.pagination).subscribe((result: PaginatedResponseOfEmployeeGridModel) => {

      this.employees = result.data || [];
      this.totalRecords = result.totalRecords;
      this.totalPages = result.totalPages;

      this.spinnerService.hide();
      this.loading = false;
    },
    (error: any) => {
      this.spinnerService.hide();
      this.loading = false;
      this.toastrService.error("Employees cannot load at this time! Please, try again.", "Error");
    });
  }

  openDeleteModal(employeeId: number): void {
    this.employeeId = employeeId;
  }

  // Delete Action
  onClickDeleteAction(): void {
    this.spinnerService.show();

    this.employeeService.delete(this.employeeId.toString()).subscribe((result: boolean) => {
      this.spinnerService.hide();
      this.toastrService.success("Selected employee deleted.", "Successful.");
      this.getEmployees();
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Selected employee cannot be deleted! Please try again.");
      return;
    });
  }

  // Pagination actions
  goToPage(page: number): void {
    if (page < 0 || page >= this.totalPages) return;
    this.pagination.page = page;
    this.getEmployees();
  }

  nextPage(): void {
    this.goToPage(this.pagination.page + 1);
  }

  prevPage(): void {
    this.goToPage(this.pagination.page - 1);
  }

  changePageSize(size: number): void {
    this.pagination.pageSize = size;
    this.pagination.page = 0;
    this.getEmployees();
  }

  // Sorting (click header)
  sortBy(field: string): void {
    if (this.pagination.sortField?.toLowerCase() === field.toLowerCase()) {
      this.pagination.sortOrder = this.pagination.sortOrder === 'ascend' ? 'descend' : 'ascend';
    } else {
      this.pagination.sortField = field;
      this.pagination.sortOrder = 'ascend';
    }
    this.pagination.page = 0;
    this.getEmployees();
  }

  // Search with debounce
  onSearch(term: string): void {
    clearTimeout(this.searchTimer);

    this.searchTimer = setTimeout(() => {
      this.pagination.searchTerm = term;
      this.pagination.page = 0;
      this.getEmployees();
    }, 400);
  }

  // For pagination UI (max 5 buttons)
  get pageButtons(): number[] {
    const total = this.totalPages;
    const current = this.pagination.page;
    const maxButtons = 5;

    if (total <= maxButtons) return Array.from({ length: total }, (_, i) => i);

    let start = Math.max(0, current - 2);
    let end = Math.min(total - 1, start + (maxButtons - 1));

    start = Math.max(0, end - (maxButtons - 1));
    return Array.from({ length: end - start + 1 }, (_, i) => start + i);
  }
}