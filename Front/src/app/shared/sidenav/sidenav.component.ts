import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter, takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { UiService } from 'src/app/services/ui.service';

interface ItemMenu {
  label: string;
  rota: string;
}

@Component({
    selector: 'app-sidenav',
    templateUrl: './sidenav.component.html',
    styleUrls: ['./sidenav.component.scss'],
    standalone: false
})
export class SidenavComponent implements OnInit {
  private destroy$ = new Subject<void>();
  mobileAberto = false;
  itens: ItemMenu[] = [
    { label: 'Documentos', rota: '/lista' },
    { label: 'Resumos', rota: '/0' },
    { label: 'Artigos', rota: '/1' },
    { label: 'Monografias', rota: '/2' },
    { label: 'Dissertações', rota: '/3' },
    { label: 'Teses', rota: '/4' },
    { label: 'Livros', rota: '/5' },
    { label: 'Projetos', rota: '/6' },
  ];
  urlAtual = '';

  constructor(private router: Router, private ui: UiService) {}

  ngOnInit() {
    this.urlAtual = this.router.url;
    this.router.events
      .pipe(
        filter((e) => e instanceof NavigationEnd),
        takeUntil(this.destroy$)
      )
      .subscribe(() => {
        this.urlAtual = this.router.url;
        this.ui.fecharMenu();
      });
    this.ui.menuAberto$.pipe(takeUntil(this.destroy$)).subscribe((aberto) => {
      this.mobileAberto = aberto;
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  navegar(rota: string) {
    this.router.navigateByUrl(rota);
  }

  isActive(item: ItemMenu): boolean {
    return this.urlAtual === item.rota;
  }
}
