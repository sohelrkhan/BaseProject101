import { CommonModule, isPlatformBrowser } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { AfterViewInit, ChangeDetectorRef, Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { BankService, BankUpdateModel, BankViewModel, SelectModel } from '../../../../../../../api/base-api';
import { CustomTosterServiceService } from '../../../../../../shared/Toster/CustomTosterService.service';

declare var $: any;

@Component({
  selector: 'app-bank-update',
  templateUrl: './bank-update.component.html',
  styleUrls: ['./bank-update.component.css'],
  standalone: true,
  imports: [ReactiveFormsModule, 
    FormsModule, 
    HttpClientModule, 
    NgxSpinnerModule, 
    RouterLink, 
    CommonModule
  ],
  providers: [BankService]
})

export class BankUpdateComponent implements OnInit, AfterViewInit {

  // Is browser
  isBrowser: boolean = false;

  // Bank id
  private _bankId: string | undefined;

  // bank update model
  bankUpdateModel: BankUpdateModel = new BankUpdateModel();

  // Select model
  statusSelectList: SelectModel[] = [];

  constructor(private bankService: BankService, private spinnerService: NgxSpinnerService, private toastrService: CustomTosterServiceService, private router: Router,
    private activatedRoute: ActivatedRoute, @Inject(PLATFORM_ID) private platformId: object, private cdRef: ChangeDetectorRef) { }
  
  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);

    if (this.isBrowser) {
      this.getBankIdByUrl();
    }
  }

  ngAfterViewInit(): void {
    // Initialize select 2 dropdown
    this.initializeSelect2Dropdown();
  }

  // Initialize select 2 dropdown
  private initializeSelect2Dropdown(): void {
    if (this.isBrowser) {
      setTimeout(() => {
        ($(".select2") as any).select2({
          placeholder: "Choose...",
          width: "100%"
        });

        // On expense status
        this.onChangeStatus();
      }, 0);
    }
  }

  // On change status
  onChangeStatus(): void {
    const status = $("#statusId");
    status.select2();

    status.on("change", () => {
      let statusId: number = Number(status.val());
      this.bankUpdateModel.statusId = statusId;
      this.cdRef.detectChanges();
    });
  }

  // Get bank id by url
  private getBankIdByUrl(): void {
    this.activatedRoute.params.subscribe((params) => {
      this._bankId = params["recordId"];

      if (this._bankId != undefined || this._bankId != null || this._bankId != "") {
        this.getBankById();
      }
    });
  }

  // Get bank by id
  private getBankById(): void {
    this.spinnerService.show();
    this.bankService.getById(this._bankId!).subscribe((result: BankViewModel) => {
      this.bankUpdateModel = result.updateModel;
      this.statusSelectList = result.optionsDataSources.StatusSelectList;

      this.cdRef.detectChanges();
      ($("#statusId") as any).val(this.bankUpdateModel.statusId);
      
      this.spinnerService.hide();      
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Bank cannot found! Please, try again.");
    });
  }

  // Check bank update from is valid or not
  private getBankFromValidResult(): boolean {
    if (this.bankUpdateModel.name == undefined || this.bankUpdateModel.name == null || this.bankUpdateModel.name == "") {
      this.toastrService.warning("Please, provied name.", "Warning");
      return false;
    } else if (this.bankUpdateModel.branchName == undefined || this.bankUpdateModel.branchName == null || this.bankUpdateModel.branchName == "") {
      this.toastrService.warning("Please, provied branch name.", "Warning");
      return false;
    } else if (this.bankUpdateModel.accountNumber == undefined || this.bankUpdateModel.accountNumber == null || this.bankUpdateModel.accountNumber == "") {
      this.toastrService.warning("Please, provied account number.", "Warning");
      return false;
    } else if (this.bankUpdateModel.openingBalance == undefined || this.bankUpdateModel.openingBalance == null || this.bankUpdateModel.openingBalance <= 0) {
      this.toastrService.warning("Please, provied opening balance.", "Warning");
      return false;
    } else if (this.bankUpdateModel.statusId == undefined || this.bankUpdateModel.statusId == null || this.bankUpdateModel.statusId <= 0) {
      this.toastrService.warning("Please, provied status.", "Warning");
      return false;
    } else {
      return true;
    }
  }

  // Update bank
  onClickUpdateBank(): void {

    // Check bank from valid or not
    let isValidFrom: boolean = this.getBankFromValidResult();

    if (isValidFrom) {
      this.spinnerService.show();
      this.bankService.update(this.bankUpdateModel).subscribe((result: BankUpdateModel) => {
        this.spinnerService.hide();
        this.toastrService.success("Bank update successful.", "Success");
        return this.router.navigateByUrl("/app/banks");
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error("Bank cannot update! Please, try again.");
      });
    }
  }
}