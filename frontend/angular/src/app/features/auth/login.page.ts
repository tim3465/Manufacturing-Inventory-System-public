import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';

type LoginResponseDto = {
  accessToken: string;
};

@Component({
  selector: 'app-mock-auth-login-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.page.html',
  styleUrl: './login.page.css',
  host: {
    class: 'block min-h-screen bg-[var(--bg)] text-[var(--fg)]'
  }
})
export class MockAuthLoginPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly http = inject(HttpClient);
  private readonly auth = inject(AuthService);

  protected submitted = false;
  protected authError: string | null = null;

  protected form = this.fb.nonNullable.group({
    email: ['', [Validators.required]],
    password: ['', [Validators.required]]
  });

  ngOnInit(): void {
    if (this.auth.isLoggedIn()) {
      this.router.navigateByUrl('/dashboard');
    }
  }

  protected onSubmit(): void {
    this.submitted = true;
    this.authError = null;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { email, password } = this.form.getRawValue();

    this.http.post<LoginResponseDto>('/api/auth/login', { email, password }).subscribe({
      next: (res) => {
        this.auth.setToken(res.accessToken);
        const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
        const target =
          returnUrl && returnUrl.startsWith('/') ? returnUrl : '/dashboard';
        void this.router.navigateByUrl(target);
      },
      error: (err: unknown) => {
        if (err instanceof HttpErrorResponse && err.status === 401) {
          this.authError = 'Invalid email or password';
          return;
        }
        // TODO: optionally surface a generic error message; keep minimal for now
      }
    });
  }

  protected showError(controlName: 'email' | 'password'): boolean {
    const control = this.form.get(controlName);
    return !!control && control.invalid && this.submitted;
  }
}

