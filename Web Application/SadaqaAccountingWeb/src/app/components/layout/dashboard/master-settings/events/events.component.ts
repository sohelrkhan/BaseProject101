import {
  Component,
  Inject,
  OnInit,
  PLATFORM_ID,
  ViewChild,
} from '@angular/core';
import { NgxSpinnerModule } from 'ngx-spinner';
import { CheckPermissionDirective } from '../../../../../../identity/directive/check-permission.directive';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { EventsCreateComponent } from './events-create/events-create.component';
import { AccessControlService } from '../../../../../../identity/services/access-control.service';
import { EventsListComponent } from './events-list/events-list.component';

@Component({
  selector: 'app-events',
  templateUrl: './events.component.html',
  styleUrls: ['./events.component.css'],
  standalone: true,
  imports: [
    NgxSpinnerModule,
    CheckPermissionDirective,
    CommonModule,
    EventsListComponent,
    EventsCreateComponent
],
  providers: [],
})
export class EventsComponent implements OnInit {
  showListComponent: boolean = true;
  isCreateModal: boolean = false;

  @ViewChild('createComponent')
  createComponent!: EventsCreateComponent;

  constructor(
    private accessControlService: AccessControlService,
    @Inject(PLATFORM_ID) private platformId: Object,
  ) {}

  ngOnInit() {
    this.accessControlService.setPermissions();
  }

  ngAfterViewInit() {
    if (isPlatformBrowser(this.platformId)) {
      const modalElement = document.getElementById('add_event_modal');
      if (modalElement) {
        modalElement.addEventListener('shown.bs.modal', () => {
          // Modal logic
        });
      }
    }
  }
  onEventCreated() {
    this.showListComponent = false;
    setTimeout(() => {
      this.showListComponent = true; // re-initialize list component
    });
  }
}
