import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  Inject,
  Output,
  PLATFORM_ID,
  ViewChild
} from "@angular/core";
import { FormErrorComponent } from "../../../../../../../shared/form-error/form-error.component";
import {
  ReportRegistryCreateModel,
  ReportRegistryGridModel,
  ReportRegistryService,
  ReportRegistryViewModel
} from "../../../../../../../../api/base-api";
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators
} from "@angular/forms";
import { CommonModule, isPlatformBrowser } from "@angular/common";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import { ToastrService } from "ngx-toastr";
import { AccessControlService } from "../../../../../../../../identity/services/access-control.service";
import { CheckPermissionDirective } from "../../../../../../../../identity/directive/check-permission.directive";
import TomSelect from "tom-select";
import { SelectModel } from "../../../../../../../shared/models/select-model";

@Component({
  selector: "app-report-registry-create",
  standalone: true,
  imports: [
    FormErrorComponent,
    CommonModule,
    CheckPermissionDirective,
    NgxSpinnerModule,
    ReactiveFormsModule
  ],
  templateUrl: "./report-registry-create.component.html",
  styleUrl: "./report-registry-create.component.scss",
  providers: [ReportRegistryService]
})
export class ReportRegistryCreateComponent {
  isBrowser: boolean = false;
  //Report Registry Created
  @Output() reportRegistryCreated = new EventEmitter<void>();

  reportRegistryForm!: FormGroup;
  reportRegistryCreateModel = new ReportRegistryCreateModel();
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

    // Get select list
    this.getSelectList();
    this.clearTomSelects();
    this.loadReportGroup();

    this.reportRegistryForm = this.fb.group({
      name: ["", Validators.required],
      url: ["", Validators.required],
      reportCode: ["", Validators.required],
      reportGroup: ["", Validators.required],
      moduleId: ["-1", [this.optionalValidator]]
    });
  }

  ngAfterViewInit(): void {
    if (this.isBrowser) {
      setTimeout(() => {
        this.reportRegistryForm.reset();

        ($(".select2") as any).select2({
          placeholder: "Choose...",
          width: "100%" // Optional, ensures it stretches full width
        });

        $(".select2").on("change", (e) => {
          const name = (e.target as HTMLSelectElement).name;
          const value = (e.target as HTMLSelectElement).value;
          this.reportRegistryForm.get(name)?.setValue(value);
          this.reportRegistryForm.get(name)?.markAsTouched();
          this.reportRegistryForm.get(name)?.updateValueAndValidity();
        });

        $("#add_report_registry_modal").on("shown.bs.modal", () => {
          ($(".select2-modal") as any).select2({
            dropdownParent: $("#add_report_registry_modal"),
            placeholder: "Choose Module...",
            width: "100%"
          });
        });

        $(".select2-modal").on("change", (e) => {
          const name = (e.target as HTMLSelectElement).name;
          const value = (e.target as HTMLSelectElement).value;
          this.reportRegistryForm.get(name)?.setValue(value);
          this.reportRegistryForm.get(name)?.markAsTouched();
          this.reportRegistryForm.get(name)?.updateValueAndValidity();
        });

        setTimeout(() => {
          this.cdr.detectChanges(); // Trigger change detection
          ($(`#moduleId`) as any).val("-1").trigger("change.select2");
        }, 0);
      }, 0);
    }
  }

  getControl(controlName: string): AbstractControl {
    return this.reportRegistryForm.get(controlName)!;
  }

  optionalValidator(control: AbstractControl): ValidationErrors | null {
    const value = control.value;
    return !value || value === "-1" ? { invalidOption: true } : null;
  }

  private getSelectList(): void {
    this.spinnerService.show();
    this.reportRegistryService.getById(-1).subscribe(
      (result: ReportRegistryViewModel) => {
        this.modules = result.optionsDataSources.ModuleSelectList;
        this.statuses = result.optionsDataSources.StatusSelectList;
        this.spinnerService.hide();
      },
      (error: any) => {
        this.spinnerService.hide();
      }
    );
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

  onClickCreateReportRegistry() {
    if (this.reportRegistryForm.valid) {
      this.reportRegistryCreateModel = this.reportRegistryForm.value;
      this.spinnerService.show();
      this.reportRegistryService.create(this.reportRegistryCreateModel).subscribe(
        () => {
          this.spinnerService.hide();
          this.toastrService.success("Report Registry created.", "Success.");

          setTimeout(() => {
            this.reportRegistryCreateModel = new ReportRegistryCreateModel();
            this.reportRegistryForm.reset();
            //this.clearTomSelects();
            this.reportRegistryCreated.emit(); // Notify parent to reload the list
          }, 1000);
        },
        () => {
          this.spinnerService.hide();
          this.toastrService.error(
            "Report Registry cannot be created. Please, try again.",
            "Error."
          );
        }
      );
    }
  }
}
