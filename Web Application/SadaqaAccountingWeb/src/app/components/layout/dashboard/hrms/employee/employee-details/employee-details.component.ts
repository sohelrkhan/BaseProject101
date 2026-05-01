import { CommonModule, isPlatformBrowser } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { AfterViewInit, ChangeDetectorRef, Component, Inject, OnDestroy, OnInit, PLATFORM_ID } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import { EmployeeImageUpdateModel, EmployeeService, EmployeeUpdateModel, EmployeeViewModel } from "../../../../../../../api/base-api";
import { OrdinalDatePipe } from "../../../../../../shared/pipe/ordinaldate.pipe";
import { SelectModel } from "../../../../../../shared/models/select-model";
import { environment } from "../../../../../../../environments/environment";
import { ImageUploadComponent } from "../../../../../shared-components/image-upload/image-upload.component";
import { CheckPermissionDirective } from "../../../../../../../identity/directive/check-permission.directive";
import { AccessControlService } from "../../../../../../../identity/services/access-control.service";
import { CustomTosterServiceService } from "../../../../../../shared/Toster/CustomTosterService.service";
declare var $: any;

@Component({
  selector: "app-employee-details",
  templateUrl: "./employee-details.component.html",
  styleUrls: ["./employee-details.component.css"],
  standalone: true,
  imports: [OrdinalDatePipe, NgxSpinnerModule, ReactiveFormsModule, FormsModule, HttpClientModule, CommonModule, ImageUploadComponent, CheckPermissionDirective, RouterLink],
  providers: [EmployeeService]
})

export class EmployeeDetailsComponent implements OnInit, AfterViewInit, OnDestroy {
  isBrowser: boolean = false;

  // Application base url
  applicationBaseUrl: string | undefined;

  // Default id
  employeeId: string | undefined;

  // Select list
  genders: SelectModel[] = [];
  bloodGroups: SelectModel[] = [];
  maritalStatuses: SelectModel[] = [];
  statuses: SelectModel[] = [];

  // All models
  employeeUpdateModel: EmployeeUpdateModel = new EmployeeUpdateModel();

  constructor(
    private employeeService: EmployeeService,
    private activatedRoute: ActivatedRoute,
    private spinnerService: NgxSpinnerService,
    private toastrService: CustomTosterServiceService,
    @Inject(PLATFORM_ID) private platformId: object,
    private accessControlService: AccessControlService,
    private cdRef: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);

    if (this.isBrowser) {
      // Get application base url
      this.applicationBaseUrl = environment.coreBaseUrl;

      this.accessControlService.setPermissions();
      this.getEmployeeIdByUrl();
    }
  }

  ngOnDestroy() {}

  ngAfterViewInit(): void {
    this.initializeSelect2DropdownForUpdate();
  }

  // Initialize select 2 dropdown for update
  private initializeSelect2DropdownForUpdate(): void {
    if (this.isBrowser) {
      setTimeout(() => {
        ($(".select2") as any).select2({
          placeholder: "Choose...",
          width: "100%"
        });

        // On change status
        this.onChangeStatus();
      }, 0);
    }
  }

  // On change status
  private onChangeStatus(): void {
    const updateStatus = $("#updateStatusId") as any;
    updateStatus.select2();

    updateStatus.on("change", () => {
      let updateStatusId: number = Number(updateStatus.val());
      this.employeeUpdateModel.statusId = updateStatusId;
      this.cdRef.detectChanges();
    });
  }

  // Get employee id by url
  private getEmployeeIdByUrl(): void {
    this.activatedRoute.params.subscribe((params) => {
      this.employeeId = params["recordId"];

      if (this.employeeId) { }
    });
  }

  onClickOpenEmployeeUpdateModal(): void {
    this.getEmployeeById(this.employeeId);
  }

  // Get employee by employee id
  private getEmployeeById(employeeId: string): void {
    this.spinnerService.show();
    this.employeeService.getById(employeeId).subscribe((result: EmployeeViewModel) => {
      this.employeeUpdateModel = result.updateModel;

      // Get select list
      this.genders = result.optionsDataSources.GenderSelectList;
      this.genders = this.genders.filter((g) => g.id != 30);
      this.bloodGroups = result.optionsDataSources.BloodGroupSelectList;
      this.maritalStatuses = result.optionsDataSources.MaritalStatusSelectList;
      this.statuses = result.optionsDataSources.StatusSelectList;

      // Initialize tom select dropdown
      this.cdRef.detectChanges();
      ($("#updateStatusId") as any).val(this.employeeUpdateModel.statusId);

      this.spinnerService.hide();
    });
  }

  // Get employee update from validation result
  private getUpdateEmployeeFromValidationResult(): boolean {
    if (
      this.employeeUpdateModel.fullName == undefined ||
      this.employeeUpdateModel.fullName == null ||
      this.employeeUpdateModel.fullName == ""
    ) {
      this.toastrService.warning("Please, provied employee full name.");
      return false;
    } else {
      return true;
    }
  }

  // Update employee
  onClickUpdateEmployee(): void {
    let getUpdateFromValidResult: boolean = this.getUpdateEmployeeFromValidationResult();
    if (getUpdateFromValidResult) {
      this.spinnerService.show();
      this.employeeService.update(this.employeeUpdateModel).subscribe(
        (result: EmployeeUpdateModel) => {
          this.spinnerService.hide();
          this.toastrService.success("Employee update.", "Success");
        },
        (error: any) => {
          this.spinnerService.hide();
          this.toastrService.error("Employee cannot create! Please, try again.", "Error");
        }
      );
    }
  }

  onClickUpdateProfileImage(event: { file: File; entityId: number; entityType?: string }): void {
    interface FileParameter {
      data: Blob;
      fileName: string;
    }

    const employeeId = event.entityId;
    const selectedFile: File = event.file;

    const fileParameter: FileParameter = {
      data: selectedFile,
      fileName: selectedFile.name
    };

    this.spinnerService.show();
    this.employeeService.employeeProfileImageUpdate(employeeId, fileParameter).subscribe(
      (result: EmployeeImageUpdateModel) => {
        this.spinnerService.hide();
        this.toastrService.success("Profile image updated.", "Success.");
      },
      () => {
        this.spinnerService.hide();
      }
    );
  }

  // Copy office email
  copyToClipboard(text: string | undefined): void {
    if (!text) {
      return;
    }

    navigator.clipboard
      .writeText(text)
      .then(() => {
        this.toastrService.info("Office email copied to clipboard!", "Information");
      })
      .catch((err) => {
        this.toastrService.error("Office email cannot copied to clipboard!", "Error");
      });
  }
}