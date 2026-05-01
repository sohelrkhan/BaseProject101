import { CommonModule, isPlatformBrowser } from "@angular/common";
import { AfterViewInit, ChangeDetectorRef, Component, Inject, OnInit, PLATFORM_ID } from "@angular/core";
import { Router, RouterLink, RouterLinkActive } from "@angular/router";
import { AccessControlService } from "../../../../identity/services/access-control.service";
import { IdentityService } from "../../../../identity/identity-shared/identity.service";
import { UserModel } from "../../../../api/base-api";
import { SidebarPermissionService } from "../../../../identity/services/sidebar-permission.service";

@Component({
  selector: "app-sidebar",
  templateUrl: "./sidebar.component.html",
  styleUrls: ["./sidebar.component.css"],
  standalone: true,
  imports: [RouterLink, CommonModule, RouterLinkActive],
  providers: []
})

export class SidebarComponent implements OnInit, AfterViewInit {

  expandedModule: string | null = null;

  private _isBrowser: boolean = false;
  submenuStates: { [key: string]: boolean } = {};

  // Login user info
  loginUserInfo: UserModel = new UserModel();

  constructor(public accessControlService: AccessControlService, private router: Router, private cdr: ChangeDetectorRef, @Inject(PLATFORM_ID) private platformId: Object,
    private identityService: IdentityService, public sidebarPermissionService: SidebarPermissionService) { }

  ngOnInit(): void {
    this._isBrowser = isPlatformBrowser(this.platformId);

    if (this._isBrowser) {
      this.expandedModule = "administration";

      this.accessControlService.setPermissions();
      this.getLoginUserInfo();
      this.cdr.detectChanges();
    }
  }

  ngAfterViewInit(): void { }

  toggleSubmenu(key: string): void {
    this.submenuStates[key] = !this.submenuStates[key];
  }

  isSubmenuOpen(key: string): boolean {
    return this.submenuStates[key] || false;
  }

  // Get login user info
  private getLoginUserInfo(): void {
    this.identityService.getLoginInfo().subscribe((result: UserModel) => {
      this.loginUserInfo = result;
      this.sidebarPermissionService.initialize(this.loginUserInfo);
      this.cdr.detectChanges();
    });
  }

  toggleModule(moduleName: string): void {
    if (this.expandedModule === moduleName) {
      this.expandedModule = null;
    } else {
      this.expandedModule = moduleName;
    }
  }
  isModuleExpanded(moduleName: string): boolean {
    return this.expandedModule === moduleName;
  }
}