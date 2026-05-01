import { ChangeDetectorRef, Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { AccessControlService } from '../../../../identity/services/access-control.service';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { UserAccountUnitGridModel, UserAccountUnitService, UserModel } from '../../../../api/base-api';
import { IdentityService } from '../../../../identity/identity-shared/identity.service';
import { ToastrService } from 'ngx-toastr';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-set-user-account-unit',
  templateUrl: './set-user-account-unit.component.html',
  styleUrls: ['./set-user-account-unit.component.css'],
  standalone: true, 
  imports: [NgxSpinnerModule, CommonModule, FormsModule],
  providers: [UserAccountUnitService]
})

export class SetUserAccountUnitComponent implements OnInit {

  // Select list
  userAccountUnit: UserAccountUnitGridModel = new UserAccountUnitGridModel();

  // Selected on change user account unit id
  selectedUserAccountUnitId: number = -1;
  
  private _isBrowser: boolean = false;

  constructor(
    private userAccountUnitService: UserAccountUnitService, 
    private spinnerService: NgxSpinnerService, 
    private accessControlService: AccessControlService, 
    private cdr: ChangeDetectorRef, 
    @Inject(PLATFORM_ID) private platformId: Object, 
    private identityService: IdentityService, 
    private toastrService: ToastrService, 
    private routerService: Router) 
  { }

  ngOnInit(): void {
    this._isBrowser = isPlatformBrowser(this.platformId);

    if (this._isBrowser) {
      this.accessControlService.setPermissions();

      // Get login user info
      this.getLoginUserInfo();
      this.cdr.detectChanges();
    }
  }

  // Get login user info
  private getLoginUserInfo(): void {
    this.identityService.getLoginInfo().subscribe((result: UserModel) => {
      this.getUserAccountUnit(result.id);
    })
  }

  // Get user account unit list
  private getUserAccountUnit(loginUserId: string): void {
    this.spinnerService.show();
    this.userAccountUnitService.getUserAccountUnit(loginUserId).subscribe((result: UserAccountUnitGridModel) => {
      this.userAccountUnit = result;
      this.spinnerService.hide();
      return;
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("User Account Unit list cannot found based on the login employee! Please, try again.", "Error");
      return;
    })
  }

  onChangeUserAccountUnit(): void {  }

  async onClickContinue(): Promise<void> {
    if (!this.selectedUserAccountUnitId || this.selectedUserAccountUnitId <= 0) {
      this.toastrService.warning("Please select an account unit.", "Warning");
      return;
    }

    const ok = await this.identityService.setLoginUserAccountUnit(this.selectedUserAccountUnitId);
    if (ok) {
      this.toastrService.success("Account unit set.", "Success");
      this.routerService.navigateByUrl("/app/dashboard");
    } else {
      this.toastrService.error("Failed to set account unit.", "Error");
    }
  }
}