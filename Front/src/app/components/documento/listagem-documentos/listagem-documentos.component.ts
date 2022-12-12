import { Component, OnInit, TemplateRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Documento, categorias } from 'src/app/models/Documento';
import { DocumentoService } from 'src/app/services/documento.service';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { PaginatedResult, Pagination } from 'src/app/models/Pagination';

@Component({
  selector: 'app-listagem-documentos',
  templateUrl: './listagem-documentos.component.html',
  styleUrls: ['./listagem-documentos.component.scss'],
})
export class ListagemDocumentosComponent implements OnInit {
  modalRef?: BsModalRef;
  public documentos: Documento[] = [];
  public categorias = categorias;
  public documentoId = 0;
  public pagination = {} as Pagination;
  public url: any;
  termoBuscaChanged: Subject<string> = new Subject<string>();

  constructor(
    private documentoService: DocumentoService,
    private modalService: BsModalService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.url = this.router.url;
    this.carregarDocumentos();
    this.pagination = {
      currentPage: 1,
      itemsPerPage: 3,
      totalItems: 1,
    } as Pagination;
  }

  openModal(event: any, template: TemplateRef<any>, documentoId: number) {
    event.stopPropagation();
    this.documentoId = documentoId;
    this.modalRef = this.modalService.show(template, { class: 'modal-sm' });
  }

  confirm(): void {
    this.modalRef?.hide();
    this.documentoService.deleteDocumento(this.documentoId).subscribe(
      (result: any) => {
        if (result.message == 'Deletado') {
          this.carregarDocumentos();
        }
      },
      (error) => console.log(error)
    );
  }

  decline(): void {
    this.modalRef?.hide();
  }

  public pageChanged(event): void {
    this.pagination.currentPage = event.page;
    this.carregarDocumentos();
  }

  public filtarDocumentos(event: any): void {
    if (this.termoBuscaChanged.observers.length === 0) {
      this.termoBuscaChanged
        .pipe(debounceTime(1000))
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
                console.log(error);
              }
            );
        });
    }
    this.termoBuscaChanged.next(event.value);
  }

  public carregarDocumentos(): void {
    if (this.url == '/lista') {
      this.documentoService
        .getDocumentos(
          this.pagination.currentPage,
          this.pagination.itemsPerPage
        )
        .subscribe(
          (paginatedResult: PaginatedResult<Documento[]>) => {
            this.documentos = paginatedResult.result;
            this.pagination = paginatedResult.pagination;
          },
          (error: any) => {
            console.log(error);
          }
        );
    } else {
      var categoria = +this.route.snapshot.paramMap.get('categoria');
      console.log(this.router.url)
      this.documentoService
        .getDocumentos(
          this.pagination.currentPage,
          this.pagination.itemsPerPage,
          '',
          categoria
        )
        .subscribe(
          (paginatedResult: PaginatedResult<Documento[]>) => {
            this.documentos = paginatedResult.result;
            this.pagination = paginatedResult.pagination;
          },
          (error: any) => {
            console.log(error);
          }
        );
    }
  }

  info(id: number) {
    this.router.navigate([`/documento/${id}`]);
  }

  redirectTo() {
    this.router.navigate(['document/novo']);
  }
}
