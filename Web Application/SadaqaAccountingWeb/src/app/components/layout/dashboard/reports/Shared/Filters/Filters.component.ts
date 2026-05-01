import { CommonModule, formatDate } from "@angular/common";
import {
  Component,
  OnInit,
  AfterViewInit,
  Input,
  Output,
  ViewChild,
  ElementRef,
  EventEmitter
} from "@angular/core";
import { FormsModule } from "@angular/forms";
import TomSelect from "tom-select";
import { DonorService, EventService, SelectModel } from "../../../../../../../api/base-api";

declare var $: any;

interface DateRangeOption {
  value: string;
  label: string;
}

@Component({
  selector: "app-Filters",
  templateUrl: "./Filters.component.html",
  styleUrls: ["./Filters.component.css"],
  standalone: true,
  imports: [CommonModule, FormsModule],
  providers: [EventService,DonorService]
})
export class FiltersComponent implements OnInit, AfterViewInit {
  @Output() filterChanged = new EventEmitter<any>();

  // Input Properties
  @Input() showDateRange: boolean = true;
  @Input() showFromDate: boolean = true;
  @Input() showToDate: boolean = true;
  @Input() showDonor: boolean = false;
  @Input() showEvent: boolean = false; // New input for company filter

  // ViewChild References
  @ViewChild("dateRangeSelect") dateRangeSelect!: ElementRef;
  @ViewChild("donorSelect") donorSelect!: ElementRef;
  @ViewChild("eventSelect") eventSelect!: ElementRef; // New ViewChild for company

  // Component Properties
  fromDateString: string = "";
  toDateString: string = "";
  selectedDateRange: string = "thisMonth";
  isUpdatingFromDropdown: boolean = false;
  donors: SelectModel[] = [];
  events: SelectModel[] = []; // New property for companies

  // Private Properties
  private dateRangeTomSelect?: TomSelect;
  private donorTomSelect?: TomSelect;
  private eventTomSelect?: TomSelect; // New TomSelect for company

  requestModel: any = {
    fromDate: null,
    toDate: null,
    donorId: null,
    eventId: null // New property for event ID
  };

  readonly dateRangeOptions: DateRangeOption[] = [
    { value: "today", label: "Today" },
    { value: "yesterday", label: "Yesterday" },
    { value: "thisWeek", label: "This Week" },
    { value: "lastWeek", label: "Last Week" },
    { value: "thisMonth", label: "This Month" },
    { value: "lastMonth", label: "Last Month" },
    { value: "thisQuarter", label: "This Quarter" },
    { value: "lastQuarter", label: "Last Quarter" },
    { value: "thisFiscalYear", label: "This Fiscal Year" },
    { value: "lastFiscalYear", label: "Last Fiscal Year" },
    { value: "thisYear", label: "This Year" },
    { value: "lastYear", label: "Last Year" },
    { value: "last7Days", label: "Last 7 Days" },
    { value: "last30Days", label: "Last 30 Days" },
    { value: "last90Days", label: "Last 90 Days" },
    { value: "custom", label: "Custom Range" }
  ];

  constructor(private eventService: EventService,private donorService: DonorService) {}

  // Lifecycle Methods
  ngOnInit(): void {
    this.onDateRangeChange();
    this.getSelectListDonor();
    this.getSelectListEvent(); // New method call
  }

  ngAfterViewInit(): void {
    this.initializeTomSelect();
    this.initializeDatePickers();
  }


  private getSelectListDonor(): void {
    this.donorService.getSelectListDonor().subscribe({
      next: (result: SelectModel[]) => {
        this.donors = result || [];
        setTimeout(() => {
          this.InitializeDonorTomSelect();
        }, 0);
      },
      error: (error) => {
        this.donors = [];
      }
    });
  }






  // New method to get events
  private getSelectListEvent(): void {
    this.eventService.getSelectListEvent().subscribe({
      next: (result: SelectModel[]) => {
        this.events = result || [];
        setTimeout(() => {
          this.InitializeEventTomSelect();
        }, 0);
      },
      error: (error) => {
        console.error("Error loading events:", error);
        this.events = [];
      }
    });
  }

