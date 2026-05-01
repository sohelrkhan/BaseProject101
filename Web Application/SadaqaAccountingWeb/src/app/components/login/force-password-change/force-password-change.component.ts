import { Component, OnInit } from "@angular/core";
import {
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
  ReactiveFormsModule
} from "@angular/forms";
import { AuthenticationService, ChangePassword, UserModel } from "../../../../api/base-api";
import { Router } from "@angular/router";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import { CommonModule } from "@angular/common";
import { IdentityService } from "../../../../identity/identity-shared/identity.service";
import { CustomTosterServiceService } from "../../../shared/Toster/CustomTosterService.service";

@Component({
  selector: "app-force-password-change",
  templateUrl: "./force-password-change.component.html",
  styleUrls: ["./force-password-change.component.css"],
  standalone: true,
  imports: [ReactiveFormsModule, NgxSpinnerModule, CommonModule]
})
export class ForcePasswordChangeComponent implements OnInit {
  form!: FormGroup;
  loginUserInfo: UserModel | null = null;
  currentYear: number = new Date().getFullYear();

  showPassword = false;
  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }

  constructor(
    private fb: FormBuilder,
    private identityService: IdentityService,
    private authenticationService: AuthenticationService,
    private router: Router,
    private toastrService: CustomTosterServiceService,
    private spinnerService: NgxSpinnerService
  ) {}

  ngOnInit() {
    this.form = this.fb.group(
      {
        password: ["", [Validators.required, Validators.minLength(5), Validators.maxLength(50)]],
        confirmPassword: ["", [Validators.required]]
      },
      { validators: this.passwordsMatchValidator }
    );

    this.identityService.getLoginInfo().subscribe((result: UserModel) => {
      this.loginUserInfo = result;
    });
  }

  // Custom Validator to check password match
  passwordsMatchValidator(group: AbstractControl): { [key: string]: boolean } | null {
    const password = group.get("password")?.value;
    const confirmPassword = group.get("confirmPassword")?.value;
    return password === confirmPassword ? null : { passwordMismatch: true };
  }

  onSubmit() {
    if (this.form.valid && this.loginUserInfo) {
      const model = new ChangePassword();
      model.email = this.loginUserInfo.email;
      model.password = this.form.value.password;
      model.confirmPassword = this.form.value.confirmPassword;
      model.forcePasswordChanged = true;

      this.spinnerService.show();
      this.authenticationService.chnagePassword(model).subscribe({
        next: () => {
          this.toastrService.success("You can now  Use Your new  Password", "Changed Password");
          this.spinnerService.hide();
          this.router.navigateByUrl("/login");
        },
        error: (err) => {
          this.toastrService.error("Set valid Password", "Password can not be changed");
          this.spinnerService.hide();
        }
      });
    } else {
      this.spinnerService.hide();
      this.form.markAllAsTouched();
    }
  }

  disableCopy(event: ClipboardEvent): void {
    event.preventDefault();
  }

  disablePaste(event: ClipboardEvent): void {
    event.preventDefault();
  }

  disableCut(event: ClipboardEvent): void {
    event.preventDefault();
  }
}
