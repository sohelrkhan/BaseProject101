import { CommonModule, isPlatformBrowser } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { BankCreateModel, BankService, UserModel } from '../../../../../../../api/base-api';
import { CustomTosterServiceService } from '../../../../../../shared/Toster/CustomTosterService.service';
import { IdentityService } from '../../../../../../../identity/identity-shared/identity.service';

@Component({
  selector: 'app-bank-create',
  templateUrl: './bank-create.component.html',
  styleUrls: ['./bank-create.component.css'],
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
    BankService
  ]
})

export class BankCreateComponent implements OnInit {

  isBrowser: boolean = false;

  // Login user account unit id
  private _loginUserAccountUnitId: number | undefined;

  // Bank create model
  bankCreateModel: BankCreateModel = new BankCreateModel();

  constructor(
    private bankService: BankService, 
    private spinnerService: NgxSpinnerService, 
    private toastrService: CustomTosterServiceService, 
    private router: Router, 
    private identityService: IdentityService, 
    @Inject(PLATFORM_ID) private platformId: object
  ) { }

  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);

    if(this.isBrowser) {
      this.getLoginUserInfo();
    }
  }

  // Get login user info
  private getLoginUserInfo(): void {
    this.spinnerService.show();
    this.identityService.getLoginInfo().subscribe((result: UserModel) => {
      this._loginUserAccountUnitId = result.accountUnitId;

      // Set login user account unit id in the create model
      this.bankCreateModel.accountUnitId = result.accountUnitId;

      this.spinnerService.hide();
      return;
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Login user info cannot found! Please, try again.", "Error");
      return;
    })
  }

  // Check bank create from is valid or not
  private getBankFromValidResult(): boolean {
    if (this.bankCreateModel.name == undefined || this.bankCreateModel.name == null || this.bankCreateModel.name == "") {
      this.toastrService.warning("Please, provied name.", "Warning");
      return false;
    } else if (this.bankCreateModel.branchName == undefined || this.bankCreateModel.branchName == null || this.bankCreateModel.branchName == "") {
      this.toastrService.warning("Please, provied branch name.", "Warning");
      return false;
    } else if (this.bankCreateModel.accountNumber == undefined || this.bankCreateModel.accountNumber == null || this.bankCreateModel.accountNumber == "") {
      this.toastrService.warning("Please, provied account number.", "Warning");
      return false;
    } else if (this.bankCreateModel.openingBalance == undefined || this.bankCreateModel.openingBalance == null || this.bankCreateModel.openingBalance <= 0) {
      this.toastrService.warning("Please, provied opening balance.", "Warning");
      return false;
    } else {
      return true;
    }
  }

  // Create bank
  onClickCreateBank(): void {

    // Check bank create from valid or not
    let isValidCreateFrom: boolean = this.getBankFromValidResult();

    if (isValidCreateFrom) {
      this.spinnerService.show();
      this.bankService.create(this.bankCreateModel).subscribe((result: BankCreateModel) => {
        this.spinnerService.hide();
        this.toastrService.success("Bank create successful.", "Success");
        return this.router.navigateByUrl("/app/banks");
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error("Bank create failed.", "Error");
      });
    }
  }
}