import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  OnChanges,
  TemplateRef,
  Output
} from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { Subject } from "rxjs";

// ═══════════════════════════════════════════════════════════════════════════
// INTERFACES - Type Definitions
// ═══════════════════════════════════════════════════════════════════════════

/**
 * TableColumn Interface
 * Defines the configuration for each column in the table
 */
export interface TableColumn {
  field: string; // Property name in data object (e.g., 'name', 'email')
  header: string; // Display text in column header
  sortable?: boolean; // Whether column can be sorted (default: true)
  width?: string; // Column width (e.g., '150px', '20%')
  type?: "text" | "number" | "date" | "boolean"; // Data type (for formatting)
  format?: (value: any, row?: any) => string; // Custom formatter function
  align?: string | "flex-start"; // Text alignment: left/center/right
}

/**
 * ServerSortEvent Interface
 * Event payload when sort changes in server-side mode
 */
export interface ServerSortEvent {
  field: string; // Column field being sorted
  order: "asc" | "desc" | null; // Sort direction (null = no sort)
}

/**
 * ServerTableRequest Interface
 * Complete request object for server-side data fetching
 */
export interface ServerTableRequest {
  page: number; // Requested page number (1-based)
  pageSize: number; // Number of items per page
  sortField?: string; // Column to sort by (optional)
  sortOrder?: "asc" | "desc"; // Sort direction (optional)
  searchTerm?: string; // Search query (optional)
}

@Component({
  selector: "app-table",
  templateUrl: "./table.component.html",
  styleUrls: ["./table.component.css"],
  standalone: true,
  imports: [CommonModule, FormsModule]
})
export class TableComponent implements OnInit, OnChanges {
  // ═══════════════════════════════════════════════════════════════════════════
  // INPUT PROPERTIES - Configuration from Parent Component
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Column definitions - configures table structure
   * Parent provides array of TableColumn objects
   * Example: [{ field: 'name', header: 'Name', sortable: true }]
   */
  @Input() columns: TableColumn[] = [];

  /**
   * Data to display in table
   * In server-side mode: Contains current page data only
   * In client-side mode: Can contain current page or full dataset
   */
  @Input() data: any[] = [];

  /**
   * Total number of records (for server-side pagination)
   * Used to calculate total pages and show "X of Y records"
   */
  @Input() totalRecords: number = 0;

  /**
   * Loading state - shows spinner when true
   * Parent sets this when fetching data from server
   */
  @Input() loading: boolean = false;

  /**
   * Current active page number (1-based)
   * In server-side mode: Tracked by parent
   * In client-side mode: Tracked by component
   */
  @Input() currentPage: number = 1;

  /**
   * Number of items to display per page
   * User can change this via page size dropdown
   */
  @Input() pageSize: number = 10;

  /**
   * Determines processing mode:
   * TRUE = Server-side (parent handles filtering, sorting, pagination)
   * FALSE = Client-side (component handles everything internally)
   */
  @Input() serverSide: boolean = true;

  /**
   * Full dataset for client-side mode
   * When serverSide = false, component processes this data locally
   * If empty, falls back to using 'data' property
   */
  @Input() allData: any[] = [];

  /**
   * Custom actions template from parent
   * Captured via @ContentChild in template
   * Parent defines action buttons for each row
   */
  @Input() actionsTemplate!: TemplateRef<any>;

  // ═══════════════════════════════════════════════════════════════════════════
  // OUTPUT EVENTS - Communication back to Parent
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Emits when table needs new data (server-side mode)
   * Contains all parameters needed for API call
   * Parent listens to this and fetches data from server
   */
  @Output() dataRequest = new EventEmitter<ServerTableRequest>();

  /**
   * Emits when page changes
   * Useful for tracking pagination state in parent
   */
  @Output() pageChange = new EventEmitter<{ page: number; pageSize: number }>();

  /**
   * Emits when action button is clicked in a row
   * Parent handles the actual action logic
   * Example: { action: 'edit', row: { id: 1, name: 'John' } }
   */
  @Output() rowAction = new EventEmitter<{ action: string; row: any }>();

  // ═══════════════════════════════════════════════════════════════════════════
  // COMPONENT STATE - Internal Properties
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Applied search term (after user presses Enter or clicks search)
   * Used for actual filtering
   */
  searchTerm: string = "";

  /**
   * Live search input value (bound to input field with ngModel)
   * Allows user to type without immediately triggering search
   */
  searchInput: string = "";

