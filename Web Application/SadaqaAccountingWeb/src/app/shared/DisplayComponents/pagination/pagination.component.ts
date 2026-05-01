/**
 * PAGINATION COMPONENT
 * ═══════════════════════════════════════════════════════════════════════════
 * A fully reusable Angular standalone component that provides comprehensive
 * pagination controls for data tables and grids.
 *
 * FEATURES:
 * - Page number navigation (First, Previous, Next, Last)
 * - Intelligent page range display (shows subset of pages with ellipsis)
 * - Page size selector with TomSelect enhancement
 * - Direct page jump input
 * - Loading state management
 * - Automatic page recalculation on data changes
 *
 * USAGE:
 * <app-pagination
 *   [totalRecords]="totalCount"
 *   [pageSize]="itemsPerPage"
 *   [currentPage]="activePage"
 *   (pageChange)="onPageChange($event)">
 * </app-pagination>
 */

import { CommonModule } from "@angular/common";
import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  SimpleChanges,
  ViewChild,
  ElementRef,
  AfterViewInit,
  OnDestroy,
  OnChanges
} from "@angular/core";
import TomSelect from "tom-select"; // Enhanced dropdown library

// ═══════════════════════════════════════════════════════════════════════════
// INTERFACES - Type Definitions
// ═══════════════════════════════════════════════════════════════════════════

/**
 * ServerPaginationConfig Interface
 * Event payload emitted when pagination state changes
 * Parent component receives this to fetch new data
 */
export interface ServerPaginationConfig {
  currentPage: number; // New page number to display (1-based)
  pageSize: number; // Number of items per page
}

@Component({
  selector: "app-pagination",
  templateUrl: "./pagination.component.html",
  styleUrls: ["./pagination.component.css"],
  standalone: true,
  imports: [CommonModule]
})
export class PaginationComponent implements OnInit, AfterViewInit, OnChanges, OnDestroy {
  // ═══════════════════════════════════════════════════════════════════════════
  // INPUT PROPERTIES - Configuration from Parent Component
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Total number of records available (from API)
   * Used to calculate total pages
   * Example: 250 records with pageSize 10 = 25 pages
   */
  @Input() totalRecords: number = 0;

  /**
   * Number of items per page (default: 10)
   * User can change this via page size dropdown
   */
  @Input() pageSize: number = 10;

  /**
   * Currently selected page number (starts from 1)
   * Updated by parent when data loads
   */
  @Input() currentPage: number = 1;

  /**
   * Dropdown list of selectable page sizes
   * Common options: [5, 10, 25, 50, 100]
   */
  @Input() pageSizeOptions: number[] = [5, 10, 25, 50, 100];

  /**
   * Maximum number of page buttons visible at once
   * Example: maxVisiblePages = 5 shows [1] [2] [3] [4] [5] ... [50]
   * Prevents cluttering UI when there are many pages
   */
  @Input() maxVisiblePages: number = 5;

  /**
   * Loading state: disables buttons when true
   * Prevents user from clicking during data fetch
   */
  @Input() loading: boolean = false;

  // ═══════════════════════════════════════════════════════════════════════════
  // OUTPUT EVENTS - Communication back to Parent
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Emits the new pagination state (currentPage + pageSize)
   * whenever the user navigates or changes page size
   *
   * Parent listens to this event to fetch appropriate data from API
   * Example: (pageChange)="onPaginationChange($event)"
   */
  @Output() pageChange = new EventEmitter<ServerPaginationConfig>();

  // ═══════════════════════════════════════════════════════════════════════════
  // VIEW CHILDREN - Direct DOM Access
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Template reference to native <select> for TomSelect binding
   * Used to initialize enhanced dropdown in AfterViewInit
   */
  @ViewChild("pageSizeSelect") pageSizeSelectRef!: ElementRef;

