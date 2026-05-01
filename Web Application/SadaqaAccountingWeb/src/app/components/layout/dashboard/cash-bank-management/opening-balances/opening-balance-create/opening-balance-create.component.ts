import { CommonModule, isPlatformBrowser } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ChangeDetectorRef, Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { BankService, CashService, OpeningBalanceCreateModel, OpeningBalanceService, OpeningBalanceViewModel, SelectModel, UserModel } from '../../../../../../../api/base-api';
import { CustomTosterServiceService } from '../../../../../../shared/Toster/CustomTosterService.service';
import { IdentityService } from '../../../../../../../identity/identity-shared/identity.service';
import { PaymentMode } from '../../../../../../shared/enum-configure/enum-configure';

@Component({
  selector: 'app-opening-balance-create',
  templateUrl: './opening-balance-create.component.html',
  styleUrls: ['./opening-balance-create.component.css'],
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
    OpeningBalanceService,
    BankService,
    CashService
  ]
})

export class OpeningBalanceCreateComponent implements OnInit {

  isBrowser: boolean = false;

  // Select bank or cash
  isSelectBank: boolean = false;
  isSelectCash: boolean = false;

  // Select model
  paymentModes: SelectModel[] = [];
  banks: SelectModel[] = [];
  cashes: SelectModel[] = [];

  // Opening balance id
  private _openingBalanceId: string = "-1";
  
  // Login user account unit id
  private _loginUserAccountUnitId: number | undefined;

  // Opening balance create model
  openingBalanceCreateModel: OpeningBalanceCreateModel = new OpeningBalanceCreateModel();

  constructor(
    private openingBalanceService: OpeningBalanceService, 
    private bankService: BankService,
    private cashService: CashService,
    private spinnerService: NgxSpinnerService, 
    private toastrService: CustomTosterServiceService, 
    private router: Router, 
    private identityService: IdentityService, 
    @Inject(PLATFORM_ID) private platformId: object,
    private cdRef: ChangeDetectorRef
  ) { }
  
  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);

    if(this.isBrowser) {
      this.getLoginUserInfo();
      this.getOpeningBalanceById(this._openingBalanceId);
    }
  }

  ngAfterViewInit(): void {
    // Create date formate setting
    this.createDateFormateSetting();

    // Initialize select 2 dropdown
    this.initializeSelect2Dropdown();
  }

  // Create date formate setting
  private createDateFormateSetting(): void {
    if (this.isBrowser) {
      setTimeout(() => {
        ($("#openingDateText") as any).datetimepicker({
          format: "DD-MM-YYYY",
          showClear: true
        })
        .on("dp.change", (e) => {
          this.openingBalanceCreateModel.openingDateText = e.date.format("DD-MM-YYYY");
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

        // On change payment mood
        this.onChangePaymentMode();

        // On change bank
        this.onChangeBank();

        // On change cash
        this.onChangeCash();
      }, 0);
    }
  }

  // On change payment mode
  onChangePaymentMode(): void {
    const paymentMode = $("#paymentModeId");
    paymentMode.select2();

    paymentMode.on("change", () => {
      let paymentModeId: number = Number(paymentMode.val());
      this.openingBalanceCreateModel.paymentModeId = paymentModeId;

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

      this.cdRef.detectChanges();
    });
  }

  // On change bank
  onChangeBank(): void {
    const bank = $("#bankId");
    bank.select2();

    bank.on("change", () => {
      let bankId: number = Number(bank.val());
      this.openingBalanceCreateModel.bankId = bankId;
      this.cdRef.detectChanges();
    });
  }

  // On change cash
  onChangeCash(): void {
    const cash = $("#cashId");
    cash.select2();

    cash.on("change", () => {
      let cashId: number = Number(cash.val());
      this.openingBalanceCreateModel.cashId = cashId;
      this.cdRef.detectChanges();
    });
  }

  // Get opening balance by id
  private getOpeningBalanceById(incomeId: string): void {
    this.spinnerService.show();
    this.openingBalanceService.getById(incomeId).subscribe((result: OpeningBalanceViewModel) => {
      
      // Get payment mood & month select list
      this.paymentModes = result.optionsDataSources.PaymentModeSelectList;
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
      this.openingBalanceCreateModel.accountUnitId = result.accountUnitId;

      // Get select lists
      if(result.accountUnitId != undefined && result.accountUnitId != null && result.accountUnitId > 0) {

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

  // Check opening balance create from is valid or not
  private getOpeningBalanceFromValidResult(): boolean {
    if (this.openingBalanceCreateModel.paymentModeId == undefined || this.openingBalanceCreateModel.paymentModeId == null 
      || this.openingBalanceCreateModel.paymentModeId <= 0) {
      this.toastrService.warning("Please, provide payment mode.", "Warning");
      return false;
    } else if (this.isSelectBank && (this.openingBalanceCreateModel.bankId == undefined 
      || this.openingBalanceCreateModel.bankId == null || this.openingBalanceCreateModel.bankId <= 0)) {
      this.toastrService.warning("Please, provide bank.", "Warning");
      return false;
    } else if (this.isSelectCash && (this.openingBalanceCreateModel.cashId == undefined 
      || this.openingBalanceCreateModel.cashId == null || this.openingBalanceCreateModel.cashId <= 0)) {
      this.toastrService.warning("Please, provide bank.", "Warning");
      return false;
    } else if (this.openingBalanceCreateModel.amount == undefined || this.openingBalanceCreateModel.amount == null 
      || this.openingBalanceCreateModel.amount <= 0) {
      this.toastrService.warning("Please, provide amount.", "Warning");
      return false;
    } else if (this.openingBalanceCreateModel.openingDateText == undefined 
      || this.openingBalanceCreateModel.openingDateText == null || this.openingBalanceCreateModel.openingDateText == "") {
      this.toastrService.warning("Please, provide receipt date.", "Warning");
      return false;
    } else {
      return true;
    }
  }

  // Create opening balance
  onClickCreateOpeningBalance(): void {

    // Check opening balance create from valid or not
    let isValidCreateFrom: boolean = this.getOpeningBalanceFromValidResult();

    if (isValidCreateFrom) {
      this.spinnerService.show();
      this.openingBalanceService.create(this.openingBalanceCreateModel).subscribe((result: OpeningBalanceCreateModel) => {
        this.spinnerService.hide();
        this.toastrService.success("Opening Balance create successful.", "Success");
        return this.router.navigateByUrl("/app/opening-balances");
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error("Opening Balance create failed.", "Error");
      });
    }
  }

  // Get banks by login user account unit id
  private getBanks(loginUserAccountUnitId: number): void {
    this.spinnerService.show();
    this.bankService.getSelectListBankByAccountUnit(loginUserAccountUnitId).subscribe((result: SelectModel[]) => {
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
}