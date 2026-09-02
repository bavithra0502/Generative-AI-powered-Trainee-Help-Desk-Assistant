export interface AskRequest {
  question: string;
}

export interface AskResponse {
  question: string;
  answer: string;
  sources: string[];
  answerFoundInKnowledgeBase: boolean;
}

export interface ChatHistoryItem {
  id: number;
  question: string;
  answer: string;
  createdAtUtc: string;
}

export interface ChatMessage {
  role: 'trainee' | 'assistant';
  text: string;
  sources?: string[];
  answerFoundInKnowledgeBase?: boolean;
  timestamp: Date;
}
