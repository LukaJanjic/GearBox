import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
})
export class LoginComponent {
  private fb          = inject(FormBuilder);
  private authService = inject(AuthService);
  private router      = inject(Router);

  form = this.fb.group({
    email:    ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });

  submitted = signal(false);
  loading   = signal(false);
  error     = signal<string | null>(null);

  onSubmit() {
    this.submitted.set(true);
    if (this.form.invalid) return;

    const { email, password } = this.form.value;
    this.loading.set(true);
    this.error.set(null);

    this.authService.login(email!, password!).subscribe({
      next: () => this.router.navigateByUrl('/shop'),
      error: (err) => {
        this.error.set(err?.error?.message ?? 'Invalid email or password');
        this.loading.set(false);
      },
    });
  }

  isInvalid(field: string) {
    const c = this.form.get(field);
    return c?.invalid && this.submitted();
  }
}
