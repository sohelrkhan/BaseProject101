import { CommonModule } from "@angular/common";
import { Component, Input, OnInit } from "@angular/core";
import { AbstractControl } from "@angular/forms";

@Component({
  selector: "app-form-error",
  templateUrl: "./form-error.component.html",
  styleUrls: ["./form-error.component.css"],
  standalone: true,
  imports: [CommonModule],
  providers: []
})
export class FormErrorComponent implements OnInit {
  @Input() control!: AbstractControl | null;
  @Input() label = "This field";

  constructor() {}

  ngOnInit() {}

  getErrors(): string[] {
    if (!this.control || !this.control.errors) return [];

    const messages: string[] = [];
    const errors = this.control.errors;

    for (const errorName in errors) {
      if (errorName === "required") {
        messages.push(`${this.label} is required.`);
      } else if (errorName === "min") {
        messages.push(`Minimum value is ${errors["min"].min}.`);
      } else if (errorName === "max") {
        messages.push(`Maximum value is ${errors["max"].max}.`);
      } else if (errorName === "maxlength") {
        messages.push(`Maximum length is ${errors["maxlength"].requiredLength}.`);
      } else if (errorName === "minlength") {
        messages.push(`Minimum length is ${errors["minlength"].requiredLength}.`);
      } else if (errorName === "pattern") {
        messages.push(`${this.label} format is invalid.`);
      } else {
        messages.push(`${this.label} is invalid.`);
      }
    }

    return messages;
  }
}
