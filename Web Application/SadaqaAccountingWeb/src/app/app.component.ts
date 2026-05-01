import { Component } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { AccessControlService } from "../identity/services/access-control.service";
import { ToastComponent } from "./shared/Toster/toast/toast.component";

@Component({
  selector: "app-root",
  standalone: true,
  imports: [RouterOutlet, ToastComponent],
  templateUrl: "./app.component.html",
  styleUrl: "./app.component.scss"
})
export class AppComponent {
  title = "SadaqaAccountingWeb";

  constructor(private accessControlService: AccessControlService) {
    this.accessControlService.setPermissions();
  }
}
