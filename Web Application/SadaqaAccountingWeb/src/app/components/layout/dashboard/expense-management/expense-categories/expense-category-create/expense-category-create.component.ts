import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { ExpenseCategoryCreateModel, ExpenseCategoryService } from '../../../../../../../api/base-api';
import { CustomTosterServiceService } from '../../../../../../shared/Toster/CustomTosterService.service';

@Component({
  selector: 'app-expense-category-create',
  templateUrl: './expense-category-create.component.html',
  styleUrls: ['./expense-category-create.component.css'],
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, HttpClientModule, NgxSpinnerModule, RouterLink, CommonModule],
  providers: [ExpenseCategoryService]
})

export class ExpenseCategoryCreateComponent implements OnInit {

  // Expense category create model
  expenseCategoryCreateModel: ExpenseCategoryCreateModel = new ExpenseCategoryCreateModel();

  constructor(private expenseCategoryService: ExpenseCategoryService, private spinnerService: NgxSpinnerService, private toastrService: CustomTosterServiceService, private router: Router) { }

  ngOnInit() { }

  // Check expense category create from is valid or not
  private getExpenseCategoryFromValidResult(): boolean {
    if (this.expenseCategoryCreateModel.name == undefined || this.expenseCategoryCreateModel.name == null || this.expenseCategoryCreateModel.name == "") {
      this.toastrService.warning("Please, provied name.", "Warning");
      return false;
    } else {
      return true;
    }
  }
  
  // Create expense category
  onClickCreateExpenseCategory(): void {

    // Check expense category create from valid or not
    let isValidCreateFrom: boolean = this.getExpenseCategoryFromValidResult();

    if (isValidCreateFrom) {
      this.spinnerService.show();
      this.expenseCategoryService.create(this.expenseCategoryCreateModel).subscribe((result: ExpenseCategoryCreateModel) => {
        this.spinnerService.hide();
        this.toastrService.success("Expense Category create successful.", "Success");
        return this.router.navigateByUrl("/app/expense-categories");
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error("Expense Category create failed.", "Error");
      });
    }
  }
}