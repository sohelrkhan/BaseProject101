import { CommonModule, isPlatformBrowser } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, Inject, OnInit, PLATFORM_ID, ViewChild } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { CheckPermissionDirective } from '../../../../../../../identity/directive/check-permission.directive';
import { EmployeeCreateModel, EmployeeService, EmployeeViewModel, SelectModel } from '../../../../../../../api/base-api';
import { ToastrService } from 'ngx-toastr';
import { AccessControlService } from '../../../../../../../identity/services/access-control.service';
declare var $: any;

@Component({
  selector: 'app-employee-create',
  templateUrl: './employee-create.component.html',
  styleUrls: ['./employee-create.component.css'],
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, HttpClientModule, NgxSpinnerModule, RouterLink, CommonModule, CheckPermissionDirective],
  providers: [EmployeeService]
})

export class EmployeeCreateComponent  implements OnInit, AfterViewInit {

  // Data table related property
  isBrowser: boolean = false;

  // Default employee id
  private _employeeId: string = "-1";

  // Employee create model
  employeeCreateModel: EmployeeCreateModel = new EmployeeCreateModel();

  // Select list
  companies: SelectModel[] = [];

  // Property for employee image
  previewUrl: string | null = null;
  imageFile: File | null = null;
  @ViewChild("image") image!: ElementRef;

  constructor(private employeeService: EmployeeService, private spinnerService: NgxSpinnerService, private toastrService: ToastrService, private router: Router,
    private cdRef: ChangeDetectorRef, private accessControlService: AccessControlService, @Inject(PLATFORM_ID) private platformId: object) { }

  ngOnInit() { 
    this.isBrowser = isPlatformBrowser(this.platformId);

    if(this.isBrowser) {
      this.accessControlService.setPermissions();

      // Get employee by id
      this.getEmployeeById();
    }
  }

  // Get employee by id
  private getEmployeeById(): void {
    this.spinnerService.show();
    this.employeeService.getById(this._employeeId).subscribe((result: EmployeeViewModel) => {

      // Get company select list
      this.companies = result.optionsDataSources.CompanySelectList;

      // Set company id
      this.employeeCreateModel.companyId = -1;
      this.cdRef.detectChanges();
      ($("#companyId") as any).val(this.employeeCreateModel.companyId);

      this.spinnerService.hide();
      return;
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Employee cannot found based on the id! Please, try again.", "Error");
      return;
    })
  }

  ngAfterViewInit() { 
    if(this.isBrowser) {
      // Initialize select 2 dropdown
      this.initializeSelect2Dropdown();
    }
  }

  // Initialize select 2 dropdown
  private initializeSelect2Dropdown(): void {
    if (this.isBrowser) {
      setTimeout(() => {
        ($(".select2") as any).select2({
          placeholder: "Choose...",
          width: "100%"
        });

        // On change company
        this.onChangeCompany();
      }, 0);
    }
  }

  // On change company
  private onChangeCompany(): void {
    const company = $("#companyId");
    company.select2();

    company.on("change", () => {
      let companyId: number = Number(company.val());
      this.employeeCreateModel.companyId = companyId;
      this.cdRef.detectChanges();
    });
  }

  onFileChange(event: any): void {
    const file = event.target.files[0];

    if (file) {
      this.imageFile = file;

      const reader = new FileReader();
      reader.onload = () => {
        const base64String = reader.result as string;

        this.previewUrl = base64String;
        this.employeeCreateModel.image = base64String;
      };

      reader.readAsDataURL(file);
    }
  }

  triggerFileInput(): void {
    this.image.nativeElement.click();
  }

  // Check employee create from is valid or not
  private getEmployeeFromValidResult(): boolean {
    if (this.employeeCreateModel.fullName == undefined || this.employeeCreateModel.fullName == null || this.employeeCreateModel.fullName == "") {
      this.toastrService.warning("Please, provied full name.", "Warning");
      return false;
    } else if (this.employeeCreateModel.phoneNumber == undefined || this.employeeCreateModel.phoneNumber == null || this.employeeCreateModel.phoneNumber == "") {
      this.toastrService.warning("Please, provied phone number.", "Warning");
      return false;
    } else if (this.employeeCreateModel.email == undefined || this.employeeCreateModel.email == null || this.employeeCreateModel.email == "") {
      this.toastrService.warning("Please, provied phone number.", "Warning");
      return false;
    } else if (this.employeeCreateModel.companyId == undefined || this.employeeCreateModel.companyId == null || this.employeeCreateModel.companyId == -1) {
      this.toastrService.warning("Please, provied company.", "Warning");
      return false;
    } else {
      return true;
    }
  }

  // Create employee
  onClickCreateEmployee(): void {

    // Check employee create from valid or not
    let isValidEmployeeCreateFrom: boolean = this.getEmployeeFromValidResult();

    if (isValidEmployeeCreateFrom) {
      this.spinnerService.show();

      this.employeeService.create(this.employeeCreateModel).subscribe((result: number) => {
        this.spinnerService.hide();
        this.toastrService.success("Employee create successful.", "Success");
        return this.router.navigate(["/app/employees"]);
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error("Employee create failed! Please, try again.", "Error");
        return; 
      });
    }
  }  
}