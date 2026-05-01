// user-context.service.ts
import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { UserModel } from "../../api/base-api";

@Injectable({
  providedIn: "root"
})
export class UserContextService {
  private userSubject = new BehaviorSubject<UserModel | null>(null);
  user$ = this.userSubject.asObservable();

  private selectedCompanyIdSubject = new BehaviorSubject<number>(0);
  public selectedCompanyId$: Observable<number> = this.selectedCompanyIdSubject.asObservable();

  private selectedCurrencySubject = new BehaviorSubject<string>("");
  public selectedCurrency$: Observable<string> = this.selectedCurrencySubject.asObservable();

  setUser(user: UserModel): void {
    this.userSubject.next(user);
  }

  getUser(): UserModel | null {
    return this.userSubject.getValue();
  }

  setSelectedCompany(companyId: number, currency: string): void {
    this.selectedCompanyIdSubject.next(companyId);
    this.selectedCurrencySubject.next(currency);
  }

  getSelectedCompanyId(): number {
    return this.selectedCompanyIdSubject.value;
  }

  getSelectedCurrency(): string {
    return this.selectedCurrencySubject.value;
  }
}
