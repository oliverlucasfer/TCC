import { Component, OnInit } from '@angular/core';
import {
  AbstractControlOptions,
  UntypedFormBuilder,
  UntypedFormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { ValidatorField } from 'src/app/helpers/ValidatorField';
import { UserUpdate } from 'src/app/models/identity/UserUpdate';
import { AccountService } from 'src/app/services/account.service';
import { ToastService } from 'src/app/services/toast.service';
import { take } from 'rxjs/operators';

@Component({
    selector: 'app-perfil',
    templateUrl: './perfil.component.html',
    styleUrls: ['./perfil.component.scss'],
    standalone: false
})
export class PerfilComponent implements OnInit {
  userUpdate = {} as UserUpdate;
  form!: UntypedFormGroup;

  constructor(
    private router: Router,
    private fb: UntypedFormBuilder,
    private service: AccountService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.validation();
    this.carregarUsuario();
  }

  private validation(): void {
    const formOptions: AbstractControlOptions = {
      validators: ValidatorField.MustMatch('password', 'confirmePassword'),
    };

    this.form = this.fb.group(
      {
        userName: [''],
        primeiroNome: ['', Validators.required],
        ultimoNome: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
        Tipo: ['UsuarioComum'],
        password: ['', [Validators.minLength(8), Validators.nullValidator]],
        confirmePassword: ['', Validators.nullValidator],
      },
      formOptions
    );
  }

  carregarUsuario(): void {
    this.service.currentUser$.pipe(take(1)).subscribe((user) => {
      if (user) {
        this.form.patchValue({
          userName: user.userName,
          primeiroNome: user.primeiroNome,
          ultimoNome: user.ultimoNome,
          email: user.email,
        });
      }
    });
  }

  get f(): any {
    return this.form.controls;
  }

  onSubmit(): void {
    this.atualizarUsuario();
  }

  public atualizarUsuario() {
    this.userUpdate = { ...this.form.value };

    this.service.updateUser(this.userUpdate).subscribe(
      () => this.toastService.success('Perfil atualizado com sucesso.'),
      (error: any) => this.toastService.error('Erro ao atualizar perfil. Tente novamente.')
    );
  }

  logout() {
    this.service.logout();
    this.router.navigate(['']);
  }
}
