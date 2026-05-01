import { CommonModule, isPlatformBrowser } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { AfterViewInit, ChangeDetectorRef, Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { BankService, CashService, EventService, ExpenseCategoryService, ExpenseService, ExpenseUpdateModel, ExpenseViewModel, SelectModel, UserModel } from '../../../../../../../api/base-api';
import { CustomTosterServiceService } from '../../../../../../shared/Toster/CustomTosterService.service';
import { IdentityService } from '../../../../../../../identity/identity-shared/identity.service';
import { PaymentMode } from '../../../../../../shared/enum-configure/enum-configure';

@Component({
  selector: 'app-expense-update',
  templateUrl: './expense-update.component.html',
  styleUrls: ['./expense-update.component.css'],
  standalone: true,
  imports: [
    ReactiveFormsModule, 
    FormsModule, 
    HttpClientModule, 
    NgxSpinnerModule, 
    RouterLink, 
    CommonModule
  ],
  providers: [
    ExpenseService, 
    ExpenseCategoryService, 
    EventService,
    BankService,
    CashService
  ]
})

export class ExpenseUpdateComponent implements OnInit, AfterViewInit {

  isBrowser: boolean = false;

  // Select bank or cash
  isSelectBank: boolean = false;
  isSelectCash: boolean = false;
  
  // Expense id
  private _expenseId: string = "-1";

  // Login user account unit id
  private _loginUserAccountUnitId: number | undefined;

  // Expense update model
  expenseUpdateModel: ExpenseUpdateModel = new ExpenseUpdateModel();

  // Select model
  expenseCategories: SelectModel[] = [];
  events: SelectModel[] = [];
  paymentModes: SelectModel[] = [];
  banks: SelectModel[] = [];
  cashes: SelectModel[] = [];

  constructor(
    private expenseService: ExpenseService, 
    private expenseCategoryService: ExpenseCategoryService, 
    private eventService: EventService, 
    private banksService: BankService,
    private cashService: CashService,
    private spinnerService: NgxSpinnerService, 
    private toastrService: CustomTosterServiceService, 
    private router: Router, 
    private identityService: IdentityService, 
    @Inject(PLATFORM_ID) private platformId: object, 
    private cdRef: ChangeDetectorRef,
    private activatedRoute: ActivatedRoute
  ) { }

 ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);

    if(this.isBrowser) {
      this.getLoginUserInfo();
      this.getExpenseIdByUrl();
    }
  }

  // Get expense id by url
  private getExpenseIdByUrl(): void {
    this.activatedRoute.params.subscribe((params) => {
      this._expenseId = params['recordId'];

      if (this._expenseId != undefined || this._expenseId != null || this._expenseId! != '') {
        this.getExpenseById(this._expenseId);
      }
    });
  }

  ngAfterViewInit(): void {
    // Update date formate setting
    this.updateDateFormateSetting();

    // Initialize select 2 dropdown
    this.initializeSelect2Dropdown();
  }

  // Update date formate setting
  private updateDateFormateSetting(): void {
    if (this.isBrowser) {
      setTimeout(() => {
        ($("#expenseDateString") as any).datetimepicker({
          format: "DD-MM-YYYY",
          showClear: true
        })
        .on("dp.change", (e) => {
          this.expenseUpdateModel.expenseDateString = e.date.format("DD-MM-YYYY");
        });
      }, 0);

      this.cdRef.detectChanges();
      return;
    }

    return;
  }

  // Initialize select 2 dropdown
  private initializeSelect2Dropdown(): void {
    if (this.isBrowser) {
      setTimeout(() => {
        ($(".select2") as any).select2({
          placeholder: "Choose...",
          width: "100%"
        });

        // On expense category
        this.onChangeExpenseCategory();

        // On change event
        this.onChangeEvent();

        // On change bank
        this.onChangeBank();

        // On change cash
        this.onChangeCash();

        // On change payment mood
        this.onChangePaymentMode();
      }, 0);
    }
  }

  // On expense category
  onChangeExpenseCategory(): void {
    const expenseCategory = $("#expenseCategoryId");
    expenseCategory.select2();

    expenseCategory.on("change", () => {
      let expenseCategoryId: number = Number(expenseCategory.val());
      this.expenseUpdateModel.expenseCategoryId = expenseCategoryId;
      this.cdRef.detectChanges();
    });
  }

  // On expense event
  onChangeEvent(): void {
    const event = $("#eventId");
    event.select2();

    event.on("change", () => {
      let eventId: number = Number(event.val());
      this.expenseUpdateModel.eventId = eventId;
      this.cdRef.detectChanges();
    });
  }

  // On change bank
  onChangeBank(): void {
    const bank = $("#bankId");
    bank.select2();

    bank.on("change", () => {
      let bankId: number = Number(bank.val());
      this.expenseUpdateModel.bankId = bankId;
      this.cdRef.detectChanges();
    });
  }

  // On change cash
  onChangeCash(): void {
    const cash = $("#cashId");
    cash.select2();

    cash.on("change", () => {
      let cashId: number = Number(cash.val());
      this.expenseUpdateModel.cashId = cashId;
      this.cdRef.detectChanges();
    });
  }

  // On change payment mode
  onChangePaymentMode(): void {
    const paymentMode = $("#paymentModeId");
    paymentMode.select2();

    paymentMode.on("change", () => {
      let paymentModeId: number = Number(paymentMode.val());
      this.expenseUpdateModel.paymentModeId = paymentModeId;
      this.showHideBankCash(this.expenseUpdateModel.paymentModeId);

      this.cdRef.detectChanges();
    });
  }

  // Get expense by id
  private getExpenseById(expenseId: string): void {
    this.spinnerService.show();
    this.expenseService.getById(expenseId).subscribe((result: ExpenseViewModel) => {
      
      this.expenseUpdateModel = result.updateModel;

      // Get payment mood select list
      this.paymentModes = result.optionsDataSources.PaymentMoodSelectList;

      this.cdRef.detectChanges();

      setTimeout(() => {
        // IMPORTANT: set value + trigger change so select2 updates UI
        ($("#expenseCategoryId") as any).val(String(this.expenseUpdateModel.expenseCategoryId)).trigger("change.select2");
        ($("#eventId") as any).val(String(this.expenseUpdateModel.eventId ?? "-1")).trigger("change.select2");
        ($("#paymentModeId") as any).val(String(this.expenseUpdateModel.paymentModeId)).trigger("change.select2");
        ($("#bankId") as any).val(String(this.expenseUpdateModel.bankId)).trigger("change.select2");
        ($("#cashId") as any).val(String(this.expenseUpdateModel.cashId)).trigger("change.select2");
      }, 0);

      this.showHideBankCash(this.expenseUpdateModel.paymentModeId);
      this.spinnerService.hide();
      return;
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Payment mood dropdown is not load yet! Please, try again.", "Error");
    })
  }

  // Get login user info
  private getLoginUserInfo(): void {
    this.spinnerService.show();
    this.identityService.getLoginInfo().subscribe((result: UserModel) => {
      this._loginUserAccountUnitId = result.accountUnitId;

      // Set login user account unit id in the create model
      this.expenseUpdateModel.accountUnitId = result.accountUnitId;

      // Get select lists
      if(result.accountUnitId != undefined && result.accountUnitId != null && result.accountUnitId > 0) {

        // Get expense categories
        this.getExpenseCategories(result.accountUnitId);

        // Get events
        this.getEvents(result.accountUnitId);

        // Get banks
        this.getBanks(result.accountUnitId);

        // Get cashes
        this.getCashes(result.accountUnitId);
      }

      this.spinnerService.hide();
      return;
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Login user info cannot found! Please, try again.", "Error");
      return;
    })
  }

  // Check expense update from is valid or not
  private getExpenseFromValidResult(): boolean {
    if (this.expenseUpdateModel.expenseCategoryId == undefined || this.expenseUpdateModel.expenseCategoryId == null || this.expenseUpdateModel.expenseCategoryId <= 0) {
      this.toastrService.warning("Please, provied expense category.", "Warning");
      return false;
    } else if (this.expenseUpdateModel.paymentModeId == undefined || this.expenseUpdateModel.paymentModeId == null || this.expenseUpdateModel.paymentModeId <= 0) {
      this.toastrService.warning("Please, provied payment mode.", "Warning");
      return false;
    } else if (this.isSelectBank && (this.expenseUpdateModel.bankId == undefined || this.expenseUpdateModel.bankId == null || this.expenseUpdateModel.bankId <= 0)) {
      this.toastrService.warning("Please, provied bank.", "Warning");
      return false;
    } else if (this.isSelectCash && (this.expenseUpdateModel.cashId == undefined || this.expenseUpdateModel.cashId == null || this.expenseUpdateModel.cashId <= 0)) {
      this.toastrService.warning("Please, provied cash.", "Warning");
      return false;
    } else if (this.expenseUpdateModel.amount == undefined || this.expenseUpdateModel.amount == null || this.expenseUpdateModel.amount <= 0) {
      this.toastrService.warning("Please, provied amount.", "Warning");
      return false;
    } else if (this.expenseUpdateModel.expenseDateString == undefined || this.expenseUpdateModel.expenseDateString == null || this.expenseUpdateModel.expenseDateString == "") {
      this.toastrService.warning("Please, provied expense date.", "Warning");
      return false;
    } else {
      return true;
    }
  }

  // Update expense
  onClickUpdate(): void {

    // Check expense update from valid or not
    let isValidUpdateFrom: boolean = this.getExpenseFromValidResult();

    if (isValidUpdateFrom) {
      this.spinnerService.show();
      this.expenseService.update(this.expenseUpdateModel).subscribe((result: ExpenseUpdateModel) => {
        this.spinnerService.hide();
        this.toastrService.success("Expense update successful.", "Success");
        return this.router.navigateByUrl("/app/expenses");
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error("Expense update failed.", "Error");
      });
    }
  }

  // Get expense categories by login user account unit id
  private getExpenseCategories(loginUserAccountUnitId: number): void {
    this.spinnerService.show();
    this.expenseCategoryService.getSelectListExpenseCategoryByAccountUnit(loginUserAccountUnitId).subscribe((result: SelectModel[]) => {
      this.expenseCategories = result;
      this.spinnerService.hide();
      return;
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Expense Categories cannot load at this moment! Please, try again.", "Error");
      return;
    })
  }

  // Get events by login user account unit id
  private getEvents(loginUserAccountUnitId: number): void {
    this.spinnerService.show();
    this.eventService.getSelectListEventByAccountUnit(loginUserAccountUnitId).subscribe((result: SelectModel[]) => {
      this.events = result;
      this.spinnerService.hide();
      return;
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Events cannot load at this moment! Please, try again.", "Error");
      return;
    })
  }

  // Get banks by login user account unit id
  private getBanks(loginUserAccountUnitId: number): void {
    this.spinnerService.show();
    this.banksService.getSelectListBankByAccountUnit(loginUserAccountUnitId).subscribe((result: SelectModel[]) => {
      this.banks = result;
      this.spinnerService.hide();
      return;
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Banks cannot load at this moment! Please, try again.", "Error");
      return;
    })
  }

  // Get cash by login user account unit id
  private getCashes(loginUserAccountUnitId: number): void {
    this.spinnerService.show();
    this.cashService.getSelectListCashByAccountUnit(loginUserAccountUnitId).subscribe((result: SelectModel[]) => {
      this.cashes = result;
      this.spinnerService.hide();
      return;
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Cash cannot load at this moment! Please, try again.", "Error");
      return;
    })
  }

  // Bank or cash show
  private showHideBankCash(paymentModeId: number): void {
    if(paymentModeId == PaymentMode.Bank) {
      this.isSelectBank = true;
      this.isSelectCash = false;

      this.onChangeBank();
    } else if(paymentModeId == PaymentMode.Cash) {
      this.isSelectCash = true;
      this.isSelectBank= false;

      this.onChangeCash();
    } else {
      this.isSelectCash = false;
      this.isSelectBank= false;
    }
  }
}