  /**
   * Currently sorted column field
   * null = no sorting applied
   */
  sortField: string | null = null;

  /**
   * Current sort direction
   * null = no sorting, 'asc' = ascending, 'desc' = descending
   */
  sortOrder: "asc" | "desc" | null = null;

  // ═══════════════════════════════════════════════════════════════════════════
  // CLIENT-SIDE MODE PROPERTIES
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Data after applying search filter (client-side)
   * Intermediate step before pagination
   */
  filteredData: any[] = [];

  /**
   * Final data to display (after filter, sort, and pagination)
   * Only used in client-side mode
   */
  paginatedData: any[] = [];

  // ═══════════════════════════════════════════════════════════════════════════
  // REACTIVE SEARCH (RxJS Subject - currently not used in template)
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * RxJS Subject for debounced search
   * Could be used for real-time search with delay
   * Currently search is triggered only on Enter key or icon click
   */
  private searchSubject = new Subject<string>();

  // ═══════════════════════════════════════════════════════════════════════════
  // COMPUTED PROPERTIES (Getters) - Derived State
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * First record number in current view
   * Example: Page 2, PageSize 10 → startRecord = 11
   * Returns 0 if no records
   */
  get startRecord(): number {
    // Get total based on mode (server-side uses totalRecords, client-side uses filteredData length)
    const total = this.serverSide ? this.totalRecords : this.filteredData.length;

    // Return 0 if no data, otherwise calculate start position
    return total === 0 ? 0 : (this.currentPage - 1) * this.pageSize + 1;
  }

  /**
   * Last record number in current view
   * Example: Page 2, PageSize 10, Total 25 → endRecord = 20
   * Ensures we don't exceed total records
   */
  get endRecord(): number {
    const total = this.serverSide ? this.totalRecords : this.filteredData.length;

    // Return minimum of (current page end) or (total records)
    return Math.min(this.currentPage * this.pageSize, total);
  }

  /**
   * Data to display in table rows
   * Switches between server-side and client-side data sources
   */
  get displayData(): any[] {
    return this.serverSide ? this.data : this.paginatedData;
  }

  /**
   * Total number of records for pagination calculations
   * Switches between server-side and client-side totals
   */
  get displayTotalRecords(): number {
    return this.serverSide ? this.totalRecords : this.filteredData.length;
  }

  // ═══════════════════════════════════════════════════════════════════════════
  // LIFECYCLE HOOKS
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * OnInit - Component initialization
   * Called once when component is created
   */
  ngOnInit(): void {
    // If client-side mode, initialize local data processing
    if (!this.serverSide) {
      this.initializeClientSideData();
    }
    // Server-side mode: parent provides data via [data] binding, nothing to initialize
  }

  /**
   * OnChanges - Detects changes to @Input properties
   * Called whenever parent updates input bindings
   */
  ngOnChanges(): void {
    // Re-process data when inputs change (in client-side mode)
    if (!this.serverSide) {
      this.initializeClientSideData();
    }
  }

  // ═══════════════════════════════════════════════════════════════════════════
  // CLIENT-SIDE DATA PROCESSING METHODS
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Initializes data for client-side processing
   * Decides whether to use allData or data property
   */
  private initializeClientSideData(): void {
    // Use allData if provided, otherwise fall back to data
    this.filteredData = [...(this.allData.length > 0 ? this.allData : this.data)];

    // Apply all client-side operations (filter, sort, paginate)
    this.performClientSideOperations();
  }

  /**
   * Main client-side processing pipeline
   * Applies filtering, sorting, and pagination in sequence
   */
  private performClientSideOperations(): void {
    // Start with full dataset
    let result = [...(this.allData.length > 0 ? this.allData : this.data)];

    // STEP 1: Apply search filter (if search term exists)
    if (this.searchTerm) {
      result = this.filterData(result, this.searchTerm);
    }

    // STEP 2: Apply sorting (if sort field is selected)
    if (this.sortField && this.sortOrder) {
      result = this.sortData(result, this.sortField, this.sortOrder);
    }

    // Store filtered/sorted result
    this.filteredData = result;

    // STEP 3: Apply pagination to get current page
    this.paginateData();
  }

