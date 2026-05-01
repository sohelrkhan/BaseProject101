import { Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { NgxSpinnerModule } from 'ngx-spinner';
import { CheckPermissionDirective } from '../../../../../../identity/directive/check-permission.directive';
import { CommonModule } from '@angular/common';
import { AccessControlService } from '../../../../../../identity/services/access-control.service';
import { RouterLink } from "@angular/router";
import { DonorListComponent } from "./donor-list/donor-list.component";

@Component({
  selector: 'app-donors',
  templateUrl: './donors.component.html',
  styleUrls: ['./donors.component.css'],
  standalone: true,
  imports: [NgxSpinnerModule, CheckPermissionDirective, CommonModule, RouterLink, DonorListComponent],
  providers: [],
})
export class DonorsComponent implements OnInit {
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
