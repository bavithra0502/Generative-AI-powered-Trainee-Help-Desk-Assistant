import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AskRequest, AskResponse, ChatHistoryItem } from '../models/chat.model';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/chat`;
  private readonly knowledgeBaseUrl = `${environment.apiBaseUrl}/knowledgebase`;

  // Sends a trainee's question to the RAG backend and returns the generated answer.
  askQuestion(question: string): Observable<AskResponse> {
    const request: AskRequest = { question };
    return this.http.post<AskResponse>(`${this.baseUrl}/ask`, request);
  }

  // Retrieves recent chat history from the backend (persisted via EF Core / SQL Server).
  getHistory(take: number = 20): Observable<ChatHistoryItem[]> {
    return this.http.get<ChatHistoryItem[]>(`${this.baseUrl}/history?take=${take}`);
  }

  // Triggers a (re)build of the Trainee Knowledge Base on the backend.
  buildKnowledgeBase(): Observable<{ message: string; totalChunks: number }> {
    return this.http.post<{ message: string; totalChunks: number }>(`${this.knowledgeBaseUrl}/build`, {});
  }

  // Lists the knowledge base source documents available on the backend.
  getKnowledgeBaseDocuments(): Observable<string[]> {
    return this.http.get<string[]>(`${this.knowledgeBaseUrl}/documents`);
  }
}
