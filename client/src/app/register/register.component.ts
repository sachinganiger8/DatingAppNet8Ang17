import { Component, inject, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  model: any = {};
  cancelRegister = output<boolean>();
  accountServicee = inject(AccountService);
  toastr=inject(ToastrService);

  register() {
    this.accountServicee.register(this.model).subscribe({
      next: response => {
        console.log(response);
        this.cancel();
      },
      error: err => this.toastr.error(err.error)
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
