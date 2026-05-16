import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';

@Component({
  selector: 'app-header',
  imports: [RouterLink, RouterLinkActive, MatTabsModule],
  templateUrl: './header.component.html',
})
export class HeaderComponent {
  navLinks = [
    { label: 'Shop',     path: '/shop' },
    { label: 'About Us', path: '/about' },
    { label: 'Contact',  path: '/contact' },
  ];
}
