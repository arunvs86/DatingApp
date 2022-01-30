import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { User } from './../../../_models/User';
import { AccountService } from './../../../_services/account.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  model: any = {};
  username: string;
  constructor(
    public accountService: AccountService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {}

  login() {
    this.accountService.login(this.model).subscribe(
      (response) => {
        this.router.navigateByUrl('/members');
        this.username = response.username;
      },
      (error) => {
        this.toastr.error(error.error);
        console.log(error);
      }
    );
  }

  logout() {
    this.router.navigateByUrl('/');
    this.accountService.logout();
  }
}
