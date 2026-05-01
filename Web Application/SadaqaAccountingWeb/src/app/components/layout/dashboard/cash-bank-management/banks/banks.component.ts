import { CommonModule, isPlatformBrowser } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { AfterViewInit, Component, Inject, OnDestroy, OnInit, PLATFORM_ID } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { BankGridModel, BankService, PaginatedResponseOfBankGridModel, PaginationRequest, UserModel } from '../../../../../../api/base-api';
import { CustomTosterServiceService } from '../../../../../shared/Toster/CustomTosterService.service';
import { AccessControlService } from '../../../../../../identity/services/access-control.service';
import { IdentityService } from '../../../../../../identity/identity-shared/identity.service';

@Component({
  selector: 'app-banks',
  templateUrl: './banks.component.html',
  styleUrls: ['./banks.component.css'],
  standalone: true,
  imports: [RouterLink, NgxSpinnerModule, CommonModule, HttpClientModule],
  providers: [BankService]
})

export class BanksComponent implements OnInit, AfterViewInit, OnDestroy {

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

  banks: BankGridModel[] = [];
  bankId: number | null = null;

  constructor(private bankService: BankService, private spinnerService: NgxSpinnerService, private toastrService: CustomTosterServiceService,
    @Inject(PLATFORM_ID) private platformId: object, private accessControlService: AccessControlService, private identityService: IdentityService) { }

  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);
  
    if(this.isBrowser) {
      this.accessControlService.setPermissions();

      // Get login user info
      this.getLoginUserInfo();

      this.setInitialPagination();
      this.getBanks();
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

  // Get banks
  private getBanks(): void {
    this.loading = true;
    this.spinnerService.show();
    
    this.bankService.getAll(this.pagination).subscribe((result: PaginatedResponseOfBankGridModel) => {

      this.banks = result.data || [];
      this.totalRecords = result.totalRecords;
      this.totalPages = result.totalPages;

      this.spinnerService.hide();
      this.loading = false;
    },
    (error: any) => {
      this.spinnerService.hide();
      this.loading = false;
      this.toastrService.error("Bank cannot load at this time! Please, try again.", "Error");
    });
  }

  openDeleteModal(selectionId: number): void {
    this.bankId = selectionId;
  }

  // Delete bank
  onClickDeleteBank(): void {
    this.spinnerService.show();

    this.bankService.delete(this.bankId).subscribe((result: boolean) => {
      this.spinnerService.hide();
      this.toastrService.success("Bank deleted.", "Successful.");
      this.getBanks();
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Bank cannot be deleted! Please try again.");
      return;
    });
  }

  // Pagination actions
  goToPage(page: number): void {
    if (page < 0 || page >= this.totalPages) return;
    this.pagination.page = page;
    this.getBanks();
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
    this.getBanks();
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
    this.getBanks();
  }

  // Search with debounce
  onSearch(term: string): void {
    clearTimeout(this.searchTimer);

    this.searchTimer = setTimeout(() => {
      this.pagination.searchTerm = term;
      this.pagination.page = 0;
      this.getBanks();
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