import { Injectable, Signal, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { Toast, ToastPhase, ToastType } from './toast.model';

const DEFAULT_DURATION_MS = 4000;
const EXIT_ANIMATION_MS = 150;

@Injectable({ providedIn: 'root' })
export class ToastService {
  private readonly _toasts = signal<Toast[]>([]);
  private readonly _timers = new Map<string, ReturnType<typeof setTimeout>>();
  private readonly _removalTimers = new Map<string, ReturnType<typeof setTimeout>>();

  readonly toasts: Signal<Toast[]> = this._toasts.asReadonly();

  success(message: string, durationMs?: number) {
    this.add('success', message, durationMs);
  }

  error(message: string, durationMs?: number) {
    this.add('error', message, durationMs);
  }


  errorMessage(err: unknown, durationMs?: number, fallbackMessage = 'Request failed') {
    this.add('error', this.extractErrorMessage(err, fallbackMessage), durationMs);
  }

  info(message: string, durationMs?: number) {
    this.add('info', message, durationMs);
  }

  warning(message: string, durationMs?: number) {
    this.add('warning', message, durationMs);
  }

  dismiss(id: string) {
    const t = this._timers.get(id);
    if (t) clearTimeout(t);
    this._timers.delete(id);

    const removalTimer = this._removalTimers.get(id);
    if (removalTimer) {
      clearTimeout(removalTimer);
    }

    this.setPhase(id, 'exit');
    const exitTimer = setTimeout(() => {
      this._toasts.update((curr) => curr.filter((x) => x.id !== id));
      this._removalTimers.delete(id);
    }, EXIT_ANIMATION_MS);
    this._removalTimers.set(id, exitTimer);
  }

  private add(type: ToastType, message: string, durationMs?: number) {
    const toast: Toast = {
      id: this.createId(),
      type,
      message,
      durationMs,
      phase: 'enter'
    };

    this._toasts.update((curr) => [...curr, toast]);
    setTimeout(() => this.setPhase(toast.id, 'shown'));

    const ms = durationMs ?? DEFAULT_DURATION_MS;
    const timer = setTimeout(() => this.dismiss(toast.id), ms);
    this._timers.set(toast.id, timer);
  }

  private setPhase(id: string, phase: ToastPhase) {
    this._toasts.update((curr) =>
      curr.map((toast) => (toast.id === id ? { ...toast, phase } : toast))
    );
  }

  private extractErrorMessage(err: unknown, fallbackMessage: string): string {
    // Angular HttpClient errors
    if (err instanceof HttpErrorResponse) {
      const body = err.error as unknown;

      // backend returns a string body (sometimes for non-JSON errors)
      if (typeof body === 'string') {
        const trimmed = body.trim();
        if (trimmed) return trimmed;
      }

      // RFC7807 ProblemDetails
      if (body && typeof body === 'object') {
        const anyBody = body as Record<string, unknown>;
        const detail = anyBody['detail'];
        if (typeof detail === 'string' && detail.trim()) return detail.trim();

        const title = anyBody['title'];
        if (typeof title === 'string' && title.trim()) return title.trim();

        // common validation shape: { errors: { Field: ["msg"] } }
        const errors = anyBody['errors'];
        if (errors && typeof errors === 'object') {
          const msgs: string[] = [];
          for (const value of Object.values(errors as Record<string, unknown>)) {
            if (typeof value === 'string' && value.trim()) msgs.push(value.trim());
            if (Array.isArray(value)) {
              for (const item of value) {
                if (typeof item === 'string' && item.trim()) msgs.push(item.trim());
              }
            }
          }
          if (msgs.length) return msgs.join('\n');
        }

        // common shape: { message: "..." }
        const message = anyBody['message'];
        if (typeof message === 'string' && message.trim()) return message.trim();
      }

      // last-resort HttpErrorResponse message
      if (typeof err.message === 'string' && err.message.trim()) return err.message.trim();
    }

    // Some callers might pass a string directly
    if (typeof err === 'string' && err.trim()) return err.trim();

    return fallbackMessage;
  }

  private createId(): string {
    // dependency-free; prefer modern browser UUID when available
    const anyCrypto = globalThis.crypto as Crypto | undefined;
    if (anyCrypto?.randomUUID) return anyCrypto.randomUUID();
    return `${Date.now()}-${Math.random().toString(16).slice(2)}`;
  }
}


