import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { ChangeDetectorRef, Component, ElementRef, OnInit, ViewChild } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Router, RouterLink } from "@angular/router";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import { ActionGridModel, ActionService, ActionViewModel, FeatureActionMappingCreateModel, FeatureActionMappingGridModel, FeatureActionMappingService, FeatureService, FeaturesViewModel } from "../../../../../../../../api/base-api";
import { SelectModel } from "../../../../../../../shared/models/select-model";
import { ToastrService } from "ngx-toastr";
import TomSelect from "tom-select/base";

@Component({
  selector: "app-feature-action-mapping-create",
  templateUrl: "./feature-action-mapping-create.component.html",
  styleUrls: ["./feature-action-mapping-create.component.css"],
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, HttpClientModule, NgxSpinnerModule, RouterLink, CommonModule],
  providers: [FeatureActionMappingService, FeatureService, ActionService]
})

export class FeatureActionMappingCreateComponent implements OnInit {

  // Feature create model
  featureActionMappingCreateModel: FeatureActionMappingCreateModel =new FeatureActionMappingCreateModel();
  featureActionMappingGridModel: FeatureActionMappingGridModel = new FeatureActionMappingGridModel();

  // Select list
  features: SelectModel[] = [];
  actions: SelectModel[] = [];
  selectedActions: ActionGridModel[] = [];

  @ViewChild("createFeatureId") createFeatureId: ElementRef;

  constructor(
    private featureService: FeatureService,
    private actionService: ActionService,
    private featureActionMappingService: FeatureActionMappingService,
    private spinnerService: NgxSpinnerService,
    private toastrService: ToastrService,
    private router: Router,
    private cdRef: ChangeDetectorRef
  ) {}

  ngOnInit() {
    // On page load, default set feature id = -1
    this.featureActionMappingCreateModel.featureId = -1;
    this.featureActionMappingCreateModel.actionIdList = [];

    // Get select list
    this.getSelectList();
    this.getActionSelectList();
  }

  private getActionSelectList(): void {
    this.actionService.getSelectListAction().subscribe(
      (result: ActionViewModel) => {
        this.actions = result.optionsDataSources.ActionSelectList;
        this.spinnerService.hide();
      },
      (error: any) => {
        this.spinnerService.hide();
      }
    );
  }

  private getSelectList(): void {
    this.spinnerService.show();
    this.featureService.getSelectListFeature().subscribe(
      (result: FeaturesViewModel) => {
        this.features = result.optionsDataSources.FeatureSelectList;

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

  onChangeFeature(event: any): void {
    if (event != -1) {
      this.getActionSelectList();
      this.featureActionMappingCreateModel.featureId = event;
      this.featureActionMappingService.getFeatureWiseActionById(event).subscribe(
        (result: FeatureActionMappingGridModel) => {
          this.selectedActions = result.actionList;
          this.spinnerService.hide();

          if (this.selectedActions.length > 0) {
            // Extract IDs from selectedAction for easy lookup
            const selectedIds = new Set(this.selectedActions.map((action) => action.id));

            // Update the actions array
            this.actions = this.actions.map((action) =>
              selectedIds.has(action.id)
                ? { ...action, isDefault: true } // Update the status
                : action
            );
            //
            this.featureActionMappingCreateModel.actionIdList = [];
            for (let i = 0; i < this.actions.length; i++) {
              if (this.actions[i].isDefault) {
                this.featureActionMappingCreateModel.actionIdList.push(this.actions[i].id); // Push only objects with status: true
              }
            }
          } else {
            this.actions = [];
            this.getActionSelectList();
          }
        },
        (error: any) => {
          this.spinnerService.hide();
        }
      );
    } else {
      this.actions = [];
      this.getActionSelectList();
    }
  }

  onClickAddAction(event, actionId: number): void {
    if (event.target.checked) {
      this.featureActionMappingCreateModel.actionIdList.push(actionId);
    } else {
      this.featureActionMappingCreateModel.actionIdList =
        this.featureActionMappingCreateModel.actionIdList.filter((item) => item !== actionId);
    }
  }

  // Create feature action mapping
  onClickCreateFeatureActionMapping(): void {
    // Check feature create from valid or not
    let isValidFeatureActionCreateFrom: boolean = this.getFeatureActionFromValidResult();

    if (isValidFeatureActionCreateFrom) {
      this.spinnerService.show();
      this.featureActionMappingService.create(this.featureActionMappingCreateModel).subscribe(
        (result: FeatureActionMappingCreateModel) => {
          this.spinnerService.hide();
          this.toastrService.success("Feature Action Mapped successfull.", "Success");
          this.features = [];
          this.actions = [];
          this.getSelectList();
          this.getActionSelectList();
          this.featureActionMappingCreateModel.actionIdList = [];
          this.featureActionMappingCreateModel.featureId = -1;
          this.featureActionMappingCreateModel = new FeatureActionMappingCreateModel();
          // return this.router.navigateByUrl("/app/features");
        },
        (error: any) => {
          this.spinnerService.hide();
        }
      );
    }
  }

  // Check features action mapping create from is valid or not
  private getFeatureActionFromValidResult(): boolean {
    if (
      this.featureActionMappingCreateModel.featureId == undefined ||
      this.featureActionMappingCreateModel.featureId == null ||
      this.featureActionMappingCreateModel.featureId == -1
    ) {
      this.toastrService.warning("Please, select feature.", "Warning");
      return false;
    } else if (this.featureActionMappingCreateModel.actionIdList.length == 0) {
      this.toastrService.warning("Please, select action.", "Warning");
      return false;
    } else {
      return true;
    }
  }

  private initializeTomSelect(): void {
    if (this.createFeatureId && this.createFeatureId.nativeElement.tomselect) {
      this.createFeatureId.nativeElement.tomselect.destroy();
    }

    new TomSelect(this.createFeatureId.nativeElement, {
      placeholder: "Choose a Feature",
      allowEmptyOption: true,
      create: false,
      plugins: ["remove_button"]
    });
  }
}