  // TomSelect Initialization Methods
  private initializeTomSelect(): void {
    if (this.showDateRange) {
      if (this.dateRangeTomSelect) {
        this.dateRangeTomSelect.destroy();
      }
      this.dateRangeTomSelect = new TomSelect(this.dateRangeSelect.nativeElement, {
        placeholder: "Select Date Range",
        plugins: ["remove_button"],
        onChange: (value: string) => {
          this.selectedDateRange = value;
          this.onDateRangeChange();
        }
      });
      this.dateRangeTomSelect.setValue(this.selectedDateRange);
    }
  }



  private InitializeDonorTomSelect(): void {
    if (!this.showDonor) return;

    if (this.donorTomSelect) {
      this.donorTomSelect.destroy();
    }

    this.donorTomSelect = new TomSelect(this.donorSelect.nativeElement, {
      placeholder: "Choose Donor",
      plugins: ["remove_button"],
      onChange: (value: string) => {
        this.requestModel.donorId = value ? +value : null;
      }
    });
  }


  private InitializeEventTomSelect(): void {
    if (!this.showEvent) return;

    if (this.eventTomSelect) {
      this.eventTomSelect.destroy();
    }

    this.eventTomSelect = new TomSelect(this.eventSelect.nativeElement, {
      placeholder: "Choose Event",
      plugins: ["remove_button"],
      onChange: (value: string) => {
        this.requestModel.eventId = value ? +value : null;
      }
    });
  }

  // Date Picker Initialization
  private initializeDatePickers(): void {
    if (!this.showDateRange) return;

    try {
      this.initializeFromDatePicker();
      this.initializeToDatePicker();
    } catch (error) {}
  }

  private initializeFromDatePicker(): void {
    $("#fromDate")
      .datetimepicker({
        format: "DD-MM-YYYY",
        showClear: true
      })
      .on("dp.change", (e: any) => {
        if (e.date) {
          this.fromDateString = e.date.format("DD-MM-YYYY");
          this.requestModel.fromDate = e.date.toDate();
          if (!this.isUpdatingFromDropdown && this.selectedDateRange !== "custom") {
            this.selectedDateRange = "custom";
            if (this.dateRangeTomSelect) {
              this.dateRangeTomSelect.setValue("custom");
            }
          }
        }
      });
  }

  private initializeToDatePicker(): void {
    $("#toDate")
      .datetimepicker({
        format: "DD-MM-YYYY",
        showClear: true
      })
      .on("dp.change", (e: any) => {
        if (e.date) {
          this.toDateString = e.date.format("DD-MM-YYYY");
          this.requestModel.toDate = e.date.toDate();
          if (!this.isUpdatingFromDropdown && this.selectedDateRange !== "custom") {
            this.selectedDateRange = "custom";
            if (this.dateRangeTomSelect) {
              this.dateRangeTomSelect.setValue("custom");
            }
          }
        }
      });
  }

  // Date Range Calculation Methods
  onDateRangeChange(): void {
    if (this.selectedDateRange === "custom") {
      return;
    }

    this.isUpdatingFromDropdown = true;
    const dateRange = this.calculateDateRange(this.selectedDateRange);

    if (!dateRange) {
      this.isUpdatingFromDropdown = false;
      return;
    }

    this.updateDatesAndUI(dateRange.fromDate, dateRange.toDate);
  }

