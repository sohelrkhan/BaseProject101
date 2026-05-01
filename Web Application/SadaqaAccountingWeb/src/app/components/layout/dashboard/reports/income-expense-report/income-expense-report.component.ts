import { Component, Inject, OnDestroy, OnInit, PLATFORM_ID } from '@angular/core';
import { ExportMonthlyIncomeExpenseReportModel, IncomeExpenseReportService, MonthlyIncomeExpenseGridModel, MonthlyIncomeExpenseRequestModel } from '../../../../../../api/base-api';
import { FiltersComponent } from '../Shared/Filters/Filters.component';
import { CommonModule, formatDate, isPlatformBrowser } from '@angular/common';
import { OrdinalDatePipe } from '../../../../../shared/pipe/ordinaldate.pipe';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-income-expense-report',
  templateUrl: './income-expense-report.component.html',
  styleUrls: ['./income-expense-report.component.css'],
  standalone: true,
  imports: [FiltersComponent, CommonModule, OrdinalDatePipe, NgxSpinnerModule],
  providers:[IncomeExpenseReportService]
})
export class IncomeExpenseReportComponent implements OnInit,OnDestroy {
  isBrowser: boolean = false;
  isTableReady: boolean = false;
  dataTable: any;
  isTableReady2: boolean = false;
  dataTable2: any;

  requestModel: MonthlyIncomeExpenseRequestModel = new MonthlyIncomeExpenseRequestModel();
  responseModel: MonthlyIncomeExpenseGridModel = new MonthlyIncomeExpenseGridModel();

  exportRequestModel: ExportMonthlyIncomeExpenseReportModel = new ExportMonthlyIncomeExpenseReportModel();

  totalIncome: number = 0;
  totalExpense: number = 0;

  constructor(
    private incomeExpenseReportService: IncomeExpenseReportService,
    private spinnerService: NgxSpinnerService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
  }

  ngOnInit() {
    if (this.isBrowser) {
      // Set dates for the current month
      const today = new Date();
      this.requestModel.fromDate = new Date(today.getFullYear(), today.getMonth(), 1);
      this.requestModel.toDate = new Date(today.getFullYear(), today.getMonth() + 1, 0);
      this.requestModel.eventId = null;
      this.requestModel.accountUnitId = 0;
      this.requestModel.dateRange = null;
      //this.getAllIncomeExpenseReportByFilter();
    }
  }

  ngOnDestroy(): void {
    if (this.dataTable) {
      this.dataTable.destroy();
    }
    if (this.dataTable2) {
      this.dataTable2.destroy();
    }
  }

  private refreshDataTable(): void {
    const tableElement = $("#example");

    if (!tableElement.length) {
      return;
    }

    // If this.dataTable already exists, destroy it directly
    if (this.dataTable) {
      this.dataTable.destroy(true);
      this.dataTable = null;
    }

    this.dataTable = tableElement.DataTable({
      responsive: true
    });
  }

  private refreshDataTable2(): void {
    const tableElement = $("#expenseTbl");

    if (!tableElement.length) {
      return;
    }

    // If this.dataTable already exists, destroy it directly
    if (this.dataTable2) {
      this.dataTable2.destroy(true);
      this.dataTable2 = null;
    }

    this.dataTable2 = tableElement.DataTable({
      responsive: true
    });
  }

   // Filter Event Handler
  onFilterChanged(filterModel: any): void {
    this.requestModel.fromDate = filterModel.fromDate;
    this.requestModel.toDate = filterModel.toDate;
    this.requestModel.eventId = filterModel.eventId;
    this.onClickSearchReport();
  }

  onClickSearchReport(): void {
    this.getAllIncomeExpenseReportByFilter();
  }

  getAllIncomeExpenseReportByFilter() {
    this.spinnerService.show();

    this.incomeExpenseReportService.getMonthlyIncomeExpense(this.requestModel).subscribe(
      (result: MonthlyIncomeExpenseGridModel) => {
        this.responseModel = result || null;
        this.totalIncome = 0;
        this.totalExpense = 0;
        // get total income
        // if(this.responseModel.incomeList.length > 0){
        //   this.responseModel.incomeList.forEach(element => {
        //     this.totalIncome += element.amount;
        //   });
        // }
        // // get total expense
        // if(this.responseModel.expenseList.length > 0){
        //   this.responseModel.expenseList.forEach(element => {
        //     this.totalExpense += element.amount;
        //   });
        // }
        // this.isTableReady = false;
        // this.isTableReady2 = false;
        // setTimeout(() => {
        //   this.isTableReady = true;
        //   this.isTableReady2 = true;

        //   setTimeout(() => {
        //     this.refreshDataTable();
        //     this.refreshDataTable2();
        //   }, 100);
        // });

        this.spinnerService.hide();
      },
      (error: any) => {
        this.spinnerService.hide();
      }
    );
  }

  downloadReport(fileFormateName: string): void {
    let eventName = 
    this.exportRequestModel.reportType = fileFormateName;
    this.exportRequestModel.reportName = "Income & Expense Report";
    this.exportRequestModel.qrCodeUrl = window.location.href;
    this.exportRequestModel.gridModel = this.responseModel;
    this.exportRequestModel.duration = this.getFromDate() + " - " + this.getToDate();
    this.exportRequestModel.eventId = this.requestModel.eventId;
    this.spinnerService.show();
    this.incomeExpenseReportService.downloadIncomeExpenseBalance(this.exportRequestModel).subscribe(  
      (fileResponse) => {
        this.spinnerService.hide();
        if (fileResponse && fileResponse.data) {
          const blob = new Blob([fileResponse.data], {
            type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
          });
          const url = window.URL.createObjectURL(blob);
          const a = document.createElement("a");
          a.href = url;
          a.download = fileResponse.fileName;
          document.body.appendChild(a);
          a.click();
          document.body.removeChild(a);
          window.URL.revokeObjectURL(url);
        }
      },
      (error: any) => {
        this.spinnerService.hide();
      }
    );
  }
  // Template Helper Methods
  getFromDate(): string {
    return this.requestModel.fromDate
      ? formatDate(this.requestModel.fromDate, "MMM dd,yyyy", "en")
      : "";
  }

  getToDate(): string {
    return this.requestModel.toDate
      ? formatDate(this.requestModel.toDate, "MMM dd,yyyy", "en")
      : "";
  }
}
