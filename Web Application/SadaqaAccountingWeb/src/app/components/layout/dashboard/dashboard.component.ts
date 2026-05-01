import { ChangeDetectorRef, Component, Inject, OnInit, PLATFORM_ID } from "@angular/core";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import { AccessControlService } from "../../../../identity/services/access-control.service";
import { isPlatformBrowser } from "@angular/common";

@Component({
  selector: "app-dashboard",
  templateUrl: "./dashboard.component.html",
  styleUrls: ["./dashboard.component.css"],
  standalone: true, 
  imports: [NgxSpinnerModule]
})

export class DashboardComponent implements OnInit {
 
  private _isBrowser: boolean = false;

  constructor(private spinnerService: NgxSpinnerService, private accessControlService: AccessControlService, private cdr: ChangeDetectorRef, @Inject(PLATFORM_ID) private platformId: Object,) { }

   ngOnInit(): void {
      this._isBrowser = isPlatformBrowser(this.platformId);
  
      if (this._isBrowser) {
        this.accessControlService.setPermissions();
        this.cdr.detectChanges();
      }
    }
}
