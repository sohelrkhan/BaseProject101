import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import {
  ModuleService,
  ModuleUpdateModel,
  ModuleViewModel
} from "../../../../../../../api/base-api";
import { SelectModel } from "../../../../../../shared/models/select-model";
import { ToastrService } from "ngx-toastr";
import { CheckPermissionDirective } from "../../../../../../../identity/directive/check-permission.directive";
import { AccessControlService } from "../../../../../../../identity/services/access-control.service";
@Component({
  selector: "app-module-update",
  templateUrl: "./module-update.component.html",
  styleUrls: ["./module-update.component.css"],
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
export class ModuleUpdateComponent implements OnInit {
  // Module id
  private _id: string | undefined;

  // Select list
  statuses: SelectModel[] = [];

  // Module update model
  moduleUpdateModel: ModuleUpdateModel = new ModuleUpdateModel();

  constructor(
    private moduleService: ModuleService,
    private spinnerService: NgxSpinnerService,
    private toastrService: ToastrService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private accessControlService: AccessControlService
  ) {}

  ngOnInit() {
    this.accessControlService.setPermissions();

    // Get module id
    this.getModuleIdByUrl();
  }

  // Get module id by url
  private getModuleIdByUrl(): void {
    this.activatedRoute.params.subscribe((params) => {
      this._id = params["recordId"];
    });

    // Get module by id
    if (this._id != undefined || this._id != null || this._id != "") {
      this.getModuleById();
    }
  }

  // Get module by id
  private getModuleById(): void {
    this.spinnerService.show();
    this.moduleService.getById(this._id!).subscribe(
      (result: ModuleViewModel) => {
        this.moduleUpdateModel = result.updateModel;
        this.statuses = result.optionsDataSources.StatusSelectList;
        this.spinnerService.hide();
      },
      (error: any) => {
        this.spinnerService.hide();
        // this.toastrService.error("Module cannot found! Please, try again.");
      }
    );
  }

  // Update module
  onClickUpdateModule(): void {
    // Check module from valid or not
    let isValidModuleFrom: boolean = this.getModuleFromValidResult();

    if (isValidModuleFrom) {
      this.spinnerService.show();
      this.moduleService.update(this.moduleUpdateModel).subscribe(
        (result: ModuleUpdateModel) => {
          this.spinnerService.hide();
          this.toastrService.success("Module update successfull.", "Success");
          return this.router.navigateByUrl("/app/modules");
        },
        (error: any) => {
          this.spinnerService.hide();
        }
      );
    }
  }

  // Check module create from is valid or not
  private getModuleFromValidResult(): boolean {
    if (
      this.moduleUpdateModel.name == undefined ||
      this.moduleUpdateModel.name == null ||
      this.moduleUpdateModel.name == ""
    ) {
      this.toastrService.warning("Please, provied module name.", "Warning");
      return false;
    } else if (
      this.moduleUpdateModel.statusId == undefined ||
      this.moduleUpdateModel.statusId == null ||
      this.moduleUpdateModel.statusId == -1
    ) {
      this.toastrService.warning("Please, select status.", "Warning");
      return false;
    } else if (
      this.moduleUpdateModel.code == undefined ||
      this.moduleUpdateModel.code == null ||
      this.moduleUpdateModel.code == ""
    ) {
      this.toastrService.warning("Please, provide unique code.", "Warning");
      return false;
    } else {
      return true;
    }
  }
}
