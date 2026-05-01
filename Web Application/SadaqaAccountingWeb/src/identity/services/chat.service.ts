import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private apiUrl = 'https://localhost:7153/api/messages';

    constructor(private http: HttpClient) {}

    sendMessage(userMessage: string): Observable<any> {
        const activity = {
            type: 'message',
            from: { id: 'user1', name: 'User' },
            text: userMessage
        };
        return this.http.post(this.apiUrl, activity);
    }
}
