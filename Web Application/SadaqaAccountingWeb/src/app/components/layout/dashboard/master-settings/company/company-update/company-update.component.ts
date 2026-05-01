import { CommonModule, isPlatformBrowser } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { AfterViewInit, ChangeDetectorRef, Component, Inject, OnInit, PLATFORM_ID } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import { ToastrService } from "ngx-toastr";
import { CompanyService, CompanyUpdateModel, CompanyViewModel } from "../../../../../../../api/base-api";
import { SelectModel } from "../../../../../../shared/models/select-model";
import { CheckPermissionDirective } from "../../../../../../../identity/directive/check-permission.directive";
import { AccessControlService } from "../../../../../../../identity/services/access-control.service";
import { ImageUploadComponent } from "../../../../../shared-components/image-upload/image-upload.component";
import { environment } from "../../../../../../../environments/environment";
declare var $: any;

@Component({
  selector: "app-company-update",
  templateUrl: "./company-update.component.html",
  styleUrls: ["./company-update.component.css"],
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, HttpClientModule, NgxSpinnerModule, RouterLink, CommonModule, CheckPermissionDirective, ImageUploadComponent],
  providers: [CompanyService]
})

export class CompanyUpdateComponent implements OnInit, AfterViewInit {

  // Data table related property
  isBrowser: boolean = false;

  applicationBaseUrl: string | undefined;

  // Company id
  private _id: string | undefined;

  // Select list
  parentCompanies: SelectModel[] = [];
  statuses: SelectModel[] = [];
  defaultCurrencies: SelectModel[] = [];
  otherCurrencies: SelectModel[] = [];  

  // Company update model
  companyUpdateModel: CompanyUpdateModel = new CompanyUpdateModel();

  constructor(private companyService: CompanyService, private spinnerService: NgxSpinnerService, private toastrService: ToastrService, private router: Router,
    private activatedRoute: ActivatedRoute, private cdRef: ChangeDetectorRef, private accessControlService: AccessControlService, 
    @Inject(PLATFORM_ID) private platformId: object) { }

  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);

    if(this.isBrowser) {

      this.applicationBaseUrl = environment.coreBaseUrl;
      this.accessControlService.setPermissions();

      // Get company id
      this.getCompanyIdByUrl();
    }
  }

  ngAfterViewInit() { 

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

        // On change status
        this.onChangeStatus();
      }, 0);
    }
  }

  // On change status
  private onChangeStatus(): void {
    const status = $("#statusId");
    status.select2();

    status.on("change", () => {
      let statusId: number = Number(status.val());
    
      if(statusId != undefined || statusId != null || statusId != -1) {
        this.companyUpdateModel.statusId = statusId;
      } 
   
      this.cdRef.detectChanges();
    });
  }

  // Get company id by url
  private getCompanyIdByUrl(): void {
    this.activatedRoute.params.subscribe((params) => {
      this._id = params["recordId"];

      // Get company by id
      if (this._id != undefined || this._id != null || this._id! != "") {
        this.getCompanyById();
      }
    });
  }

  // Get company by id
  private getCompanyById(): void {
    this.spinnerService.show();
    this.companyService.getById(this._id).subscribe((result: CompanyViewModel) => {

      // Set company update model
      this.companyUpdateModel = result.updateModel;
        
      // Get select list
      this.parentCompanies = result.optionsDataSources.CompanySelectList;
      this.statuses = result.optionsDataSources.StatusSelectList;
      this.defaultCurrencies = result.optionsDataSources.CurrencySelectList;
      this.otherCurrencies = result.optionsDataSources.CurrencySelectList;
      
      // Initialize tom select dropdown
      this.cdRef.detectChanges();

      // Initialize tom select dropdown
      this.cdRef.detectChanges();
      setTimeout(() => {
        
        $("#statusId").select2({ width: "100%" });
        $("#statusId").val(this.companyUpdateModel.statusId.toString()).trigger("change");
      }, 10);

      this.spinnerService.hide();
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Country cannot found! Please, try again.");
    });
  }

  // Update company
  onClickUpdateCompany(): void {

    // Check company from valid or not
    let isValidCompanyFrom: boolean = this.getCompanyFromValidResult();

    if (isValidCompanyFrom) {
      this.spinnerService.show();

      this.companyService.update(this.companyUpdateModel).subscribe((result: CompanyUpdateModel) => {
        this.spinnerService.hide();
        this.toastrService.success("Company update successful.", "Success");
        return this.router.navigateByUrl("/app/companies");
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error("Company update failed. Please, try again.", "Error");
      });
    }
  }

  // Check company create from is valid or not
  private getCompanyFromValidResult(): boolean {
    if (this.companyUpdateModel.name == undefined || this.companyUpdateModel.name == null || this.companyUpdateModel.name == "") {
      this.toastrService.warning("Please, provied company name.", "Warning");
      return false;
    } else if (this.companyUpdateModel.country == undefined || this.companyUpdateModel.country == null || this.companyUpdateModel.country == "") {
      this.toastrService.warning("Please, provied country name.", "Warning");
      return false;
    } else {
      return true;
    }
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const selectedFile = input.files[0];
    }
  }

  onClickUpdateImage(event: { file: File; entityId: number; entityType?: string }): void {

    interface FileParameter {
      data: Blob; // Or a suitable type based on your requirements
      fileName: string;
    }

    const compnayId = event.entityId;
    const selectedFile: File = event.file; // Assuming this is your File object

    // Convert File to FileParameter
    const fileParameter: FileParameter = {
      data: selectedFile, // Assign the file as data
      fileName: selectedFile.name // Use the file name
    };

    this.spinnerService.show();
    this.companyService.companyLogoUpdate(compnayId, fileParameter).subscribe((result: any) => {
        this.spinnerService.hide();
        this.toastrService.success("Logo updated.", "Success.");
        this.router.navigateByUrl(`app/company/update/${this._id}`);
      },
      () => {
        this.spinnerService.hide();
        this.toastrService.error("Logo update failed! Please, try again.", "Error.");
      }
    );
  }
}