import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { AuthService } from '../../../core/services/auth.service';
import { RouterLink } from '@angular/router';
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './login.component.html',
})
export class LoginComponent {
  email = '';
  password = '';

  errorMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router,
  ) {}

  login(): void {
    this.errorMessage = '';

    this.authService
      .login({
        email: this.email,
        password: this.password,
      })
      .subscribe({
        next: () => {
          this.router.navigate(['/tasks']);
        },
        error: () => {
          this.errorMessage = 'Invalid email or password';
        },
      });
  }
}
