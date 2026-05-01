import { CommonModule, isPlatformBrowser } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { AfterViewInit, ChangeDetectorRef, Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { BankService, CashService, OpeningBalanceService, OpeningBalanceUpdateModel, OpeningBalanceViewModel, SelectModel, UserModel } from '../../../../../../../api/base-api';
import { CustomTosterServiceService } from '../../../../../../shared/Toster/CustomTosterService.service';
import { IdentityService } from '../../../../../../../identity/identity-shared/identity.service';
import { PaymentMode } from '../../../../../../shared/enum-configure/enum-configure';

@Component({
  selector: 'app-opening-balance-update',
  templateUrl: './opening-balance-update.component.html',
  styleUrls: ['./opening-balance-update.component.css'],
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

export class OpeningBalanceUpdateComponent implements OnInit, AfterViewInit {

  isBrowser: boolean = false;
      
  // Select bank or cash
  isSelectBank: boolean = false;
  isSelectCash: boolean = false;

  // Opening Balance id
  private _openingBalanceId: string = "-1";

  // Login user account unit id
  private _loginUserAccountUnitId: number | undefined;

  // Opening Balance update model
  openingBalanceUpdateModel: OpeningBalanceUpdateModel = new OpeningBalanceUpdateModel();

  // Select model
  paymentModes: SelectModel[] = [];
  banks: SelectModel[] = [];
  cashes: SelectModel[] = [];

  constructor(
    private openingBalanceService: OpeningBalanceService, 
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
      this.getOpeningBalanceIdByUrl();
    }
  }

  // Get opening balance id by url
  private getOpeningBalanceIdByUrl(): void {
    this.activatedRoute.params.subscribe((params) => {
      this._openingBalanceId = params['recordId'];

      if (this._openingBalanceId != undefined || this._openingBalanceId != null || this._openingBalanceId! != '') {
        this.getOpeningBalanceById(this._openingBalanceId);
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
        ($("#openingDateText") as any).datetimepicker({
          format: "DD-MM-YYYY",
          showClear: true
        })
        .on("dp.change", (e) => {
          this.openingBalanceUpdateModel.openingDateText = e.date.format("DD-MM-YYYY");
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
      this.openingBalanceUpdateModel.paymentModeId = paymentModeId;
      this.showHideBankCash(this.openingBalanceUpdateModel.paymentModeId);

      this.cdRef.detectChanges();
    });
  }

  // On change bank
  onChangeBank(): void {
    const bank = $("#bankId");
    bank.select2();

    bank.on("change", () => {
      let bankId: number = Number(bank.val());
      this.openingBalanceUpdateModel.bankId = bankId;
      this.cdRef.detectChanges();
    });
  }

  // On change cash
  onChangeCash(): void {
    const cash = $("#cashId");
    cash.select2();

    cash.on("change", () => {
      let cashId: number = Number(cash.val());
      this.openingBalanceUpdateModel.cashId = cashId;
      this.cdRef.detectChanges();
    });
  }

  // Get opening balance by id
  private getOpeningBalanceById(openingBalanceId: string): void {
    this.spinnerService.show();
    this.openingBalanceService.getById(openingBalanceId).subscribe((result: OpeningBalanceViewModel) => {
      
      this.openingBalanceUpdateModel = result.updateModel;

      // Get payment mood select list
      this.paymentModes = result.optionsDataSources.PaymentModeSelectList;
      this.cdRef.detectChanges();

      setTimeout(() => {
        // IMPORTANT: set value + trigger change so select2 updates UI
        ($("#paymentModeId") as any).val(String(this.openingBalanceUpdateModel.paymentModeId)).trigger("change.select2");
        ($("#bankId") as any).val(String(this.openingBalanceUpdateModel.bankId)).trigger("change.select2");
        ($("#cashId") as any).val(String(this.openingBalanceUpdateModel.cashId)).trigger("change.select2");
      }, 0);

      this.showHideBankCash(this.openingBalanceUpdateModel.paymentModeId);
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
      this.openingBalanceUpdateModel.accountUnitId = result.accountUnitId;

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

  // Check opening balance update from is valid or not
  private getOpeningBalanceFromValidResult(): boolean {
    if (this.openingBalanceUpdateModel.paymentModeId == undefined || this.openingBalanceUpdateModel.paymentModeId == null 
      || this.openingBalanceUpdateModel.paymentModeId <= 0) {
      this.toastrService.warning("Please, provide payment mode.", "Warning");
      return false;
    } else if (this.isSelectBank && (this.openingBalanceUpdateModel.bankId == undefined 
      || this.openingBalanceUpdateModel.bankId == null || this.openingBalanceUpdateModel.bankId <= 0)) {
      this.toastrService.warning("Please, provide bank.", "Warning");
      return false;
    } else if (this.isSelectCash && (this.openingBalanceUpdateModel.cashId == undefined 
      || this.openingBalanceUpdateModel.cashId == null || this.openingBalanceUpdateModel.cashId <= 0)) {
      this.toastrService.warning("Please, provide bank.", "Warning");
      return false;
    } else if (this.openingBalanceUpdateModel.amount == undefined || this.openingBalanceUpdateModel.amount == null 
      || this.openingBalanceUpdateModel.amount <= 0) {
      this.toastrService.warning("Please, provide amount.", "Warning");
      return false;
    } else if (this.openingBalanceUpdateModel.openingDateText == undefined || this.openingBalanceUpdateModel.openingDateText == null 
      || this.openingBalanceUpdateModel.openingDateText == "") {
      this.toastrService.warning("Please, provide receipt date.", "Warning");
      return false;
    } else {
      return true;
    }
  }

  // Update opening balance
  onClickUpdate(): void {

    let isValidUpdateFrom: boolean = this.getOpeningBalanceFromValidResult();

    if (isValidUpdateFrom) {
      this.spinnerService.show();
      this.openingBalanceService.update(this.openingBalanceUpdateModel).subscribe((result: OpeningBalanceUpdateModel) => {
        this.spinnerService.hide();
        this.toastrService.success("Opening Balance update successful.", "Success");
        return this.router.navigateByUrl("/app/opening-balances");
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error("Opening Balance update failed.", "Error");
      });
    }
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
}