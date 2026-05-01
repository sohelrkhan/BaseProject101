import { CommonModule, isPlatformBrowser } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ChangeDetectorRef, Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { ExpenseCategoryService, ExpenseCategoryUpdateModel, ExpenseCategoryViewModel } from '../../../../../../../api/base-api';
import { CustomTosterServiceService } from '../../../../../../shared/Toster/CustomTosterService.service';

@Component({
  selector: 'app-expense-category-update',
  templateUrl: './expense-category-update.component.html',
  styleUrls: ['./expense-category-update.component.css'],
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, HttpClientModule, NgxSpinnerModule, RouterLink, CommonModule],
  providers: [ExpenseCategoryService]
})

export class ExpenseCategoryUpdateComponent implements OnInit {

  // Is browser
  isBrowser: boolean = false;

  // Expense Category id
  private _expenseCategoryId: string | undefined;

  // Expense Category update model
  expenseCategoryUpdateModel: ExpenseCategoryUpdateModel = new ExpenseCategoryUpdateModel();

  constructor(private expenseCategoryService: ExpenseCategoryService, private spinnerService: NgxSpinnerService, private toastrService: CustomTosterServiceService, private router: Router,
    private activatedRoute: ActivatedRoute, @Inject(PLATFORM_ID) private platformId: object, private cdRef: ChangeDetectorRef) { }

  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);

    if (this.isBrowser) {
      this.getExpenseCategoryIdByUrl();
    }
  }

  // Get expense category id by url
  private getExpenseCategoryIdByUrl(): void {
    this.activatedRoute.params.subscribe((params) => {
      this._expenseCategoryId = params["recordId"];

      if (this._expenseCategoryId != undefined || this._expenseCategoryId != null || this._expenseCategoryId != "") {
        this.getExpenseCategoryById();
      }
    });
  }

  // Get expense category by id
  private getExpenseCategoryById(): void {
    this.spinnerService.show();
    this.expenseCategoryService.getById(this._expenseCategoryId!).subscribe((result: ExpenseCategoryViewModel) => {
      this.expenseCategoryUpdateModel = result.updateModel;
      this.spinnerService.hide();
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Expense Category cannot found! Please, try again.");
    });
  }

  // Update expense category
  onClickUpdateExpenseCategory(): void {

    // Check expense category from valid or not
    let isValidFrom: boolean = this.getExpenseCategoryFromValidResult();

    if (isValidFrom) {
      this.spinnerService.show();
      this.expenseCategoryService.update(this.expenseCategoryUpdateModel).subscribe((result: ExpenseCategoryUpdateModel) => {
        this.spinnerService.hide();
        this.toastrService.success("Expense category update successful.", "Success");
        return this.router.navigateByUrl("/app/expense-categories");
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error("Expense category cannot update! Please, try again.");
      });
    }
  }

  // Check expense category create from is valid or not
  private getExpenseCategoryFromValidResult(): boolean {
    if (this.expenseCategoryUpdateModel.name == undefined || this.expenseCategoryUpdateModel.name == null || this.expenseCategoryUpdateModel.name == "") {
      this.toastrService.warning("Please, provied name.", "Warning");
      return false;
    } else {
      return true;
    }
  }
}