import { Component, Input, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { Location } from "@angular/common";

@Component({
  selector: "app-back-button",
  templateUrl: "./back-button.component.html",
  styleUrls: ["./back-button.component.css"],
  standalone: true,
  providers: [],
  imports: []
})
export class BackButtonComponent implements OnInit {
  @Input() label: string = "Back";
  @Input() fallbackRoute: string = "/";

  constructor(
    private location: Location,
    private router: Router
  ) {}

  ngOnInit() {}

  goBack(): void {
    if (window.history.length > 1) {
      this.location.back();
    } else {
      this.router.navigate([this.fallbackRoute]);
    }
  }
}
