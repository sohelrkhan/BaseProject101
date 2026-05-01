import { CommonModule, Location } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import {
  FeatureService,
  FeaturesUpdateModel,
  FeaturesViewModel
} from "../../../../../../../api/base-api";
import { SelectModel } from "../../../../../../shared/models/select-model";
import { ToastrService } from "ngx-toastr";
import { CheckPermissionDirective } from "../../../../../../../identity/directive/check-permission.directive";
import { AccessControlService } from "../../../../../../../identity/services/access-control.service";
import TomSelect from "tom-select";

@Component({
  selector: "app-feature-update",
  templateUrl: "./feature-update.component.html",
  styleUrls: ["./feature-update.component.css"],
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
  providers: [FeatureService]
})
export class FeatureUpdateComponent implements OnInit {
  // Feature id
  private _id: string | undefined;

  // Select list
  modules: SelectModel[] = [];
  statuses: SelectModel[] = [];

  // Feature update model
  featureUpdateModel: FeaturesUpdateModel = new FeaturesUpdateModel();

  @ViewChild("moduleSelect") moduleSelect: ElementRef;

  constructor(
    private featureService: FeatureService,
    private spinnerService: NgxSpinnerService,
    private toastrService: ToastrService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private location: Location,
    private accessControlService: AccessControlService,
    private cdRef: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.accessControlService.setPermissions();

    // Get feature id
    this.getFeatureIdByUrl();
  }

  // Get feature id by url
  private getFeatureIdByUrl(): void {
    this.activatedRoute.params.subscribe((params) => {
      this._id = params["recordId"];
    });

    // Get feature by id
    if (this._id != undefined || this._id != null || this._id != "") {
      this.getFeatureById();
    }
  }

  // Get feature by id
  private getFeatureById(): void {
    this.spinnerService.show();
    this.featureService.getById(this._id!).subscribe((result: FeaturesViewModel) => {
      this.featureUpdateModel = result.updateModel;
      this.modules = result.optionsDataSources.ModuleSelectList;
      this.statuses = result.optionsDataSources.StatusSelectList;

      this.cdRef.detectChanges();
      setTimeout(() => {
        this.initializeTomSelect();
      }, 200);

      this.spinnerService.hide();
    });
  }

  private initializeTomSelect(): void {
    if (this.moduleSelect && this.moduleSelect.nativeElement.tomselect) {
      this.moduleSelect.nativeElement.tomselect.destroy();
    }

    new TomSelect(this.moduleSelect.nativeElement, {
      placeholder: "Choose a Module",
      allowEmptyOption: true,
      create: false,
      plugins: ["remove_button"]
    });
  }

  // Update feature
  onClickUpdateFeature(): void {
    // Check feature from valid or not
    let isValidFeatureFrom: boolean = this.getFeatureFromValidResult();

    if (isValidFeatureFrom) {
      this.spinnerService.show();
      this.featureService.update(this.featureUpdateModel).subscribe(
        (result: FeaturesViewModel) => {
          this.spinnerService.hide();
          this.toastrService.success("Feature update successful.", "Success");
          return this.location.back();
        },
        (error: any) => {
          this.spinnerService.hide();
        }
      );
    }
  }

  // Check features create from is valid or not
  private getFeatureFromValidResult(): boolean {
    if (
      this.featureUpdateModel.name == undefined ||
      this.featureUpdateModel.name == null ||
      this.featureUpdateModel.name == ""
    ) {
      this.toastrService.warning("Please, provied feature name.", "Warning");
      return false;
    } else if (
      this.featureUpdateModel.statusId == undefined ||
      this.featureUpdateModel.statusId == null ||
      this.featureUpdateModel.statusId == -1
    ) {
      this.toastrService.warning("Please, select status.", "Warning");
      return false;
    } else if (
      this.featureUpdateModel.moduleId == undefined ||
      this.featureUpdateModel.moduleId == null ||
      this.featureUpdateModel.moduleId == -1
    ) {
      this.toastrService.warning("Please, select module.", "Warning");
      return false;
    } else {
      return true;
    }
  }

  onClickBack(): void {
    return this.location.back();
  }
}
