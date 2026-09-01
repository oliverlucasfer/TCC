import { Component, HostListener, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from 'src/app/services/account.service';
import { ThemeService } from 'src/app/services/theme.service';
import { UiService } from 'src/app/services/ui.service';
import { User } from 'src/app/models/identity/User';

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html',
    styleUrls: ['./header.component.scss'],
    standalone: false
})
export class HeaderComponent implements OnInit {
  user: User | null = null;
  menuAberto = false;
  rolou = false;

  constructor(
    private router: Router,
    private accountService: AccountService,
    public theme: ThemeService,
    public ui: UiService
  ) {}

  ngOnInit() {
    this.accountService.currentUser$.subscribe((user) => {
      this.user = user || null;
    });
  }

  get primeiroNome(): string {
    return this.user?.primeiroNome || this.user?.userName || '';
  }

  get iniciais(): string {
    const nome = this.primeiroNome.trim();
    if (!nome) return '?';
    const partes = nome.split(/\s+/);
    const primeira = partes[0].charAt(0);
    const segunda = partes.length > 1 ? partes[partes.length - 1].charAt(0) : '';
    return (primeira + segunda).toUpperCase();
  }

  @HostListener('window:scroll')
  onScroll() {
    this.rolou = window.scrollY > 4;
  }

  @HostListener('document:click')
  onClickFora() {
    if (this.menuAberto) this.menuAberto = false;
  }

  redirectTo() {
    this.menuAberto = false;
    this.router.navigate(['/user/perfil']);
  }

  logout() {
    this.menuAberto = false;
    this.accountService.logout();
    this.router.navigate(['/user/login']);
  }

  toggleMenu(event: Event) {
    event.stopPropagation();
    this.menuAberto = !this.menuAberto;
  }

  toggleTema(event: Event) {
    event.stopPropagation();
    this.theme.toggle();
  }
}