  // ═══════════════════════════════════════════════════════════════════════════
  // COMPONENT STATE - Derived/Calculated Properties
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Total number of pages based on totalRecords and pageSize
   * Calculated: Math.ceil(totalRecords / pageSize)
   * Minimum: 1 (even with 0 records)
   */
  totalPages: number = 1;

  /**
   * Array of page numbers to display as buttons
   * Example: [3, 4, 5, 6, 7] when on page 5 with maxVisiblePages=5
   * Dynamically calculated to show pages around current page
   */
  visiblePages: number[] = [];

  /**
   * Whether to show ellipsis (...) at end of page buttons
   * True when there are more pages beyond visible range
   * Example: [1] [2] [3] [4] [5] ... [50]
   */
  showEllipsisEnd: boolean = false;

  /**
   * First record number in current page
   * Example: Page 3, PageSize 10 → startItem = 21
   * Display: "Showing 21-30 of 100"
   */
  startItem: number = 0;

  /**
   * Last record number in current page
   * Ensures we don't exceed totalRecords
   * Example: Last page with 5 items → endItem = totalRecords
   */
  endItem: number = 0;

  // ═══════════════════════════════════════════════════════════════════════════
  // INTERNAL STATE - Private Tracking Variables
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * TomSelect instance for page size dropdown
   * Stored to manage lifecycle (enable/disable/destroy)
   */
  private pageSizeTomSelect: any;

  /**
   * Tracks if component has completed initialization
   * Prevents onChange events during setup
   * Set to true after TomSelect initialization delay
   */
  private isInitialized: boolean = false;

  /**
   * Tracks the last pageSize value emitted to parent
   * Prevents duplicate events when parent updates pageSize
   * Used to detect if change is user-initiated or parent-initiated
   */
  private lastEmittedPageSize: number = 0;

  // ═══════════════════════════════════════════════════════════════════════════
  // LIFECYCLE HOOKS
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * OnInit - Component initialization
   * Called once when component is created
   */
  ngOnInit(): void {
    // Calculate initial pagination state (pages, ranges, etc.)
    this.calculatePagination();

    // Track initial page size to prevent duplicate events
    this.lastEmittedPageSize = this.pageSize;
  }

  /**
   * AfterViewInit - After view is fully initialized
   * Safe to access @ViewChild elements here
   * Delayed to ensure DOM is ready
   */
  ngAfterViewInit(): void {
    // Delay TomSelect initialization to ensure DOM is stable
    setTimeout(() => {
      this.initializeTomSelect();
    }, 100);
  }

  /**
   * OnChanges - Detects changes to @Input properties
   * Called whenever parent updates input bindings
   *
   * @param changes - Object containing all changed properties with previous/current values
   */
  ngOnChanges(changes: SimpleChanges): void {
    // Track pageSize changes to prevent duplicate events
    if (changes["pageSize"] && !changes["pageSize"].firstChange) {
      this.lastEmittedPageSize = this.pageSize;
    }

    // Recalculate pagination whenever any input changes
    // (totalRecords, pageSize, currentPage, etc.)
    this.calculatePagination();

    // Update TomSelect dropdown if pageSize changed after initialization
    if (changes["pageSize"] && this.isInitialized) {
      this.updateTomSelectValue();
    }

    // Handle enable/disable of controls when loading state changes
    if (changes["loading"]) {
      this.handleLoadingStateChange();
    }
  }

  /**
   * OnDestroy - Cleanup before component is destroyed
   * Called once before component is removed from DOM
   */
  ngOnDestroy(): void {
    // Clean up TomSelect instance to prevent memory leaks
    this.destroyTomSelect();
  }

