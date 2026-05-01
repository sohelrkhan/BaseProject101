import { CommonModule, isPlatformBrowser } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  Inject,
  OnInit,
  PLATFORM_ID,
  ViewChild
} from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Router, RouterLink } from "@angular/router";
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import {
  ActionGridModel,
  ActionService,
  ActionViewModel,
  FeatureActionMappingCreateModel,
  FeatureActionMappingGridModel,
  FeatureActionMappingService,
  FeatureService,
  RoleActionMappingGridModel,
  RoleActionMappingService,
  UserAccessMappingCreateModel,
  UserAccessMappingGridModel,
  UserAccessMappingService,
  UserService,
  UserViewModel
} from "../../../../../../../../api/base-api";
import { SelectModel } from "../../../../../../../shared/models/select-model";
import { ToastrService } from "ngx-toastr";
import TomSelect from "tom-select";
import { AccessControlService } from "../../../../../../../../identity/services/access-control.service";

@Component({
  selector: "app-create-user-access-control",
  templateUrl: "./create-user-access-control.component.html",
  styleUrls: ["./create-user-access-control.component.css"],
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule,
    NgxSpinnerModule,
    RouterLink,
    CommonModule
  ],
  providers: [
    UserAccessMappingService,
    FeatureActionMappingService,
    FeatureService,
    ActionService,
    UserService,
    RoleActionMappingService
  ]
})
export class CreateUserAccessControlComponent implements OnInit, AfterViewInit {
  isBrowser: boolean = false;
  // Default user access Mapping id
  private employeeId: number = 0;

  dataTable: any;
  isTableReady: boolean = false;
  dataTableInitialized: boolean = false;

  // Feature create model
  userAccessMappingCreateModel: UserAccessMappingCreateModel = new UserAccessMappingCreateModel();

  // Select list
  users: SelectModel[] = [];
  actions: SelectModel[] = [];
  featureActionsMappings: FeatureActionMappingGridModel[] = [];
  featureActionsMappingsCreate: FeatureActionMappingCreateModel[] = [];
  selectedFeatureActionMappings: FeatureActionMappingGridModel[] = [];
  userAccessMappings: UserAccessMappingGridModel[] = [];
  RoleActionMappings: RoleActionMappingGridModel[] = [];
  @ViewChild("user") user: ElementRef;
  userTomSelect: any;
  @ViewChild("role") role: ElementRef;
  roleTomSelect: any;

  constructor(
    private userAccessMappingService: UserAccessMappingService,
    private userService: UserService,
    private featureService: FeatureService,
    private actionService: ActionService,
    private featureActionMappingService: FeatureActionMappingService,
    private spinnerService: NgxSpinnerService,
    private toastrService: ToastrService,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: object,
    private roleMappingService: RoleActionMappingService,
    private changeDetectorRef: ChangeDetectorRef,
    private accessControlService: AccessControlService
  ) {}

  ngAfterViewInit() {
    if (this.isBrowser) {
      setTimeout(() => {
        ($(".select2") as any).select2({
          placeholder: "Choose...",
          width: "100%" // Optional, ensures it stretches full width
        });

        $(".select2").on("change", (e) => {
          const name = (e.target as HTMLSelectElement).name;
          const event = (e.target as HTMLSelectElement).value;
          if (event != "") {
            // Usage:
            this.spinnerService.show();
            this.getFeatureActionMappingList();
            this.userAccessMappingCreateModel.userId = event;
            this.userAccessMappingService.getUserWiseAccessById(event).subscribe(
              (result: UserAccessMappingGridModel) => {
                this.selectedFeatureActionMappings = result.featureActionMappingGridModelList;

                if (this.selectedFeatureActionMappings.length > 0) {
                  let featureActionMap = new Map<number, Set<number>>();

                  // Step 1: Collect all existing action IDs for each feature separately
                  this.selectedFeatureActionMappings.forEach((feature) => {
                    let actionIds = new Set<number>();
                    feature.actionList.forEach((action) => actionIds.add(action.id));
                    featureActionMap.set(feature.featureId, actionIds);
                  });

                  // Step 2: Update `isChecked` inside `actionList`, feature-wise
                  this.featureActionsMappings.forEach((feature) => {
                    if (featureActionMap.has(feature.featureId)) {
                      let existingActionIds = featureActionMap.get(feature.featureId);
                      feature.actionList.forEach((action) => {
                        action.isChecked = existingActionIds.has(action.id); // ✅ Feature-specific update
                      });
                    }
                  });

                  this.featureActionsMappingsCreate = []; // Clear previous entries
                  this.featureActionsMappings.forEach((feature) => {
                    // Collect all checked action IDs for this feature
                    let checkedActionIds = feature.actionList
                      .filter((action) => action.isChecked) // ✅ Only get checked actions
                      .map((action) => action.id); // Extract IDs

                    if (checkedActionIds.length > 0) {
                      let featureAction = new FeatureActionMappingCreateModel();
                      featureAction.featureId = feature.featureId;
                      featureAction.actionIdList = checkedActionIds; // ✅ Assign checked actions

                      this.featureActionsMappingsCreate.push(featureAction);
                    }
                  });
                } else {
                  // Usage:
                  this.getActionSelectList().then(() => {
                    this.getFeatureActionMappingList();
                  });
                }
                this.changeDetectorRef.detectChanges();
                this.spinnerService.hide();
              },
              (error: any) => {
                this.spinnerService.hide();
              }
            );
          } else {
            // Usage:
            this.getActionSelectList().then(() => {
              this.getFeatureActionMappingList();
            });
          }
        });
      }, 100);
    }
  }

