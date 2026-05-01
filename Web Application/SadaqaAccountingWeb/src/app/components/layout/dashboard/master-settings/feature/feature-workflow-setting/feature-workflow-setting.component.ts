import { CommonModule, isPlatformBrowser } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  Inject,
  Input,
  OnInit,
  PLATFORM_ID,
  SimpleChanges,
  ViewChild
} from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import { FeatureService } from "../../../../../../../api/base-api";
import { ToastrService } from "ngx-toastr";
import { SelectModel } from "../../../../../../shared/models/select-model";
import TomSelect from "tom-select/base";
import { CheckPermissionDirective } from "../../../../../../../identity/directive/check-permission.directive";
import { AccessControlService } from "../../../../../../../identity/services/access-control.service";

@Component({
  selector: "app-feature-workflow-setting",
  templateUrl: "./feature-workflow-setting.component.html",
  styleUrls: ["./feature-workflow-setting.component.css"],
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
export class FeatureWorkflowSettingComponent implements OnInit, AfterViewInit {
  isBrowser = false;

  // Workflow process create modal
  isVisibleWorkflowProcessCreateModal: boolean = false;

  // Select list for workflow process create
  features: SelectModel[] = [];
  companies: SelectModel[] = [];
  statuses: SelectModel[] = [];
  isChecked: boolean = false;
  // Feature id
  private _featureId: number | undefined;
  @Input() featureId!: number | undefined;

  // Default workflow id
  private _workflowProcessId: number = 0;
  @ViewChild("companySelect") companySelect: ElementRef;

  constructor(
    private featureService: FeatureService,
    private spinnerService: NgxSpinnerService,
    private toastrService: ToastrService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private cdr: ChangeDetectorRef,
    @Inject(PLATFORM_ID) private platformId: object,
    private accessControlService: AccessControlService
  ) {}

  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);
    this.accessControlService.setPermissions();
  }

  ngAfterViewInit(): void {
    this.isBrowser = isPlatformBrowser(this.platformId);
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes["featureId"] && this.featureId) {
      this._featureId = this.featureId;
      this.getFeatureIdByUrl();
    }
  }

  // Get feature id by url
  private getFeatureIdByUrl(): void {
    // Get feature workflow process by feature id
    if (this._featureId != undefined || this._featureId != null || this._featureId! <= 0) {

    }
  }

  // On click setup workflow
  onClickShowSetupWorkFlowFrom(): void {
    this.isVisibleWorkflowProcessCreateModal = true;
  }

  handleCancelWorkflowProcessCreateModal(): void {
    this.isVisibleWorkflowProcessCreateModal = false;
  }

  handleOkWorkflowProcessCreateModal(): void {
    this.isVisibleWorkflowProcessCreateModal = true;
  }

  onChangeStatus(id:number, event: any) {
    this.isChecked = event.target.checked;  // or event.target.value if needed
    this._workflowProcessId = id;
  }

  private initializeTomSelect() {
    if (this.companySelect && this.companySelect.nativeElement.tomselect) {
      this.companySelect.nativeElement.tomselect.destroy();
    }

    new TomSelect(this.companySelect.nativeElement, {
      placeholder: "Choose a company",
      allowEmptyOption: true,
      create: false,
      plugins: ["remove_button"]
    });
  }
}