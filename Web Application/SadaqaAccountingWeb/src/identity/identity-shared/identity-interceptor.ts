import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { catchError, Observable, throwError } from "rxjs";
import { IdentityService } from "./identity.service";
import { Router } from "@angular/router";

@Injectable()
export class IdentityInterceptor implements HttpInterceptor {

  constructor(private identityService: IdentityService, private router: Router) { }

  handleError(httpErrorResponse: HttpErrorResponse) {
    // Pass the entire error object downstream without wrapping in generic Error
    return throwError(() => httpErrorResponse);
  }

  intercept(httpRequest: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const jwtToken = this.identityService.getJwtToken();

    if (jwtToken && jwtToken.trim() !== "") {
      httpRequest = this.addToken(httpRequest, jwtToken);
    }

    return next.handle(httpRequest).pipe(
      catchError((error) => {
        if (error instanceof HttpErrorResponse) {
          if (error.status === 401) {
            this.identityService.logout();
          } else if (error.status === 403) {
            this.router.navigate(['/app/access-denied']);
          }
        }

        return this.handleError(error);
      })
    );
  }

  private addToken(request: HttpRequest<any>, token: string) {
    return request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }
}