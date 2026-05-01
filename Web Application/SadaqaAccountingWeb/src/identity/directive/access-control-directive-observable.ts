import { Injectable, signal, computed, effect, inject } from "@angular/core";
import { NgxSpinnerService } from "ngx-spinner";
import { IdentityService } from "../identity-shared/identity.service";
import { UserAccessMappingDetails, UserModel } from "../../api/base-api";
import { catchError, tap } from "rxjs/operators";
import { Observable, of } from "rxjs";

@Injectable({
  providedIn: "root"
})
export class AccessControlServiceObservable {
  private _permissionsLoaded = signal(false);

  readonly permissionsLoaded = computed(() => this._permissionsLoaded());

  // Using inject() for DI
  private identityService = inject(IdentityService);
  private spinner = inject(NgxSpinnerService);

  constructor() {
    // Effect with allowSignalWrites enabled
    effect(
      () => {
        const perms = this._userAccessMappingDetails();
        // Writing to a signal inside an effect (allowed now)
        this._permissionsLoaded.set(perms.length > 0);
      },
      { allowSignalWrites: true }
    );
  }

  /**
   * Load permissions from server once
   */
  private readonly _userAccessMappingDetails = signal<UserAccessMappingDetails[]>([]);

  readonly hasEmployeeListAccess = computed(() =>
    this._userAccessMappingDetails().some(
      (x) => x.featureName === "Employee" && x.actionName === "List"
    )
  );

  loadPermissions$(): Observable<UserModel> {
    return this.identityService.getLoginInfo().pipe(
      tap((res) => {
        this._userAccessMappingDetails.set(res.userAccessMappingDetails ?? []);
      }),
      catchError((error) => {
        this._userAccessMappingDetails.set([]);
        return of({ userAccessMappingDetails: [] } as UserModel);
      })
    );
  }

  /**
   * Generic access checker
   */
  hasAccess(feature: string, action: string | string[]): boolean {
    const mappings = this._userAccessMappingDetails();

    if (!mappings || mappings.length === 0) return false;

    if (Array.isArray(action)) {
      return action.some((a) =>
        mappings.some((m) => m.featureName === feature && m.actionName === a)
      );
    }

    return mappings.some((m) => m.featureName === feature && m.actionName === action);
  }
}
