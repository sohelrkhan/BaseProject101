import { CommonModule, isPlatformBrowser } from "@angular/common";
import { AfterViewInit, ChangeDetectorRef, Component, Inject, OnDestroy, OnInit, PLATFORM_ID } from "@angular/core";
import { RouterLink } from "@angular/router";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import { CheckPermissionDirective } from "../../../../../../../identity/directive/check-permission.directive";
import { FormsModule } from "@angular/forms";
import { FeatureService, FeaturesGridModel, FeaturesViewModel } from "../../../../../../../api/base-api";
import { ToastrService } from "ngx-toastr";
import { AccessControlService } from "../../../../../../../identity/services/access-control.service";
import { SelectModel } from "../../../../../../shared/models/select-model";

@Component({
  selector: "app-feature-list-global",
  templateUrl: "./feature-list-global.component.html",
  styleUrls: ["./feature-list-global.component.css"],
  standalone: true,
  imports: [RouterLink, NgxSpinnerModule, CommonModule, CheckPermissionDirective, FormsModule],
  providers: [FeatureService]
})

export class FeatureListGlobalComponent implements OnInit, AfterViewInit, OnDestroy {

  // Data table related property
  isBrowser: boolean = false;
  isTableReady: boolean = false;
  show: boolean = false;
  dataTable: any;

  // feature data source
  features: FeaturesGridModel[] = [];

  // Select list
  modules: SelectModel[] = [];

  // feature id
  private _featureEncryptDeleteId: string | undefined;
  private _defaultFeatureId: string = "-1";

  constructor(private featureService: FeatureService, private spinnerService: NgxSpinnerService, private toastrService: ToastrService,
    @Inject(PLATFORM_ID) private platformId: object, private cdr: ChangeDetectorRef, private accessControlService: AccessControlService, private cdRef: ChangeDetectorRef) { }

  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);

    if (this.isBrowser) {
      this.accessControlService.setPermissions();

      // Get feature by id
      this.getFeatureById();
    }
  }

  ngAfterViewInit() { 
    // Initialize select 2 dropdown
    this.initializeSelect2Dropdown();
  }

  ngOnDestroy(): void {
    if (this.dataTable) {
      this.dataTable.destroy();
    }
  }

  // refresh data table
  private refreshDataTable(): void {
    const tableElement = $("#example");

    if (!tableElement.length) {
      return;
    }

    // If this.dataTable already exists, destroy it directly
    if (this.dataTable) {
      this.dataTable.destroy(true);
      this.dataTable = null;
    }

    this.dataTable = tableElement.DataTable({
      responsive: true,
      pageLength: 50 // Change your value
    });
  }

  // Initialize select 2 dropdown
  private initializeSelect2Dropdown(): void {
    if (this.isBrowser) {
      setTimeout(() => {
        ($(".select2") as any).select2({
          placeholder: "Choose...",
          width: "100%"
        });

        // On change module
        this.onChangeModule();
      }, 0);
    }
  }

  // On change module
  onChangeModule(): void {
    const module = $("#moduleId");
    module.select2();

    module.on("change", () => {
      let moduleId: number = Number(module.val());
      
      // Set filter dataset
      this.getFeatures();
      this.cdRef.detectChanges();
    });
  }


  private getFeatureById(): void {
    this.spinnerService.show();
    this.featureService.getById(this._defaultFeatureId).subscribe((result: FeaturesViewModel) => {
      // Get select list
      this.modules = result.optionsDataSources.ModuleSelectList;

      // Get features
      this.getFeatures();

      this.spinnerService.hide();
      return;
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Select list cannot load! Please, try again.", "Error.");
      return;
    })
  }

  // Get features
  getFeatures(): void {
    this.spinnerService.show();
    this.featureService.getAll().subscribe((result: FeaturesGridModel[]) => {
      this.features = result || [];

      this.isTableReady = false;
      setTimeout(() => {
        this.isTableReady = true;

        setTimeout(() => {
          this.refreshDataTable();
        }, 100);
      });

      this.spinnerService.hide();
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Feature cannot load! Please, try again.", "Error");
    });
  }

  // On click open delete modal
  onClickOpenDeleteModal(encryptedId: string): void {
    this._featureEncryptDeleteId = encryptedId;
  }

  // On click delete
  onClickDelete(): void {
    this.spinnerService.show();
    this.featureService.delete(this._featureEncryptDeleteId).subscribe((result: boolean) => {
      this.spinnerService.hide();
      this.closeDeleteModal();
      this.getFeatures();
      this.toastrService.success("Feature deleted.", "Success.");
    });
  }

  // Close delete modal
  private closeDeleteModal(): void {
    // Manually close the modal
    const deleteModal = document.getElementById("delete_feature_modal");
    if (deleteModal) {
      deleteModal.classList.remove("show"); // Hide modal visually
      deleteModal.setAttribute("aria-hidden", "true");
      deleteModal.style.display = "none"; // Hide modal

      // Remove modal backdrop
      const modalBackdrop = document.querySelector(".modal-backdrop");
      if (modalBackdrop) {
        modalBackdrop.remove();
      }

      // Restore body scroll (important!)
      document.body.classList.remove("modal-open");
      document.body.style.overflow = "";
      document.body.style.paddingRight = "";
    }
  }
}