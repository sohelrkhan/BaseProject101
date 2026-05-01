import { CommonModule, isPlatformBrowser } from "@angular/common";
import { Component, OnInit, AfterViewInit, OnDestroy, Inject, PLATFORM_ID } from "@angular/core";
import { RouterLink } from "@angular/router";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import { ActionGridModel, ActionService, PaginatedResponseOfActionGridModel, PaginationRequest } from "../../../../../../../../api/base-api";
import { HttpClientModule } from "@angular/common/http";
import { AccessControlService } from "../../../../../../../../identity/services/access-control.service";
import { CustomTosterServiceService } from "../../../../../../../shared/Toster/CustomTosterService.service";
declare var $: any;

@Component({
  selector: "app-action-list",
  templateUrl: "./action-list.component.html",
  styleUrls: ["./action-list.component.css"],
  standalone: true,
  imports: [RouterLink, NgxSpinnerModule, CommonModule, HttpClientModule],
  providers: [ActionService]
})

export class ActionListComponent implements OnInit, AfterViewInit, OnDestroy {

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

  actions: ActionGridModel[] = [];
  actionId: number | null = null;

  constructor(private actionService: ActionService, private spinnerService: NgxSpinnerService, private toastrService: CustomTosterServiceService,
    @Inject(PLATFORM_ID) private platformId: object, private accessControlService: AccessControlService) { }

  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);

    if(this.isBrowser) {
      this.accessControlService.setPermissions();
      this.setInitialPagination();
      this.getActions();
    }    
  }

  ngAfterViewInit() { }

  ngOnDestroy() {
    if (this.dataTable) {
      this.dataTable.destroy();
    }
  }

  // Set initial pagination
  private setInitialPagination(): void {
    this.pagination.page = 0;
    this.pagination.pageSize = 10;
    this.pagination.sortField = "name";
    this.pagination.sortOrder = "async";
    this.pagination.searchTerm = "";
  }
  
  // Get all User
  private getActions(): void {
    this.loading = true;
    this.spinnerService.show();
    
    this.actionService.getAll(this.pagination).subscribe((result: PaginatedResponseOfActionGridModel) => {

      this.actions = result.data || [];
      this.totalRecords = result.totalRecords;
      this.totalPages = result.totalPages;

      this.spinnerService.hide();
      this.loading = false;
    },
    (error: any) => {
      this.spinnerService.hide();
      this.loading = false;
      this.toastrService.error("Action cannot load at this time! Please, try again.", "Error");
    });
  }

  openDeleteModal(selectionId: number): void {
    this.actionId = selectionId;
  }

  // Delete Action
  onClickDeleteAction(): void {
    this.spinnerService.show();

    this.actionService.delete(this.actionId).subscribe((result: boolean) => {
      this.spinnerService.hide();
      this.toastrService.success("Selected action deleted.", "Successful.");
      this.getActions();
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Selected action cannot be deleted! Please try again.");
      return;
    });
  }

  // Pagination actions
  goToPage(page: number): void {
    if (page < 0 || page >= this.totalPages) return;
    this.pagination.page = page;
    this.getActions();
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
    this.getActions();
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
    this.getActions();
  }

  // Search with debounce
  onSearch(term: string): void {
    clearTimeout(this.searchTimer);

    this.searchTimer = setTimeout(() => {
      this.pagination.searchTerm = term;
      this.pagination.page = 0;
      this.getActions();
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