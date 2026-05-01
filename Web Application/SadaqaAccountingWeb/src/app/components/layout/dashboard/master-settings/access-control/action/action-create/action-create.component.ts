import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Router, RouterLink } from "@angular/router";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import { ActionCreateModel, ActionService, } from "../../../../../../../../api/base-api";
import { SelectModel } from "../../../../../../../shared/models/select-model";
import { CustomTosterServiceService } from "../../../../../../../shared/Toster/CustomTosterService.service";

@Component({
  selector: "app-action-create",
  templateUrl: "./action-create.component.html",
  styleUrls: ["./action-create.component.css"],
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, HttpClientModule, NgxSpinnerModule, RouterLink, CommonModule],
  providers: [ActionService]
})

export class ActionCreateComponent implements OnInit {

  // Action create model
  actionCreateModel: ActionCreateModel = new ActionCreateModel();

  // Select list
  statuses: SelectModel[] = [];

  constructor(private actionService: ActionService, private spinnerService: NgxSpinnerService, private toastrService: CustomTosterServiceService, private router: Router) { }

  ngOnInit() { }

  // Create action
  onClickCreateAction(): void {

    // Check action create from valid or not
    let isValidActionCreateFrom: boolean = this.getActionFromValidResult();

    if (isValidActionCreateFrom) {
      this.spinnerService.show();
      this.actionService.create(this.actionCreateModel).subscribe((result: ActionCreateModel) => {
        this.spinnerService.hide();
        this.toastrService.success("Action create successful.", "Success");
        return this.router.navigateByUrl("/app/actions");
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error("Action create failed.", "Error");
      });
    }
  }

  // Check action create from is valid or not
  private getActionFromValidResult(): boolean {
    if (this.actionCreateModel.name == undefined || this.actionCreateModel.name == null || this.actionCreateModel.name == "") {
      this.toastrService.warning("Please, provied action name.", "Warning");
      return false;
    } else {
      return true;
    }
  }
}