  /**
   * CLIENT-SIDE FILTERING
   * Searches through all columns for matching text
   *
   * @param data - Array of data objects to filter
   * @param searchTerm - Text to search for
   * @returns Filtered array
   */
  private filterData(data: any[], searchTerm: string): any[] {
    const term = searchTerm.toLowerCase();

    // Keep rows where ANY column contains the search term
    return data.filter((item) =>
      this.columns.some((column) => {
        const value = item[column.field];

        // Skip null/undefined values
        if (value === null || value === undefined) return false;

        // Convert to string and check if it includes search term
        return String(value).toLowerCase().includes(term);
      })
    );
  }

  /**
   * CLIENT-SIDE SORTING
   * Sorts data by specified field and order
   *
   * @param data - Array to sort
   * @param field - Property name to sort by
   * @param order - Sort direction ('asc' or 'desc')
   * @returns Sorted array (new array, doesn't mutate original)
   */
  private sortData(data: any[], field: string, order: "asc" | "desc"): any[] {
    return [...data].sort((a, b) => {
      const aVal = a[field];
      const bVal = b[field];

      // Handle null/undefined - push to end
      if (aVal === null || aVal === undefined) return 1;
      if (bVal === null || bVal === undefined) return -1;

      let comparison = 0;

      // Type-specific comparison logic
      if (typeof aVal === "string" && typeof bVal === "string") {
        // String comparison (case-insensitive, locale-aware)
        comparison = aVal.toLowerCase().localeCompare(bVal.toLowerCase());
      } else if (typeof aVal === "number" && typeof bVal === "number") {
        // Number comparison
        comparison = aVal - bVal;
      } else if (aVal instanceof Date && bVal instanceof Date) {
        // Date comparison
        comparison = aVal.getTime() - bVal.getTime();
      } else {
        // Fallback: convert to string and compare
        comparison = String(aVal).localeCompare(String(bVal));
      }

      // Apply sort order (asc = normal, desc = reverse)
      return order === "asc" ? comparison : -comparison;
    });
  }

  /**
   * CLIENT-SIDE PAGINATION
   * Slices filtered/sorted data to get current page
   */
  private paginateData(): void {
    // Calculate slice indices
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;

    // Get page slice from filtered data
    this.paginatedData = this.filteredData.slice(startIndex, endIndex);
  }

  // ═══════════════════════════════════════════════════════════════════════════
  // SEARCH HANDLERS
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Handles search input changes (for potential debounced search)
   * Currently not used - search triggers only on Enter or icon click
   *
   * @param term - Current search input value
   */
  onSearchInput(term: string): void {
    this.searchSubject.next(term);
  }

  /**
   * Handles keypress events in search input
   * Triggers search when Enter key is pressed
   *
   * @param event - Keyboard event
   */
  onSearchKeyPress(event: KeyboardEvent): void {
    if (event.key === "Enter") {
      this.performSearch();
    }
  }

  /**
   * Handles click on search icon
   * Alternative way to trigger search
   */
  onSearchIconClick(): void {
    this.performSearch();
  }

  /**
   * Performs the actual search operation
   * Different behavior for server-side vs client-side
   */
  private performSearch(): void {
    // Update applied search term (trim whitespace)
    this.searchTerm = this.searchInput.trim();

    if (this.serverSide) {
      // SERVER-SIDE: Emit event to parent with search term
      // Parent will fetch filtered data from server
      this.emitDataRequest(1); // Reset to page 1 on search
    } else {
      // CLIENT-SIDE: Filter data locally
      this.currentPage = 1; // Reset to first page
      this.performClientSideOperations(); // Reprocess with new search term
    }
  }

  /**
   * Clears the search and resets to show all data
   */
  clearSearch(): void {
    this.searchInput = ""; // Clear input field
    this.searchTerm = ""; // Clear applied search term

    if (this.serverSide) {
      // SERVER-SIDE: Request all data from server
      this.emitDataRequest(1);
    } else {
      // CLIENT-SIDE: Reset and show all data
      this.currentPage = 1;
      this.performClientSideOperations();
    }
  }

  // ═══════════════════════════════════════════════════════════════════════════
  // SORT HANDLERS
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Handles column header click for sorting
   * Cycles through: no sort → asc → desc → no sort
   *
   * @param field - Column field to sort by
   */
  onSort(field: string): void {
    if (this.sortField === field) {
      // Clicking same column - cycle through sort states
      if (this.sortOrder === "asc") {
        this.sortOrder = "desc"; // asc → desc
      } else if (this.sortOrder === "desc") {
        // desc → no sort
        this.sortOrder = null;
        this.sortField = null;
      }
    } else {
      // Clicking different column - start with ascending
      this.sortField = field;
      this.sortOrder = "asc";
    }

    if (this.serverSide) {
      // SERVER-SIDE: Request sorted data from server
      this.emitDataRequest(this.currentPage);
    } else {
      // CLIENT-SIDE: Sort data locally
      this.performClientSideOperations();
    }
  }

