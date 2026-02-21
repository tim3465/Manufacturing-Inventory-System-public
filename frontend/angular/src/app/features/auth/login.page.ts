import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthApi } from '../../core/api/auth.api';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-auth-login-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.page.html',
  styleUrl: './login.page.css',
  host: {
    class: 'block min-h-screen bg-[var(--bg)] text-[var(--fg)]'
  }
})
export class AuthLoginPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
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
    const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
    const { email, password} = this.form.getRawValue();

    this.auth.login(email,password,returnUrl ).subscribe((errorMessage)=>{
      this.authError = errorMessage;
    });

  }

  protected showError(controlName: 'email' | 'password'): boolean {
    const control = this.form.get(controlName);
    return !!control && control.invalid && this.submitted;
  }
}

