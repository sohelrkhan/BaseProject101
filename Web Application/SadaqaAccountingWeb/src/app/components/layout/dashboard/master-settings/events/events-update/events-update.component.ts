import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  SimpleChanges,
} from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { CheckPermissionDirective } from '../../../../../../../identity/directive/check-permission.directive';
import {
  EventService,
  EventUpdateModel,
  EventViewModel,
} from '../../../../../../../api/base-api';
import { ToastrService } from 'ngx-toastr';
import { AccessControlService } from '../../../../../../../identity/services/access-control.service';

@Component({
  selector: 'app-events-update',
  templateUrl: './events-update.component.html',
  styleUrls: ['./events-update.component.css'],
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule,
    NgxSpinnerModule,
    RouterLink,
    CommonModule,
    CheckPermissionDirective,
  ],
  providers: [EventService],
})
export class EventsUpdateComponent implements OnInit {
  @Output() updateSuccess = new EventEmitter<void>();
  @Input() selectedEventId: string = '';
  // event update model
  eventUpdateModel: EventUpdateModel = new EventUpdateModel();

  constructor(
    private eventService: EventService,
    private spinnerService: NgxSpinnerService,
    private toastrService: ToastrService,
    private router: Router,
    private accessControlService: AccessControlService,
  ) {}

  ngOnInit() {
    this.accessControlService.setPermissions();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['selectedEventId']) {
      const id = changes['selectedEventId'].currentValue;

      if (id) {
        this.getEventById(id);
      }
    }
  }

  ngAfterViewInit() {
    setTimeout(() => {
      // Initialize the datetime picker (both date and time)
      $('.datetimepicker')
        .datetimepicker({
          format: 'DD-MM-YYYY', // Format for date and time
          showClear: true,
          //minDate: moment().startOf('day') // Disable selection of any date before today
        })
        .on('dp.change', (e) => {
          const name = (e.target as HTMLSelectElement).name;
          const value = (e.target as HTMLSelectElement).value;
          if (name === 'startDateString' || name === 'endDateString') {
            this.eventUpdateModel[name] = value;
          }
        });
    }, 0);
  }

  // Get event by id
  private getEventById(id: string): void {
    this.spinnerService.show();
    this.eventService.getById(id).subscribe(
      (result: EventViewModel) => {
        this.eventUpdateModel = result.updateModel;
        this.spinnerService.hide();
      },
      (error: any) => {
        this.spinnerService.hide();
        this.toastrService.error('Event cannot found! Please, try again.');
      },
    );
  }

  onClickUpdateEvent(): void {
    // Check event update from valid or not
    let isValidEventUpdateFrom: boolean = this.getEventFromValidResult();

    if (isValidEventUpdateFrom) {
      this.spinnerService.show();
      this.eventService.update(this.eventUpdateModel).subscribe(
        (result: EventUpdateModel) => {
          this.spinnerService.hide();
          this.toastrService.success('Event update successful.', 'Success');
          // Emit the updateSuccess event to notify parent component
          this.updateSuccess.emit();
          // Reset the selectedEventId after update
          this.selectedEventId = '';
          return this.router.navigateByUrl('/app/events');
        },
        (error: any) => {
          this.spinnerService.hide();
          this.toastrService.error('Event update failed.', 'Error');
        },
      );
    }
  }
  // Check event update from is valid or not
  private getEventFromValidResult(): boolean {
    if (
      this.eventUpdateModel.name == undefined ||
      this.eventUpdateModel.name == null ||
      this.eventUpdateModel.name == ''
    ) {
      this.toastrService.warning('Please, provide event name.', 'Warning');
      return false;
    } else {
      return true;
    }
  }
}
