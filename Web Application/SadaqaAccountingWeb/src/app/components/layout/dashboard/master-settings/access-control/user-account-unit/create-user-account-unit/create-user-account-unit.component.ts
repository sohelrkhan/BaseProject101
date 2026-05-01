import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  OnInit,
  ViewChild,
} from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import {
  AccountUnitGridModel,
  AccountUnitService,
  SelectModel,
  UserAccountUnitCreateModel,
  UserAccountUnitGridModel,
  UserAccountUnitService,
  UserService,
  UserViewModel,
} from '../../../../../../../../api/base-api';
import { ToastrService } from 'ngx-toastr';
import TomSelect from 'tom-select';

@Component({
  selector: 'app-create-user-account-unit',
  templateUrl: './create-user-account-unit.component.html',
  styleUrls: ['./create-user-account-unit.component.css'],
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule,
    NgxSpinnerModule,
    RouterLink,
    CommonModule,
  ],
  providers: [UserAccountUnitService, UserService, AccountUnitService],
})
export class CreateUserAccountUnitComponent implements OnInit {
  // User Account Unit create model
  userAccountUnitCreateModel: UserAccountUnitCreateModel =
    new UserAccountUnitCreateModel();
  userAccountUnitGridModel: UserAccountUnitGridModel =
    new UserAccountUnitGridModel();

  // Select list
  users: SelectModel[] = [];
  accountUnits: SelectModel[] = [];
  selectedAccountUnits: AccountUnitGridModel[] = [];

  @ViewChild('createUserId') createUserId: ElementRef;

  constructor(
    private userService: UserService,
    private accountUnitService: AccountUnitService,
    private userAccountUnitService: UserAccountUnitService,
    private spinnerService: NgxSpinnerService,
    private toastrService: ToastrService,
    private router: Router,
    private cdRef: ChangeDetectorRef,
  ) {}

  ngOnInit() {
    // On page load, default set user id = -1
    this.userAccountUnitCreateModel.userId = '-1';
    this.userAccountUnitCreateModel.accountUnitIds = [];

    // Get select list
    this.getSelectList();
    this.getAccountUnitSelectList();
  }

  private getAccountUnitSelectList(): void {
    this.accountUnitService.getSelectListAccountUnit().subscribe(
      (result: SelectModel[]) => {
        this.accountUnits = result;
        this.spinnerService.hide();
      },
      (error: any) => {
        this.spinnerService.hide();
      },
    );
  }

  private getSelectList(): void {
    this.spinnerService.show();
    this.userService.getSelectListUser().subscribe(
      (result: UserViewModel) => {
        this.users = result.optionsDataSources.UserSelectList;

        this.cdRef.detectChanges();
        setTimeout(() => {
          this.initializeTomSelect();
        }, 200);

        this.spinnerService.hide();
      },
      (error: any) => {
        this.spinnerService.hide();
      },
    );
  }

  onChangeUser(event: any): void {
    if (event != -1) {
      this.getAccountUnitSelectList();
      this.userAccountUnitCreateModel.userId = event;
      this.userAccountUnitService.getUserAccountUnit(event).subscribe(
        (result: UserAccountUnitGridModel) => {
          this.selectedAccountUnits = result.accountUnitList;
          this.spinnerService.hide();

          if (this.selectedAccountUnits.length > 0) {
            const selectedIds = new Set(
              this.selectedAccountUnits.map((accountUnit) => accountUnit.id),
            );

            this.accountUnits = this.accountUnits.map((accountUnit) => {
              if (selectedIds.has(accountUnit.id)) {
                accountUnit.isDefault = true;
              }
              return accountUnit;
            });

            //
            this.userAccountUnitCreateModel.accountUnitIds = [];
            for (let i = 0; i < this.accountUnits.length; i++) {
              if (this.accountUnits[i].isDefault) {
                this.userAccountUnitCreateModel.accountUnitIds.push(
                  this.accountUnits[i].id,
                ); // Push only objects with status: true
              }
            }
          } else {
            this.accountUnits = [];
            this.getAccountUnitSelectList();
          }
        },
        (error: any) => {
          this.spinnerService.hide();
        },
      );
    } else {
      this.accountUnits = [];
      this.getAccountUnitSelectList();
    }
  }

  onClickAddAccountUnit(event, accountUnitId: number): void {
    if (event.target.checked) {
      this.userAccountUnitCreateModel.accountUnitIds.push(accountUnitId);
    } else {
      this.userAccountUnitCreateModel.accountUnitIds =
        this.userAccountUnitCreateModel.accountUnitIds.filter(
          (item) => item !== accountUnitId,
        );
    }
  }
  // Create feature action mapping
  onClickCreateUserAccountUnitMapping(): void {
    // Check feature create from valid or not
    let isValidUserAccountUnitCreateFrom: boolean =
      this.getUserAccountUnitFromValidResult();

    if (isValidUserAccountUnitCreateFrom) {
      this.spinnerService.show();
      this.userAccountUnitService
        .create(this.userAccountUnitCreateModel)
        .subscribe(
          (result: UserAccountUnitCreateModel) => {
            this.spinnerService.hide();
            this.toastrService.success(
              'User Account Unit Mapped successfull.',
              'Success',
            );
            this.users = [];
            this.accountUnits = [];
            this.getSelectList();
            this.getAccountUnitSelectList();
            this.userAccountUnitCreateModel.accountUnitIds = [];
            this.userAccountUnitCreateModel.userId = '-1';
            this.userAccountUnitCreateModel = new UserAccountUnitCreateModel();
            // return this.router.navigateByUrl("/app/features");
          },
          (error: any) => {
            this.spinnerService.hide();
          },
        );
    }
  }

  // Check features action mapping create from is valid or not
  private getUserAccountUnitFromValidResult(): boolean {
    if (
      this.userAccountUnitCreateModel.userId == undefined ||
      this.userAccountUnitCreateModel.userId == null ||
      this.userAccountUnitCreateModel.userId == '-1'
    ) {
      this.toastrService.warning('Please, select user.', 'Warning');
      return false;
    } else if (this.userAccountUnitCreateModel.accountUnitIds.length == 0) {
      this.toastrService.warning('Please, select account unit.', 'Warning');
      return false;
    } else {
      return true;
    }
  }
  private initializeTomSelect(): void {
    if (this.createUserId && this.createUserId.nativeElement.tomselect) {
      this.createUserId.nativeElement.tomselect.destroy();
    }

    new TomSelect(this.createUserId.nativeElement, {
      placeholder: 'Choose a User',
      allowEmptyOption: true,
      create: false,
      plugins: ['remove_button'],
    });
  }
}
