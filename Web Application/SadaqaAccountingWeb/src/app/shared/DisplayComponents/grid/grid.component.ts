import { CommonModule } from "@angular/common";
import {
  Component,
  ContentChild,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
  TemplateRef,
  ViewChild,
  ElementRef,
  AfterViewInit,
  OnDestroy,
  ChangeDetectorRef
} from "@angular/core";
import { FormsModule } from "@angular/forms";
import TomSelect from "tom-select"; // Third-party dropdown library for enhanced select boxes

// Interface for sort dropdown options
export interface GridSortOption {
  field: string; // Property name to sort by (can be nested like "user.name")
  label: string; // Display text in dropdown
}

@Component({
  selector: "app-grid",
  templateUrl: "./grid.component.html",
  styleUrls: ["./grid.component.css"],
  standalone: true,
  imports: [CommonModule, FormsModule],
  providers: []
})
export class GridComponent implements OnInit, AfterViewInit, OnChanges, OnDestroy {
  // ═══════════════════════════════════════════════════════════════════════════
  // INPUT PROPERTIES - Data from Parent Component
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Main data array to display in grid
   * Parent provides this array which gets processed based on serverSide flag
   */
  @Input() allData: any[] = [];

  /**
   * Determines if pagination/filtering happens on server or client
   * TRUE = Parent handles all data processing, this component just displays
   * FALSE = Component handles search, sort, pagination internally
   */
  @Input() serverSide: boolean = false;

  /** Number of items per page */
  @Input() pageSize: number = 10;

  /** Current active page number (1-based) */
  @Input() currentPage: number = 1;

  /** Loading state - shows spinner when true */
  @Input() loading: boolean = false;

  /** Number of columns in grid layout (1-6) */
  @Input() gridColumns: number = 3;

  /** Array of sortable fields with labels for dropdown */
  @Input() sortOptions: GridSortOption[] = [];

  /** Enable/disable animation on grid items */
  @Input() animate: boolean = true;

  // ═══════════════════════════════════════════════════════════════════════════
  // OUTPUT EVENTS - Communication back to Parent
  // ═══════════════════════════════════════════════════════════════════════════

  /** Emits when page changes (for server-side pagination) */
  @Output() pageChange = new EventEmitter<{ page: number; pageSize: number }>();

  /** Emits when search term changes (for server-side search) */
  @Output() searchChange = new EventEmitter<string>();

  /** Emits when sort field or order changes (for server-side sorting) */
  @Output() sortChange = new EventEmitter<{ field: string; order: "asc" | "desc" }>();

  // ═══════════════════════════════════════════════════════════════════════════
  // CONTENT PROJECTION - Custom Card Template from Parent
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Captures the <ng-template #cardTemplate> from parent component
   * Parent defines how each grid item should look
   * If null, falls back to default JSON display
   */
  @ContentChild("cardTemplate") cardTemplate: TemplateRef<any> | null = null;

  // ═══════════════════════════════════════════════════════════════════════════
  // VIEW CHILDREN - Direct DOM Access
  // ═══════════════════════════════════════════════════════════════════════════

  /** Reference to sort dropdown select element (for TomSelect initialization) */
  @ViewChild("sortSelect") sortSelectRef!: ElementRef;

  /** Reference to columns dropdown select element (for TomSelect initialization) */
  @ViewChild("columnsSelect") columnsSelectRef!: ElementRef;

  /** Reference to search input (for focus management) */
  @ViewChild("searchInputElement") searchInputRef!: ElementRef;

  // ═══════════════════════════════════════════════════════════════════════════
  // COMPONENT STATE - Internal Properties
  // ═══════════════════════════════════════════════════════════════════════════

  /** Applied search term (only updated when user hits Enter or clicks search icon) */
  searchTerm: string = "";

  /** Live search input value (bound to input field with ngModel) */
  searchInput: string = "";

  /** Currently selected sort field (empty string means no sorting) */
  selectedSortField: string = "";

  /** Sort direction: ascending or descending */
  sortOrder: "asc" | "desc" = "asc";

  /** Actual items displayed in grid (after filtering, sorting, pagination) */
  displayItems: any[] = [];

  /** Items after search filter applied (used for client-side processing) */
  filteredData: any[] = [];

  /** Total number of records (after filtering) */
  displayTotalRecords: number = 0;

  /** First record number in current page display */
  startRecord: number = 0;

