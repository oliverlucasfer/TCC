import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter, takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { UiService } from 'src/app/services/ui.service';
import { DocumentoService } from 'src/app/services/documento.service';
import { CATEGORIA_LABELS, CATEGORIA_CORES } from 'src/app/shared/categorias';

interface ItemMenu {
  label: string;
  rota: string;
  indice: number;
  cor: string;
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
  contagens: { [key: string]: number } = {};

  categorias: ItemMenu[] = CATEGORIA_LABELS.map((label, i) => ({
    label,
    rota: `/${i}`,
    indice: i,
    cor: CATEGORIA_CORES[i],
  }));

  urlAtual = '';

  constructor(
    private router: Router,
    private ui: UiService,
    private documentoService: DocumentoService
  ) {}

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
        this.carregarContagens();
      });
    this.ui.menuAberto$.pipe(takeUntil(this.destroy$)).subscribe((aberto) => {
      this.mobileAberto = aberto;
    });
    this.carregarContagens();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  navegar(rota: string) {
    this.router.navigateByUrl(rota);
  }

  isActive(rota: string): boolean {
    return this.urlAtual === rota;
  }

  private carregarContagens(): void {
    this.documentoService.getContagem().subscribe(
      (c) => (this.contagens = c || {}),
      () => {}
    );
  }
}