  private calculateDateRange(range: string): { fromDate: Date; toDate: Date } | null {
    const today = new Date();

    switch (range) {
      case "today":
        return { fromDate: new Date(today), toDate: new Date(today) };

      case "yesterday":
        const yesterday = new Date(today.getFullYear(), today.getMonth(), today.getDate() - 1);
        return { fromDate: yesterday, toDate: yesterday };

      case "thisWeek":
        const startOfWeek = new Date(today);
        startOfWeek.setDate(today.getDate() - today.getDay());
        const endOfWeek = new Date(startOfWeek);
        endOfWeek.setDate(startOfWeek.getDate() + 6);
        return { fromDate: startOfWeek, toDate: endOfWeek };

      case "lastWeek":
        const lastWeekStart = new Date(today);
        lastWeekStart.setDate(today.getDate() - today.getDay() - 7);
        const lastWeekEnd = new Date(lastWeekStart);
        lastWeekEnd.setDate(lastWeekStart.getDate() + 6);
        return { fromDate: lastWeekStart, toDate: lastWeekEnd };

      case "thisMonth":
        return {
          fromDate: new Date(today.getFullYear(), today.getMonth(), 1),
          toDate: new Date(today.getFullYear(), today.getMonth() + 1, 0)
        };

      case "lastMonth":
        return {
          fromDate: new Date(today.getFullYear(), today.getMonth() - 1, 1),
          toDate: new Date(today.getFullYear(), today.getMonth(), 0)
        };

      case "thisQuarter":
        const quarterStart = new Date(today.getFullYear(), Math.floor(today.getMonth() / 3) * 3, 1);
        const quarterEnd = new Date(
          today.getFullYear(),
          Math.floor(today.getMonth() / 3) * 3 + 3,
          0
        );
        return { fromDate: quarterStart, toDate: quarterEnd };

      case "lastQuarter":
        const lastQuarterStart = new Date(
          today.getFullYear(),
          Math.floor(today.getMonth() / 3) * 3 - 3,
          1
        );
        const lastQuarterEnd = new Date(
          today.getFullYear(),
          Math.floor(today.getMonth() / 3) * 3,
          0
        );
        return { fromDate: lastQuarterStart, toDate: lastQuarterEnd };

      case "thisFiscalYear":
        const fiscalYearStart =
          today.getMonth() >= 6
            ? new Date(today.getFullYear(), 6, 1)
            : new Date(today.getFullYear() - 1, 6, 1);
        const fiscalYearEnd =
          today.getMonth() >= 6
            ? new Date(today.getFullYear() + 1, 5, 30)
            : new Date(today.getFullYear(), 5, 30);
        return { fromDate: fiscalYearStart, toDate: fiscalYearEnd };

      case "lastFiscalYear":
        const lastFiscalStart =
          today.getMonth() >= 6
            ? new Date(today.getFullYear() - 1, 6, 1)
            : new Date(today.getFullYear() - 2, 6, 1);
        const lastFiscalEnd =
          today.getMonth() >= 6
            ? new Date(today.getFullYear(), 5, 30)
            : new Date(today.getFullYear() - 1, 5, 30);
        return { fromDate: lastFiscalStart, toDate: lastFiscalEnd };

      case "thisYear":
        return {
          fromDate: new Date(today.getFullYear(), 0, 1),
          toDate: new Date(today.getFullYear(), 11, 31)
        };

      case "lastYear":
        return {
          fromDate: new Date(today.getFullYear() - 1, 0, 1),
          toDate: new Date(today.getFullYear() - 1, 11, 31)
        };

      case "last7Days":
        return {
          fromDate: new Date(today.getFullYear(), today.getMonth(), today.getDate() - 6),
          toDate: new Date(today)
        };

      case "last30Days":
        return {
          fromDate: new Date(today.getFullYear(), today.getMonth(), today.getDate() - 29),
          toDate: new Date(today)
        };

      case "last90Days":
        return {
          fromDate: new Date(today.getFullYear(), today.getMonth(), today.getDate() - 89),
          toDate: new Date(today)
        };

      default:
        return null;
    }
  }

  private updateDatesAndUI(fromDate: Date, toDate: Date): void {
    this.requestModel.fromDate = fromDate;
    this.requestModel.toDate = toDate;
    this.fromDateString = formatDate(fromDate, "dd-MM-yyyy", "en");
    this.toDateString = formatDate(toDate, "dd-MM-yyyy", "en");

    setTimeout(() => {
      this.updateDatePickers(fromDate, toDate);
      setTimeout(() => {
        this.isUpdatingFromDropdown = false;
      }, 100);
    }, 0);
  }

  private updateDatePickers(fromDate: Date, toDate: Date): void {
    try {
      const fromDatePicker = $("#fromDate").data("DateTimePicker");
      const toDatePicker = $("#toDate").data("DateTimePicker");

      if (fromDatePicker) {
        fromDatePicker.date(fromDate);
      }
      if (toDatePicker) {
        toDatePicker.date(toDate);
      }
    } catch (error) {}
  }
  toUtcDateOnly(date: Date): Date {
    // Create a date in UTC without time shift
    return new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));
  }
  // Event Handlers
  onSearchClick(): void {
    this.requestModel.fromDate = this.toUtcDateOnly(this.requestModel.fromDate);
    this.requestModel.toDate = this.toUtcDateOnly(this.requestModel.toDate);
    this.filterChanged.emit(this.requestModel);
  }
}
