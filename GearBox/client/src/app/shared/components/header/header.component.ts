import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-header',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  authService = inject(AuthService);

  navLinks = [
    { label: 'Shop',     path: '/shop' },
    { label: 'About Us', path: '/about' },
    { label: 'Contact',  path: '/contact' },
  ];
}
