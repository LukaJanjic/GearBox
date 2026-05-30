import { Component, OnInit, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';

interface Stat {
  label: string;
  target: number;
  suffix: string;
  current: number;
}

@Component({
  selector: 'app-about',
  standalone: true,
  imports: [RouterLink, DecimalPipe],
  templateUrl: './about.component.html',
  styles: [`
    @keyframes fadeUp {
      from { opacity: 0; transform: translateY(40px); }
      to   { opacity: 1; transform: translateY(0); }
    }
    @keyframes fadeIn {
      from { opacity: 0; }
      to   { opacity: 1; }
    }
    @keyframes rotateSlow {
      from { transform: rotate(0deg); }
      to   { transform: rotate(360deg); }
    }
    @keyframes glowPulse {
      0%, 100% { opacity: 0.4; transform: scale(1); }
      50%       { opacity: 0.7; transform: scale(1.05); }
    }
    @keyframes slideRight {
      from { width: 0; }
      to   { width: 100%; }
    }
    .anim-fade-up   { animation: fadeUp 0.9s ease forwards; }
    .anim-fade-up-1 { animation: fadeUp 0.9s ease 0.15s forwards; opacity: 0; }
    .anim-fade-up-2 { animation: fadeUp 0.9s ease 0.30s forwards; opacity: 0; }
    .anim-fade-up-3 { animation: fadeUp 0.9s ease 0.45s forwards; opacity: 0; }
    .anim-fade-up-4 { animation: fadeUp 0.9s ease 0.60s forwards; opacity: 0; }
    .anim-fade-up-5 { animation: fadeUp 0.9s ease 0.75s forwards; opacity: 0; }
    .rotate-slow    { animation: rotateSlow 25s linear infinite; }
    .glow-blob      { animation: glowPulse 3s ease-in-out infinite; }
    .card-lift      { transition: transform 0.3s ease, box-shadow 0.3s ease; }
    .card-lift:hover { transform: translateY(-6px); box-shadow: 0 24px 48px rgba(0,0,0,0.4); }
    .line-grow::after {
      content: '';
      display: block;
      height: 3px;
      background: #16a34a;
      animation: slideRight 1s ease 0.5s forwards;
      width: 0;
    }
  `],
})
export class AboutComponent implements OnInit {
  stats = signal<Stat[]>([
    { label: 'Products in Stock',   target: 2400,  suffix: '+', current: 0 },
    { label: 'Brands Available',    target: 80,    suffix: '+', current: 0 },
    { label: 'Orders Delivered',    target: 15000, suffix: '+', current: 0 },
    { label: 'Years of Experience', target: 12,    suffix: '',  current: 0 },
  ]);

  values = [
    {
      icon: 'M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z',
      title: 'Quality First',
      desc: 'Every part we sell goes through rigorous quality checks. We only stock components from manufacturers that meet our strict standards.',
    },
    {
      icon: 'M13 10V3L4 14h7v7l9-11h-7z',
      title: 'Fast Delivery',
      desc: 'Same-day dispatch on all orders placed before 2PM. Our logistics network ensures your parts arrive when you need them.',
    },
    {
      icon: 'M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0z',
      title: 'Expert Support',
      desc: 'Our team of certified mechanics is available 7 days a week to help you find the right part for your specific vehicle.',
    },
    {
      icon: 'M4 7v10c0 2.21 3.582 4 8 4s8-1.79 8-4V7M4 7c0 2.21 3.582 4 8 4s8-1.79 8-4M4 7c0-2.21 3.582-4 8-4s8 1.79 8 4',
      title: 'Genuine Parts',
      desc: 'We are an authorised distributor for all major brands. Every OEM and aftermarket part comes with full manufacturer warranty.',
    },
  ];

  ngOnInit() {
    this.animateCounters();
  }

  private animateCounters() {
    const duration = 2000;
    const steps    = 60;
    const interval = duration / steps;

    const timer = setInterval(() => {
      const updated = this.stats().map(stat => {
        const increment = stat.target / steps;
        const next      = Math.min(stat.current + increment, stat.target);
        return { ...stat, current: next };
      });
      this.stats.set(updated);

      if (updated.every(s => s.current >= s.target)) {
        clearInterval(timer);
      }
    }, interval);
  }
}