  ngOnInit() {
    this.isBrowser = isPlatformBrowser(this.platformId);

    if(this.isBrowser) {
      this.accessControlService.setPermissions();
      this.userAccessMappingCreateModel.userId = "-1";

      this.getSelectList();
      this.getUserAccessMappings();
      this.getRoleActionMappings();

      // Usage:
      this.getActionSelectList().then(() => {
        this.getFeatureActionMappingList();
      });
    }   
  }

  private getUserAccessMappings() {
    this.spinnerService.show();
    this.userAccessMappingService.getAll().subscribe(
      (result: UserAccessMappingGridModel[]) => {
        this.userAccessMappings = result;

        setTimeout(() => {
          this.initializeTomSelectForUserAccessMapping();
        }, 0);
        this.spinnerService.hide();
      },
      (error: any) => {
        this.spinnerService.hide();
      }
    );
  }

  private initializeTomSelectForUserAccessMapping() {
    this.userTomSelect = new TomSelect(this.user.nativeElement, {
      placeholder: "Choose an user",
      plugins: ["remove_button"],
      onChange: (value: string) => {
        if (!value) {
          this.clearUser();
        } else {
          this.ChangeUser();
        }
      }
    });

    this.userTomSelect.setValue(null);
  }

  clearUser() {
    this.featureActionsMappings.forEach((feature) => {
      feature.actionList.forEach((action) => {
        action.isChecked = false;
      });
    });

    this.featureActionsMappingsCreate = [];
  }

  ChangeUser() {
    const selectedId: string = this.userTomSelect.getValue();
    this.spinnerService.show();

    const copiedUser = this.userAccessMappings.find((x) => x.userId === selectedId);

    if (!copiedUser || !copiedUser.featureActionMappingGridModelList) {
      this.toastrService.warning("Selected user has no access mappings to copy.");
      this.spinnerService.hide();
      return;
    }

    // Reset previous mappings
    this.featureActionsMappingsCreate = [];

    // Create map of featureId → actionId[] to track checked actions
    const featureActionMap = new Map<number, number[]>();

    copiedUser.featureActionMappingGridModelList.forEach((feature) => {
      const actionIds = feature.actionList.filter((a) => a.isChecked).map((a) => a.id);
      featureActionMap.set(feature.featureId, actionIds);
    });

    // Wait for DOM to be ready before updating checkboxes
    setTimeout(() => {
      // Update featureActionsMappings table
      this.featureActionsMappings.forEach((feature) => {
        const existingIds = featureActionMap.get(feature.featureId) || [];

        feature.actionList.forEach((action) => {
          action.isChecked = existingIds.includes(action.id);
        });

        if (existingIds.length > 0) {
          const mapping = new FeatureActionMappingCreateModel();
          mapping.featureId = feature.featureId;
          mapping.actionIdList = existingIds;
          this.featureActionsMappingsCreate.push(mapping);
        }
      });

      this.spinnerService.hide();
    }, 200); // Increased timeout for DOM readiness
  }

