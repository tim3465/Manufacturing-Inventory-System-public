import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ShiftsApi } from '../../../../core/api/shifts.api';
import { RunningShiftDto } from '../../../../core/dtos/shifts/running-shift.dto';
import { ToastService } from '../../../../core/ui/toast/toast.service';

@Component({
  selector: 'app-running-shift-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './running-shift-form.component.html',
  styleUrl: './running-shift-form.component.css'
})
export class RunningShiftFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly shiftsApi = inject(ShiftsApi);
  private readonly toast = inject(ToastService);

  @Input({ required: true }) shiftId!: number;
  @Output() saved = new EventEmitter<void>();
  @Output() closed = new EventEmitter<void>();

  protected readonly loading = signal(true);
  protected readonly shift = signal<RunningShiftDto | null>(null);
  protected readonly submittingAction = signal<'save' | 'close' | null>(null);

  protected readonly form = this.fb.nonNullable.group({
    startTime: ['', Validators.required],
    stopTime: [null as string | null],
    partsMade: [0, [Validators.required, Validators.min(0)]],
    scrap: [0, [Validators.required, Validators.min(0)]],
    barsConsumed: [0, [Validators.required, Validators.min(0)]],
    partsPerBar: [null as number | null],
    downtimeHours: [0, [Validators.min(0)]],
    downtimeMinutes: [0, [Validators.min(0), Validators.max(59)]]
  });

  ngOnInit(): void {
    this.shiftsApi.getRunning(this.shiftId).subscribe({
      next: (data) => {
        this.shift.set(data);
        this.patchForm(data);
        this.loading.set(false);
      },
      error: () => {
        this.toast.error('Failed to load shift');
        this.loading.set(false);
      }
    });
  }

  private patchForm(data: RunningShiftDto): void {
    const toDatetimeLocal = (iso: string | null): string => {
      if (!iso) return '';
      const d = new Date(iso);
      const pad = (n: number) => n.toString().padStart(2, '0');
      return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
    };

    let downtimeHours = 0;
    let downtimeMinutes = 0;
    if (data.downtime) {
      const parts = data.downtime.split(':');
      downtimeHours = parseInt(parts[0], 10) || 0;
      downtimeMinutes = parseInt(parts[1], 10) || 0;
    }

    this.form.patchValue({
      startTime: toDatetimeLocal(data.startTime),
      stopTime: toDatetimeLocal(data.stopTime),
      partsMade: data.partsMade,
      scrap: data.scrap,
      barsConsumed: data.barsConsumed,
      partsPerBar: data.partsPerBar,
      downtimeHours,
      downtimeMinutes
    });
  }

  private buildDto() {
    const raw = this.form.getRawValue();
    const h = raw.downtimeHours;
    const m = raw.downtimeMinutes;
    const downtime = (h === 0 && m === 0)
      ? null
      : `${h.toString().padStart(2, '0')}:${m.toString().padStart(2, '0')}:00`;

    return {
      startTime: raw.startTime ? raw.startTime + ':00' : '',
      stopTime: raw.stopTime ? raw.stopTime + ':00' : null,
      partsMade: raw.partsMade,
      scrap: raw.scrap,
      barsConsumed: raw.barsConsumed,
      partsPerBar: raw.partsPerBar,
      downtime
    };
  }

  protected onSave(): void {
    if (this.submittingAction() !== null) return;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submittingAction.set('save');
    this.shiftsApi.saveShift(this.shiftId, this.buildDto()).subscribe({
      next: () => {
        this.toast.success('Shift saved');
        this.saved.emit();
        this.submittingAction.set(null);
      },
      error: (err: unknown) => {
        this.toast.errorMessage(err, undefined, 'Failed to save shift');
        this.submittingAction.set(null);
      }
    });
  }

  protected onClose(): void {
    if (this.submittingAction() !== null) return;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();

    // NEW: require stopTime before closing
    if (!raw.stopTime) {
      this.form.controls.stopTime.markAsTouched();
      return;
    }

    // Client-side validation: stopTime must be after startTime
    if (raw.stopTime) {
      const start = new Date(raw.startTime).getTime();
      const stop = new Date(raw.stopTime).getTime();
      if (stop <= start) {
        this.toast.error('Stop time must be after start time');
        return;
      }
    }

    this.submittingAction.set('close');
    this.shiftsApi.closeShift(this.shiftId, this.buildDto()).subscribe({
      next: () => {
        this.toast.success('Shift closed');
        this.closed.emit();
      },
      error: (err: unknown) => {
        this.toast.errorMessage(err, undefined, 'Failed to close shift');
        this.submittingAction.set(null);
      }
    });
  }
}
