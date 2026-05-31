import { Component, inject, computed } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CartService } from '../../core/services/cart.service';
import { AuthService } from '../../core/services/auth.service';
import { CartItem } from '../../core/models/cart.model';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CurrencyPipe, RouterLink],
  templateUrl: './cart.component.html',
})
export class CartComponent {
  cartService = inject(CartService);
  private authService = inject(AuthService);

  isLoggedIn = computed(() => this.authService.isLoggedIn());

  increment(item: CartItem) {
    this.cartService.updateQuantity(item.productId, item.quantity + 1).subscribe();
  }

  decrement(item: CartItem) {
    if (item.quantity <= 1) {
      this.cartService.removeItem(item.productId).subscribe();
    } else {
      this.cartService.updateQuantity(item.productId, item.quantity - 1).subscribe();
    }
  }

  remove(productId: number) {
    this.cartService.removeItem(productId).subscribe();
  }

  clear() {
    this.cartService.clearCart().subscribe();
  }
}
