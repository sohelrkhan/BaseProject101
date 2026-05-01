import { Injectable } from "@angular/core";
import * as signalR from "@microsoft/signalr";
import { environment } from "../../../environments/environment";

@Injectable({
  providedIn: "root"
})
export class SingalRService {
  private _hubConnection!: signalR.HubConnection;
  private _baseUrl: string = environment.coreBaseUrl;

  public startConnection() {
    let token: string = localStorage.getItem("jwt_token");

    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${this._baseUrl}/hubs/notification`, {
        accessTokenFactory: () => token || ""
      })
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this._hubConnection
      .start()
      .then(() => console.log(""))
      .catch((err) => console.log("", err));
  }

  public onNotificationReceived(callback: () => void) {
    this._hubConnection.on("ReceiveNotification", () => {
      callback();
    });
  }
}
