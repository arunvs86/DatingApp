import { ToastrService } from 'ngx-toastr';
import { AccountService } from './../../../_services/account.service';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  @Output() cancelButton = new EventEmitter();

  model: any = {};
  constructor(
    private accountService: AccountService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {}

  register() {
    return this.accountService.register(this.model).subscribe(
      (response) => {
        console.log(response);
        this.cancel();
      },
      (error) => {
        this.toastr.error(error.error);
        console.log(error);
      }
    );
  }

  cancel() {
    this.cancelButton.emit(false);
    console.log('cancelled');
  }
}