  /** Last record number in current page display */
  endRecord: number = 0;

  /** CSS Grid template string (e.g., "repeat(3, 1fr)" for 3 columns) */
  gridTemplateColumns: string = "";

  // TomSelect instances (enhanced dropdown library)
  private sortTomSelect: any; // Enhanced sort dropdown
  private columnsTomSelect: any; // Enhanced columns dropdown

  /** Tracks if search input currently has focus (for maintaining focus after data updates) */
  private isSearchFocused: boolean = false;

  constructor(
    private cdr: ChangeDetectorRef, // For manual change detection if needed
  ) {}

  // ═══════════════════════════════════════════════════════════════════════════
  // LIFECYCLE HOOKS
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * OnInit - Component initialization
   * Called once when component is created
   */
  ngOnInit(): void {
    this.updateGridColumns(); // Set initial grid column CSS
    this.processData(); // Process initial data
  }

  /**
   * AfterViewInit - After view is fully initialized
   * Called once after first ngAfterContentChecked
   * Safe to access @ViewChild elements here
   */
  ngAfterViewInit(): void {
    // Delay to ensure DOM elements are ready
    setTimeout(() => {
      this.initializeTomSelects(); // Initialize enhanced dropdowns

      // Add focus tracking for search input
      if (this.searchInputRef) {
        // Track when user focuses search box
        this.searchInputRef.nativeElement.addEventListener("focus", () => {
          this.isSearchFocused = true;
        });

        // Track when user leaves search box
        this.searchInputRef.nativeElement.addEventListener("blur", () => {
          this.isSearchFocused = false;
        });
      }
    }, 100);
  }

  /**
   * OnChanges - Detects changes to @Input properties
   * Called whenever parent updates input bindings
   * @param changes - Object containing all changed properties
   */
  ngOnChanges(changes: SimpleChanges): void {
    // If gridColumns changed (but not on first initialization)
    if (changes["gridColumns"] && !changes["gridColumns"].firstChange) {
      this.updateGridColumns(); // Recalculate CSS grid template
      this.updateColumnsTomSelectValue(); // Update dropdown to match
    }

    // If data or pagination settings changed, reprocess everything
    if (changes["allData"] || changes["pageSize"] || changes["currentPage"]) {
      this.processData(); // Filter, sort, paginate data
      this.restoreSearchFocus(); // Keep search box focused if user was typing
    }
  }

  /**
   * OnDestroy - Cleanup before component is destroyed
   * Called once before component is removed from DOM
   */
  ngOnDestroy(): void {
    this.destroyTomSelects(); // Clean up TomSelect instances to prevent memory leaks
  }

  // ═══════════════════════════════════════════════════════════════════════════
  // TOM SELECT INITIALIZATION & MANAGEMENT
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Initializes both TomSelect dropdowns
   */
  private initializeTomSelects(): void {
    this.initializeSortTomSelect(); // Initialize sort dropdown
    this.initializeColumnsTomSelect(); // Initialize columns dropdown
  }

  /**
   * Initialize enhanced sort dropdown with TomSelect
   * Provides better UX than native <select>
   */
  private initializeSortTomSelect(): void {
    // Only initialize if element exists and not already initialized
    if (this.sortSelectRef && !this.sortTomSelect) {
      try {
        this.sortTomSelect = new TomSelect(this.sortSelectRef.nativeElement, {
          create: false, // Don't allow creating new options
          allowEmptyOption: true, // Allow selecting "None"
          placeholder: "None", // Placeholder text

          onInitialize: () => {
            console.log("Sort TomSelect initialized");
          },

          // Called when user selects a different option
          onChange: (value: string) => {
            this.selectedSortField = value || ""; // Update selected field (empty = no sort)

            // If server-side mode, emit event to parent
            if (this.serverSide) {
              if (this.selectedSortField) {
                // Tell parent to handle sorting on server
                this.sortChange.emit({ field: this.selectedSortField, order: this.sortOrder });
              }
            } else {
              // Client-side: process data locally
              this.processData();
            }
          }
        });

        // Set initial value without triggering onChange
        this.sortTomSelect.setValue(this.selectedSortField || "", false);
      } catch (error) {
        console.error("Error initializing Sort TomSelect:", error);
      }
    }
  }

