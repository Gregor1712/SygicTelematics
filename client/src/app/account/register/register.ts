import {Component, inject} from '@angular/core';
import {FormBuilder, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {MatCard} from '@angular/material/card';
import {MatFormField, MatInput, MatLabel} from '@angular/material/input';
import {AccountService} from '../../service/account-service';
import {RegisterDto} from '../../service/api-client';
import {Router} from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [
    FormsModule,
    MatButton,
    MatCard,
    MatFormField,
    MatInput,
    MatLabel,
    ReactiveFormsModule
  ],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  private fb = inject(FormBuilder);
  private accountService = inject(AccountService);
  private router = inject(Router);
  passwordMismatch = false;

  registerForm = this.fb.group({
    displayName: this.fb.control<string>('', Validators.required),
    email: this.fb.control<string>('', [Validators.required, Validators.email]),
    password: this.fb.control<string>('', [Validators.required, Validators.minLength(6)]),
    confirmPassword: this.fb.control<string>('', Validators.required)
  })

  protected onSubmit() {
    const form = this.registerForm.getRawValue();

    if (form.password !== form.confirmPassword) {
      this.passwordMismatch = true;
      return;
    }
    this.passwordMismatch = false;

    const register = new RegisterDto({
      displayName: form.displayName!,
      email: form.email!,
      password: form.password!
    });

    this.accountService.register(register).subscribe({
      next: () => {
        this.router.navigateByUrl('/');
      },
      error: (err: unknown) => {
        console.error('Registration failed', err);
      }
    });
  }
}