  // ═══════════════════════════════════════════════════════════════════════════
  // TOM SELECT INITIALIZATION & MANAGEMENT
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Initialize TomSelect for page size dropdown
   * Enhances native <select> with better UX
   * Configures onChange handler to emit events to parent
   */
  private initializeTomSelect(): void {
    // Only initialize if element exists and not already initialized
    if (this.pageSizeSelectRef && !this.pageSizeTomSelect) {
      // Ensure any existing instance is cleaned up
      this.destroyTomSelect();

      // Create new TomSelect instance
      this.pageSizeTomSelect = new TomSelect(this.pageSizeSelectRef.nativeElement, {
        create: false, // Don't allow creating custom options
        allowEmptyOption: false, // Must always have a value selected

        /**
         * onChange handler - Called when user selects different page size
         * @param value - Selected page size as string (e.g., "25")
         */
        onChange: (value: string) => {
          // Only process if:
          // 1. Component is fully initialized (prevents event during setup)
          // 2. Not currently loading (prevents change during data fetch)
          // 3. Value is not empty
          if (this.isInitialized && !this.loading && value) {
            const newPageSize = parseInt(value, 10);

            // Validate parsed number and check if it's actually different
            if (!isNaN(newPageSize) && newPageSize !== this.lastEmittedPageSize) {
              // Update tracking variable
              this.lastEmittedPageSize = newPageSize;

              // SMART PAGE RECALCULATION:
              // Keep the first visible item in the same range when changing page size
              // Example: User on page 3 (items 21-30), changes to pageSize 50
              // → First visible item is 21, so new page = ceil(21/50) = 1
              const firstVisibleItem = (this.currentPage - 1) * this.pageSize + 1;
              const newPage = Math.ceil(firstVisibleItem / newPageSize);

              // Emit event to parent with new page and page size
              this.emitPageChange(newPage, newPageSize);
            }
          }
        }
      });

      // Set initial value without triggering onChange event
      // true = silent mode (no event)
      this.pageSizeTomSelect.setValue(this.pageSize.toString(), true);

      // Mark as initialized after a delay to prevent immediate onChange events
      setTimeout(() => (this.isInitialized = true), 150);
    }
  }

  /**
   * Update TomSelect value when parent changes page size
   * Temporarily disables change detection to prevent loop
   * Used when parent updates pageSize input property
   */
  private updateTomSelectValue(): void {
    if (this.pageSizeTomSelect) {
      // Store initialization state
      const wasInitialized = this.isInitialized;

      // Temporarily mark as not initialized to prevent onChange event
      this.isInitialized = false;

      // Update dropdown value silently
      this.pageSizeTomSelect.setValue(this.pageSize.toString(), true);

      // Update tracking variable
      this.lastEmittedPageSize = this.pageSize;

      // Restore initialization state after a delay
      setTimeout(() => (this.isInitialized = wasInitialized), 100);
    }
  }

  /**
   * Handle loading state change
   * Disables dropdown during loading to prevent user interaction
   */
  private handleLoadingStateChange(): void {
    if (this.pageSizeTomSelect) {
      // Disable dropdown when loading, enable when done
      this.loading ? this.pageSizeTomSelect.disable() : this.pageSizeTomSelect.enable();
    }
  }

  /**
   * Cleanup TomSelect instance to prevent memory leaks
   * Called in ngOnDestroy and before re-initialization
   */
  private destroyTomSelect(): void {
    if (this.pageSizeTomSelect) {
      // Destroy TomSelect instance
      this.pageSizeTomSelect.destroy();

      // Clear reference
      this.pageSizeTomSelect = null;

      // Reset initialization flag
      this.isInitialized = false;
    }
  }

  // ═══════════════════════════════════════════════════════════════════════════
  // PAGINATION CALCULATIONS
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Compute all pagination details
   * Main calculation method called on init and whenever inputs change
   * Calculates: totalPages, currentPage bounds, item ranges, visible pages
   */
  private calculatePagination(): void {
    // Calculate total pages (minimum 1)
    // Example: 48 records ÷ 10 per page = 4.8 → 5 pages
    this.totalPages = Math.max(1, Math.ceil(this.totalRecords / this.pageSize));

    // Ensure currentPage doesn't exceed totalPages
    // Example: If totalPages = 5, currentPage can't be 6
    this.currentPage = Math.min(this.currentPage, this.totalPages);

    // Ensure currentPage is at least 1
    this.currentPage = Math.max(1, this.currentPage);

    // Calculate range of items shown in current page
    // Example: Page 3, PageSize 10 → Shows items 21-30
    this.startItem = this.totalRecords === 0 ? 0 : (this.currentPage - 1) * this.pageSize + 1;
    this.endItem = Math.min(this.currentPage * this.pageSize, this.totalRecords);

    // Determine which page numbers to show as buttons
    this.calculateVisiblePages();
  }

