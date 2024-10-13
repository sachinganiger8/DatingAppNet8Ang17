import { Component, inject, OnInit } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { Member } from '../../_models/member';
import { MemberCardComponent } from "../member-card/member-card.component";

@Component({
  selector: 'app-member-list',
  standalone: true,
  imports: [MemberCardComponent],
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.scss'
})
export class MemberListComponent implements OnInit {
  private membersservice=inject(MembersService);
  members:Member[]=[];

  ngOnInit(): void {
    this.getMembers();
  }

  getMembers(){
    this.membersservice.getMembers().subscribe({
      next:members=>this.members=members
    });
  }
}
