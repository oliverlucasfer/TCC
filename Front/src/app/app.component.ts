import { Component, OnInit } from '@angular/core';
import { AccountService } from './services/account.service';
import { UiService } from './services/ui.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    standalone: false
})
export class AppComponent implements OnInit {
  title = 'Client';
  menuAberto = false;

  constructor(private accountService: AccountService, private ui: UiService) {}

  ngOnInit(): void {
    const user = JSON.parse(localStorage.getItem('user'));
    if (user) {
      this.accountService.setCurrentUser(user);
    }
    this.ui.menuAberto$.subscribe((aberto) => {
      this.menuAberto = aberto;
    });
  }

  fecharMenu(): void {
    this.ui.fecharMenu();
  }
}
