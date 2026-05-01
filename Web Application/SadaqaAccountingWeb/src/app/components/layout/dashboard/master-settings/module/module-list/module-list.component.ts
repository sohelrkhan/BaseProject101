import { CommonModule, isPlatformBrowser } from "@angular/common";
import { AfterViewInit, Component, Inject, OnDestroy, OnInit, PLATFORM_ID } from "@angular/core";
import { RouterLink } from "@angular/router";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import { ModuleGridModel, ModuleService } from "../../../../../../../api/base-api";
import { ToastrService } from "ngx-toastr";
import { CheckPermissionDirective } from "../../../../../../../identity/directive/check-permission.directive";
import { AccessControlService } from "../../../../../../../identity/services/access-control.service";

@Component({
  selector: "app-module-list",
  templateUrl: "./module-list.component.html",
  styleUrls: ["./module-list.component.css"],
  standalone: true,
  imports: [RouterLink, NgxSpinnerModule, CommonModule, CheckPermissionDirective],
  providers: [ModuleService]
})

export class ModuleListComponent implements OnInit, AfterViewInit, OnDestroy {
  // Module data source
  modules: ModuleGridModel[] = [];
 
  // Data table related property
  isBrowser: boolean = false;
  isTableReady: boolean = false;
  show: boolean = false;
  dataTable: any;

  // Delete module id
  private _deleteModuleId: number | undefined

  constructor(private moduleService: ModuleService, private spinnerService: NgxSpinnerService, private toastrService: ToastrService, 
    private accessControlService: AccessControlService, @Inject(PLATFORM_ID) private platformId: object) { }

  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);

    if(this.isBrowser) {
      this.accessControlService.setPermissions();
      this.getModules();
    }
  }

  ngAfterViewInit() { }

  ngOnDestroy() {
    if (this.dataTable) {
      this.dataTable.destroy();
    }
  }

  // Get all Module
  private getModules(): void {
    this.spinnerService.show();
    this.moduleService.getAll().subscribe((result: ModuleGridModel[]) => {
      this.modules = result || [];
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
    });
  }

  // On click open delete modal
  onClickOpenDeleteModal(moduleId: number): void {
    this._deleteModuleId = moduleId;
  }

  // On click delete module
  onClickDeleteModule(): void {
    this.spinnerService.show();
    this.moduleService.delete(this._deleteModuleId).subscribe(
      (result: boolean) => {
        this.spinnerService.hide();
        this.toastrService.success("Delete successful.", "Success", { closeButton: true });
        this.getModules();
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error("Module cannot be deleted! Please, try again.", "Error.", {
          closeButton: true
        });
      }
    );
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
      pageLength: 50
    });
  }

  cancel(): void { }
}