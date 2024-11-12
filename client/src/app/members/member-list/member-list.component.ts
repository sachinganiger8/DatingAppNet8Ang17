import { Component, inject, OnInit } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { MemberCardComponent } from "../member-card/member-card.component";
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { FormsModule } from '@angular/forms';
import { ButtonsModule } from 'ngx-bootstrap/buttons'

@Component({
  selector: 'app-member-list',
  standalone: true,
  imports: [MemberCardComponent, PaginationModule, FormsModule, ButtonsModule],
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.scss'
})
export class MemberListComponent implements OnInit {
  membersservice = inject(MembersService);
  gnderList = [{ value: "male", display: "Males" }, { value: "female", display: "Females" }];

  ngOnInit(): void {
    if (!this.membersservice.paginatedResult()) this.loadMembers();
  }

  loadMembers() {
    this.membersservice.getMembers();
  }

  pageChanged(event: any) {
    if (this.membersservice.userParams().pageNumber !== event.page) {
      this.membersservice.userParams().pageNumber = event.page;
      this.loadMembers();
    }
  }

  resetFilters() {
    this.membersservice.resetParams();
    this.loadMembers();
  }
}
