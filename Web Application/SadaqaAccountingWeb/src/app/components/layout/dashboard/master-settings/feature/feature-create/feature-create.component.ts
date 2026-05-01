import { CommonModule, Location } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Router, RouterLink } from "@angular/router";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import {
  FeaturesCreateModel,
  FeatureService,
  FeaturesViewModel
} from "../../../../../../../api/base-api";
import { SelectModel } from "../../../../../../shared/models/select-model";
import { ToastrService } from "ngx-toastr";
import { CheckPermissionDirective } from "../../../../../../../identity/directive/check-permission.directive";
import { AccessControlService } from "../../../../../../../identity/services/access-control.service";
import TomSelect from 'tom-select';

@Component({
  selector: "app-feature-create",
  templateUrl: "./feature-create.component.html",
  styleUrls: ["./feature-create.component.css"],
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
export class FeatureCreateComponent implements OnInit {
  // Default feature id
  private id: string = "-1";

  // Feature create model
  featureCreateModel: FeaturesCreateModel = new FeaturesCreateModel();

  // Select list
  modules: SelectModel[] = [];
  statuses: SelectModel[] = [];

  @ViewChild("moduleSelect") moduleSelect: ElementRef;

  constructor(
    private featureService: FeatureService,
    private spinnerService: NgxSpinnerService,
    private toastrService: ToastrService,
    private router: Router,
    private accessControlService: AccessControlService,
    private location: Location,
    private cdRef: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.accessControlService.setPermissions();

    // On page load, default set module id and status id = -1
    this.featureCreateModel.moduleId = -1;

    // Get select list
    this.getSelectList();
  }

  private getSelectList(): void {
    this.spinnerService.show();
    this.featureService.getById(this.id).subscribe(
      (result: FeaturesViewModel) => {
        this.modules = result.optionsDataSources.ModuleSelectList;
        this.statuses = result.optionsDataSources.StatusSelectList;
        
        this.cdRef.detectChanges();
        setTimeout(() => {
          this.initializeTomSelect();
        }, 200);
        
        this.spinnerService.hide();
      },
      (error: any) => {
        this.spinnerService.hide();
      }
    );
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

  // Create feature
  onClickCreateFeature(): void {
    // Check feature create from valid or not
    let isValidFeatureCreateFrom: boolean = this.getFeatureFromValidResult();

    if (isValidFeatureCreateFrom) {

      // Check if module id == -1 then set module id == null
      this.featureCreateModel.moduleId = this.featureCreateModel.moduleId == -1 ? null : this.featureCreateModel.moduleId;

      this.spinnerService.show();
      this.featureService.create(this.featureCreateModel).subscribe(
        (result: FeaturesCreateModel) => {
          this.spinnerService.hide();
          this.toastrService.success("Feature create successful.", "Success");
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
      this.featureCreateModel.name == undefined ||
      this.featureCreateModel.name == null ||
      this.featureCreateModel.name == ""
    ) {
      this.toastrService.warning("Please, provied feature name.", "Warning");
      return false;
    } else if (
      this.featureCreateModel.moduleId == undefined ||
      this.featureCreateModel.moduleId == null ||
      this.featureCreateModel.moduleId == -1
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
