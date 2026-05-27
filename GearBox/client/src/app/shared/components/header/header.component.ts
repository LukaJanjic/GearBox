import { Component, computed, inject, OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { CartService } from '../../../core/services/cart.service';

@Component({
  selector: 'app-header',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent implements OnInit {
  authService = inject(AuthService);
  cartService = inject(CartService);

  cartBadgeVisible = computed(() => this.cartService.itemCount() !== 0);
  cartBadgeLabel   = computed(() => {
    const n = this.cartService.itemCount();
    return n >= 10 ? '9+' : String(n);
  });

  ngOnInit() {
    if (this.authService.isLoggedIn()) {
      this.cartService.loadCart().subscribe();
    } else {
      this.cartService.loadGuestCart();
    }
  }

  navLinks = [
    { label: 'Shop',     path: '/shop' },
    { label: 'About Us', path: '/about' },
    { label: 'Contact',  path: '/contact' },
  ];
}
