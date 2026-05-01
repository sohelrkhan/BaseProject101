import { CommonModule, isPlatformBrowser } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ChangeDetectorRef, Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { CheckPermissionDirective } from '../../../../../../../identity/directive/check-permission.directive';
import { EmployeeService, EmployeeUpdateModel, EmployeeViewModel, SelectModel } from '../../../../../../../api/base-api';
import { ToastrService } from 'ngx-toastr';
import { AccessControlService } from '../../../../../../../identity/services/access-control.service';
import { environment } from '../../../../../../../environments/environment';

@Component({
  selector: 'app-employee-update',
  templateUrl: './employee-update.component.html',
  styleUrls: ['./employee-update.component.css'],
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, HttpClientModule, NgxSpinnerModule, RouterLink, CommonModule, CheckPermissionDirective],
  providers: [EmployeeService]
})

export class EmployeeUpdateComponent implements OnInit {

  // Is browser
  isBrowser: boolean = false;

  // Employee id
  private _employeeId: string | undefined;
  applicationBaseUrl: string | undefined;

  // Select list
  companies: SelectModel[] = [];
  statuses: SelectModel[] = [];

  // Employee update model
  employeeUpdateModel: EmployeeUpdateModel = new EmployeeUpdateModel();

  constructor(private employeeService: EmployeeService, private spinnerService: NgxSpinnerService, private toastrService: ToastrService, private router: Router,
    private activatedRoute: ActivatedRoute, private cdRef: ChangeDetectorRef, private accessControlService: AccessControlService, 
    @Inject(PLATFORM_ID) private platformId: object) { }

  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);

    if(this.isBrowser) {

      this.applicationBaseUrl = environment.coreBaseUrl;
      this.accessControlService.setPermissions();

      // Get employee id
      this.getEmployeeIdByUrl();
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
        ($(".select2") as any).select2({
          placeholder: "Choose...",
          width: "100%"
        });

        // On change company
        this.onChangeCompany();

        // On change status
        this.onChangeStatus();
      }, 0);
    }
  }

  // On change company
  private onChangeCompany(): void {
    const company = $("#companyId");
    company.select2();

    company.on("change", () => {
      let companyId: number = Number(company.val());
    
      if(companyId != undefined || companyId != null || companyId != -1) {
        this.employeeUpdateModel.companyId = companyId;
      } 
   
      this.cdRef.detectChanges();
    });
  }

  // On change status
  private onChangeStatus(): void {
    const status = $("#statusId");
    status.select2();

    status.on("change", () => {
      let statusId: number = Number(status.val());
    
      if(statusId != undefined || statusId != null || statusId != -1) {
        this.employeeUpdateModel.statusId = statusId;
      } 
   
      this.cdRef.detectChanges();
    });
  }

  // Get employee id by url
  private getEmployeeIdByUrl(): void {
    this.activatedRoute.params.subscribe((params) => {
      this._employeeId = params["recordId"];

      // Get employee by id
      if (this._employeeId != undefined || this._employeeId != null || this._employeeId! != "") {
        this.getEmployeeById();
      }
    });
  }

  // Get employee by id
  private getEmployeeById(): void {
    this.spinnerService.show();
    this.employeeService.getById(this._employeeId).subscribe((result: EmployeeViewModel) => {

      // Set employee update model
      this.employeeUpdateModel = result.updateModel;
        
      // Get select list
      this.companies = result.optionsDataSources.CompanySelectList;
      this.statuses = result.optionsDataSources.StatusSelectList;
      
      // Initialize tom select dropdown
      this.cdRef.detectChanges();

      // Initialize tom select dropdown
      this.cdRef.detectChanges();
      setTimeout(() => {
        
        $("#companyId").select2({ width: "100%" });
        $("#companyId").val(this.employeeUpdateModel.companyId.toString()).trigger("change");

        $("#statusId").select2({ width: "100%" });
        $("#statusId").val(this.employeeUpdateModel.statusId.toString()).trigger("change");
      }, 10);

      this.spinnerService.hide();
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Employee cannot found! Please, try again.");
    });
  }

  // Check employee update from is valid or not
  private getEmployeeUpdateFromValidResult(): boolean {
    if (this.employeeUpdateModel.fullName == undefined || this.employeeUpdateModel.fullName == null || this.employeeUpdateModel.fullName == "") {
      this.toastrService.warning("Please, provied full name.", "Warning");
      return false;
    } else if (this.employeeUpdateModel.phoneNumber == undefined || this.employeeUpdateModel.phoneNumber == null || this.employeeUpdateModel.phoneNumber == "") {
      this.toastrService.warning("Please, provied phone number.", "Warning");
      return false;
    } else if (this.employeeUpdateModel.companyId == undefined || this.employeeUpdateModel.companyId == null || this.employeeUpdateModel.companyId == -1) {
      this.toastrService.warning("Please, provied company.", "Warning");
      return false;
    } else if (this.employeeUpdateModel.statusId == undefined || this.employeeUpdateModel.statusId == null || this.employeeUpdateModel.statusId == -1) {
      this.toastrService.warning("Please, provied status.", "Warning");
      return false;
    } else {
      return true;
    }
  }

  // Update employee
  onClickUpdateEmployee(): void {
  
    // Check employee from valid or not
    let isValidEmployeeFrom: boolean = this.getEmployeeUpdateFromValidResult();

    if (isValidEmployeeFrom) {
      this.spinnerService.show();

      this.employeeService.update(this.employeeUpdateModel).subscribe((result: EmployeeUpdateModel) => {
        this.spinnerService.hide();
        this.toastrService.success("Employee update successful.", "Success");
        return this.router.navigateByUrl("/app/employees");
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error("Employee update failed. Please, try again.", "Error");
      });
    }
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const selectedFile = input.files[0];
    }
  }

  onClickUpdateImage(event: { file: File; entityId: number; entityType?: string }): void {

    interface FileParameter {
      data: Blob; 
      fileName: string;
    }

    const employeeId = event.entityId;
    const selectedFile: File = event.file; // Assuming this is your File object

    // Convert File to FileParameter
    const fileParameter: FileParameter = {
      data: selectedFile, // Assign the file as data
      fileName: selectedFile.name // Use the file name
    };

    this.spinnerService.show();
    this.employeeService.employeeProfileImageUpdate(employeeId, fileParameter).subscribe((result: any) => {
        this.spinnerService.hide();
        this.toastrService.success("Image updated.", "Success.");
        this.router.navigateByUrl(`app/employees`);
      },
      () => {
        this.spinnerService.hide();
        this.toastrService.error("Image update failed! Please, try again.", "Error.");
      }
    );
  }
}