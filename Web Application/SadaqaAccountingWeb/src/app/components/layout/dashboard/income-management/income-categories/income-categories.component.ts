import { Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { NgxSpinnerModule } from 'ngx-spinner';
import { CheckPermissionDirective } from '../../../../../../identity/directive/check-permission.directive';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AccessControlService } from '../../../../../../identity/services/access-control.service';
import { IncomeCategoryListComponent } from "./income-category-list/income-category-list.component";

@Component({
  selector: 'app-income-categories',
  templateUrl: './income-categories.component.html',
  styleUrls: ['./income-categories.component.css'],
  standalone: true,
  imports: [
    NgxSpinnerModule,
    CheckPermissionDirective,
    CommonModule,
    RouterLink,
    IncomeCategoryListComponent
],
  providers: [],
})
export class IncomeCategoriesComponent implements OnInit {
  showListComponent: boolean = true;
  isCreateModal: boolean = false;

  constructor(
    private accessControlService: AccessControlService,
    @Inject(PLATFORM_ID) private platformId: Object,
  ) {}

  ngOnInit() {
    this.accessControlService.setPermissions();
  }
  onDonorCreated() {
    this.showListComponent = false;
    setTimeout(() => {
      this.showListComponent = true; // re-initialize list component
    });
  }
}
