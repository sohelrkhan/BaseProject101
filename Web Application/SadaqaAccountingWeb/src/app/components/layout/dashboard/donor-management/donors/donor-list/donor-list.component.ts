import { CommonModule, isPlatformBrowser } from '@angular/common';
import {
  AfterViewInit,
  Component,
  Inject,
  OnDestroy,
  OnInit,
  PLATFORM_ID,
} from '@angular/core';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { CheckPermissionDirective } from '../../../../../../../identity/directive/check-permission.directive';
import { ConfirmModalComponent } from '../../../../../../shared/confirm-modal/confirm-modal.component';
import {
  DonorGridModel,
  DonorService,
  PaginatedResponseOfDonorGridModel,
  PaginationRequest,
} from '../../../../../../../api/base-api';
import { ToastrService } from 'ngx-toastr';
import { AccessControlService } from '../../../../../../../identity/services/access-control.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-donor-list',
  templateUrl: './donor-list.component.html',
  styleUrls: ['./donor-list.component.css'],
  standalone: true,
  imports: [
    NgxSpinnerModule,
    CommonModule,
    CheckPermissionDirective,
    ConfirmModalComponent,
    RouterLink,
  ],
  providers: [DonorService],
})
export class DonorListComponent implements OnInit, AfterViewInit, OnDestroy {
  //Pagination request model
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
  donors: DonorGridModel[] = [];
  donorId: number | null = null;
  updateDonorId: string = '';
  constructor(
    private donorService: DonorService,
    private spinnerService: NgxSpinnerService,
    private toastrService: ToastrService,
    @Inject(PLATFORM_ID) private platformId: object,
    private accessControlService: AccessControlService,
  ) {}

  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);
    if (this.isBrowser) {
      this.accessControlService.setPermissions();
      this.setInitialPagination();
      this.getDonors();
    }
  }
  ngAfterViewInit() {}

  ngOnDestroy() {
    if (this.dataTable) {
      this.dataTable.destroy();
    }
  }

  // Set initial pagination
  private setInitialPagination(): void {
    this.pagination.page = 0;
    this.pagination.pageSize = 10;
    this.pagination.sortField = 'name';
    this.pagination.sortOrder = 'async';
    this.pagination.searchTerm = '';
  }

  // Get all donors with pagination
  private getDonors(): void {
    this.loading = true;
    this.spinnerService.show();

    this.donorService.getAllDonorFilter(this.pagination).subscribe(
      (result: PaginatedResponseOfDonorGridModel) => {
        this.donors = result.data || [];
        this.totalRecords = result.totalRecords;
        this.totalPages = result.totalPages;

        this.spinnerService.hide();
        this.loading = false;
      },
      (error: any) => {
        this.spinnerService.hide();
        this.loading = false;
        this.toastrService.error(
          'Donors cannot load at this time! Please, try again.',
          'Error',
        );
      },
    );
  }
  openDeleteModal(selectionId: number): void {
    this.donorId = selectionId;
  }

  // Delete Donor
  onClickDeleteDonor(): void {
    this.spinnerService.show();

    this.donorService.delete(this.donorId).subscribe(
      (result: boolean) => {
        this.spinnerService.hide();
        this.toastrService.success('Selected donor deleted.', 'Successful.');
        this.getDonors();
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error(
          'Selected donor cannot be deleted! Please try again.',
        );
        return;
      },
    );
  }

  onClickUpdateDonorModal(donorId: string): void {
    this.updateDonorId = donorId;
  }

  // Pagination actions
  goToPage(page: number): void {
    if (page < 0 || page >= this.totalPages) return;
    this.pagination.page = page;
    this.getDonors();
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
    this.getDonors();
  }

  // Sorting (click header)
  sortBy(field: string): void {
    if (this.pagination.sortField?.toLowerCase() === field.toLowerCase()) {
      this.pagination.sortOrder =
        this.pagination.sortOrder === 'ascend' ? 'descend' : 'ascend';
    } else {
      this.pagination.sortField = field;
      this.pagination.sortOrder = 'ascend';
    }
    this.pagination.page = 0;
    this.getDonors();
  }

  // Search with debounce
  onSearch(term: string): void {
    clearTimeout(this.searchTimer);

    this.searchTimer = setTimeout(() => {
      this.pagination.searchTerm = term;
      this.pagination.page = 0;
      this.getDonors();
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