  /**
   * Initialize enhanced columns dropdown with TomSelect
   * Controls how many columns to display in grid
   */
  private initializeColumnsTomSelect(): void {
    // Only initialize if element exists and not already initialized
    if (this.columnsSelectRef && !this.columnsTomSelect) {
      try {
        this.columnsTomSelect = new TomSelect(this.columnsSelectRef.nativeElement, {
          create: false, // Don't allow creating new options
          allowEmptyOption: false, // Must always have a value

          onInitialize: () => {
            console.log("Columns TomSelect initialized");
          },

          // Called when user changes column count
          onChange: (value: string) => {
            this.gridColumns = parseInt(value, 10); // Convert string to number
            this.updateGridColumns(); // Recalculate CSS grid
          }
        });

        // Set initial value without triggering onChange
        this.columnsTomSelect.setValue(this.gridColumns.toString(), false);
      } catch (error) {
        console.error("Error initializing Columns TomSelect:", error);
      }
    }
  }

  /**
   * Updates the columns dropdown value programmatically
   * Used when gridColumns changes from parent
   */
  private updateColumnsTomSelectValue(): void {
    if (this.columnsTomSelect) {
      try {
        // Update dropdown without triggering onChange event
        this.columnsTomSelect.setValue(this.gridColumns.toString(), false);
      } catch (error) {
        console.error("Error updating Columns TomSelect value:", error);
      }
    }
  }

  /**
   * Cleanup TomSelect instances to prevent memory leaks
   * Called in ngOnDestroy
   */
  private destroyTomSelects(): void {
    // Destroy sort dropdown
    if (this.sortTomSelect) {
      try {
        this.sortTomSelect.destroy();
      } catch (error) {
        console.error("Error destroying Sort TomSelect:", error);
      }
      this.sortTomSelect = null;
    }

    // Destroy columns dropdown
    if (this.columnsTomSelect) {
      try {
        this.columnsTomSelect.destroy();
      } catch (error) {
        console.error("Error destroying Columns TomSelect:", error);
      }
      this.columnsTomSelect = null;
    }
  }

  // ═══════════════════════════════════════════════════════════════════════════
  // GRID LAYOUT MANAGEMENT
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Updates CSS Grid template string based on selected columns
   * Creates equal-width columns (e.g., "repeat(3, 1fr)" = 3 equal columns)
   */
  updateGridColumns(): void {
    this.gridTemplateColumns = `repeat(${this.gridColumns}, 1fr)`;
  }

  // ═══════════════════════════════════════════════════════════════════════════
  // DATA PROCESSING - The Heart of the Component
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Main data processing pipeline
   * Decides whether to use server-side or client-side processing
   */
  private processData(): void {
    if (this.serverSide) {
      // SERVER-SIDE MODE:
      // Parent already filtered/sorted/paginated data
      // Just display what parent gives us
      this.displayItems = this.allData;
      this.displayTotalRecords = this.allData.length;
    } else {
      // CLIENT-SIDE MODE:
      // We handle everything locally
      this.applyClientSideFilter(); // 1. Filter by search term
      this.applyClientSideSort(); // 2. Sort filtered data
      this.applyClientSidePagination(); // 3. Get current page slice
    }

    // Calculate which records are showing (e.g., "Showing 1-10 of 50")
    this.calculateRecordRange();
  }

  /**
   * Restores focus to search input if user was typing
   * Prevents losing focus when data updates
   */
  private restoreSearchFocus(): void {
    if (this.isSearchFocused && this.searchInputRef) {
      setTimeout(() => {
        this.searchInputRef.nativeElement.focus();

        // Restore cursor position to end of input
        const input = this.searchInputRef.nativeElement;
        const length = input.value.length;
        input.setSelectionRange(length, length); // Set cursor at end
      }, 0);
    }
  }

  /**
   * CLIENT-SIDE FILTERING
   * Searches through all object properties for matching text
   */
  private applyClientSideFilter(): void {
    // If no search term, show all data
    if (!this.searchTerm.trim()) {
      this.filteredData = [...this.allData]; // Clone array
    } else {
      const term = this.searchTerm.toLowerCase();

      // Filter items where ANY property value contains search term
      this.filteredData = this.allData.filter((item) =>
        Object.values(item).some((val) => val && val.toString().toLowerCase().includes(term))
      );
    }

    // Update total count after filtering
    this.displayTotalRecords = this.filteredData.length;
  }

