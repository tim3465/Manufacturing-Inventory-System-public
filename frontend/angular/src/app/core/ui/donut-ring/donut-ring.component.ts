import { Component, computed, input } from '@angular/core';

@Component({
  selector: 'app-donut-ring',
  standalone: true,
  templateUrl: './donut-ring.component.html'
})
export class DonutRingComponent {
  readonly target = input.required<number>();
  readonly good = input.required<number>();
  readonly scrap = input.required<number>();
  readonly size = input<number>(80);

  protected readonly completedParts = computed(() => this.good() + this.scrap());

  protected readonly isOverflow = computed(() => this.completedParts() > this.target());

  protected readonly completionFraction = computed(() => {
    const t = this.target();
    if (t <= 0) return 0;
    return Math.min(this.completedParts() / t, 1);
  });

  protected readonly goodPercent = computed(() => {
    const t = this.target();
    if (t <= 0) return 0;
    const cf = this.completionFraction();
    const sf = this.scrapPercent() / 100;
    return (cf - sf) * 100;
  });

  protected readonly scrapPercent = computed(() => {
    const t = this.target();
    if (t <= 0) return 0;
    const cf = this.completionFraction();
    return Math.min(this.scrap() / t, cf) * 100;
  });

  protected readonly scrapOffset = computed(() => 100 - this.goodPercent());

  protected readonly centerLabel = computed(() => `${this.good()} / ${this.target()}`);

  protected readonly scrapLabel = computed(() => {
    const s = this.scrap();
    return s > 0 ? `${s} scrap` : '';
  });

  protected readonly overflowLabel = computed(() => {
    const t = this.target();
    if (t <= 0) return '';
    const pct = Math.round((this.completedParts() / t) * 100);
    return `${pct}%`;
  });
}
