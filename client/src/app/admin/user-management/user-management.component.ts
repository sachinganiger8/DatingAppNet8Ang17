import { Component, Inject, inject, OnInit } from '@angular/core';
import { AdminService } from '../../_services/admin.service';
import { User } from '../../_models/user';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { RolesModalComponent } from '../../modals/roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.scss'
})
export class UserManagementComponent implements OnInit {
  private adminService = inject(AdminService);
  private modalService = inject(BsModalService);
  bsModalRef: BsModalRef<RolesModalComponent> = new BsModalRef<RolesModalComponent>();
  users: User[] = [];

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    this.adminService.getUserWithRoles().subscribe({
      next: users => this.users = users
    });
  }

  openRolesModal(user: User) {
    const initialStatte: ModalOptions = {
      class: "modal-lg",
      initialState: {
        title: "User Roles",
        username: user.username,
        selectedRoles: [...user.roles],
        availableRoles: ["Admin", "Moderator", "Member"],
        users: this.users,
        rolesUpdated: false
      }
    }

    this.bsModalRef = this.modalService.show(RolesModalComponent, initialStatte);
    this.bsModalRef.onHide?.subscribe({
      next: () => {
        if (this.bsModalRef.content && this.bsModalRef.content.rolesUpdated) {
          const selectedRoles = this.bsModalRef.content.selectedRoles;
          this.adminService.updateUserRoles(user.username, selectedRoles).subscribe({
            next: roles => user.roles = roles
          });
        }
      }
    });
  }
}
