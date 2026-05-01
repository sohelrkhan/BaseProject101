import { ApplicationConfig, provideZoneChangeDetection } from "@angular/core";
import { DefaultUrlSerializer, provideRouter, UrlSerializer, UrlTree, withComponentInputBinding } from "@angular/router";
import { provideClientHydration } from "@angular/platform-browser";
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi, withFetch } from "@angular/common/http";
import { provideAnimations } from "@angular/platform-browser/animations";
import { provideToastr } from "ngx-toastr";
import { API_BASE_URL, AuthenticationService } from "../api/base-api";
import { environment } from "../environments/environment";
import { IdentityInterceptor } from "../identity/identity-shared/identity-interceptor";
import { IdentityService } from "../identity/identity-shared/identity.service";
import { routes } from "./app.routes";

export class LowerCaseUrlSerializer extends DefaultUrlSerializer {
  override parse(url: string): UrlTree {
    return super.parse(url.toLowerCase());
  }
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withComponentInputBinding()),
    provideClientHydration(),
    provideHttpClient(withFetch(), withInterceptorsFromDi()),
    provideAnimations(),

    provideToastr({
      timeOut: 3000, // Notification disappears after 3 seconds
      positionClass: "toast-bottom-right", // Position of Toastr messages
      closeButton: true, // Show close button
      progressBar: true, // Show progress bar
      preventDuplicates: true, // Prevent duplicate messages
      newestOnTop: true // New messages appear on top
    }),

    AuthenticationService,
    IdentityService,

    { provide: UrlSerializer, useClass: LowerCaseUrlSerializer },
    { provide: HTTP_INTERCEPTORS, useClass: IdentityInterceptor, multi: true },
    { provide: API_BASE_URL, useValue: environment.coreBaseUrl }
  ]
};