  private getRoleActionMappings() {
    this.spinnerService.show();
    this.roleMappingService.getAll().subscribe(
      (result: RoleActionMappingGridModel[]) => {
        this.RoleActionMappings = result;

        setTimeout(() => {
          this.initializeTomSelectForRoleActionMapping();
        }, 0);
        this.spinnerService.hide();
      },
      (error: any) => {
        this.spinnerService.hide();
      }
    );
  }

  private initializeTomSelectForRoleActionMapping() {
    this.roleTomSelect = new TomSelect(this.role.nativeElement, {
      placeholder: "Choose an Role",
      plugins: ["remove_button"],
      onChange: (value: any) => {
        if (!value) {
          this.clear();
        } else {
          this.Change();
        }
      }
    });

    this.roleTomSelect.setValue(null);
  }

  clear() {
    this.featureActionsMappings.forEach((feature) => {
      feature.actionList.forEach((action) => {
        action.isChecked = false;
      });
    });

    this.featureActionsMappingsCreate = [];
  }

  Change() {
    const selectedId: number = this.roleTomSelect.getValue();

    this.spinnerService.show();

    const roleActionMapping = this.RoleActionMappings.find((x) => x.roleId == selectedId);

    if (!roleActionMapping || !roleActionMapping.featureActionMappingGridModelList) {
      this.toastrService.warning("Selected user has no access mappings to copy.");
      this.spinnerService.hide();
      return;
    }

    // Reset previous mappings
    this.featureActionsMappingsCreate = [];

    // Create map of featureId → actionId[] to track checked actions
    const featureActionMap = new Map<number, number[]>();

    roleActionMapping.featureActionMappingGridModelList.forEach((feature) => {
      const actionIds = feature.actionList.filter((a) => a.isChecked).map((a) => a.id);
      featureActionMap.set(feature.featureId, actionIds);
    });

    // Update featureActionsMappings table
    this.featureActionsMappings.forEach((feature) => {
      const existingIds = featureActionMap.get(feature.featureId) || [];

      feature.actionList.forEach((action) => {
        action.isChecked = existingIds.includes(action.id);
      });

      if (existingIds.length > 0) {
        const mapping = new FeatureActionMappingCreateModel();
        mapping.featureId = feature.featureId;
        mapping.actionIdList = existingIds;
        this.featureActionsMappingsCreate.push(mapping);
      }
    });

    this.spinnerService.hide();
  }

  private getSelectList(): void {
    this.spinnerService.show();
    this.userService.getSelectListUser().subscribe((result: UserViewModel) => {
        this.users = result.optionsDataSources.UserSelectList;
        this.spinnerService.hide();
      },
      (error: any) => {
        this.spinnerService.hide();
      }
    );
  }

  private getActionSelectList(): Promise<void> {
    return new Promise((resolve) => {
      // Simulate fetching data
      setTimeout(() => {
        this.actionService.getSelectListAction().subscribe(
          (result: ActionViewModel) => {
            this.actions = result.optionsDataSources.ActionSelectList;
            this.spinnerService.hide();
          },
          (error: any) => {
            this.spinnerService.hide();
          }
        );
        resolve();
      }, 100);
    });
  }

  private getFeatureActionMappingList(): void {
    this.featureActionMappingService.getAllFeatureWiseActionsync().subscribe(
      (result: FeatureActionMappingGridModel[]) => {
        this.featureActionsMappings = result;
        this.spinnerService.hide();
        if (this.featureActionsMappings?.length > 0) {
          this.featureActionsMappings.forEach((feature) => {
            // Extract existing action IDs from actionsList
            const existingActionIds = feature.actionList.map((action) => action.id);

            // Find missing actions from the actions array
            this.actions.forEach((action) => {
              if (!existingActionIds.includes(action.id)) {
                // Add the missing action with statusName as 'Inactive'
                var obj = new ActionGridModel();
                obj.id = action.id;
                obj.name = action.name;
                obj.statusName = "Active";
                obj.isExist = false;
                obj.isChecked = false;
                feature.actionList.push(obj);
              }
            });
          });

          this.featureActionsMappings.forEach((feature) =>
            feature.actionList.sort((a, b) => a.id - b.id)
          );

          this.isTableReady = false;

          setTimeout(() => {
            this.isTableReady = true;

            setTimeout(() => {
              this.refreshDataTable();
            }, 100);
          });
        }
      },
      (error: any) => {
        this.spinnerService.hide();
      }
    );
  }

