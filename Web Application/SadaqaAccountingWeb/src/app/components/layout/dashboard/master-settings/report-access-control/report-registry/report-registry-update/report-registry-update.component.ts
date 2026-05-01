import { CommonModule, isPlatformBrowser } from "@angular/common";
import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  Inject,
  Input,
  OnChanges,
  OnInit,
  Output,
  PLATFORM_ID,
  SimpleChanges,
  ViewChild
} from "@angular/core";
import { FormErrorComponent } from "../../../../../../../shared/form-error/form-error.component";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators
} from "@angular/forms";
import {
  ReportRegistryGridModel,
  ReportRegistryService,
  ReportRegistryUpdateModel,
  ReportRegistryViewModel,
} from "../../../../../../../../api/base-api";
import { ToastrService } from "ngx-toastr";
import { AccessControlService } from "../../../../../../../../identity/services/access-control.service";
import { CheckPermissionDirective } from "../../../../../../../../identity/directive/check-permission.directive";
import TomSelect from "tom-select";
import { SelectModel } from "../../../../../../../shared/models/select-model";

@Component({
  selector: "app-report-registry-update",
  standalone: true,
  imports: [
    CommonModule,
    FormErrorComponent,
    NgxSpinnerModule,
    ReactiveFormsModule,
    CheckPermissionDirective
  ],
  templateUrl: "./report-registry-update.component.html",
  styleUrl: "./report-registry-update.component.scss"
})
export class ReportRegistryUpdateComponent implements OnInit, AfterViewInit, OnChanges {
  isBrowser: boolean = false;
  //When Data Updated
  @Output() updateSuccess = new EventEmitter<void>();

  @Input() selectedReportRegistryId: number = 0;

  reportRegistryForm!: FormGroup;
  reportRegistryUpdateModel: ReportRegistryUpdateModel = new ReportRegistryUpdateModel();
  reportRegistryGridModel: ReportRegistryGridModel[] = [];

  // Select list
  modules: SelectModel[] = [];
  statuses: SelectModel[] = [];

  @ViewChild("reportGroup") reportGroup!: ElementRef;
  private tomSelectReportGroup: any;

  constructor(
    private reportRegistryService: ReportRegistryService,
    private spinnerService: NgxSpinnerService,
    private toastrService: ToastrService,
    private accessControlService: AccessControlService,
    private fb: FormBuilder,
    @Inject(PLATFORM_ID) private platformId: object,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);

    this.accessControlService.setPermissions();

    this.reportRegistryForm = this.fb.group({
      id: [null, Validators.required],
      name: ["", Validators.required],
      url: ["", Validators.required],
      reportCode: ["", Validators.required],
      reportGroup: ["", Validators.required],
      moduleId: ["-1", [this.optionalValidator]],
      statusId: ["-1", [this.optionalValidator]]
    });

    this.reportRegistryForm.get("reportCode")?.disable();
  }

  ngAfterViewInit(): void {
    if (this.isBrowser) {
      setTimeout(() => {
        this.reportRegistryForm.reset();

        $("#update_report_registry_modal").on("shown.bs.modal", () => {
          ($(".select2-update-Modal") as any).select2({
            dropdownParent: $("#update_report_registry_modal"),
            placeholder: "Choose...",
            width: "100%"
          });
        });

        $("#update_report_registry_modal").on("shown.bs.modal", () => {
          $(".select2-update-Modal").on("change", (e) => {
            const name = (e.target as HTMLSelectElement).name;
            const value = (e.target as HTMLSelectElement).value;
            this.reportRegistryForm.get(name)?.setValue(value);
            this.reportRegistryForm.get(name)?.markAsTouched();
            this.reportRegistryForm.get(name)?.updateValueAndValidity();
          });
        });

        setTimeout(() => {
          this.cdr.detectChanges(); // Trigger change detection
          ($(`#statusId`) as any).val("-1").trigger("change.select2");
          ($(`#moduleId`) as any).val("-1").trigger("change.select2");
        }, 0);
      }, 0);
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes["selectedReportRegistryId"]) {
      const id = changes["selectedReportRegistryId"].currentValue;
      if (id) {
        this.clearTomSelects();
        this.getReportRegistryById(id);
      }
    }
  }

  getControl(controlName: string): AbstractControl {
    return this.reportRegistryForm.get(controlName)!;
  }

  optionalValidator(control: AbstractControl): ValidationErrors | null {
    const value = control.value;
    return !value || value === "-1" ? { invalidOption: true } : null;
  }

  loadReportGroup() {
    this.reportRegistryService.getAll().subscribe((result: ReportRegistryGridModel[]) => {
      this.reportRegistryGridModel = result;
      setTimeout(() => this.initTomSelectReportGroup(), 100);
    });
  }

  initTomSelectReportGroup() {
    if (this.reportGroup?.nativeElement) {
      this.tomSelectReportGroup = new TomSelect(this.reportGroup.nativeElement, {
        placeholder: "Search for an report group or enter a new one",
        create: true,
        maxItems: 1,
        plugins: ["remove_button"],

        onItemAdd: (value: string, item: HTMLElement) => {
          this.reportRegistryForm.get("reportGroup")?.setValue(value);
        },

        onItemRemove: (value: string) => {
          // Clear item name field
          this.reportRegistryForm.get("reportGroup")?.reset();
        }
      });
    }
  }

  clearTomSelects() {
    if (this.tomSelectReportGroup) {
      this.tomSelectReportGroup.destroy();
      this.tomSelectReportGroup = null;
    }

    if (this.reportGroup?.nativeElement) {
      this.reportGroup.nativeElement.value = "";
    }
  }

  // Get report registry by id
  private getReportRegistryById(reportRegistryId: number): void {
    this.spinnerService.show();
    this.reportRegistryService
      .getById(reportRegistryId)
      .subscribe((result: ReportRegistryViewModel) => {
        this.reportRegistryUpdateModel = new ReportRegistryUpdateModel();

        if (result.updateModel) {
          this.modules = result.optionsDataSources.ModuleSelectList;
          this.statuses = result.optionsDataSources.StatusSelectList;

          // Wait until statuses are set before patching the form
          setTimeout(() => {
            this.reportRegistryForm.patchValue({
              id: result.updateModel.id,
              name: result.updateModel.name,
              url: result.updateModel.url,
              reportCode: result.updateModel.reportCode,
              reportGroup: result.updateModel.reportGroup,
              moduleId: result.updateModel.moduleId,
              statusId: result.updateModel.statusId
            });

            this.loadReportGroup();
            this.cdr.detectChanges();
          });
        }

        this.spinnerService.hide();
      });
  }

  // Update Report Registry
  onClickUpdateReportRegistry(): void {
    if (this.reportRegistryForm.valid) {
      this.reportRegistryUpdateModel = this.reportRegistryForm.getRawValue();
      this.spinnerService.show();
      this.reportRegistryService.update(this.reportRegistryUpdateModel).subscribe(
        (result: ReportRegistryUpdateModel) => {
          this.spinnerService.hide();
          this.toastrService.success("Report registry update successful.", "Success");

          setTimeout(() => {
            this.reportRegistryUpdateModel = new ReportRegistryUpdateModel();
            // Emit to notify report registry list component
            this.updateSuccess.emit();
          }, 1000);
        },
        (error: any) => {
          this.spinnerService.hide();
          this.toastrService.error(
            "Report registry cannot be updated. Please, try again.",
            "Error."
          );
        }
      );
    }
  }
}