  // ═══════════════════════════════════════════════════════════════════════════
  // PAGINATION HANDLERS
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Handles page change (from pagination component)
   *
   * @param page - New page number to navigate to
   */
  onPageChange(page: number): void {
    if (this.serverSide) {
      // SERVER-SIDE: Request new page from server
      this.emitDataRequest(page);
    } else {
      // CLIENT-SIDE: Update current page and re-paginate
      this.currentPage = page;
      this.paginateData(); // Slice data for new page

      // Emit event to parent (for tracking)
      this.pageChange.emit({ page, pageSize: this.pageSize });
    }
  }

  /**
   * Handles page size change (items per page dropdown)
   * Recalculates which page to show to keep similar data visible
   *
   * @param event - Change event from select element
   */
  onPageSizeChange(event: Event): void {
    const select = event.target as HTMLSelectElement;
    const newPageSize = parseInt(select.value, 10);

    this.pageSize = newPageSize;

    if (this.serverSide) {
      // SERVER-SIDE MODE:
      // Calculate which page to show based on the first item currently visible
      // Example: User on page 3 (items 21-30), changes to 50 per page
      // → First visible item is 21, so new page = ceil(21/50) = 1
      const firstVisibleItem = (this.currentPage - 1) * this.pageSize + 1;
      const newPage = Math.ceil(firstVisibleItem / newPageSize);
      this.emitDataRequest(newPage);
    } else {
      // CLIENT-SIDE MODE:
      // Recalculate current page to show similar data
      const firstVisibleItem = (this.currentPage - 1) * this.pageSize + 1;
      this.currentPage = Math.ceil(firstVisibleItem / newPageSize);

      // Reprocess with new page size
      this.performClientSideOperations();

      // Emit event to parent
      this.pageChange.emit({ page: this.currentPage, pageSize: newPageSize });
    }
  }

  // ═══════════════════════════════════════════════════════════════════════════
  // SERVER-SIDE DATA REQUEST
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Emits data request event to parent (server-side mode)
   * Parent receives this and fetches data from API
   *
   * @param page - Page number to request
   */
  emitDataRequest(page: number): void {
    // Build request object with all parameters
    const request: ServerTableRequest = {
      page,
      pageSize: this.pageSize,
      searchTerm: this.searchTerm || undefined, // Only include if not empty
      sortField: this.sortField || undefined, // Only include if sorting
      sortOrder: this.sortOrder || undefined // Only include if sorting
    };

    // Emit to parent component
    this.dataRequest.emit(request);
  }

  // ═══════════════════════════════════════════════════════════════════════════
  // CELL FORMATTING & DISPLAY
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Formats cell value for display
   * Uses custom formatter if provided in column config
   *
   * @param value - Raw cell value
   * @param column - Column configuration
   * @param row - Full row object (optional, for formatters that need other fields)
   * @returns Formatted string (can contain HTML)
   */
  formatValue(value: any, column: TableColumn, row?: any): string {
    // If column has custom format function, use it
    if (column.format && typeof column.format === "function") {
      return column.format(value, row);
    }

    // Handle null/undefined
    if (value === null || value === undefined) {
      return "";
    }

    // Default: convert to string
    return String(value);
  }

  // ═══════════════════════════════════════════════════════════════════════════
  // ROW ACTION HANDLER
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Handles action button clicks in table rows
   * Called from parent's actions template via context binding
   * Emits event back to parent for actual action handling
   *
   * @param action - Action identifier (e.g., 'edit', 'delete', 'view')
   * @param row - Row data object
   */
  onRowAction(action: string, row: any): void {
    this.rowAction.emit({ action, row });
  }

  // ═══════════════════════════════════════════════════════════════════════════
  // UTILITY METHODS
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Converts text alignment to flexbox justify-content value
   * Used for aligning content within table headers
   *
   * @param align - Text alignment ('left', 'center', 'right')
   * @returns Flexbox justify-content value
   */
  getJustifyContent(align?: string): string {
    switch (align) {
      case "left":
        return "flex-start";
      case "center":
        return "center";
      case "right":
        return "flex-end";
      default:
        return "flex-start"; // Default to left alignment
    }
  }
}