  /**
   * CLIENT-SIDE SORTING
   * Sorts filtered data by selected field and order
   */
  private applyClientSideSort(): void {
    // No sorting if no field selected
    if (!this.selectedSortField) {
      return;
    }

    this.filteredData.sort((a, b) => {
      // Get values for comparison (supports nested properties like "user.name")
      const aVal = this.getNestedValue(a, this.selectedSortField);
      const bVal = this.getNestedValue(b, this.selectedSortField);

      // Handle null/undefined values (push to end)
      if (aVal === null || aVal === undefined) return 1;
      if (bVal === null || bVal === undefined) return -1;

      let comparison = 0;

      // Type-specific comparison
      if (typeof aVal === "string" && typeof bVal === "string") {
        // String comparison (locale-aware)
        comparison = aVal.localeCompare(bVal);
      } else if (typeof aVal === "number" && typeof bVal === "number") {
        // Number comparison
        comparison = aVal - bVal;
      } else if (aVal instanceof Date && bVal instanceof Date) {
        // Date comparison
        comparison = aVal.getTime() - bVal.getTime();
      } else {
        // Fallback: convert to string and compare
        const aStr = String(aVal).toLowerCase();
        const bStr = String(bVal).toLowerCase();
        comparison = aStr.localeCompare(bStr);
      }

      // Apply sort order (asc = normal, desc = reverse)
      return this.sortOrder === "asc" ? comparison : -comparison;
    });
  }

  /**
   * Safely gets nested property value from object
   * Example: getNestedValue(user, "address.city") -> user.address.city
   * @param obj - Object to extract value from
   * @param path - Dot-separated property path
   */
  private getNestedValue(obj: any, path: string): any {
    return path.split(".").reduce((current, prop) => current?.[prop], obj);
  }

  /**
   * CLIENT-SIDE PAGINATION
   * Slices filtered/sorted data to get current page
   */
  private applyClientSidePagination(): void {
    const start = (this.currentPage - 1) * this.pageSize; // Calculate start index
    const end = start + this.pageSize; // Calculate end index
    this.displayItems = this.filteredData.slice(start, end); // Get slice
  }

  /**
   * Calculates which records are currently showing
   * Used for "Showing 1-10 of 50" type displays
   */
  private calculateRecordRange(): void {
    if (this.displayTotalRecords === 0) {
      // No records
      this.startRecord = 0;
      this.endRecord = 0;
    } else {
      // Calculate first record number (1-based)
      this.startRecord = (this.currentPage - 1) * this.pageSize + 1;

      // Calculate last record number (don't exceed total)
      this.endRecord = Math.min(this.currentPage * this.pageSize, this.displayTotalRecords);
    }
  }

  // ═══════════════════════════════════════════════════════════════════════════
  // USER INTERACTION HANDLERS
  // ═══════════════════════════════════════════════════════════════════════════

  /**
   * Handles keypress events in search input
   * Triggers search when Enter key is pressed
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
      // SERVER-SIDE: Emit event to parent to handle search
      this.searchChange.emit(this.searchTerm);
    } else {
      // CLIENT-SIDE: Process data locally
      this.currentPage = 1; // Reset to first page when searching
      this.processData(); // Re-process with new search term
    }
  }

  /**
   * Clears the search and resets to show all data
   */
  clearSearch(): void {
    this.searchInput = ""; // Clear input field
    this.searchTerm = ""; // Clear applied search term

    if (this.serverSide) {
      // SERVER-SIDE: Notify parent to clear search
      this.searchChange.emit("");
    } else {
      // CLIENT-SIDE: Reset and show all data
      this.currentPage = 1; // Back to first page
      this.processData(); // Reprocess without filter
    }

    // Return focus to search input
    if (this.searchInputRef) {
      setTimeout(() => {
        this.searchInputRef.nativeElement.focus();
      }, 0);
    }
  }

  /**
   * Toggles sort order between ascending and descending
   * Called when user clicks the sort order button
   */
  toggleSortOrder(): void {
    // Toggle between asc and desc
    this.sortOrder = this.sortOrder === "asc" ? "desc" : "asc";

    if (this.serverSide && this.selectedSortField) {
      // SERVER-SIDE: Emit event to parent
      this.sortChange.emit({ field: this.selectedSortField, order: this.sortOrder });
    } else {
      // CLIENT-SIDE: Re-sort data
      this.processData();
    }
  }
}
