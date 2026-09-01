import { Component, OnInit } from '@angular/core';
import {
  AbstractControlOptions,
  UntypedFormBuilder,
  UntypedFormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { ValidatorField } from 'src/app/helpers/ValidatorField';
import { User } from 'src/app/models/identity/User';
import { AccountService } from 'src/app/services/account.service';
import { ToastService } from 'src/app/services/toast.service';

@Component({
    selector: 'app-registration',
    templateUrl: './registration.component.html',
    styleUrls: ['./registration.component.scss'],
    standalone: false
})
export class RegistrationComponent implements OnInit {
  user = {} as User;
  form!: UntypedFormGroup;

  constructor(
    public fb: UntypedFormBuilder,
    private accountService: AccountService,
    private router: Router,
    private toastService: ToastService
  ) {}

  get f(): any {
    return this.form.controls;
  }

  ngOnInit(): void {
    this.validation();
  }

  private validation(): void {
    const formOptions: AbstractControlOptions = {
      validators: ValidatorField.MustMatch('password', 'confirmPassword'),
    };

    this.form = this.fb.group(
      {
        primeiroNome: ['', Validators.required],
        ultimoNome: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
        userName: ['', Validators.required],
        password: ['', [Validators.required, Validators.minLength(4)]],
        confirmPassword: ['', Validators.required],
      },
      formOptions
    );
  }

  register(): void {
    this.user = { ...this.form.value };
    this.accountService.register(this.user).subscribe(
      () => {
        this.toastService.success('Conta criada com sucesso.');
        this.router.navigateByUrl('');
      },
      (error: any) => this.toastService.error('Erro ao criar conta. Tente novamente.')
    );
  }
}
