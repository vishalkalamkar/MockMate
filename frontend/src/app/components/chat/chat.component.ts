import { Component } from '@angular/core';
import { ChatService, Message } from '../../services/chat.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.scss'
})
export class ChatComponent {
  messages: Message[] = [];
  userInput = '';
  isLoading = false;

  constructor(private chatService: ChatService) {}

  sendMessage() {
    if (!this.userInput.trim()) return;

    const userMsg: Message = { role: 'user', content: this.userInput };
    this.messages.push(userMsg);
    this.isLoading = true;
    const input = this.userInput;
    this.userInput = '';

    this.chatService.sendMessage(input, this.messages).subscribe({
      next: (res) => {
        this.messages.push({ role: 'assistant', content: res.reply });
        this.isLoading = false;
      },
      error: () => {
        this.messages.push({ 
          role: 'assistant', 
          content: '⚠️ Error connecting to MockMate. Please try again.' 
        });
        this.isLoading = false;
      }
    });
  }

  onEnter(event: KeyboardEvent) {
    if (event.key === 'Enter') this.sendMessage();
  }
}