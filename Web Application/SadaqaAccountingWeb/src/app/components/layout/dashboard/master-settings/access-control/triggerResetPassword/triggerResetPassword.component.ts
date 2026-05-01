import { CommonModule } from "@angular/common";
import { Component, ElementRef, OnInit, ViewChild } from "@angular/core";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import { ToastrService } from "ngx-toastr";
import { UserModel, AuthenticationService } from "../../../../../../../api/base-api";
import { FormsModule } from "@angular/forms";
import TomSelect from "tom-select";
import { UserContextService } from "../../../../../../shared/user-context.service";
import { SelectModel } from "../../../../../../shared/models/select-model";

@Component({
  selector: "app-trigger-reset-password",
  templateUrl: "./triggerResetPassword.component.html",
  styleUrls: ["./triggerResetPassword.component.css"],
  standalone: true,
  imports: [FormsModule, NgxSpinnerModule, CommonModule],
  providers: []
})

export class TriggerResetPasswordComponent implements OnInit {
  
  users: UserModel[] = [];
  filteredUsers: UserModel[] = [];
  selectedEmails: string[] = [];
  searchText: string = "";
  allSelected: boolean = false;
  companies: SelectModel[] = [];
  companyId: number = -1;

  @ViewChild("Company") company: ElementRef;

  filterDepartments: SelectModel[] = [];
  private companyTomSelect: any;
  private teamTomSelect: any;
  private departmentSelect: any;
  loginUserInfo: UserModel | null = null;

  constructor(
    private authenticationService: AuthenticationService,
    private toastrService: ToastrService,
    private spinnerService: NgxSpinnerService,
    private userContext: UserContextService
  ) {}

  ngOnInit() {
    this.userContext.user$.subscribe((user) => {
      this.loginUserInfo = user;
      if (user?.companyId) {
        this.companyId = user.companyId;
      }
    });
    this.getAllUsers();
  }

  getAllUsers() {
    this.spinnerService.show();
    this.authenticationService.getAllUsers(this.companyId).subscribe((result: UserModel[]) => {
      this.users = result;
      this.filteredUsers = [...this.users];
      this.spinnerService.hide();
    });
  }

  private initializeTomSelect() {
    // Company Select
    this.companyTomSelect = new TomSelect(this.company.nativeElement, {
      placeholder: "Choose a Company"
    });

    if (this.companyId > 0) {
      this.companyTomSelect.setValue(this.companyId.toString());
    }

  }

  onClickSearch() {
    const selectedCompanyId: number = Number(this.companyTomSelect.getValue());
    const selectedDepartmentIds: number[] = this.departmentSelect.getValue().map(Number);
    this.companyId = selectedCompanyId;
    this.spinnerService.show();
    this.authenticationService.getAllUsers(this.companyId).subscribe((result: UserModel[]) => {
      this.users = result;
      this.filteredUsers = [...this.users];
      this.spinnerService.hide();
    });
  }

  toggleEmail(email: string, event: any) {
    if (event.target.checked) {
      if (!this.selectedEmails.includes(email)) {
        this.selectedEmails.push(email);
      }
    } else {
      this.selectedEmails = this.selectedEmails.filter((e) => e !== email);
    }
  }

  // Select/Deselect All
  toggleSelectAll(event: any) {
    this.allSelected = event.target.checked;
    if (this.allSelected) {
      this.selectedEmails = this.filteredUsers.map((u) => u.email);
    } else {
      this.selectedEmails = [];
    }
  }

  // Search Users
  filterUsers() {
    const text = this.searchText.toLowerCase();
    this.filteredUsers = this.users.filter(
      (u) => u.fullName.toLowerCase().includes(text) || u.email.toLowerCase().includes(text)
    );

    // Update select all checkbox based on filtered users
    this.allSelected =
      this.filteredUsers.length > 0 &&
      this.filteredUsers.every((u) => this.selectedEmails.includes(u.email));
  }

  submitEmails() {
    if (this.selectedEmails.length === 0) {
      this.toastrService.warning("Please select at least one user.");
      return;
    }

    this.spinnerService.show();
    this.authenticationService.resetForceChangePasswords(this.selectedEmails).subscribe(
      (result: boolean) => {
        if (result) {
          this.toastrService.success(
            "Passwords have been successfully reset for selected users.",
            "Success"
          );
          this.getAllUsers();
          this.selectedEmails = [];
          this.allSelected = false;
        } else {
          this.toastrService.error("Failed to reset passwords. Please try again.", "Error");
        }
        this.spinnerService.hide();
      },
      (err) => {
        this.toastrService.error("An unexpected error occurred. Please try again.", "Error");
        this.spinnerService.hide();
      }
    );
  }
}
