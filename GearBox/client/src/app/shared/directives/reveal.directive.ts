import { Directive, ElementRef, Input, OnDestroy, OnInit } from '@angular/core';

export type RevealEffect = 'fade-up' | 'fade-down' | 'fade-left' | 'fade-right' | 'zoom' | 'fade';

@Directive({
  selector: '[appReveal]',
  standalone: true,
})
export class RevealDirective implements OnInit, OnDestroy {
  @Input() revealEffect: RevealEffect = 'fade-up';
  @Input() revealDelay = 0;
  @Input() revealThreshold = 0.12;

  private observer!: IntersectionObserver;

  constructor(private el: ElementRef<HTMLElement>) {}

  ngOnInit() {
    const el = this.el.nativeElement;
    el.classList.add('rv', `rv-${this.revealEffect}`);
    if (this.revealDelay) el.style.transitionDelay = `${this.revealDelay}ms`;

    this.observer = new IntersectionObserver(
      ([entry]) => {
        if (entry.isIntersecting) {
          el.classList.add('rv-visible');
          this.observer.unobserve(el);
        }
      },
      { threshold: this.revealThreshold }
    );

    this.observer.observe(el);
  }

  ngOnDestroy() {
    this.observer?.disconnect();
  }
}
