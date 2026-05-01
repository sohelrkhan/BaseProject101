import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  Inject,
  OnInit,
  PLATFORM_ID,
} from '@angular/core';
import {
  CompanyViewModel,
  DonorService,
  DonorUpdateModel,
  DonorViewModel,
  SelectModel,
} from '../../../../../../../api/base-api';
import { CheckPermissionDirective } from '../../../../../../../identity/directive/check-permission.directive';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AccessControlService } from '../../../../../../../identity/services/access-control.service';

@Component({
  selector: 'app-donor-update',
  templateUrl: './donor-update.component.html',
  styleUrls: ['./donor-update.component.css'],
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
export class DonorUpdateComponent implements OnInit, AfterViewInit {
  // Data table related property
  isBrowser: boolean = false;

  // Default donor id
  private _id: string = '-1';

  // Donor UPDATE model
  donorUpdateModel: DonorUpdateModel = new DonorUpdateModel();
  statuses: SelectModel[] = [];

  constructor(
    private donorService: DonorService,
    private spinnerService: NgxSpinnerService,
    private toastrService: ToastrService,
    private router: Router,
    private cdRef: ChangeDetectorRef,
    private accessControlService: AccessControlService,
    @Inject(PLATFORM_ID) private platformId: object,
    private activatedRoute: ActivatedRoute,
  ) {}

  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);
    if (this.isBrowser) {
      this.accessControlService.setPermissions();
      this.getDonorIdByUrl();
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

        // On change status
        this.onChangeStatus();
      }, 0);
    }
  }

  // On change status
  private onChangeStatus(): void {
    const status = $('#statusId');
    status.select2();

    status.on('change', () => {
      let statusId: number = Number(status.val());

      if (statusId != undefined || statusId != null || statusId != -1) {
        this.donorUpdateModel.statusId = statusId;
      }

      this.cdRef.detectChanges();
    });
  }
  // Get donor id by url
  private getDonorIdByUrl(): void {
    this.activatedRoute.params.subscribe((params) => {
      this._id = params['recordId'];

      // Get donor by id
      if (this._id != undefined || this._id != null || this._id! != '') {
        this.getDonorById();
      }
    });
  }

  // Get donor by id
  private getDonorById(): void {
    this.spinnerService.show();
    this.donorService.getById(this._id).subscribe(
      (result: DonorViewModel) => {
        // Set donor update model
        this.donorUpdateModel = result.updateModel;

        // Get select list
        this.statuses = result.optionsDataSources.StatusSelectList;

        // Initialize tom select dropdown
        this.cdRef.detectChanges();

        // Initialize tom select dropdown
        this.cdRef.detectChanges();
        setTimeout(() => {
          $('#statusId').select2({ width: '100%' });
          $('#statusId')
            .val(this.donorUpdateModel.statusId.toString())
            .trigger('change');
        }, 10);

        this.spinnerService.hide();
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error('Donor cannot found! Please, try again.');
      },
    );
  }

  // Update donor
  onClickUpdateDonor(): void {
    // Check donor from valid or not
    let isValidDonorFrom: boolean = this.getDonorFromValidResult();

    if (isValidDonorFrom) {
      this.spinnerService.show();

      this.donorService.update(this.donorUpdateModel).subscribe(
        (result: DonorUpdateModel) => {
          this.spinnerService.hide();
          this.toastrService.success('Donor update successful.', 'Success');
          return this.router.navigateByUrl('/app/donors');
        },
        (error: any) => {
          this.spinnerService.hide();
          this.toastrService.error(
            'Donor update failed. Please, try again.',
            'Error',
          );
        },
      );
    }
  }
  // Check donor update from is valid or not
  private getDonorFromValidResult(): boolean {
    if (
      this.donorUpdateModel.name == undefined ||
      this.donorUpdateModel.name == null ||
      this.donorUpdateModel.name == ''
    ) {
      this.toastrService.warning('Please, provied donor name.', 'Warning');
      return false;
    } else if (
      this.donorUpdateModel.code == undefined ||
      this.donorUpdateModel.code == null ||
      this.donorUpdateModel.code == ''
    ) {
      this.toastrService.warning('Please, provied donor code.', 'Warning');
      return false;
    } else if (
      this.donorUpdateModel.mobileNo == undefined ||
      this.donorUpdateModel.mobileNo == null ||
      this.donorUpdateModel.mobileNo == ''
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
