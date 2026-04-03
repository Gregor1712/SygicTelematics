import {Component, inject} from '@angular/core';
import {FormBuilder, ReactiveFormsModule} from '@angular/forms';
import {MatCard} from '@angular/material/card';
import {MatFormField, MatInput, MatLabel} from '@angular/material/input';
import {MatButton} from '@angular/material/button';
import {ActivatedRoute, Router} from '@angular/router';
import {AccountClient, LoginDto, UserDto} from '../../service/api-client';
import {AccountService} from '../../service/account-service';

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
    MatCard,
    MatFormField,
    MatInput,
    MatButton,
    MatLabel],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  private fb = inject(FormBuilder);
  private accountService = inject(AccountService);
  private router = inject(Router);
  private activatedRoute = inject(ActivatedRoute);
  returnUrl = '/products';
  //userAccount: UserDto = new UserDto();

  constructor() {
    const url = this.activatedRoute.snapshot.queryParams['returnUrl'];
    if (url) this.returnUrl = url;
  }

  loginForm = this.fb.group({
    email: this.fb.control<string | null>(null),
    password: this.fb.control<string | null>(null)
  })

  onSubmit() {

    const form = this.loginForm.getRawValue();

    const login = new LoginDto({
      email: form.email ?? undefined,
      password: form.password ?? undefined
    });


    this.accountService.login(login).subscribe({
      next: () => {
        console.log('currentUser signal:', this.accountService.currentUser());
        this.router.navigateByUrl(this.returnUrl);
      },
      error: () => this.router.navigate(['/'])
    });

    //this.accountService.login(login);

    // if(this.accountService.currentUser()?.token !== undefined ){
    //   console.log('currentUser signal:', this.accountService.currentUser());
    //   this.router.navigateByUrl(this.returnUrl);
    // }else{
    //   console.log('login failed:', this.accountService.currentUser());
    //   this.router.navigate(['/']);
    // }



    // {
    //   this.router.navigateByUrl(this.returnUrl);
    // }

  }
}
