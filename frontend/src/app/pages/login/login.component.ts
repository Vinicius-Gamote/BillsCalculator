import { NgIf } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth.service';

type AuthMode = 'login' | 'register';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [NgIf, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  mode: AuthMode = 'login';
  loading = false;
  error = '';

  readonly form = this.fb.nonNullable.group({
    name: [''],
    email: ['demo@gastos.local', [Validators.required, Validators.email]],
    password: ['Demo@123', [Validators.required, Validators.minLength(6)]]
  });

  constructor(
    private readonly fb: FormBuilder,
    private readonly auth: AuthService,
    private readonly router: Router
  ) {}

  changeMode(mode: AuthMode): void {
    this.mode = mode;
    this.error = '';
    this.form.controls.name.setValidators(mode === 'register' ? [Validators.required] : []);
    this.form.controls.name.updateValueAndValidity();
  }

  submit(): void {
    if (this.form.invalid || this.loading) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.error = '';

    const { name, email, password } = this.form.getRawValue();
    const request = this.mode === 'register'
      ? this.auth.register(name, email, password)
      : this.auth.login(email, password);

    request.subscribe({
      next: () => this.router.navigateByUrl('/dashboard'),
      error: (response) => {
        this.error = response?.error?.detail ?? 'Nao foi possivel autenticar.';
        this.loading = false;
      },
      complete: () => {
        this.loading = false;
      }
    });
  }
}
