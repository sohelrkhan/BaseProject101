import { CommonModule } from "@angular/common";
import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from "@angular/core";

@Component({
  selector: "app-confirm-modal",
  templateUrl: "./confirm-modal.component.html",
  styleUrls: ["./confirm-modal.component.css"],
  standalone: true,
  imports: [CommonModule],
  providers: []
})
export class ConfirmModalComponent {
  @Input() message: string = "Are you sure you want to proceed?";
  @Input() header: string = "Confirm Delete";
  @Input() id: string = "modal";
  @Input() confirmButtonText: string = "Yes";
  @Input() cancelButtonText: string = "Cancel";
  @Input() confirmButtonColor: string = "btn-danger";
  @Input() iconClass: string = "ti ti-trash-x fs-36";
  @Input() iconBackgroundColor: string = "bg-transparent-danger";
  @Input() iconTextColor: string = "text-danger";

  @Output() confirmAction: EventEmitter<void> = new EventEmitter();
  @Output() cancelAction: EventEmitter<void> = new EventEmitter();

  onConfirm(): void {
    this.confirmAction.emit();
  }

  onCancel(): void {
    this.cancelAction.emit();
  }
}
