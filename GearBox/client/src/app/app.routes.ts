import { Routes } from '@angular/router';
import { ShopComponent }          from './components/shop/shop.component';
import { AboutComponent }         from './components/about/about.component';
import { ContactComponent }       from './components/contact/contact.component';
import { LoginComponent }         from './components/login/login.component';
import { RegisterComponent }      from './components/register/register.component';
import { ProductDetailComponent } from './components/product-detail/product-detail.component';
import { CheckoutComponent }      from './components/checkout/checkout.component';

export const routes: Routes = [
  { path: 'shop',           component: ShopComponent },
  { path: 'product/:id',    component: ProductDetailComponent },
  { path: 'checkout',       component: CheckoutComponent },
  { path: 'about',          component: AboutComponent },
  { path: 'contact',        component: ContactComponent },
  { path: 'login',          component: LoginComponent },
  { path: 'register',       component: RegisterComponent },
  { path: '',               redirectTo: 'shop', pathMatch: 'full' },
  { path: '**',             redirectTo: 'shop' },
];
