import { Directive, Input, TemplateRef, ViewContainerRef } from "@angular/core";
import { AccessControlService } from "../services/access-control.service";

@Directive({
  selector: "[appCheckPermission]",
  standalone: true
})

export class CheckPermissionDirective {

  @Input() set appCheckPermission(permission: { feature: string; action: string | string[] }) {
   
    if (this.accessControlService.hasAccess(permission.feature, permission.action)) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainer.clear();
    }
  }

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private accessControlService: AccessControlService
  ) {}
}
