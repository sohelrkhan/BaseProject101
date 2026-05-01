import { CommonModule, isPlatformBrowser } from "@angular/common";
import { AfterViewInit, ChangeDetectorRef, Component, Inject, OnDestroy, OnInit, PLATFORM_ID } from "@angular/core";
import { ReportRegistryGridModel, ReportRegistryService, ReportRegistryViewModel } from "../../../../../../../../api/base-api";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import { ToastrService } from "ngx-toastr";
import { AccessControlService } from "../../../../../../../../identity/services/access-control.service";
import { CheckPermissionDirective } from "../../../../../../../../identity/directive/check-permission.directive";
import { ConfirmModalComponent } from "../../../../../../../shared/confirm-modal/confirm-modal.component";
import { ReportRegistryUpdateComponent } from "../report-registry-update/report-registry-update.component";
import { SelectModel } from "../../../../../../../shared/models/select-model";
declare var $: any;

@Component({
  selector: "app-report-registry-list",
  standalone: true,
  imports: [CommonModule, CheckPermissionDirective, ConfirmModalComponent, ReportRegistryUpdateComponent, NgxSpinnerModule],
  templateUrl: "./report-registry-list.component.html",
  styleUrl: "./report-registry-list.component.scss",
  providers: [ReportRegistryService]
})

export class ReportRegistryListComponent implements OnInit, AfterViewInit, OnDestroy {
  
  // Data table related property
  isBrowser: boolean = false;
  isTableReady: boolean = false;
  show: boolean = false;
  dataTable: any;

  // Select List
  modules: SelectModel[] = [];
  reportGroups: SelectModel[] = [];

  reportRegistries: ReportRegistryGridModel[] = [];
  deleteReportRegistryId: number | undefined;
  updateReportRegistryId: number = 0;

  // Default report registry id
  private _defaultReportRegistryId: number = 1;
  private _selectedModuleId: number = -1;
  private _selectedReportGroupName: string = "-1";

  constructor(private reportRegistryService: ReportRegistryService, private spinnerService: NgxSpinnerService, private toastrService: ToastrService, 
    private accessControlService: AccessControlService, private cdr: ChangeDetectorRef, @Inject(PLATFORM_ID) private platformId: object) { }

  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);

    if(this.isBrowser) {
      this.accessControlService.setPermissions();

      // Get report registry by id
      this.getReportRegistryById(this._defaultReportRegistryId);

      // Get all report registries
      this.getReportRegistries(this._selectedModuleId, this._selectedReportGroupName);
    }    
  }

  ngOnDestroy(): void {
    if (this.dataTable) {
      this.dataTable.destroy();
    }
  }

  ngAfterViewInit(): void {

    // Initialize select 2 dropdown
    this.initializeSelect2Dropdown();
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

        // On change report group name
        this.onChangeModuleName();
      }, 0);
    }
  }

  // On change module
  private onChangeModule(): void {
    const module = $("#moduleId");
    module.select2();

    module.on("change", () => {
      let moduleId: number = Number(module.val());  
      this._selectedModuleId = moduleId;
      this.getReportRegistries(this._selectedModuleId, this._selectedReportGroupName);
      this.cdr.detectChanges();
    });
  }

  // On change report group name
  private onChangeModuleName(): void {
    const reportGroup = $("#reportGroupName");
    reportGroup.select2();

    reportGroup.on("change", () => {
      let reportGroupName: string = String(reportGroup.val());  
      this._selectedReportGroupName = reportGroupName;
      this.getReportRegistries(this._selectedModuleId, this._selectedReportGroupName);
      this.cdr.detectChanges();
    });
  }

  // Get report registry by id
  private getReportRegistryById(reportRegistryId: number): void {
    this.spinnerService.show();
    this.reportRegistryService.getById(reportRegistryId).subscribe((result: ReportRegistryViewModel) => {

      // Get select lists
      this.modules = result.optionsDataSources.ModuleSelectList;
      this.reportGroups = result.optionsDataSources.ReportGroupSelectList;
      this.spinnerService.hide();
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Failed to load select list. Please try again.", "Error");
    });
  }

  // When report registry updated
  onReportRegistryUpdated(): void {
    this.getReportRegistries(this._selectedModuleId, this._selectedReportGroupName);
  }

  // Get all report registry
  private getReportRegistries(moduleId: number, reportGroupName: string): void {
    this.spinnerService.show();
    this.reportRegistryService.getReportRegisterByModuleAndReportGroup(moduleId, reportGroupName).subscribe((result: ReportRegistryGridModel[]) => {
      this.reportRegistries = result || [];
      this.isTableReady = false;

      setTimeout(() => {
        this.isTableReady = true;

        setTimeout(() => {
          this.refreshDataTable();
        }, 0);
      });
      
      this.spinnerService.hide();
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Failed to load report registries. Please try again.", "Error");
    });
  }

  onClickDeleteReportRegistryModal(reportRegistryId: number): void {
    this.deleteReportRegistryId = reportRegistryId;
  }

  onClickUpdateReportRegistryModal(reportRegistryId: number): void {
    this.updateReportRegistryId = reportRegistryId;
  }

  // On click delete Report Registry
  onClickDeleteReportRegistry(): void {
    this.spinnerService.show();
    this.reportRegistryService.delete(this.deleteReportRegistryId).subscribe((result: boolean) => {
        this.spinnerService.hide();
        this.toastrService.success("Delete successful.", "Success");
        this.getReportRegistries(this._selectedModuleId, this._selectedReportGroupName);
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error("Report registry cannot be deleted! Please, try again.", "Error.");
      }
    );
  }

  // Refresh data table
  private refreshDataTable(): void {
    const tableElement = ($("#example") as any);

    if (!tableElement.length) {
      return;
    }

    if (this.dataTable) {
      this.dataTable.destroy(true);
      this.dataTable = null;
    }

    this.dataTable = tableElement.DataTable({
      responsive: true,
      pageLength: 50
    });
  }
}