import { Component, inject, OnInit, signal } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ProductService } from '../../core/services/product.service';
import { Product } from '../../core/models/product.model';

@Component({
  selector: 'app-shop',
  imports: [CurrencyPipe, MatProgressSpinnerModule],
  templateUrl: './shop.component.html',
})
export class ShopComponent implements OnInit {
  private productService = inject(ProductService);

  products = signal<Product[]>([]);
  totalCount = signal(0);
  loading = signal(true);
  error = signal<string | null>(null);

  ngOnInit() {
    this.productService.getProducts().subscribe({
      next: (res) => {
        this.products.set(res.data);
        this.totalCount.set(res.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load products. Please try again later.');
        this.loading.set(false);
      },
    });
  }
}
