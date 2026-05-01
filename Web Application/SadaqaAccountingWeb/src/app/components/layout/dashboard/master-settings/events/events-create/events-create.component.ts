import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import {
  AfterViewInit,
  Component,
  EventEmitter,
  OnInit,
  Output,
} from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import {
  ActionCreateModel,
  EventCreateModel,
  EventService,
} from '../../../../../../../api/base-api';
import { ToastrService } from 'ngx-toastr';
import { AccessControlService } from '../../../../../../../identity/services/access-control.service';
import { CheckPermissionDirective } from '../../../../../../../identity/directive/check-permission.directive';

@Component({
  selector: 'app-events-create',
  templateUrl: './events-create.component.html',
  styleUrls: ['./events-create.component.css'],
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
export class EventsCreateComponent implements OnInit, AfterViewInit {
  @Output() eventCreated = new EventEmitter<void>();
  // event create model
  eventCreateModel: EventCreateModel = new EventCreateModel();

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
            this.eventCreateModel[name] = value;
          }
        });
    }, 0);
  }
  // Create event
  onClickCreateEvent(): void {
    // Check event create from valid or not
    let isValidEventCreateFrom: boolean = this.getEventFromValidResult();

    if (isValidEventCreateFrom) {
      this.spinnerService.show();
      this.eventService.create(this.eventCreateModel).subscribe(
        (result: EventCreateModel) => {
          this.spinnerService.hide();
          this.toastrService.success('Event create successful.', 'Success');
          this.eventCreated.emit(); // Emit event created event
          return this.router.navigateByUrl('/app/events');
        },
        (error: any) => {
          this.spinnerService.hide();
          this.toastrService.error('Event create failed.', 'Error');
        },
      );
    }
  }

  // Check event create from is valid or not
  private getEventFromValidResult(): boolean {
    if (
      this.eventCreateModel.name == undefined ||
      this.eventCreateModel.name == null ||
      this.eventCreateModel.name == ''
    ) {
      this.toastrService.warning('Please, provide event name.', 'Warning');
      return false;
    } else {
      return true;
    }
  }
}
