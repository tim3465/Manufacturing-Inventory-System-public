export type ToastType = 'success' | 'error' | 'info' | 'warning';
export type ToastPhase = 'enter' | 'shown' | 'exit';

export interface Toast {
  id: string;
  type: ToastType;
  message: string;
  durationMs?: number;
  phase?: ToastPhase;
}


