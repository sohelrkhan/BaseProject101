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
import { RouterLink } from '@angular/router';
import {
  IncomeCategoryGridModel,
  IncomeCategoryService,
  PaginatedResponseOfIncomeCategoryGridModel,
  PaginationRequest,
} from '../../../../../../../api/base-api';
import { ToastrService } from 'ngx-toastr';
import { AccessControlService } from '../../../../../../../identity/services/access-control.service';

@Component({
  selector: 'app-income-category-list',
  templateUrl: './income-category-list.component.html',
  styleUrls: ['./income-category-list.component.css'],
  standalone: true,
  imports: [
    NgxSpinnerModule,
    CommonModule,
    CheckPermissionDirective,
    ConfirmModalComponent,
    RouterLink,
  ],
  providers: [IncomeCategoryService],
})
export class IncomeCategoryListComponent
  implements OnInit, AfterViewInit, OnDestroy
{
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
  categories: IncomeCategoryGridModel[] = [];
  categoryId: number | null = null;
  updateCategoryId: string = '';

  constructor(
    private incomeCategoryService: IncomeCategoryService,
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
      this.getCategories();
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
  private getCategories(): void {
    this.loading = true;
    this.spinnerService.show();

    this.incomeCategoryService.getAll(this.pagination).subscribe(
      (result: PaginatedResponseOfIncomeCategoryGridModel) => {
        this.categories = result.data || [];
        this.totalRecords = result.totalRecords;
        this.totalPages = result.totalPages;

        this.spinnerService.hide();
        this.loading = false;
      },
      (error: any) => {
        this.spinnerService.hide();
        this.loading = false;
        this.toastrService.error(
          'Income categories cannot load at this time! Please, try again.',
          'Error',
        );
      },
    );
  }
  openDeleteModal(selectionId: number): void {
    this.categoryId = selectionId;
  }

  // Delete Income Category
  onClickDeleteCategory(): void {
    this.spinnerService.show();

    this.incomeCategoryService.delete(this.categoryId).subscribe(
      (result: boolean) => {
        this.spinnerService.hide();
        this.toastrService.success(
          'Selected income category deleted.',
          'Successful.',
        );
        this.getCategories();
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error(
          'Selected income category cannot be deleted! Please try again.',
        );
        return;
      },
    );
  }

  onClickUpdateIncomeCategoryModal(categoryId: string): void {
    this.updateCategoryId = categoryId;
  }

  // Pagination actions
  goToPage(page: number): void {
    if (page < 0 || page >= this.totalPages) return;
    this.pagination.page = page;
    this.getCategories();
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
    this.getCategories();
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
    this.getCategories();
  }

  // Search with debounce
  onSearch(term: string): void {
    clearTimeout(this.searchTimer);

    this.searchTimer = setTimeout(() => {
      this.pagination.searchTerm = term;
      this.pagination.page = 0;
      this.getCategories();
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
