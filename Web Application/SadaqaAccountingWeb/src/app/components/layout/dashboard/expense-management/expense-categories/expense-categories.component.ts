import { CommonModule, isPlatformBrowser } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { AfterViewInit, Component, Inject, OnDestroy, OnInit, PLATFORM_ID } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { ExpenseCategoryGridModel, ExpenseCategoryService, PaginatedResponseOfExpenseCategoryGridModel, PaginationRequest, UserModel } from '../../../../../../api/base-api';
import { AccessControlService } from '../../../../../../identity/services/access-control.service';
import { IdentityService } from '../../../../../../identity/identity-shared/identity.service';
import { CustomTosterServiceService } from '../../../../../shared/Toster/CustomTosterService.service';

@Component({
  selector: 'app-expense-categories',
  templateUrl: './expense-categories.component.html',
  styleUrls: ['./expense-categories.component.css'],
  standalone: true,
  imports: [RouterLink, NgxSpinnerModule, CommonModule, HttpClientModule],
  providers: [ExpenseCategoryService]
})

export class ExpenseCategoriesComponent implements OnInit, AfterViewInit, OnDestroy {

  // Pagination request model
  pagination: PaginationRequest = new PaginationRequest();

  // Login user account unit id
  private _accountUnitId: number = 0;

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

  expenseCategories: ExpenseCategoryGridModel[] = [];
  expenseCategoryId: number | null = null;

  constructor(private expenseCategoryService: ExpenseCategoryService, private spinnerService: NgxSpinnerService, private toastrService: CustomTosterServiceService,
    @Inject(PLATFORM_ID) private platformId: object, private accessControlService: AccessControlService, private identityService: IdentityService) { }

  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);
 
    if(this.isBrowser) {
      this.accessControlService.setPermissions();

      // Get login user info
      this.getLoginUserInfo();

      this.setInitialPagination();
      this.getExpenseCategories();
    }    
  }

  ngAfterViewInit() { }

  ngOnDestroy() {
    if (this.dataTable) {
      this.dataTable.destroy();
    }
  }

  // Get login user info
  private getLoginUserInfo(): void {
    this.identityService.getLoginInfo().subscribe((result: UserModel) => {
      this._accountUnitId = result.accountUnitId;
      return;
    },
    (error: any) => {
      this.toastrService.error("Login user information cannot found! Please, try again.", "Error");
      return;
    })
  }

  // Set initial pagination
  private setInitialPagination(): void {
    this.pagination.page = 0;
    this.pagination.pageSize = 10;
    this.pagination.sortField = "name";
    this.pagination.sortOrder = "async";
    this.pagination.accountUnitId = this._accountUnitId;
    this.pagination.searchTerm = "";
  }

  // Get expense categories
  private getExpenseCategories(): void {
    this.loading = true;
    this.spinnerService.show();
    
    this.expenseCategoryService.getAll(this.pagination).subscribe((result: PaginatedResponseOfExpenseCategoryGridModel) => {

      this.expenseCategories = result.data || [];
      this.totalRecords = result.totalRecords;
      this.totalPages = result.totalPages;

      this.spinnerService.hide();
      this.loading = false;
    },
    (error: any) => {
      this.spinnerService.hide();
      this.loading = false;
      this.toastrService.error("Expense Categories cannot load at this time! Please, try again.", "Error");
    });
  }

  openDeleteModal(selectionId: number): void {
    this.expenseCategoryId = selectionId;
  }

  // Delete Action
  onClickDeleteExpenseCategory(): void {
    this.spinnerService.show();

    this.expenseCategoryService.delete(this.expenseCategoryId).subscribe((result: boolean) => {
      this.spinnerService.hide();
      this.toastrService.success("Expense Category deleted.", "Successful.");
      this.getExpenseCategories();
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Expense Category cannot be deleted! Please try again.");
      return;
    });
  }

  // Pagination actions
  goToPage(page: number): void {
    if (page < 0 || page >= this.totalPages) return;
    this.pagination.page = page;
    this.getExpenseCategories();
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
    this.getExpenseCategories();
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
    this.getExpenseCategories();
  }

  // Search with debounce
  onSearch(term: string): void {
    clearTimeout(this.searchTimer);

    this.searchTimer = setTimeout(() => {
      this.pagination.searchTerm = term;
      this.pagination.page = 0;
      this.getExpenseCategories();
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