import {
  ChangeDetectorRef,
  Component,
  Inject,
  OnInit,
  PLATFORM_ID,
} from '@angular/core';
import {
  CompanyCreateModel,
  DonorCreateModel,
  DonorService,
  DonorViewModel,
} from '../../../../../../../api/base-api';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { Router, RouterLink } from '@angular/router';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { CheckPermissionDirective } from '../../../../../../../identity/directive/check-permission.directive';
import { ToastrService } from 'ngx-toastr';
import { AccessControlService } from '../../../../../../../identity/services/access-control.service';

@Component({
  selector: 'app-donor-create',
  templateUrl: './donor-create.component.html',
  styleUrls: ['./donor-create.component.css'],
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
  providers: [DonorService],
})
export class DonorCreateComponent implements OnInit {
  // Data table related property
  isBrowser: boolean = false;

  // Default donor id
  private _id: string = '-1';

  // Donor create model
  donorCreateModel: DonorCreateModel = new DonorCreateModel();
  constructor(
    private donorService: DonorService,
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
      this.getDonorById();
    }
  }
  // Get donor by id
  private getDonorById(): void {
    this.spinnerService.show();
    this.donorService.getById(this._id).subscribe(
      (result: DonorViewModel) => {
        // Set donor create model
        this.donorCreateModel.code = result.generateDonorCode;
        this.spinnerService.hide();
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error('Country cannot found! Please, try again.');
      },
    );
  }
  // Create donor
  onClickCreateDonor(): void {
    // Check donor create from valid or not
    let isValidDonorCreateFrom: boolean = this.getDonorFromValidResult();

    if (isValidDonorCreateFrom) {
      this.spinnerService.show();

      this.donorService.create(this.donorCreateModel).subscribe(
        (result: DonorCreateModel) => {
          this.spinnerService.hide();
          this.toastrService.success('Donor create successful.', 'Success');
          return this.router.navigate(['/app/donors']);
        },
        (error: any) => {
          this.spinnerService.hide();
          this.toastrService.error(
            'Donor create failed! Please, try again.',
            'Error',
          );
          return;
        },
      );
    }
  }

  // Check donor create from is valid or not
  private getDonorFromValidResult(): boolean {
    if (
      this.donorCreateModel.name == undefined ||
      this.donorCreateModel.name == null ||
      this.donorCreateModel.name == ''
    ) {
      this.toastrService.warning('Please, provied donor name.', 'Warning');
      return false;
    } else if (
      this.donorCreateModel.code == undefined ||
      this.donorCreateModel.code == null ||
      this.donorCreateModel.code == ''
    ) {
      this.toastrService.warning('Please, provied donor code.', 'Warning');
      return false;
    } else if (
      this.donorCreateModel.mobileNo == undefined ||
      this.donorCreateModel.mobileNo == null ||
      this.donorCreateModel.mobileNo == ''
    ) {
      this.toastrService.warning(
        'Please, provied donor mobile number.',
        'Warning',
      );
      return false;
    } else {
      return true;
    }
  }
}
