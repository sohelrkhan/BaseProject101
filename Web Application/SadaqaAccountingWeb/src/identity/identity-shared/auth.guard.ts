import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from "@angular/router";
import { IdentityService } from "./identity.service";
import { Injectable } from "@angular/core";
import { map, Observable } from "rxjs";
import { ToastrService } from "ngx-toastr";

@Injectable({
  providedIn: "root"
})

export class AuthGuard implements CanActivate {
  constructor(
    private router: Router,
    private identityService: IdentityService,
    private tasterService: ToastrService
  ) {}

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.identityService.getLoginInfo().pipe(
      map((user) => {
        if (user != null) {
          return true;
        } else {
          this.tasterService.warning("Please, login first.", "Warning");
          this.router.navigateByUrl("/login");
          return false;
        }
      })
    );
  }
}
