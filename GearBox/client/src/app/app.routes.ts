import { Routes } from '@angular/router';
import { ShopComponent } from './components/shop/shop.component';
import { AboutComponent } from './components/about/about.component';
import { ContactComponent } from './components/contact/contact.component';

export const routes: Routes = [
  { path: 'shop',    component: ShopComponent },
  { path: 'about',   component: AboutComponent },
  { path: 'contact', component: ContactComponent },
  { path: '',        redirectTo: 'shop', pathMatch: 'full' },
  { path: '**',      redirectTo: 'shop' },
];
