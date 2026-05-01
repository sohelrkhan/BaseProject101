import { Directive, HostListener, Input } from "@angular/core";
import { GlobalModalService } from "./services/global-modal.service";

@Directive({
  selector: "[appModalTrigger]",
  standalone: true
})
export class ModalTriggerDirective {
  @Input() modalData: any; // Accepts data to pass to the modal

  constructor(private modalService: GlobalModalService) {}

  @HostListener("click") onClick() {
    this.modalService.openModal(this.modalData);
  }
}
