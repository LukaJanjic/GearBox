import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-header',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  navLinks = [
    { label: 'Shop',     path: '/shop' },
    { label: 'About Us', path: '/about' },
    { label: 'Contact',  path: '/contact' },
  ];
}
