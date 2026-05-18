import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ProductService } from '../../core/services/product.service';
import { BrandService } from '../../core/services/brand.service';
import { CategoryService } from '../../core/services/category.service';
import { Product } from '../../core/models/product.model';
import { Brand } from '../../core/models/brand.model';
import { Category } from '../../core/models/category.model';

@Component({
  selector: 'app-shop',
  imports: [CurrencyPipe, FormsModule, MatProgressSpinnerModule],
  templateUrl: './shop.component.html',
})
export class ShopComponent implements OnInit {
  private productService  = inject(ProductService);
  private brandService    = inject(BrandService);
  private categoryService = inject(CategoryService);

  products   = signal<Product[]>([]);
  brands     = signal<Brand[]>([]);
  categories = signal<Category[]>([]);

  selectedBrands     = signal<string[]>([]);
  selectedCategories = signal<string[]>([]);
  search   = signal('');
  minPrice = signal<number | null>(null);
  maxPrice = signal<number | null>(null);
  sort     = signal('nameAsc');

  totalCount = signal(0);
  pageIndex  = signal(1);
  pageSize   = signal(10);
  loading    = signal(true);
  error      = signal<string | null>(null);

  totalPages  = computed(() => Math.ceil(this.totalCount() / this.pageSize()));
  pageNumbers = computed(() => {
    const total = this.totalPages(), current = this.pageIndex();
    const pages: number[] = [];
    for (let i = Math.max(1, current - 2); i <= Math.min(total, current + 2); i++) pages.push(i);
    return pages;
  });

  hasActiveFilters = computed(() =>
    this.selectedBrands().length > 0 ||
    this.selectedCategories().length > 0 ||
    this.search().trim().length > 0 ||
    this.minPrice() !== null ||
    this.maxPrice() !== null
  );

  sortOptions = [
    { value: 'nameAsc',   label: 'Name A–Z' },
    { value: 'nameDesc',  label: 'Name Z–A' },
    { value: 'priceAsc',  label: 'Price: Low to High' },
    { value: 'priceDesc', label: 'Price: High to Low' },
  ];

  ngOnInit() {
    this.brandService.getBrands().subscribe(b => this.brands.set(b));
    this.categoryService.getCategories().subscribe(c => this.categories.set(c));
    this.loadProducts();
  }

  loadProducts() {
    this.loading.set(true);
    this.error.set(null);
    this.productService.getProducts({
      pageIndex:  this.pageIndex(),
      pageSize:   this.pageSize(),
      brands:     this.selectedBrands(),
      categories: this.selectedCategories(),
      search:     this.search().trim() || undefined,
      minPrice:   this.minPrice() ?? undefined,
      maxPrice:   this.maxPrice() ?? undefined,
      sort:       this.sort(),
    }).subscribe({
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

  private resetPage() {
    this.pageIndex.set(1);
    this.loadProducts();
  }

  onSearchChange(value: string) {
    this.search.set(value);
    this.resetPage();
  }

  onSortChange(value: string) {
    this.sort.set(value);
    this.pageIndex.set(1);
    this.loadProducts();
  }

  onMinPriceChange(value: number | null) {
    this.minPrice.set(value === null || isNaN(value as number) ? null : value);
    this.resetPage();
  }

  onMaxPriceChange(value: number | null) {
    this.maxPrice.set(value === null || isNaN(value as number) ? null : value);
    this.resetPage();
  }

  clearPriceFilter() {
    this.minPrice.set(null);
    this.maxPrice.set(null);
    this.resetPage();
  }

  clearSearchFilter() {
    this.search.set('');
    this.resetPage();
  }

  toggleBrand(name: string) {
    const cur = this.selectedBrands();
    this.selectedBrands.set(cur.includes(name) ? cur.filter(b => b !== name) : [...cur, name]);
    this.resetPage();
  }

  toggleCategory(name: string) {
    const cur = this.selectedCategories();
    this.selectedCategories.set(cur.includes(name) ? cur.filter(c => c !== name) : [...cur, name]);
    this.resetPage();
  }

  clearFilters() {
    this.selectedBrands.set([]);
    this.selectedCategories.set([]);
    this.search.set('');
    this.minPrice.set(null);
    this.maxPrice.set(null);
    this.pageIndex.set(1);
    this.loadProducts();
  }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages() || page === this.pageIndex()) return;
    this.pageIndex.set(page);
    this.loadProducts();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}
