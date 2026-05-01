import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Router, RouterLink } from "@angular/router";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import {
  ModuleCreateModel,
  ModuleService,
  ModuleViewModel
} from "../../../../../../../api/base-api";
import { ToastrService } from "ngx-toastr";
import { SelectModel } from "../../../../../../shared/models/select-model";
import { CheckPermissionDirective } from "../../../../../../../identity/directive/check-permission.directive";
import { AccessControlService } from "../../../../../../../identity/services/access-control.service";

@Component({
  selector: "app-module-create",
  templateUrl: "./module-create.component.html",
  styleUrls: ["./module-create.component.css"],
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule,
    NgxSpinnerModule,
    RouterLink,
    CommonModule,
    CheckPermissionDirective
  ],
  providers: [ModuleService]
})
export class ModuleCreateComponent implements OnInit {
  // Default module id
  private id: string = "-1";

  // Module create model
  moduleCreateModel: ModuleCreateModel = new ModuleCreateModel();

  // Select list
  statuses: SelectModel[] = [];

  constructor(
    private moduleService: ModuleService,
    private spinnerService: NgxSpinnerService,
    private toastrService: ToastrService,
    private router: Router,
    private accessControlService: AccessControlService
  ) {}

  ngOnInit() {
    this.accessControlService.setPermissions();

    // Get select list
    this.getSelectList();
  }

  private getSelectList(): void {
    this.spinnerService.show();
    this.moduleService.getById(this.id).subscribe(
      (result: ModuleViewModel) => {
        this.statuses = result.optionsDataSources.StatusSelectList;
        this.spinnerService.hide();
      },
      (error: any) => {
        this.spinnerService.hide();
      }
    );
  }

  // Check module create from is valid or not
  private getModuleFromValidResult(): boolean {
    if (
      this.moduleCreateModel.name == undefined ||
      this.moduleCreateModel.name == null ||
      this.moduleCreateModel.name == ""
    ) {
      this.toastrService.warning("Please, provied module name.", "Warning");
      return false;
    } else if (
      this.moduleCreateModel.code == undefined ||
      this.moduleCreateModel.code == null ||
      this.moduleCreateModel.code == ""
    ) {
      this.toastrService.warning("Please, provide unique code.", "Warning");
      return false;
    } else {
      return true;
    }
  }

  // Create module
  onClickCreateModule(): void {
    // Check module create from valid or not
    let isValidModuleCreateFrom: boolean = this.getModuleFromValidResult();

    if (isValidModuleCreateFrom) {
      this.spinnerService.show();
      this.moduleService.create(this.moduleCreateModel).subscribe(
        (result: ModuleCreateModel) => {
          this.spinnerService.hide();
          this.toastrService.success("Module create successfull.", "Success");
          return this.router.navigateByUrl("/app/modules");
        },
        (error: any) => {
          this.spinnerService.hide();
        }
      );
    }
  }
}
