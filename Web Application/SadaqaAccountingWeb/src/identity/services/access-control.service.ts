import { Injectable, signal, computed } from "@angular/core";
import { NgxSpinnerService } from "ngx-spinner";
import { IdentityService } from "../identity-shared/identity.service";
import { UserAccessMappingDetails, UserModel } from "../../api/base-api";

@Injectable({
  providedIn: "root"
})
export class AccessControlService {
  private _userAccessMappingDetails: UserAccessMappingDetails[] = [];

  employeeListAccess = signal<boolean>(false);

  constructor(
    private spinnerService: NgxSpinnerService,
    private identityService: IdentityService
  ) {}

  /**
   * Loads permissions if not already loaded.
   */
  setPermissions(): Promise<void> {
    if (this._userAccessMappingDetails && this._userAccessMappingDetails.length > 0) {
      // Already loaded
      return Promise.resolve();
    }

    return new Promise((resolve) => {
      this.getLoginUserAccessInfo(resolve);
    });
  }

  /**
   * Checks if the user has access to a feature with a specific action or list of actions.
   */
  hasAccess(feature: string, action: string | string[]): boolean {
    if (!this._userAccessMappingDetails || this._userAccessMappingDetails.length === 0) {
      return false;
    }

    if (Array.isArray(action)) {
      return action.some((act) =>
        this._userAccessMappingDetails.some(
          (p) => p.featureName === feature && p.actionName === act
        )
      );
    }

    return this._userAccessMappingDetails.some(
      (p) => p.featureName === feature && p.actionName === action
    );
  }

  /**
   * Loads the access mapping details from API and updates signals.
   */
  private getLoginUserAccessInfo(callback?: () => void): void {
    this.spinnerService.show();

    this.identityService.getLoginInfo().subscribe(
      (result: UserModel) => {
        this.spinnerService.hide();
        this._userAccessMappingDetails = result.userAccessMappingDetails;

        const hasEmployeeList = this.hasAccess("Employee", "List");
        this.employeeListAccess.set(hasEmployeeList);
        if (callback) callback?.();
      },
      (error: any) => {
        this.spinnerService.hide();
        this.employeeListAccess.set(false); // fail-safe
        if (callback) callback?.();
      }
    );
  }
}
