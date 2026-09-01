import { Component, OnInit, TemplateRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Documento, categorias, areas } from 'src/app/models/Documento';
import { DocumentoService } from 'src/app/services/documento.service';
import { Subject } from 'rxjs';
import { debounceTime, take, takeUntil } from 'rxjs/operators';
import { PaginatedResult, Pagination } from 'src/app/models/Pagination';
import { AccountService } from 'src/app/services/account.service';
import { ToastService } from 'src/app/services/toast.service';

@Component({
    selector: 'app-listagem-documentos',
    templateUrl: './listagem-documentos.component.html',
    styleUrls: ['./listagem-documentos.component.scss'],
    standalone: false
})
export class ListagemDocumentosComponent implements OnInit {
  modalRef?: BsModalRef;
  public documentos: Documento[] = [];
  public categorias = categorias;
  public documentoId = 0;
  public pagination = {} as Pagination;
  public categoriaAtual?: number;
  tipo: string;
  podeEditar = false;
  podeBackup = false;
  filtroArea = '';
  filtroAno = '';
  anos: number[] = Array.from({ length: 30 }, (_, i) => 2026 - i);
  areas = areas;
  termoBuscaChanged: Subject<string> = new Subject<string>();
  private destroy$ = new Subject<void>();

  constructor(
    private documentoService: DocumentoService,
    private accountService: AccountService,
    private modalService: BsModalService,
    private router: Router,
    private route: ActivatedRoute,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.accountService.currentUser$.pipe(take(1)).subscribe((user) => {
      this.tipo = user ? user.tipo : undefined;
      this.podeEditar = this.tipo === 'Administrador' || this.tipo === 'UsuarioAvancado';
      this.podeBackup = this.tipo === 'Administrador';
    });
    this.pagination = {
      currentPage: 1,
      itemsPerPage: 3,
      totalItems: 1,
    } as Pagination;
    this.route.paramMap
      .pipe(takeUntil(this.destroy$))
      .subscribe((params) => {
        const categoria = params.get('categoria');
        this.categoriaAtual = categoria !== null ? Number(categoria) : undefined;
        this.carregarDocumentos();
      });
    this.termoBuscaChanged
      .pipe(debounceTime(1000), takeUntil(this.destroy$))
      .subscribe((filtrarPor) => {
        this.documentoService
          .getDocumentos(
            this.pagination.currentPage,
            this.pagination.itemsPerPage,
            filtrarPor
          )
          .subscribe(
            (paginatedResult: PaginatedResult<Documento[]>) => {
              this.documentos = paginatedResult.result;
              this.pagination = paginatedResult.pagination;
            },
            (error: any) => {
              this.toastService.error('Erro ao buscar documentos.');
            }
          );
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  openModal(event: any, template: TemplateRef<any>) {
    event.stopPropagation();
    this.modalRef = this.modalService.show(template, { class: 'modal-sm' });
  }

  public pageChanged(event): void {
    this.pagination.currentPage = event.page;
    this.carregarDocumentos();
  }

  public filtarDocumentos(event: any): void {
    this.termoBuscaChanged.next(event.value);
  }

  public carregarDocumentos(): void {
    this.documentoService
      .getDocumentos(
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        '',
        this.categoriaAtual
      )
      .subscribe(
        (paginatedResult: PaginatedResult<Documento[]>) => {
          this.documentos = paginatedResult.result;
          this.pagination = paginatedResult.pagination;
        },
        (error: any) => {
          this.toastService.error('Erro ao buscar documentos.');
        }
      );
  }

  info(id: number) {
    this.router.navigate([`/documento/${id}`]);
  }

  redirectTo() {
    this.router.navigate(['document/novo']);
  }

  limpar() {
    if (this.router.url == '/lista') {
      window.location.reload();
    } else {
      this.router.navigate(['/lista']);
    }
    this.modalRef?.hide();
  }

  aplicar() {
    this.documentoService
      .getFiltro(this.filtroArea, this.filtroAno, this.pagination.currentPage, this.pagination.itemsPerPage)
      .subscribe(
        (paginatedResult: PaginatedResult<Documento[]>) => {
          this.documentos = paginatedResult.result;
          this.pagination = paginatedResult.pagination;
          this.modalRef?.hide();
        },
(error: any) => {
        this.toastService.error('Erro ao aplicar filtro.');
      }
    );
  }

  baixarBackup(): void {
    this.documentoService.getBackup().subscribe(
      (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `prodocs-backup-${new Date().toISOString().slice(0, 10)}.zip`;
        a.click();
        window.URL.revokeObjectURL(url);
        this.toastService.success('Backup gerado.');
      },
      () => this.toastService.error('Erro ao gerar backup.')
    );
  }
}
