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
import { Router, RouterLink } from '@angular/router';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { CheckPermissionDirective } from '../../../../../../../identity/directive/check-permission.directive';
import {
  IncomeCategoryCreateModel,
  IncomeCategoryService,
} from '../../../../../../../api/base-api';
import { ToastrService } from 'ngx-toastr';
import { AccessControlService } from '../../../../../../../identity/services/access-control.service';

@Component({
  selector: 'app-income-category-create',
  templateUrl: './income-category-create.component.html',
  styleUrls: ['./income-category-create.component.css'],
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
export class IncomeCategoryCreateComponent implements OnInit {
  // Data table related property
  isBrowser: boolean = false;

  // Default income category id
  private _id: string = '-1';

  // Income category create model
  incomeCategoryCreateModel: IncomeCategoryCreateModel =
    new IncomeCategoryCreateModel();

  constructor(
    private incomeCategoryService: IncomeCategoryService,
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
      // Perform any browser-specific initialization here
    }
  }

  // Create income category
  onClickCreateIncomeCategory(): void {
    // Check income category create from valid or not
    let isValidIncomeCategoryCreateFrom: boolean =
      this.getIncomeCategoryFromValidResult();

    if (isValidIncomeCategoryCreateFrom) {
      this.spinnerService.show();

      this.incomeCategoryService
        .create(this.incomeCategoryCreateModel)
        .subscribe(
          (result: IncomeCategoryCreateModel) => {
            this.spinnerService.hide();
            this.toastrService.success(
              'Income category create successful.',
              'Success',
            );
            return this.router.navigate(['/app/income-categories']);
          },
          (error: any) => {
            this.spinnerService.hide();
            this.toastrService.error(
              'Income category create failed! Please, try again.',
              'Error',
            );
            return;
          },
        );
    }
  }

  // Check income category create from is valid or not
  private getIncomeCategoryFromValidResult(): boolean {
    if (
      this.incomeCategoryCreateModel.name == undefined ||
      this.incomeCategoryCreateModel.name == null ||
      this.incomeCategoryCreateModel.name == ''
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
      this.incomeCategoryCreateModel.isDonorBased = true;
    } else {
      this.incomeCategoryCreateModel.isDonorBased = false;
    }
  }
}
