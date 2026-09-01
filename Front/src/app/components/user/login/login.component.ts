import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserLogin } from 'src/app/models/identity/UserLogin';
import { AccountService } from 'src/app/services/account.service';
import { ToastService } from 'src/app/services/toast.service';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss'],
    standalone: false
})
export class LoginComponent implements OnInit {
  model = {} as UserLogin;
  mostrarSenha = false;
  enviando = false;

  constructor(
    private accountService: AccountService,
    private router: Router,
    private route: ActivatedRoute,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {}

  public login(): void {
    this.enviando = true;
    this.accountService.login(this.model).subscribe(
      () => {
        const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
        this.router.navigateByUrl(returnUrl || '');
      },
      (error: any) => {
        this.enviando = false;
        if (error.status == 401) this.toastService.error('Usuário ou senha inválidos.');
        else this.toastService.error('Erro ao entrar. Tente novamente.');
      }
    );
  }
}
