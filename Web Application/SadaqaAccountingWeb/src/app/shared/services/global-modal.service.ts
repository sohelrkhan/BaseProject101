import { Injectable } from "@angular/core";
import { Subject } from "rxjs";

@Injectable({
  providedIn: "root"
})
export class GlobalModalService {
  private modalTrigger = new Subject<any>();

  modalTriggered$ = this.modalTrigger.asObservable();

  openModal(data: any) {
    this.modalTrigger.next(data);
  }
  constructor() {}
}
