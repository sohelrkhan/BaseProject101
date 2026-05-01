import { CommonModule, isPlatformBrowser } from "@angular/common";
import { Component, Inject, OnInit, PLATFORM_ID } from "@angular/core";
import { RouterLink } from "@angular/router";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import { FeatureService, FeaturesGridModel } from "../../../../../../../api/base-api";
import { ToastrService } from "ngx-toastr";
import { AccessControlService } from "../../../../../../../identity/services/access-control.service";
import { FeatureWorkflowSettingComponent } from "../feature-workflow-setting/feature-workflow-setting.component";
import { CheckPermissionDirective } from "../../../../../../../identity/directive/check-permission.directive";
import { FormsModule } from "@angular/forms";
import { CustomTosterServiceService } from "../../../../../../shared/Toster/CustomTosterService.service";

@Component({
  selector: "app-feature-list",
  templateUrl: "./feature-list.component.html",
  styleUrls: ["./feature-list.component.css"],
  standalone: true,
  imports: [
    RouterLink,
    NgxSpinnerModule,
    CommonModule,
    FeatureWorkflowSettingComponent,
    CheckPermissionDirective,
    FormsModule
  ],
  providers: [FeatureService]
})
export class FeatureListComponent implements OnInit {
  isBrowser = false;
  allFeatures: any[] = [];

  // Feature data source
  features: FeaturesGridModel[] = [];
  filterFeatures: FeaturesGridModel[] = [];

  // Store DataTable instance
  dataTable: any;

  isCreateWorkflowProcess: boolean = false;
  featureName: string | undefined;

  constructor(
    private featureService: FeatureService,
    private spinnerService: NgxSpinnerService,
    private toastrService: CustomTosterServiceService,
    private accessControlService: AccessControlService,
    @Inject(PLATFORM_ID) private platformId: object
  ) {}

  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);

    if (this.isBrowser) {
      this.accessControlService.setPermissions();
    }
  }

  ngAfterViewInit() {}

  ngOnDestroy() {
    if (this.dataTable) {
      this.dataTable.destroy();
    }
  }

  selectedFeatureId: number | null = null;

  toggleWorkflowSettings(featureId: number) {
    this.selectedFeatureId = this.selectedFeatureId === featureId ? null : featureId;
  }


  // On click delete feature
  onClickDeleteFeature(featureId: string): void {
    this.spinnerService.show();
    this.featureService.delete(featureId).subscribe(
      (result: boolean) => {
        this.spinnerService.hide();
        this.toastrService.success("Delete successful.", "Success");

        // Remove feature from the array
        this.features = this.features.filter((feature) => feature.encryptedId !== featureId);

        // Refresh DataTable after deletion
        this.refreshDataTable();
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error("Feature cannot be deleted! Please, try again.", "Error.");
      }
    );
  }

  private refreshDataTable(): void {
    if (this.dataTable) {
      this.dataTable.destroy(); // Destroy previous instance
    }

    setTimeout(() => {
      if (this.features.length > 0) {
        // Initialize only if data exists
        this.dataTable = $("#featureTable").DataTable();
      }
    }, 100);
  }

  cancel(): void {}

  onChangeFeatureName(event: any): void {
    const searchTerm = this.featureName?.trim().toLowerCase();

    if (searchTerm) {
      this.filterFeatures = this.features.filter((f) => f.name.toLowerCase().includes(searchTerm));
    } else {
      this.filterFeatures = [...this.features]; // shallow copy (safe)
    }
  }
}
