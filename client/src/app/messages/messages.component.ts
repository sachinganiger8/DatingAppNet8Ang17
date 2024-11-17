import { Component, inject, OnInit } from '@angular/core';
import { MessageService } from '../_services/message.service';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { TimeagoModule } from 'ngx-timeago';
import { FormsModule } from '@angular/forms';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { RouterLink } from '@angular/router';
import { Message } from '../_models/message';

@Component({
  selector: 'app-messages',
  standalone: true,
  imports: [ButtonsModule, TimeagoModule, FormsModule, PaginationModule, RouterLink],
  templateUrl: './messages.component.html',
  styleUrl: './messages.component.scss'
})
export class MessagesComponent implements OnInit {
  messageService = inject(MessageService);

  container = "Inbox";
  pageNumber: number = 1;
  pageSize: number = 5;
  isOutbox = this.container == "Outbox";

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages() {
    this.messageService.getMessages(this.pageNumber, this.pageSize, this.container);
  }

  getRoute(message: Message) {
    return this.container == "Outbox" ? `/members/${message.recipientUsername}` : `/members/${message.senderUsername}`;
  }

  pageChanged(event: any) {
    if (this.pageNumber != event.page) {
      this.pageNumber = event.page;
      this.loadMessages();
    }
  }

  deleteMesssage(id: number) {
    this.messageService.deleteMessage(id).subscribe({
      next: _ => {
        this.messageService.paginatedResult.update(prev => {
          if (prev && prev.items) {
            prev.items.splice(prev.items.findIndex(x => x.id == id), 1);
            return prev;
          }
          else
            return prev;
        })
      }
    });
  }
}
