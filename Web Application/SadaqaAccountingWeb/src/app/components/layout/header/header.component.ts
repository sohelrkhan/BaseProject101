import { Component, Inject, OnInit, PLATFORM_ID } from "@angular/core";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import { IdentityService } from "../../../../identity/identity-shared/identity.service";
import { Router, RouterLink } from "@angular/router";
import { AccountUnitService, AccountUnitViewModel, NotificationGridModel, NotificationService, UserModel } from "../../../../api/base-api";
import { CommonModule, isPlatformBrowser } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { SingalRService } from "../../../shared/services/singalR-service";
import { environment } from "../../../../environments/environment";
import { CustomTosterServiceService } from "../../../shared/Toster/CustomTosterService.service";

@Component({
  selector: "app-header",
  templateUrl: "./header.component.html",
  styleUrls: ["./header.component.css"],
  standalone: true,
  imports: [NgxSpinnerModule, RouterLink, CommonModule, FormsModule],
  providers: [AccountUnitService, NotificationService]
})

export class HeaderComponent implements OnInit {

  private _isBrowser: boolean = false;

  // Login user information
  loginUserInfo: UserModel = new UserModel();

  // Unread notification
  unreadNotifications: NotificationGridModel[] = [];

  // Login user account unit name
  accountUnitName: string | undefined;

  // Application base url
  applicationBaseUrl: string | undefined;
  mobileSearchQuery: string = "";

  constructor(private accountUnitService: AccountUnitService, private spinnerService: NgxSpinnerService, private identityService: IdentityService, private toastrService: CustomTosterServiceService, 
    private router: Router, private notificationService: NotificationService, private singalRService: SingalRService, @Inject(PLATFORM_ID) private platformId: object) { }

  ngOnInit() {
    if (typeof window !== "undefined") {
      this._isBrowser = isPlatformBrowser(this.platformId);

      if (this._isBrowser) {
        this.applicationBaseUrl = environment.coreBaseUrl;
        this.getLoginUserInfo();
      }
    }
  }

  async onClickLogout(event: Event): Promise<void> {
    this.spinnerService.hide();
    let getLoginResult: boolean = await this.identityService.logout();

    if (getLoginResult) {
      this.toastrService.success("Logout Success.", "Success");
      event.preventDefault();
      this.router.navigateByUrl("/login");
    } else {
      this.spinnerService.hide();
      this.toastrService.error("Logout cannot Success.! Please, try again.", "Wrong");
      return;
    }
  }

  // Get login user info
  private getLoginUserInfo(): void {
    this.identityService.getLoginInfo().subscribe((result: UserModel) => {
      this.loginUserInfo = result;

      // Get login user account unit
      if(this.loginUserInfo.accountUnitId != undefined && this.loginUserInfo.accountUnitId != null && this.loginUserInfo.accountUnitId > 0) {
        this.getLoginUserAccountUnit(this.loginUserInfo.accountUnitId);
      }

      this.getLoginUserUnreadNotification();
      this.singalRService.startConnection();

      this.singalRService.onNotificationReceived(() => {
        this.getLoginUserUnreadNotification();
      });
    });
  }

  // Get login user account unit
  private getLoginUserAccountUnit(accountUnitId: number): void {
    this.spinnerService.show();
    this.accountUnitService.getById(accountUnitId).subscribe((result: AccountUnitViewModel) => {
      this.accountUnitName = result.updateModel.name;
      this.spinnerService.hide();
      return;
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Login user account unit is not found yet! Please, try again.", "Error");
      return;
    })
  }

  // Get login user unread notification
  private getLoginUserUnreadNotification(): void {
    if (!this.loginUserInfo?.employeeId) {
      return;
    }

    this.spinnerService.show();
    this.notificationService.getAllUnreadNotificationByReceiverId(this.loginUserInfo.employeeId.toString()).subscribe((result: NotificationGridModel[]) => {
        this.unreadNotifications = result;
        this.spinnerService.hide();
      },
      (error: any) => {
        this.spinnerService.hide();
      }
    );
  }

  // On click read all login user notification
  onClickReadAllNotification(): void {
    this.spinnerService.show();
    this.notificationService.readAllUnreadNotificationByReceiverId(this.loginUserInfo.employeeId.toString()).subscribe((result: boolean) => {
      this.getLoginUserUnreadNotification();
      this.spinnerService.hide();
      return;
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("All notifications is not read yet! Please, try again.", "Error");
      return;
    });
  }

  // Sidebar toggle
  toggleSidebar(): void {

    // Add your sidebar toggle logic here
    const sidebarElement = document.querySelector(".sidebar");

    if (sidebarElement) {
      sidebarElement.classList.toggle("collapsed");
    }
  }

  // Fullscreen toggle
  toggleFullscreen(): void {
    if (!document.fullscreenElement) {
      document.documentElement.requestFullscreen().catch((err) => { });
    } else {
      document.exitFullscreen();
    }
  }

  onClickReadNotification(notificationId: string): void { }

  trackNotification(index: number, notification: any): any {
    return notification.id;
  }

  // User methods
  getUserAvatarUrl(): string {
    if (this.loginUserInfo?.image) {
      return this.applicationBaseUrl + this.loginUserInfo.image;
    }

    return "assets/img/profiles/avatar-12.jpg";
  }

  getUserDisplayName(): string {
    return this.loginUserInfo?.fullName || "Guest User";
  }

  getUserEmail(): string {
    return this.loginUserInfo?.email || "guest@example.com";
  }

  getEmployeeId(): string {
    return this.loginUserInfo?.employeeEncryptedId || "";
  }
}