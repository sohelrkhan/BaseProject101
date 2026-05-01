import { CommonModule, isPlatformBrowser } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { AfterViewInit, ChangeDetectorRef, Component, Inject, OnInit, PLATFORM_ID } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import { ActionService, ActionUpdateModel, ActionViewModel } from "../../../../../../../../api/base-api";
import { SelectModel } from "../../../../../../../shared/models/select-model";
import { CustomTosterServiceService } from "../../../../../../../shared/Toster/CustomTosterService.service";

declare var $: any;

@Component({
  selector: "app-action-update",
  templateUrl: "./action-update.component.html",
  styleUrls: ["./action-update.component.css"],
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, HttpClientModule, NgxSpinnerModule, RouterLink, CommonModule],
  providers: [ActionService]
})

export class ActionUpdateComponent implements OnInit, AfterViewInit {

  // Is browser
  isBrowser: boolean = false;

  // Action id
  private _id: string | undefined;

  // Select list
  statuses: SelectModel[] = [];

  // Action update model
  actionUpdateModel: ActionUpdateModel = new ActionUpdateModel();

  constructor(private actionService: ActionService, private spinnerService: NgxSpinnerService, private toastrService: CustomTosterServiceService, private router: Router,
    private activatedRoute: ActivatedRoute, @Inject(PLATFORM_ID) private platformId: object, private cdRef: ChangeDetectorRef) { }

  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);

    if (this.isBrowser) {
      this.getActionIdByUrl();
    }
  }

  ngAfterViewInit() { 
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
    const status = ($("#statusId") as any);
    status.select2();

    status.on("change", () => {
      let statusId: number = Number(status.val());
    
      if(statusId != undefined || statusId != null || statusId != -1) {        
        this.actionUpdateModel.statusId = statusId;
      } 
   
      this.cdRef.detectChanges();
    });
  }

  // Get action id by url
  private getActionIdByUrl(): void {
    this.activatedRoute.params.subscribe((params) => {
      this._id = params["recordId"];

      // Get action by id
      if (this._id != undefined || this._id != null || this._id != "") {
        this.getActionById();
      }
    });
  }

  // Get action by id
  private getActionById(): void {
    this.spinnerService.show();
    this.actionService.getById(this._id!).subscribe((result: ActionViewModel) => {
      this.actionUpdateModel = result.updateModel;
      this.statuses = result.optionsDataSources.StatusSelectList;

      // Initialize tom select dropdown
      this.cdRef.detectChanges();
      
      // Set status id
      ($("#statusId") as any).val(this.actionUpdateModel.statusId);
      this.spinnerService.hide();
    },
    (error: any) => {
      this.spinnerService.hide();
      this.toastrService.error("Action cannot found! Please, try again.");
    });
  }

  // Update action
  onClickUpdateAction(): void {

    // Check action from valid or not
    let isValidActionFrom: boolean = this.getActionFromValidResult();

    if (isValidActionFrom) {
      this.spinnerService.show();
      this.actionService.update(this.actionUpdateModel).subscribe((result: ActionUpdateModel) => {
        this.spinnerService.hide();
        this.toastrService.success("Action update successful.", "Success");
        return this.router.navigateByUrl("/app/actions");
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error("Action cannot update! Please, try again.");
      });
    }
  }

  // Check action create from is valid or not
  private getActionFromValidResult(): boolean {
    if (this.actionUpdateModel.name == undefined || this.actionUpdateModel.name == null || this.actionUpdateModel.name == "") {
      this.toastrService.warning("Please, provied action name.", "Warning");
      return false;
    } else if (this.actionUpdateModel.statusId == undefined || this.actionUpdateModel.statusId == null || this.actionUpdateModel.statusId == -1) {
      this.toastrService.warning("Please, select status.", "Warning");
      return false;
    } else {
      return true;
    }
  }
}