  /**
   * Determine visible page range for buttons
   * Creates array of page numbers to display
   * Shows pages around current page with ellipsis for large ranges
   *
   * Algorithm:
   * 1. Try to center current page in visible range
   * 2. Adjust if too close to start/end
   * 3. Always show maxVisiblePages buttons (or less if fewer total pages)
   *
   * Examples with maxVisiblePages = 5:
   * - Current: 1  → [1] [2] [3] [4] [5] ... [50]
   * - Current: 10 → [8] [9] [10] [11] [12] ... [50]
   * - Current: 50 → [46] [47] [48] [49] [50]
   */
  private calculateVisiblePages(): void {
    const pages: number[] = [];

    // Calculate how many pages to show on each side of current page
    // Example: maxVisiblePages = 5 → half = 2 (show 2 before, current, 2 after)
    const half = Math.floor(this.maxVisiblePages / 2);

    // Calculate start of visible range
    // Try to center current page, but not before page 1
    let start = Math.max(1, this.currentPage - half);

    // Calculate end of visible range
    // Try to show maxVisiblePages total, but not beyond totalPages
    let end = Math.min(this.totalPages, start + this.maxVisiblePages - 1);

    // Adjust start if we're near the end and have room at the beginning
    // Example: Total 50 pages, current 49, maxVisible 5
    // → Initially [47,48,49,50,51] but 51 doesn't exist
    // → Adjust to [46,47,48,49,50]
    if (end - start + 1 < this.maxVisiblePages) {
      start = Math.max(1, end - this.maxVisiblePages + 1);
    }

    // Build array of page numbers in visible range
    for (let i = start; i <= end; i++) {
      pages.push(i);
    }

    // Store results
    this.visiblePages = pages;

    // Show ellipsis if there are more pages after visible range
    this.showEllipsisEnd = end < this.totalPages;
  }

  // ═══════════════════════════════════════════════════════════════════════════
  // USER INTERACTION HANDLERS
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Navigate to selected page
   * Called when user clicks page button or navigation arrow
   *
   * @param page - Page number to navigate to (1-based)
   */
  goToPage(page: number): void {
    // Only navigate if:
    // 1. Page is valid (between 1 and totalPages)
    // 2. Page is different from current page
    // 3. Not currently loading data
    if (page >= 1 && page <= this.totalPages && page !== this.currentPage && !this.loading) {
      this.emitPageChange(page, this.pageSize);
    }
  }

  /**
   * Handle direct page input (on Enter key)
   * Called when user types page number and presses Enter
   * Validates input and navigates to page if valid
   *
   * @param event - Input event from text field
   */
  onPageJump(event: Event): void {
    // Don't process if loading
    if (this.loading) return;

    // Get input element and parse value
    const input = event.target as HTMLInputElement;
    const page = parseInt(input.value, 10);

    // Validate page number
    if (!isNaN(page) && page >= 1 && page <= this.totalPages) {
      // Valid page - navigate to it
      this.goToPage(page);
    } else {
      // Invalid page - reset input to current page
      input.value = this.currentPage.toString();
    }
  }

  // ═══════════════════════════════════════════════════════════════════════════
  // EVENT EMISSION
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Emit page change event to parent
   * Parent receives this and fetches appropriate data from API
   *
   * @param page - New page number
   * @param pageSize - Number of items per page
   */
  private emitPageChange(page: number, pageSize: number): void {
    this.pageChange.emit({ currentPage: page, pageSize });
  }
}