  // check and uncheck checkbox
  onClickAddAction(event, featureId: number, actionId: number): void {
    if (event.target.checked) {
      // Check if the feature already exists in the list
      let featureAction = this.featureActionsMappingsCreate.find(
        (fa) => fa.featureId === featureId
      );

      if (!featureAction) {
        // If the feature does not exist, create a new entry
        featureAction = new FeatureActionMappingCreateModel();
        featureAction.featureId = featureId;
        featureAction.actionIdList = [actionId]; // Initialize with the action
        this.featureActionsMappingsCreate.push(featureAction);
      } else {
        // If feature already exists, add action if not already present
        if (!featureAction.actionIdList.includes(actionId)) {
          featureAction.actionIdList.push(actionId);
        }
      }
    } else {
      // Find the feature and remove the actionId from its actionIdList
      this.featureActionsMappingsCreate.forEach((featureAction) => {
        if (featureAction.featureId === featureId) {
          featureAction.actionIdList = featureAction.actionIdList.filter((id) => id !== actionId);
        }
      });
      // Remove empty feature objects if no action IDs left
      this.featureActionsMappingsCreate = this.featureActionsMappingsCreate.filter(
        (featureAction) => featureAction.actionIdList.length > 0
      );
    }
  }

  onClickAddAll(event: any, actionId: number): void {
    // Iterate over all featureActionsMappings
    this.featureActionsMappings.forEach((feature) => {
      // Iterate over the actionList for each feature
      feature.actionList.forEach((action) => {
        if (action.id === actionId) {
          // If action.id matches the actionId passed, update isChecked status
          action.isChecked = event.target.checked; // Check/uncheck based on the parent checkbox
          this.onClickAddAction(event, feature.featureId, action.id);
        } else {
          // If action.id does not match, uncheck the checkbox (only if needed)
          // action.isChecked = false; // Uncheck the rest of the actions if needed
        }
      });
    });
  }

  // Create feature action mapping
  onClickCreateUserAccessMapping(): void {
    this.userAccessMappingCreateModel.featureActionMappingCreateModel =
      this.featureActionsMappingsCreate;
    // Check feature create from valid or not
    let isValidUserAccessCreateFrom: boolean = this.getUserAccessFromValidResult();

    if (isValidUserAccessCreateFrom) {
      this.spinnerService.show();
      this.userAccessMappingService.create(this.userAccessMappingCreateModel).subscribe(
        (result: UserAccessMappingCreateModel) => {
          this.spinnerService.hide();
          this.toastrService.success("User Access Mapped successfull.", "Success");
          this.users = [];
          this.actions = [];
          this.featureActionsMappings = [];
          this.getSelectList();
          this.userAccessMappingCreateModel.userId = "-1";
          this.userTomSelect.setValue(null);
          this.roleTomSelect.setValue(null);

          // Usage:
          this.getActionSelectList().then(() => {
            this.getFeatureActionMappingList();
          });

          //this.router.navigate(["/login"]);
        },
        (error: any) => {
          this.spinnerService.hide();
        }
      );
    }
  }

  // Check user access mapping create from is valid or not
  private getUserAccessFromValidResult(): boolean {
    if (
      this.userAccessMappingCreateModel.userId == undefined ||
      this.userAccessMappingCreateModel.userId == null ||
      this.userAccessMappingCreateModel.userId == ""
    ) {
      this.toastrService.warning("Please, select user.", "Warning");
      return false;
    } else if (this.userAccessMappingCreateModel.featureActionMappingCreateModel.length == 0) {
      this.toastrService.warning("Please, select action.", "Warning");
      return false;
    } else {
      return true;
    }
  }

  private refreshDataTable(): void {
    const tableElement = $("#activityTable");

    if (!tableElement.length) {
      console.warn("Table element not found in DOM.");
      return;
    }

    // If this.dataTable already exists, destroy it directly
    if (this.dataTable) {
      this.dataTable.destroy(true);
      this.dataTable = null;
    }

    this.dataTable = tableElement.DataTable({
      responsive: true
    });
  }
}
