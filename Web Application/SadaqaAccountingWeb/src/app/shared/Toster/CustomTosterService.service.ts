import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";

export interface ToastMessage {
  id: string;
  type: "success" | "error" | "warning" | "info";
  title: string;
  message: string;
  duration?: number;
  closable?: boolean;
}

@Injectable({
  providedIn: "root"
})
export class CustomTosterServiceService {
  private toastsSubject = new BehaviorSubject<ToastMessage[]>([]);
  public toasts$ = this.toastsSubject.asObservable();

  private defaultDuration = 5000; // 5 seconds
  private toastCounter = 0;

  constructor() {}

  /**
   * Show success toast
   */
  success(message: string, title: string = "Success", duration?: number): void {
    this.show("success", title, message, duration);
  }

  /**
   * Show error toast
   */
  error(message: string, title: string = "Error", duration?: number): void {
    this.show("error", title, message, duration);
  }

  /**
   * Show warning toast
   */
  warning(message: string, title: string = "Warning", duration?: number): void {
    this.show("warning", title, message, duration);
  }

  /**
   * Show info toast
   */
  info(message: string, title: string = "Info", duration?: number): void {
    this.show("info", title, message, duration);
  }

  /**
   * Show toast with custom configuration
   */
  private show(
    type: "success" | "error" | "warning" | "info",
    title: string,
    message: string,
    duration?: number
  ): void {
    const toast: ToastMessage = {
      id: `toast-${++this.toastCounter}`,
      type,
      title,
      message,
      duration: duration || this.defaultDuration,
      closable: true
    };

    // Add toast to current array
    const currentToasts = this.toastsSubject.value;
    this.toastsSubject.next([...currentToasts, toast]);

    // Auto remove after duration
    if (toast.duration && toast.duration > 0) {
      setTimeout(() => {
        this.remove(toast.id);
      }, toast.duration);
    }
  }

  /**
   * Remove toast by ID
   */
  remove(id: string): void {
    const currentToasts = this.toastsSubject.value;
    const filteredToasts = currentToasts.filter((toast) => toast.id !== id);
    this.toastsSubject.next(filteredToasts);
  }

  /**
   * Clear all toasts
   */
  clear(): void {
    this.toastsSubject.next([]);
  }

  /**
   * Get current toasts
   */
  getToasts(): ToastMessage[] {
    return this.toastsSubject.value;
  }
}
