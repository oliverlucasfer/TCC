import { Component, OnInit } from '@angular/core';
import {
  AbstractControlOptions,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { ValidatorField } from 'src/app/helpers/ValidatorField';
import { UserUpdate } from 'src/app/models/identity/UserUpdate';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'app-perfil',
  templateUrl: './perfil.component.html',
  styleUrls: ['./perfil.component.scss'],
})
export class PerfilComponent implements OnInit {
  userUpdate = {} as UserUpdate;
  form!: FormGroup;

  constructor(
    private router: Router,
    private fb: FormBuilder,
    private service: AccountService
  ) {}

  ngOnInit(): void {
    this.validation();
    this.carregarUsuario();
    console.log(localStorage.getItem('user'))
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
    // this.service.getUser().subscribe((userRetorno: UserUpdate) => {
    //   this.userUpdate = userRetorno;
    //   this.form.patchValue(this.userUpdate);
    // });
  }

  get f(): any {
    return this.form.controls;
  }

  onSubmit(): void {
    this.atualizarUsuario();
  }

  public atualizarUsuario() {
    this.userUpdate = { ...this.form.value };

    this.service.updateUser(this.userUpdate).subscribe();
  }

  logout() {
    this.service.logout();
    this.router.navigate(['']);
  }
}
