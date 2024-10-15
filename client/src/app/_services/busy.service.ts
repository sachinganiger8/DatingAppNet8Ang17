import { Injectable, inject } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  busyRequestsCount: number = 0;
  private spinnerService = inject(NgxSpinnerService);

  busy() {
    this.busyRequestsCount++;
    this.spinnerService.show(undefined, {
      type: 'ball-scale-ripple-multiple',
      bdColor: 'rgba(255,255,255,0)',
      color: '#32fbe2'
    });
  }

  idle() {
    this.busyRequestsCount--;
    if (this.busyRequestsCount <= 0) {
      this.busyRequestsCount = 0;
      this.spinnerService.hide();
    }
  }
}
