import { Injectable } from '@angular/core';
import { UserModel } from '../../api/base-api';

@Injectable({
  providedIn: 'root'
})

export class SidebarPermissionService {

  // Store access
  private accessMap = new Map<string, Set<string>>();

  constructor() { }

  initialize(user: UserModel): void {
    this.accessMap.clear();

    if (!user.userAccessMappingDetails) {
      return;
    }

    for (const access of user.userAccessMappingDetails) {

      // Get feature and function name
      const feature: string = access.featureName?.toLowerCase().trim();
      const action: string = access.actionName?.toLowerCase().trim();

      if (!feature || !action) {
        continue;
      }

      if (!this.accessMap.has(feature)) {
        this.accessMap.set(feature, new Set());
      }

      this.accessMap.get(feature)?.add(action);
    }
  }

  hasAccess(featureName: string, actionName: string): boolean {

    // Get feature and function name
    const feature = featureName.toLowerCase().trim();
    const action = actionName.toLowerCase().trim();

    return this.accessMap.has(feature) && this.accessMap.get(feature)?.has(action) === true;
  }

  hasAnyAccessToFeature(featureNames: string[]): boolean {
    if (!featureNames || featureNames.length === 0) {
      return false;
    }

    for (const name of featureNames) {
      const feature = name.toLowerCase().trim();
      if (this.accessMap.has(feature) && (this.accessMap.get(feature)?.size ?? 0) > 0) {
        return true;
      }
    }

    return false;
  }
}