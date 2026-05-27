import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CurrencyPipe } from '@angular/common';
import { ProductService } from '../../core/services/product.service';
import { CartService } from '../../core/services/cart.service';
import { Product } from '../../core/models/product.model';

@Component({
  selector: 'app-product-detail',
  imports: [RouterLink, CurrencyPipe],
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.scss',
})
export class ProductDetailComponent implements OnInit {
  private route          = inject(ActivatedRoute);
  private productService = inject(ProductService);
  private cartService    = inject(CartService);

  product  = signal<Product | null>(null);
  loading  = signal(true);
  error    = signal<string | null>(null);
  adding   = signal(false);

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.productService.getProduct(id).subscribe({
      next:  (p) => { this.product.set(p); this.loading.set(false); },
      error: ()  => { this.error.set('Product not found.'); this.loading.set(false); },
    });
  }

  addToCart(p: Product) {
    this.adding.set(true);
    this.cartService.addItem({
      productId:  p.id,
      name:       p.name,
      price:      p.price,
      quantity:   1,
      pictureUrl: p.pictureUrl,
      brand:      p.brand,
    }).subscribe({
      next: () => {
        this.adding.set(false);
        this.cartService.openCart();
      },
      error: (err) => { this.adding.set(false); console.error('Cart error:', err); },
    });
  }
}
