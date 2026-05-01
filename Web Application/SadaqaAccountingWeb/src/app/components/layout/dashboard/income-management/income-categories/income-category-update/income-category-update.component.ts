import { CommonModule, isPlatformBrowser } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import {
  ChangeDetectorRef,
  Component,
  Inject,
  OnInit,
  PLATFORM_ID,
} from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { CheckPermissionDirective } from '../../../../../../../identity/directive/check-permission.directive';
import {
  IncomeCategoryService,
  IncomeCategoryUpdateModel,
  IncomeCategoryViewModel,
} from '../../../../../../../api/base-api';
import { ToastrService } from 'ngx-toastr';
import { AccessControlService } from '../../../../../../../identity/services/access-control.service';

@Component({
  selector: 'app-income-category-update',
  templateUrl: './income-category-update.component.html',
  styleUrls: ['./income-category-update.component.css'],
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
  providers: [IncomeCategoryService],
})
export class IncomeCategoryUpdateComponent implements OnInit {
  // Data table related property
  isBrowser: boolean = false;

  // Default income category id
  private _id: string = '-1';

  // Income category update model
  incomeCategoryUpdateModel: IncomeCategoryUpdateModel =
    new IncomeCategoryUpdateModel();

  constructor(
    private incomeCategoryService: IncomeCategoryService,
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
      // Perform any browser-specific initialization here
      this.getIncomeCategoryIdByUrl();
    }
  }
  // Get income category id by url
  private getIncomeCategoryIdByUrl(): void {
    this.activatedRoute.params.subscribe((params) => {
      this._id = params['recordId'];

      // Get income category by id
      if (this._id != undefined || this._id != null || this._id! != '') {
        this.getIncomeCategoryById();
      }
    });
  }
  // Get income category by id
  private getIncomeCategoryById(): void {
    this.spinnerService.show();
    this.incomeCategoryService.getById(this._id).subscribe(
      (result: IncomeCategoryViewModel) => {
        // Set income category update model
        this.incomeCategoryUpdateModel = result.updateModel;
        // Initialize tom select dropdown
        this.cdRef.detectChanges();
        this.spinnerService.hide();
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error(
          'Income category cannot found! Please, try again.',
        );
      },
    );
  }
  // Update income category
  onClickUpdateIncomeCategory(): void {
    // Check income category update from valid or not
    let isValidIncomeCategoryUpdateFrom: boolean =
      this.getIncomeCategoryFromValidResult();

    if (isValidIncomeCategoryUpdateFrom) {
      this.spinnerService.show();

      this.incomeCategoryService
        .update(this.incomeCategoryUpdateModel)
        .subscribe(
          (result: IncomeCategoryUpdateModel) => {
            this.spinnerService.hide();
            this.toastrService.success(
              'Income category update successful.',
              'Success',
            );
            return this.router.navigate(['/app/income-categories']);
          },
          (error: any) => {
            this.spinnerService.hide();
            this.toastrService.error(
              'Income category update failed! Please, try again.',
              'Error',
            );
            return;
          },
        );
    }
  }

  // Check income category update from is valid or not
  private getIncomeCategoryFromValidResult(): boolean {
    if (
      this.incomeCategoryUpdateModel.name == undefined ||
      this.incomeCategoryUpdateModel.name == null ||
      this.incomeCategoryUpdateModel.name == ''
    ) {
      this.toastrService.warning(
        'Please, provied income category name.',
        'Warning',
      );
      return false;
    } else {
      return true;
    }
  }
  onCheckUncheck(event: any): void {
    if (event.target.checked) {
      this.incomeCategoryUpdateModel.isDonorBased = true;
    } else {
      this.incomeCategoryUpdateModel.isDonorBased = false;
    }
  }
}
