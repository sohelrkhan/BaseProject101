import { Component, OnInit, OnDestroy } from "@angular/core";
import { CommonModule } from "@angular/common";
import { Subscription } from "rxjs";
import { ToastMessage, CustomTosterServiceService } from "../CustomTosterService.service";

@Component({
  selector: "app-toast",
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="toast-container">
      <div
        *ngFor="let toast of toasts"
        class="toast-item"
        [class.toast-success]="toast.type === 'success'"
        [class.toast-error]="toast.type === 'error'"
        [class.toast-warning]="toast.type === 'warning'"
        [class.toast-info]="toast.type === 'info'"
        [attr.data-toast-id]="toast.id">
        <div class="toast-content">
          <div class="toast-icon">
            <i
              [class.feather-check-circle]="toast.type === 'success'"
              [class.feather-x-circle]="toast.type === 'error'"
              [class.feather-alert-triangle]="toast.type === 'warning'"
              [class.feather-info]="toast.type === 'info'"></i>
          </div>

          <div class="toast-text">
            <div class="toast-title">{{ toast.title }}</div>
            <div class="toast-message">{{ toast.message }}</div>
          </div>

          <button
            *ngIf="toast.closable"
            type="button"
            class="toast-close"
            (click)="removeToast(toast.id)">
            <i class="feather-x"></i>
          </button>
        </div>

        <!-- Progress bar for auto-dismiss -->
        <div
          *ngIf="toast.duration && toast.duration > 0"
          class="toast-progress"
          [style.animation-duration.ms]="toast.duration"></div>
      </div>
    </div>
  `,
  styles: [
    `
      .toast-container {
        position: fixed;
        bottom: 24px;
        right: 24px;
        z-index: 9999;
        max-width: 400px;
        width: 100%;
      }

      .toast-item {
        background: #ffffff;
        border-radius: 8px;
        box-shadow: 0 8px 32px rgba(0, 0, 0, 0.12);
        margin-bottom: 12px;
        overflow: hidden;
        position: relative;
        border-left: 4px solid;
        animation: slideInRight 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275);
      }

      .toast-item.removing {
        animation: slideOutRight 0.25s ease-in forwards;
      }

      /* Toast type colors */
      .toast-success {
        border-left-color: #10b981;
      }

      .toast-error {
        border-left-color: #ef4444;
      }

      .toast-warning {
        border-left-color: #f59e0b;
      }

      .toast-info {
        border-left-color: #3b82f6;
      }

      .toast-content {
        display: flex;
        align-items: flex-start;
        padding: 16px;
        gap: 12px;
      }

      .toast-icon {
        flex-shrink: 0;
        width: 20px;
        height: 20px;
        display: flex;
        align-items: center;
        justify-content: center;
        margin-top: 2px;
      }

      .toast-success .toast-icon i {
        color: #10b981;
        font-size: 18px;
      }

      .toast-error .toast-icon i {
        color: #ef4444;
        font-size: 18px;
      }

      .toast-warning .toast-icon i {
        color: #f59e0b;
        font-size: 18px;
      }

      .toast-info .toast-icon i {
        color: #3b82f6;
        font-size: 18px;
      }

      .toast-text {
        flex: 1;
        min-width: 0;
      }

      .toast-title {
        font-size: 14px;
        font-weight: 600;
        color: #1f2937;
        margin-bottom: 4px;
        line-height: 1.4;
      }

      .toast-message {
        font-size: 13px;
        color: #6b7280;
        line-height: 1.4;
        word-wrap: break-word;
      }

      .toast-close {
        flex-shrink: 0;
        background: none;
        border: none;
        padding: 0;
        width: 20px;
        height: 20px;
        display: flex;
        align-items: center;
        justify-content: center;
        color: #9ca3af;
        cursor: pointer;
        border-radius: 4px;
        transition: all 0.2s ease;
      }

      .toast-close:hover {
        background-color: #f3f4f6;
        color: #6b7280;
      }

      .toast-close i {
        font-size: 14px;
      }

      /* Progress bar */
      .toast-progress {
        position: absolute;
        bottom: 0;
        left: 0;
        height: 3px;
        background: linear-gradient(90deg, rgba(0, 0, 0, 0.1) 0%, rgba(0, 0, 0, 0.05) 100%);
        animation: progress linear forwards;
      }

      .toast-success .toast-progress {
        background: linear-gradient(90deg, #10b981 0%, #059669 100%);
      }

      .toast-error .toast-progress {
        background: linear-gradient(90deg, #ef4444 0%, #dc2626 100%);
      }

      .toast-warning .toast-progress {
        background: linear-gradient(90deg, #f59e0b 0%, #d97706 100%);
      }

      .toast-info .toast-progress {
        background: linear-gradient(90deg, #3b82f6 0%, #2563eb 100%);
      }

      /* Animations */
      @keyframes slideInRight {
        from {
          transform: translateX(100%);
          opacity: 0;
        }
        to {
          transform: translateX(0);
          opacity: 1;
        }
      }

      @keyframes slideOutRight {
        from {
          transform: translateX(0);
          opacity: 1;
        }
        to {
          transform: translateX(100%);
          opacity: 0;
        }
      }

      @keyframes progress {
        from {
          width: 100%;
        }
        to {
          width: 0%;
        }
      }

      /* Responsive design */
      @media (max-width: 480px) {
        .toast-container {
          bottom: 16px;
          right: 16px;
          left: 16px;
          max-width: none;
        }

        .toast-content {
          padding: 14px;
          gap: 10px;
        }

        .toast-title {
          font-size: 13px;
        }

        .toast-message {
          font-size: 12px;
        }
      }
    `
  ]
})
export class ToastComponent implements OnInit, OnDestroy {
  toasts: ToastMessage[] = [];
  private subscription: Subscription = new Subscription();

  constructor(private toasterService: CustomTosterServiceService) {}

  ngOnInit(): void {
    this.subscription = this.toasterService.toasts$.subscribe((toasts) => {
      this.toasts = toasts;
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  removeToast(id: string): void {
    // Add removing animation class
    const toastElement = document.querySelector(`[data-toast-id="${id}"]`);
    if (toastElement) {
      toastElement.classList.add("removing");

      // Remove after animation completes
      setTimeout(() => {
        this.toasterService.remove(id);
      }, 250);
    } else {
      this.toasterService.remove(id);
    }
  }
}
