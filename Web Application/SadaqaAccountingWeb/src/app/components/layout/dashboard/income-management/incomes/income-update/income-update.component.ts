import { CommonModule, isPlatformBrowser } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { AfterViewInit, ChangeDetectorRef, Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { BankService, CashService, DonorService, EventService, IncomeCategoryService, IncomeService, IncomeUpdateModel, IncomeViewModel, SelectModel, UserModel } from '../../../../../../../api/base-api';
import { CustomTosterServiceService } from '../../../../../../shared/Toster/CustomTosterService.service';
import { IdentityService } from '../../../../../../../identity/identity-shared/identity.service';
import { PaymentMode } from '../../../../../../shared/enum-configure/enum-configure';

@Component({
  selector: 'app-income-update',
  templateUrl: './income-update.component.html',
  styleUrls: ['./income-update.component.css'],
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
    IncomeService, 
    IncomeCategoryService, 
    EventService,
    BankService,
    CashService,
    DonorService
  ]
})

export class IncomeUpdateComponent implements OnInit, AfterViewInit {

  isBrowser: boolean = false;
    
  // Select bank or cash
  isSelectBank: boolean = false;
  isSelectCash: boolean = false;

  // Income id
  private _incomeId: string = "-1";

  // Login user account unit id
  private _loginUserAccountUnitId: number | undefined;

  // Income update model
  incomeUpdateModel: IncomeUpdateModel = new IncomeUpdateModel();

  // Select model
  incomeCategories: SelectModel[] = [];
  events: SelectModel[] = [];
  donors: SelectModel[] = [];
  paymentModes: SelectModel[] = [];
  months: SelectModel[] = [];
  years: SelectModel[] = [];
  banks: SelectModel[] = [];
  cashes: SelectModel[] = [];

