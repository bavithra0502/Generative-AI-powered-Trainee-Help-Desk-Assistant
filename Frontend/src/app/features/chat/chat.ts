import { CommonModule } from '@angular/common';
import { Component, ElementRef, ViewChild, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ChatService } from '../../core/services/chat.service';
import { ChatMessage } from '../../core/models/chat.model';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat.html',
  styleUrl: './chat.scss'
})
export class ChatComponent {
  private readonly chatService = inject(ChatService);

  @ViewChild('scrollAnchor') private scrollAnchor?: ElementRef<HTMLDivElement>;

  protected readonly suggestedQuestions: string[] = [
    'What happens if I miss a training session?',
    'Can I collaborate with someone on an assignment?',
    'How are trainees assessed during the program?',
    'Who do I contact for a technical support issue?'
  ];

  protected readonly messages = signal<ChatMessage[]>([
    {
      role: 'assistant',
      text:
        "Hi! I'm your Trainee Help Desk Assistant. Ask me anything about the training " +
        'program, attendance, assignments, projects, learning methodology, assessments, ' +
        'training guidelines, or technical support.',
      timestamp: new Date()
    }
  ]);

  protected readonly questionText = signal('');
  protected readonly isLoading = signal(false);
  protected readonly errorMessage = signal('');

  protected askQuestion(question?: string): void {
    const trimmedQuestion = (question ?? this.questionText()).trim();

    if (!trimmedQuestion || this.isLoading()) {
      return;
    }

    this.errorMessage.set('');

    this.messages.update((current) => [
      ...current,
      { role: 'trainee', text: trimmedQuestion, timestamp: new Date() }
    ]);

    this.questionText.set('');
    this.isLoading.set(true);
    this.scrollToBottomSoon();

    this.chatService.askQuestion(trimmedQuestion).subscribe({
      next: (response) => {
        this.messages.update((current) => [
          ...current,
          {
            role: 'assistant',
            text: response.answer,
            sources: response.sources,
            answerFoundInKnowledgeBase: response.answerFoundInKnowledgeBase,
            timestamp: new Date()
          }
        ]);
        this.isLoading.set(false);
        this.scrollToBottomSoon();
      },
      error: () => {
        this.errorMessage.set(
          "Sorry, I couldn't reach the Help Desk Assistant service. Please try again in a moment."
        );
        this.isLoading.set(false);
      }
    });
  }

  protected onEnterKey(event: Event): void {
    event.preventDefault();
    this.askQuestion();
  }

  private scrollToBottomSoon(): void {
    setTimeout(() => {
      this.scrollAnchor?.nativeElement.scrollIntoView({ behavior: 'smooth' });
    }, 50);
  }
}
