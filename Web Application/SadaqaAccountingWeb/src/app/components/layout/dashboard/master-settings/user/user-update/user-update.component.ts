import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import { UserService, UserUpdateModel, UserViewModel } from "../../../../../../../api/base-api";
import { ToastrService } from "ngx-toastr";
import { CheckPermissionDirective } from "../../../../../../../identity/directive/check-permission.directive";
import { AccessControlService } from "../../../../../../../identity/services/access-control.service";

@Component({
  selector: "app-user-update",
  templateUrl: "./user-update.component.html",
  styleUrls: ["./user-update.component.css"],
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, HttpClientModule, NgxSpinnerModule, RouterLink, CommonModule, CheckPermissionDirective],
  providers: [UserService]
})

export class UserUpdateComponent implements OnInit {

  // Tom select property
  private _tomSelectInitCount: number = 0;
  maxInitializeDropdown: number = 1;

  // User id
  private _id: string | undefined;

  // User update model
  userUpdateModel: UserUpdateModel = new UserUpdateModel();

  constructor(private userService: UserService, private spinnerService: NgxSpinnerService, private toastrService: ToastrService,
    private router: Router, private activatedRoute: ActivatedRoute, private accessControlService: AccessControlService) { }

  ngOnInit() {

    this.accessControlService.setPermissions();

    // Get user id
    this.getUserIdByUrl();
  }

  // Get user id by url
  private getUserIdByUrl(): void {
    this.activatedRoute.params.subscribe((params) => {
      this._id = params["recordId"];

      // Get module by id
      if (this._id != undefined || this._id != null || this._id != "") {
        this.getUserById();
      }
    });
  }

  // Get user by id
  private getUserById(): void {
    this.spinnerService.show();
    this.userService.getById(this._id!).subscribe((result: UserViewModel) => {
      this.userUpdateModel = result.userUpdateModel;
      this.spinnerService.hide();
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("User cannot found! Please, try again.");
    });
  }

  // Update user
  onClickUpdateUser(): void {

    // Check user from valid or not
    let isValidUserFrom: boolean = this.getUserFromValidResult();

    if (isValidUserFrom) {
      this.spinnerService.show();
      this.userService.update(this.userUpdateModel).subscribe((result: UserUpdateModel) => {
        this.spinnerService.hide();
        this.toastrService.success("User update successful.", "Success");
        return this.router.navigateByUrl("/app/users");
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error("User update failed! Please, try again.", "Error");
      });
    }
  }

  // Check user update from is valid or not
  private getUserFromValidResult(): boolean {
    if (this.userUpdateModel.email == undefined || this.userUpdateModel.email == null || this.userUpdateModel.email == "") {
      this.toastrService.warning("Please, provied valid email.", "Warning");
      return false;
    } else if (this.userUpdateModel.fullName == undefined || this.userUpdateModel.fullName == null || this.userUpdateModel.fullName == "") {
      this.toastrService.warning("Please, provide fullname.", "Warning");
      return false;
    } else {
      return true;
    }
  }
}