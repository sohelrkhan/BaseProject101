import { Component, OnInit } from "@angular/core";
import { LoginModel } from "../../../api/base-api";
import { ReactiveFormsModule, FormsModule } from "@angular/forms";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import { IdentityService } from "../../../identity/identity-shared/identity.service";
import { Router } from "@angular/router";
import { CommonModule } from "@angular/common";
import { CustomTosterServiceService } from "../../shared/Toster/CustomTosterService.service";

@Component({
  selector: "app-login",
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, NgxSpinnerModule, CommonModule],
  templateUrl: "./login.component.html",
  styleUrls: ["./login.component.css"],
  providers: []
})

export class LoginComponent implements OnInit {

  // Login model
  loginModel: LoginModel = new LoginModel();

  // Current Year
  currentYear: number = new Date().getFullYear();

  showPassword: boolean = false;

  constructor(private identityService: IdentityService, private spinnerService: NgxSpinnerService, private toastrService: CustomTosterServiceService, private router: Router) { }

  ngOnInit() { }

  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  async onClickLogin(event: Event): Promise<void> {
    const isLoginFormValid: boolean = this.getLoginFromValidationResult();

    if (isLoginFormValid) {
      this.spinnerService.show();

      // Get the full login result 
      const loginResult = await this.identityService.SignIn(this.loginModel);
      this.spinnerService.hide();

      if (loginResult.isSuccess) {
        if (loginResult.message == "Change Password.") {
          this.toastrService.warning("Password update required. For your security, please choose a new password. Your current one cannot be reused.", "Warning");
          this.router.navigateByUrl("/change-password");
        } else {
          event.preventDefault();
           
          // Check, login user is super admin or user
          if(this.loginModel.email == "super_admin@gmail.com") {
            this.router.navigateByUrl("/app/dashboard");
          } else {
            this.router.navigateByUrl("/set-user-account-unit");
          }
          
          this.toastrService.success("Login Success.", "Success");
        }
      } else {
        this.toastrService.error(loginResult.message ?? "Login failed. Please try again.", "Error");
      }
    }
  }

  // Check login from validation
  private getLoginFromValidationResult(): boolean {
    if (this.loginModel.email == undefined || this.loginModel.email == null || this.loginModel.email == "") {
      this.toastrService.warning("Please, provide valid email address.", "Warning.");
      return false;
    } else if (this.loginModel.password == undefined || this.loginModel.password == null || this.loginModel.password == "") {
      this.toastrService.warning("Please, provide password.", "Warning.");
      return false;
    } else {
      return true;
    }
  }
}