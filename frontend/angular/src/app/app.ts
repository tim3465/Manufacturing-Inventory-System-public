import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ThemeService } from './core/theme/theme.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})



export class App {
  public readonly themeService = inject(ThemeService);
  protected readonly title = signal('angular');

  constructor() {
    this.themeService.initialize();
  }




}
