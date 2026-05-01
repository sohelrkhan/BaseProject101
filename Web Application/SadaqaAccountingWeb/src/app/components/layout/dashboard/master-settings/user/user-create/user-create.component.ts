import { CommonModule, isPlatformBrowser } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { AfterViewInit, ChangeDetectorRef, Component, Inject, OnInit, PLATFORM_ID } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Router, RouterLink } from "@angular/router";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import { UserAccessMappingService, UserCreateModel, UserService, UserViewModel } from "../../../../../../../api/base-api";
import { ToastrService } from "ngx-toastr";
import { CheckPermissionDirective } from "../../../../../../../identity/directive/check-permission.directive";
import { AccessControlService } from "../../../../../../../identity/services/access-control.service";
import { SelectModel } from "../../../../../../shared/models/select-model";
declare var $: any;

@Component({
  selector: "app-user-create",
  templateUrl: "./user-create.component.html",
  styleUrls: ["./user-create.component.css"],
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, HttpClientModule, NgxSpinnerModule, RouterLink, CommonModule, CheckPermissionDirective],
  providers: [UserService, UserAccessMappingService]
})

export class UserCreateComponent implements OnInit, AfterViewInit {

  // User type
  userType: string = "Employee";

  isBrowser: boolean = false;

  hide: boolean = true;
  showPassword() {
    this.hide = !this.hide;
  }

  password: string = "";
  onClickGeneratePassword() {
    this.password = Math.random().toString(36).slice(-8);
  }

  // Default user id
  private id: string = "-1";

  // User create model
  userCreateModel: UserCreateModel = new UserCreateModel();

  // Select list
  companies: SelectModel[] = [];
  employees: SelectModel[] = [];

  // Selected id
  private _selectedCompanyId: number | undefined;
  private _selectedEmployeeId: number | undefined;

  constructor(private userService: UserService, private spinnerService: NgxSpinnerService, private toastrService: ToastrService, private router: Router,
    @Inject(PLATFORM_ID) private platformId: object, private cdRef: ChangeDetectorRef, private accessControlService: AccessControlService) { }

  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);

    if(this.isBrowser) {
      this.accessControlService.setPermissions();

      // On page load, default set status id = -1
      this.userCreateModel.employeeId = -1;

      // Get company select list
      this.getCompanySelectList();
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

        // On change company
        this.onChangeCompany();      

        // On change employee
        this.onChangeEmployee();
      }, 0);
    }
  }

  // On change company
  private onChangeCompany(): void {
    const company = ($("#companyId") as any);
    company.select2();

    company.on("change", () => {
      let companyId: number = Number(company.val());

      if (companyId != -1 || companyId != undefined || companyId != null) {

        // Get department by company
        this._selectedCompanyId = companyId;
      }

      this.cdRef.detectChanges();
    });
  }

  // On change employee
  private onChangeEmployee(): void {
    const employee = ($("#employeeId") as any);
    employee.select2();

    employee.on("change", () => {
      let employeeId: number = Number(employee.val());

      if (employeeId != -1 || employeeId != undefined || employeeId != null) {
        this._selectedEmployeeId = employeeId;
      }

      this.cdRef.detectChanges();
    });
  }


  // Get company select list
  private getCompanySelectList(): void {
    this.spinnerService.show();
    this.userService.getById(this.id).subscribe((result: UserViewModel) => {
      this.companies = result.optionsDataSources.CompanySelectList;

      // Initialize tom select dropdown
      this.cdRef.detectChanges();
      ($("#companyId") as any).val(this.companies.find(c => c.isDefault).id);
      this._selectedCompanyId = this.companies.find(c => c.isDefault).id;

      this.spinnerService.hide();
      return;
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Company select list cannot load! Please, try again.", "Error");
      return;
    });
  }

  // Create user
  onClickCreateUser(): void {
    this.userCreateModel.employeeId = this._selectedEmployeeId;

    // Check user create from valid or not
    let isValidUserCreateFrom: boolean = this.getUserFromValidResult();

    this.userCreateModel.employeeId = this.userCreateModel.employeeId == -1 ? null : this.userCreateModel.employeeId;

    if (isValidUserCreateFrom) {
      this.spinnerService.show();
      this.userService.create(this.userCreateModel).subscribe((result: boolean) => {
        this.spinnerService.hide();
        this.toastrService.success("User create successful.", "Success");
        return this.router.navigateByUrl("/app/users");
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error("User cannot create! Please, try again.", "Error");
      });
    }
  }

  // Check user create from is valid or not
  private getUserFromValidResult(): boolean {
    if (this.userCreateModel.email == undefined || this.userCreateModel.email == null || this.userCreateModel.email == "") {
      this.toastrService.warning("Please, provied valid email.", "Warning");
      return false;
    } else if (this.userCreateModel.fullName == undefined || this.userCreateModel.fullName == null || this.userCreateModel.fullName == "") {
      this.toastrService.warning("Please, provide fullname.", "Warning");
      return false;
    } else if (this.userCreateModel.password == undefined || this.userCreateModel.password == null || this.userCreateModel.password == "") {
      this.toastrService.warning("Please, provide strong password.", "Warning");
      return false;
    } else {
      return true;
    }
  }
}