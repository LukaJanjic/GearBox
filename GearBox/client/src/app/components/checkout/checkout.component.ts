import { Component, OnInit, OnDestroy, inject, signal, ElementRef, ViewChild } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { loadStripe, Stripe, StripeElements } from '@stripe/stripe-js';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CartService } from '../../core/services/cart.service';
import { PaymentService } from '../../core/services/payment.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CurrencyPipe, ReactiveFormsModule, RouterLink],
  templateUrl: './checkout.component.html',
})
export class CheckoutComponent implements OnInit, OnDestroy {
  @ViewChild('paymentElement') paymentElementRef!: ElementRef;

  private fb             = inject(FormBuilder);
  private router         = inject(Router);
  cartService            = inject(CartService);
  private paymentService = inject(PaymentService);
  private authService    = inject(AuthService);

  step         = signal<1 | 2 | 3 | 4>(1);
  loading      = signal(false);
  paymentError = signal<string | null>(null);

  private stripe:   Stripe | null = null;
  private elements: StripeElements | null = null;

  addressForm = this.fb.group({
    fullName:   ['', Validators.required],
    address:    ['', Validators.required],
    city:       ['', Validators.required],
    postalCode: ['', Validators.required],
    country:    ['', Validators.required],
  });

  submitted = signal(false);

  ngOnInit() {
    if (!this.authService.isLoggedIn()) {
      this.router.navigateByUrl('/login');
      return;
    }
    if (!this.cartService.cart() || this.cartService.itemCount() === 0) {
      this.router.navigateByUrl('/shop');
    }
  }

  ngOnDestroy() {
    this.elements = null;
    this.stripe   = null;
  }

  goToAddress() { this.step.set(2); }

  async goToPayment() {
    this.submitted.set(true);
    if (this.addressForm.invalid) return;

    this.loading.set(true);
    this.paymentError.set(null);

    this.paymentService.createPaymentIntent().subscribe({
      next: async ({ clientSecret }) => {
        this.stripe = await loadStripe(environment.stripePublishableKey);

        if (!this.stripe) {
          this.paymentError.set('Stripe failed to load.');
          this.loading.set(false);
          return;
        }

        this.elements = this.stripe.elements({
          clientSecret,
          appearance: { theme: 'night', variables: { colorPrimary: '#16a34a' } },
        });

        const paymentEl = this.elements.create('payment');

        this.step.set(3);
        this.loading.set(false);

        // mount after Angular renders the element
        setTimeout(() => {
          paymentEl.mount(this.paymentElementRef.nativeElement);
        }, 0);
      },
      error: (err) => {
        this.paymentError.set(err?.error?.message ?? 'Failed to initialize payment.');
        this.loading.set(false);
      },
    });
  }

  async confirmPayment() {
    if (!this.stripe || !this.elements) return;

    this.loading.set(true);
    this.paymentError.set(null);

    const { error } = await this.stripe.confirmPayment({
      elements: this.elements,
      confirmParams: { return_url: `${window.location.origin}/checkout` },
      redirect: 'if_required',
    });

    if (error) {
      this.paymentError.set(error.message ?? 'Payment failed.');
      this.loading.set(false);
    } else {
      await firstValueFrom(this.cartService.clearCart());
      this.step.set(4);
      this.loading.set(false);
    }
  }

  backToReview()  { this.step.set(1); }
  backToAddress() { this.step.set(2); }
}