  constructor(
    private incomeService: IncomeService, 
    private incomeCategoryService: IncomeCategoryService, 
    private eventService: EventService, 
    private banksService: BankService,
    private cashService: CashService,
    private donorService: DonorService,
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
      this.getIncomeIdByUrl();
    }
  }

  // Get income id by url
  private getIncomeIdByUrl(): void {
    this.activatedRoute.params.subscribe((params) => {
      this._incomeId = params['recordId'];

      if (this._incomeId != undefined || this._incomeId != null || this._incomeId! != '') {
        this.getIncomeById(this._incomeId);
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
        ($("#receiptDateString") as any).datetimepicker({
          format: "DD-MM-YYYY",
          showClear: true
        })
        .on("dp.change", (e) => {
          this.incomeUpdateModel.receiptDateString = e.date.format("DD-MM-YYYY");
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

        // On income category
        this.onChangeIncomeCategory();

        // On change event
        this.onChangeEvent();

        // On change donor
        this.onChangeDonor();

        // On change bank
        this.onChangeBank();

        // On change cash
        this.onChangeCash();

        // On change payment mood
        this.onChangePaymentMode();

        // On change month
        this.onChangeMonth();

        // On change year
        this.onChangeYear();
      }, 0);
    }
  }

  // On change income category
  onChangeIncomeCategory(): void {
    const incomeCategory = $("#incomeCategoryId");
    incomeCategory.select2();

    incomeCategory.on("change", () => {
      let incomeCategoryId: number = Number(incomeCategory.val());
      this.incomeUpdateModel.incomeCategoryId = incomeCategoryId;
      this.cdRef.detectChanges();
    });
  }

  // On expense event
  onChangeEvent(): void {
    const event = $("#eventId");
    event.select2();

    event.on("change", () => {
      let eventId: number = Number(event.val());
      this.incomeUpdateModel.eventId = eventId;
      this.cdRef.detectChanges();
    });
  }

  // On expense donor
  onChangeDonor(): void {
    const donor = $("#donorId");
    donor.select2();

    donor.on("change", () => {
      let donorId: number = Number(donor.val());
      this.incomeUpdateModel.donorId = donorId;
      this.cdRef.detectChanges();
    });
  }

  // On change bank
  onChangeBank(): void {
    const bank = $("#bankId");
    bank.select2();

    bank.on("change", () => {
      let bankId: number = Number(bank.val());
      this.incomeUpdateModel.bankId = bankId;
      this.cdRef.detectChanges();
    });
  }

  // On change cash
  onChangeCash(): void {
    const cash = $("#cashId");
    cash.select2();

    cash.on("change", () => {
      let cashId: number = Number(cash.val());
      this.incomeUpdateModel.cashId = cashId;
      this.cdRef.detectChanges();
    });
  }

  // On change payment mode
  onChangePaymentMode(): void {
    const paymentMode = $("#paymentModeId");
    paymentMode.select2();

    paymentMode.on("change", () => {
      let paymentModeId: number = Number(paymentMode.val());
      this.incomeUpdateModel.paymentModeId = paymentModeId;
      this.showHideBankCash(this.incomeUpdateModel.paymentModeId);

      this.cdRef.detectChanges();
    });
  }

  // Get income by id
  private getIncomeById(incomeId: string): void {
    this.spinnerService.show();
    this.incomeService.getById(incomeId).subscribe((result: IncomeViewModel) => {
      
      this.incomeUpdateModel = result.updateModel;

      // Get payment mood select list
      this.paymentModes = result.optionsDataSources.PaymentMoodSelectList;
      this.months = result.optionsDataSources.MonthSelectList;      
      this.generateYears();
      this.cdRef.detectChanges();

      setTimeout(() => {
        // IMPORTANT: set value + trigger change so select2 updates UI
        ($("#incomeCategoryId") as any).val(String(this.incomeUpdateModel.incomeCategoryId)).trigger("change.select2");
        ($("#eventId") as any).val(String(this.incomeUpdateModel.eventId ?? "-1")).trigger("change.select2");
        ($("#paymentModeId") as any).val(String(this.incomeUpdateModel.paymentModeId)).trigger("change.select2");
        ($("#bankId") as any).val(String(this.incomeUpdateModel.bankId)).trigger("change.select2");
        ($("#cashId") as any).val(String(this.incomeUpdateModel.cashId)).trigger("change.select2");
        ($("#donorId") as any).val(String(this.incomeUpdateModel.donorId)).trigger("change.select2");
        ($("#monthId") as any).val(String(this.incomeUpdateModel.monthId)).trigger("change.select2");
        ($("#year") as any).val(String(this.incomeUpdateModel.year)).trigger("change.select2");
      }, 0);

      this.showHideBankCash(this.incomeUpdateModel.paymentModeId);
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
      this.incomeUpdateModel.accountUnitId = result.accountUnitId;

      // Get select lists
      if(result.accountUnitId != undefined && result.accountUnitId != null && result.accountUnitId > 0) {

        // Get income categories
        this.getIncomeCategories(result.accountUnitId);

        // Get events
        this.getEvents(result.accountUnitId);

        // Get donors
        this.getDonors(result.accountUnitId);

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

  // Check income update from is valid or not
  private getIncomeFromValidResult(): boolean {
    if (this.incomeUpdateModel.incomeCategoryId == undefined || this.incomeUpdateModel.incomeCategoryId == null || this.incomeUpdateModel.incomeCategoryId <= 0) {
      this.toastrService.warning("Please, provied income category.", "Warning");
      return false;
    } else if (this.incomeUpdateModel.paymentModeId == undefined || this.incomeUpdateModel.paymentModeId == null || this.incomeUpdateModel.paymentModeId <= 0) {
      this.toastrService.warning("Please, provied payment mode.", "Warning");
      return false;
    } else if (this.isSelectBank && (this.incomeUpdateModel.bankId == undefined || this.incomeUpdateModel.bankId == null || this.incomeUpdateModel.bankId <= 0)) {
      this.toastrService.warning("Please, provied bank.", "Warning");
      return false;
    } else if (this.isSelectCash && (this.incomeUpdateModel.cashId == undefined || this.incomeUpdateModel.cashId == null || this.incomeUpdateModel.cashId <= 0)) {
      this.toastrService.warning("Please, provied bank.", "Warning");
      return false;
    } else if (this.incomeUpdateModel.amount == undefined || this.incomeUpdateModel.amount == null || this.incomeUpdateModel.amount <= 0) {
      this.toastrService.warning("Please, provied amount.", "Warning");
      return false;
    } else if (this.incomeUpdateModel.receiptDateString == undefined || this.incomeUpdateModel.receiptDateString == null || this.incomeUpdateModel.receiptDateString == "") {
      this.toastrService.warning("Please, provied receipt date.", "Warning");
      return false;
    } else {
      return true;
    }
  }

  // Update income
  onClickUpdate(): void {

    // Check income update from valid or not
    let isValidUpdateFrom: boolean = this.getIncomeFromValidResult();

    if (isValidUpdateFrom) {
      this.spinnerService.show();
      this.incomeService.update(this.incomeUpdateModel).subscribe((result: IncomeUpdateModel) => {
        this.spinnerService.hide();
        this.toastrService.success("Income update successful.", "Success");
        return this.router.navigateByUrl("/app/incomes");
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error("Income update failed.", "Error");
      });
    }
  }

  // On change month
  onChangeMonth(): void {
    const month = $("#monthId");
    month.select2();

    month.on("change", () => {
      let monthId: number = Number(month.val());
      this.incomeUpdateModel.monthId = monthId;
      this.cdRef.detectChanges();
    });
  }

  // On change year
  onChangeYear(): void {
    const year = $("#year");
    year.select2();

    year.on("change", () => {
      let yearId: number = Number(year.val());
      this.incomeUpdateModel.year = yearId;
      this.cdRef.detectChanges();
    });
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

  // Get income categories by login user account unit id
  private getIncomeCategories(loginUserAccountUnitId: number): void {
    this.spinnerService.show();
    this.incomeCategoryService.getSelectListIncomeCategoryByAccountUnit(loginUserAccountUnitId).subscribe((result: SelectModel[]) => {
      this.incomeCategories = result;
      this.spinnerService.hide();
      return;
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Income Categories cannot load at this moment! Please, try again.", "Error");
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

  // Get donors by login user account unit id
  private getDonors(loginUserAccountUnitId: number): void {
    this.spinnerService.show();
    this.donorService.getSelectListDonorByAccountUnit(loginUserAccountUnitId).subscribe((result: SelectModel[]) => {
      this.donors = result;
      this.spinnerService.hide();
      return;
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Donors cannot load at this moment! Please, try again.", "Error");
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

  // Generate year 
  private generateYears(range: number = 2): void {
    const currentYear = new Date().getFullYear();

    this.years = Array.from({ length: range * 2 + 1 }, (_, index) => {
      const year = currentYear - range + index;

      return {
        id: year,
        name: year.toString(),
        isDefault: year === currentYear
      } as SelectModel;
    });
  }
}