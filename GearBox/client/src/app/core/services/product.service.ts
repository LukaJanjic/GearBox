import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Product } from '../models/product.model';
import { Pagination } from '../models/pagination.model';

export interface ProductParams {
  pageIndex?: number;
  pageSize?: number;
  brands?: string[];
  categories?: string[];
  search?: string;
  minPrice?: number;
  maxPrice?: number;
  sort?: string;
}

@Injectable({ providedIn: 'root' })
export class ProductService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/products`;

  getProducts(params: ProductParams = {}): Observable<Pagination<Product>> {
    let httpParams = new HttpParams()
      .set('pageIndex', params.pageIndex ?? 1)
      .set('pageSize', params.pageSize ?? 10);

    params.brands?.forEach(b => httpParams = httpParams.append('brands', b));
    params.categories?.forEach(c => httpParams = httpParams.append('categories', c));

    if (params.search)   httpParams = httpParams.set('search', params.search);
    if (params.minPrice) httpParams = httpParams.set('minPrice', params.minPrice);
    if (params.maxPrice) httpParams = httpParams.set('maxPrice', params.maxPrice);
    if (params.sort)     httpParams = httpParams.set('sort', params.sort);

    return this.http.get<Pagination<Product>>(this.apiUrl, { params: httpParams });
  }

  getProduct(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/${id}`);
  }
}
