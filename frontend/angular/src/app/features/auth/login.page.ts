import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MockAuthService } from '../../core/auth/mock-auth.service';

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
  private readonly mockAuth = inject(MockAuthService);

  protected submitted = false;

  protected form = this.fb.nonNullable.group({
    username: ['', [Validators.required]],
    password: ['', [Validators.required]]
  });

  ngOnInit(): void {
    if (this.mockAuth.isLoggedIn()) {
      this.router.navigateByUrl('/dashboard');
    }
  }

  protected onSubmit(): void {
    this.submitted = true;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { username, password } = this.form.getRawValue();
    this.mockAuth.login(username, password);
    this.router.navigateByUrl('/dashboard');
  }

  protected showError(controlName: 'username' | 'password'): boolean {
    const control = this.form.get(controlName);
    return !!control && control.invalid && this.submitted;
  }
}

