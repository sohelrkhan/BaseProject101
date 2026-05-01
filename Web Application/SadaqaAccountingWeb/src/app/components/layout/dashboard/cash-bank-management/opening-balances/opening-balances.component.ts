import { CommonModule, isPlatformBrowser } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { AfterViewInit, Component, Inject, OnDestroy, OnInit, PLATFORM_ID } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { OpeningBalanceGridModel, OpeningBalanceService, PaginatedResponseOfOpeningBalanceGridModel, PaginationRequest, UserModel } from '../../../../../../api/base-api';
import { CustomTosterServiceService } from '../../../../../shared/Toster/CustomTosterService.service';
import { AccessControlService } from '../../../../../../identity/services/access-control.service';
import { IdentityService } from '../../../../../../identity/identity-shared/identity.service';
import { OrdinalDatePipe } from '../../../../../shared/pipe/ordinaldate.pipe';

@Component({
  selector: 'app-opening-balances',
  templateUrl: './opening-balances.component.html',
  styleUrls: ['./opening-balances.component.css'],
  standalone: true,
  imports: [
    RouterLink, 
    NgxSpinnerModule, 
    CommonModule, 
    HttpClientModule,
    OrdinalDatePipe
  ],
  providers: [OpeningBalanceService]
})

export class OpeningBalancesComponent implements OnInit, AfterViewInit, OnDestroy {

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

  openingBalances: OpeningBalanceGridModel[] = [];
  openingBalanceId: number | null = null;

  constructor(
    private openingBalanceService: OpeningBalanceService, 
    private spinnerService: NgxSpinnerService, 
    private toastrService: CustomTosterServiceService,
    @Inject(PLATFORM_ID) private platformId: object, 
    private accessControlService: AccessControlService, 
    private identityService: IdentityService) { }

  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);
  
    if(this.isBrowser) {
      this.accessControlService.setPermissions();

      // Get login user info
      this.getLoginUserInfo();

      this.setInitialPagination();
      this.getOpeningBalances();
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
    this.pagination.sortField = "paymentmethodname";
    this.pagination.sortOrder = "async";
    this.pagination.accountUnitId = this._accountUnitId;
    this.pagination.searchTerm = "";
  }

  // Get opening balance
  private getOpeningBalances(): void {
    this.loading = true;
    this.spinnerService.show();
    
    this.openingBalanceService.getAll(this.pagination).subscribe((result: PaginatedResponseOfOpeningBalanceGridModel) => {

      this.openingBalances = result.data || [];
      this.totalRecords = result.totalRecords;
      this.totalPages = result.totalPages;

      this.spinnerService.hide();
      this.loading = false;
    },
    (error: any) => {
      this.spinnerService.hide();
      this.loading = false;
      this.toastrService.error("Opening Balance cannot load at this time! Please, try again.", "Error");
    });
  }

  openDeleteModal(selectionId: number): void {
    this.openingBalanceId = selectionId;
  }

  // Delete opening balance
  onClickDeleteOpeningBalance(): void {
    this.spinnerService.show();

    this.openingBalanceService.delete(this.openingBalanceId).subscribe((result: boolean) => {
      this.spinnerService.hide();
      this.toastrService.success("Opening Balance deleted.", "Successful.");
      this.getOpeningBalances();
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Opening Balance cannot be deleted! Please try again.");
      return;
    });
  }

  // Pagination actions
  goToPage(page: number): void {
    if (page < 0 || page >= this.totalPages) return;
    this.pagination.page = page;
    this.getOpeningBalances();
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
    this.getOpeningBalances();
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
    this.getOpeningBalances();
  }

  // Search with debounce
  onSearch(term: string): void {
    clearTimeout(this.searchTimer);

    this.searchTimer = setTimeout(() => {
      this.pagination.searchTerm = term;
      this.pagination.page = 0;
      this.getOpeningBalances();
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