import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PaymentIntentResponse } from '../models/payment.model';

@Injectable({ providedIn: 'root' })
export class PaymentService {
  private http   = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/payment`;

  createPaymentIntent(guestTotal?: number): Observable<PaymentIntentResponse> {
    const body = guestTotal !== undefined ? { amount: guestTotal } : {};
    return this.http.post<PaymentIntentResponse>(`${this.apiUrl}/create-intent`, body);
  }
}