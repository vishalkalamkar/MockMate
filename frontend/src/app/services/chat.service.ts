import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Message {
  role: 'user' | 'assistant';
  content: string;
}

@Injectable({ providedIn: 'root' })
export class ChatService {
  private apiUrl = 'http://localhost:5038/api/chat';

  constructor(private http: HttpClient) {}

  sendMessage(message: string, history: Message[]): Observable<any> {
    return this.http.post(this.apiUrl, { message, history });
  }
}