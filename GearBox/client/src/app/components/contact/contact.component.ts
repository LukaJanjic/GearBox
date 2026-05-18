import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RevealDirective } from '../../shared/directives/reveal.directive';

@Component({
  selector: 'app-contact',
  standalone: true,
  imports: [ReactiveFormsModule, RevealDirective],
  templateUrl: './contact.component.html',
  styleUrl: './contact.component.scss',
})
export class ContactComponent {
  private fb = inject(FormBuilder);

  form = this.fb.group({
    name:    ['', [Validators.required, Validators.minLength(2)]],
    email:   ['', [Validators.required, Validators.email]],
    subject: ['', [Validators.required, Validators.minLength(3)]],
    message: ['', [Validators.required, Validators.minLength(10)]],
  });

  submitted = signal(false);
  loading   = signal(false);
  success   = signal(false);

  onSubmit() {
    this.submitted.set(true);
    if (this.form.invalid) return;
    this.loading.set(true);
    setTimeout(() => {
      this.loading.set(false);
      this.success.set(true);
      this.form.reset();
      this.submitted.set(false);
      setTimeout(() => this.success.set(false), 6000);
    }, 1500);
  }

  isInvalid(field: string): boolean {
    const c = this.form.get(field);
    return !!(c?.invalid && this.submitted());
  }

  hasError(field: string, error: string): boolean {
    return !!(this.form.get(field)?.hasError(error) && this.submitted());
  }

  readonly hours = [
    { day: 'Monday – Friday', time: '08:00 – 17:00', open: true },
    { day: 'Saturday',        time: '09:00 – 14:00', open: true },
    { day: 'Sunday',          time: 'Closed',         open: false },
  ];
}
