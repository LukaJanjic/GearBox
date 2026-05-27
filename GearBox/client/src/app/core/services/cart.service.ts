import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, from } from 'rxjs';
import { tap, concatMap, last, map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { CartItem, ShoppingCart } from '../models/cart.model';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class CartService {
  private http        = inject(HttpClient);
  private authService = inject(AuthService);
  private apiUrl      = `${environment.apiUrl}/cart`;
  private readonly GUEST_KEY = 'gb_guest_cart';

  private _cart = signal<ShoppingCart | null>(null);

  cart      = this._cart.asReadonly();
  itemCount = computed(() => this._cart()?.itemCount ?? 0);
  total     = computed(() => this._cart()?.total ?? 0);

  isOpen = signal(false);

  openCart()   { this.isOpen.set(true); }
  closeCart()  { this.isOpen.set(false); }
  toggleCart() { this.isOpen.update(v => !v); }

  loadCart(): Observable<ShoppingCart> {
    return this.http.get<ShoppingCart>(this.apiUrl).pipe(
      tap(cart => this._cart.set(cart))
    );
  }

  loadGuestCart(): void {
    const raw = localStorage.getItem(this.GUEST_KEY);
    if (raw) this._cart.set(JSON.parse(raw));
  }

  addItem(item: CartItem): Observable<ShoppingCart> {
    if (this.authService.isLoggedIn()) {
      return this.http.post<ShoppingCart>(`${this.apiUrl}/items`, item).pipe(
        tap(cart => this._cart.set(cart))
      );
    }
    return this.patchGuest(cart => {
      const existing = cart.items.find(i => i.productId === item.productId);
      if (existing) existing.quantity += item.quantity;
      else cart.items.push({ ...item });
    });
  }

  updateQuantity(productId: number, quantity: number): Observable<ShoppingCart> {
    if (this.authService.isLoggedIn()) {
      return this.http.put<ShoppingCart>(`${this.apiUrl}/items/${productId}`, quantity).pipe(
        tap(cart => this._cart.set(cart))
      );
    }
    return this.patchGuest(cart => {
      const idx = cart.items.findIndex(i => i.productId === productId);
      if (idx !== -1) {
        if (quantity <= 0) cart.items.splice(idx, 1);
        else cart.items[idx].quantity = quantity;
      }
    });
  }

  removeItem(productId: number): Observable<ShoppingCart> {
    if (this.authService.isLoggedIn()) {
      return this.http.delete<ShoppingCart>(`${this.apiUrl}/items/${productId}`).pipe(
        tap(cart => this._cart.set(cart))
      );
    }
    return this.patchGuest(cart => {
      cart.items = cart.items.filter(i => i.productId !== productId);
    });
  }

  clearCart(): Observable<void> {
    if (this.authService.isLoggedIn()) {
      return this.http.delete<void>(this.apiUrl).pipe(
        tap(() => this._cart.set(null))
      );
    }
    localStorage.removeItem(this.GUEST_KEY);
    this._cart.set(null);
    return of(void 0);
  }

  syncGuestCartOnLogin(): Observable<void> {
    const raw = localStorage.getItem(this.GUEST_KEY);
    localStorage.removeItem(this.GUEST_KEY);
    if (!raw) return of(void 0);

    const guestCart: ShoppingCart = JSON.parse(raw);
    if (!guestCart.items.length) return of(void 0);

    return from(guestCart.items).pipe(
      concatMap(item => this.http.post<ShoppingCart>(`${this.apiUrl}/items`, item)),
      last(),
      tap(cart => this._cart.set(cart)),
      map(() => void 0)
    );
  }

  private patchGuest(fn: (cart: ShoppingCart) => void): Observable<ShoppingCart> {
    const current = this._cart() ?? { userId: 'guest', items: [], total: 0, itemCount: 0 };
    const cart: ShoppingCart = { ...current, items: current.items.map(i => ({ ...i })) };
    fn(cart);
    cart.total     = cart.items.reduce((s, i) => s + i.price * i.quantity, 0);
    cart.itemCount = cart.items.reduce((s, i) => s + i.quantity, 0);
    this._cart.set(cart);
    localStorage.setItem(this.GUEST_KEY, JSON.stringify(cart));
    return of(cart);
  }
}
