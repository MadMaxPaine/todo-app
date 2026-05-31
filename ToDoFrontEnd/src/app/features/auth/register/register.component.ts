import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './register.component.html',  
})
export class RegisterComponent {

  email = '';
  password = '';
  confirmPassword = '';

  errorMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  register(): void {
    this.errorMessage = '';

    // 🔥 client-side validation
    if (!this.email || !this.password) {
      this.errorMessage = 'Email and password are required';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'Passwords do not match';
      return;
    }

    this.authService.register({
      email: this.email,
      password: this.password
    })
    .subscribe({
      next: () => {
        // UX: auto login after register
        this.authService.login({
          email: this.email,
          password: this.password
        }).subscribe({
          next: () => this.router.navigate(['/tasks']),
          error: () => this.router.navigate(['/login'])
        });
      },
      error: () => {
        this.errorMessage = 'Registration failed (email may already exist)';
      }
    });
  }
}