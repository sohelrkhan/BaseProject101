import { HttpClientModule } from '@angular/common/http';
import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  Inject,
  OnInit,
  PLATFORM_ID,
  ViewChild,
} from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import {
  CompanyCreateModel,
  CompanyService,
  CompanyViewModel,
} from '../../../../../../../api/base-api';
import { SelectModel } from '../../../../../../shared/models/select-model';
import { CheckPermissionDirective } from '../../../../../../../identity/directive/check-permission.directive';
import { AccessControlService } from '../../../../../../../identity/services/access-control.service';
declare var $: any;

@Component({
  selector: 'app-company-create',
  templateUrl: './company-create.component.html',
  styleUrls: ['./company-create.component.css'],
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule,
    NgxSpinnerModule,
    RouterLink,
    CommonModule,
    CheckPermissionDirective,
  ],
  providers: [CompanyService],
})
export class CompanyCreateComponent implements OnInit, AfterViewInit {
  // Data table related property
  isBrowser: boolean = false;

  // Default company id
  private _id: string = '-1';

  // Company create model
  companyCreateModel: CompanyCreateModel = new CompanyCreateModel();

  // Select list
  parentCompanies: SelectModel[] = [];
  defaultCurrencies: SelectModel[] = [];
  otherCurrencies: SelectModel[] = [];

  constructor(
    private companyService: CompanyService,
    private spinnerService: NgxSpinnerService,
    private toastrService: ToastrService,
    private router: Router,
    private cdRef: ChangeDetectorRef,
    private accessControlService: AccessControlService,
    @Inject(PLATFORM_ID) private platformId: object,
  ) {}

  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);

    if (this.isBrowser) {
      this.accessControlService.setPermissions();

      // Get select list
      this.getSelectList();
    }
  }

  ngAfterViewInit() {
    // Initialize select 2 dropdown
    this.initializeSelect2Dropdown();
  }

  // Initialize select 2 dropdown
  private initializeSelect2Dropdown(): void {
    if (this.isBrowser) {
      setTimeout(() => {
        ($('.select2') as any).select2({
          placeholder: 'Choose...',
          width: '100%',
        });
      }, 0);
    }
  }

  // Get select list
  private getSelectList(): void {
    this.spinnerService.show();
    this.companyService.getById(this._id).subscribe(
      (result: CompanyViewModel) => {
        this.parentCompanies = result.optionsDataSources.CompanySelectList;
        this.defaultCurrencies = result.optionsDataSources.CurrencySelectList;
        this.otherCurrencies = result.optionsDataSources.CurrencySelectList;

        // Initialize tom select dropdown
        this.cdRef.detectChanges();

        this.spinnerService.hide();
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error(
          'An error occurred while fetching select list data.',
          'Error',
        );
      },
    );
  }

  previewUrl: string | null = null;
  logoFile: File | null = null;
  @ViewChild('logoInput') logoInput!: ElementRef;

  onFileChange(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.logoFile = file;

      const reader = new FileReader();
      reader.onload = () => {
        const base64String = reader.result as string;

        this.previewUrl = base64String;
        this.companyCreateModel.logo = base64String;
        // this.companyCreateModel.logo = file.name;
      };

      reader.readAsDataURL(file);
    }
  }

  triggerFileInput(): void {
    this.logoInput.nativeElement.click();
  }

  // Create company
  onClickCreateCompany(): void {
    // Check company create from valid or not
    let isValidCompanyCreateFrom: boolean = this.getCompanyFromValidResult();

    if (isValidCompanyCreateFrom) {
      this.spinnerService.show();

      this.companyService.create(this.companyCreateModel).subscribe(
        (result: CompanyCreateModel) => {
          this.spinnerService.hide();
          this.toastrService.success('Company create successful.', 'Success');
          return this.router.navigate(['/app/companies']);
        },
        (error: any) => {
          this.spinnerService.hide();
          this.toastrService.error(
            'Company create failed! Please, try again.',
            'Error',
          );
          return;
        },
      );
    }
  }

  // Check company create from is valid or not
  private getCompanyFromValidResult(): boolean {
    if (
      this.companyCreateModel.name == undefined ||
      this.companyCreateModel.name == null ||
      this.companyCreateModel.name == ''
    ) {
      this.toastrService.warning('Please, provied company name.', 'Warning');
      return false;
    } else if (
      this.companyCreateModel.country == undefined ||
      this.companyCreateModel.country == null ||
      this.companyCreateModel.country == ''
    ) {
      this.toastrService.warning('Please, provied country name.', 'Warning');
      return false;
    } else {
      return true;
    }
  }
}
