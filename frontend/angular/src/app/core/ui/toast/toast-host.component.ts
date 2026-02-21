import { NgClass, NgFor } from '@angular/common';
import { Component, inject } from '@angular/core';
import { Toast } from './toast.model';
import { ToastService } from './toast.service';

@Component({
  selector: 'app-toast-host',
  standalone: true,
  imports: [NgFor, NgClass],
  templateUrl: './toast-host.component.html',
  styleUrl: './toast-host.component.css'
})
export class ToastHostComponent {
  private readonly toastService = inject(ToastService);
  readonly toasts = this.toastService.toasts;

  dismiss(id: string) {
    this.toastService.dismiss(id);
  }

  trackById(_: number, t: Toast) {
    return t.id;
  }

  label(t: Toast): string {
    switch (t.type) {
      case 'success':
        return 'Success';
      case 'error':
        return 'Error';
      case 'warning':
        return 'Warning';
      default:
        return 'Info';
    }
  }

  typeClasses(t: Toast): string {
    switch (t.type) {
      case 'success':
        return 'border-l-4 border-green-300 bg-green-600 text-white';
      case 'error':
        return 'border-l-4 border-red-300 bg-red-600 text-white';
      case 'warning':
        return 'border-l-4 border-yellow-300 bg-yellow-600 text-white';
      default:
        return 'border-l-4 border-blue-300 bg-blue-600 text-white';
    }
  }

  animationClasses(t: Toast): string {
    switch (t.phase) {
      case 'enter':
        return 'opacity-0 translate-y-[66%]';
      case 'exit':
        return 'opacity-0 translate-x-[66%] duration-150';
      default:
        return 'opacity-100 translate-y-0';
    }
  }
}


