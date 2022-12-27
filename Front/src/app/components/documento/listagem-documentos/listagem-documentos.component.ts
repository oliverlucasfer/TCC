import { Component, OnInit, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Documento, categorias, areas } from 'src/app/models/Documento';
import { DocumentoService } from 'src/app/services/documento.service';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { PaginatedResult, Pagination } from 'src/app/models/Pagination';
import { EmitterService } from 'src/app/services/emitter.service';
import * as _ from 'lodash';
import { AccountService } from 'src/app/services/account.service';

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
  tipo: any;
  areas = areas;
  termoBuscaChanged: Subject<string> = new Subject<string>();

  constructor(
    private documentoService: DocumentoService,
    private accountService: AccountService,
    private modalService: BsModalService,
    private router: Router,
    private emitter: EmitterService
  ) {}

  ngOnInit(): void {
    this.accountService.getUser().subscribe((u) => {
      var user = JSON.parse(localStorage.getItem('user'));
      for (let index = 0; index < u.length; index++) {
        if (user.userName == u[index].userName) {
          this.tipo = u[index].tipo;
        }
      }
    });
    this.emitter.categoria.subscribe((value: number) => {
      this.carregarDocumentos(value);
    });
    this.url = this.router.url;
    if (this.url == '/lista') {
      this.carregarDocumentos();
    }
    this.pagination = {
      currentPage: 1,
      itemsPerPage: 3,
      totalItems: 1,
    } as Pagination;
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

  public carregarDocumentos(categoria?: number): void {
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
      var cat = categoria;
      this.documentoService
        .getDocumentos(
          this.pagination.currentPage,
          this.pagination.itemsPerPage,
          '',
          cat
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

  limpar() {
    if (this.router.url == '/lista') {
      window.location.reload();
    } else {
      this.router.navigate(['/lista']);
    }
    this.modalRef?.hide();
  }

  date(valor: string) {
    // this.ano = valor;
  }

  area(valor: string) {
    // this._area = valor;
  }

  aplicar() {
    // this.documentoService.getFiltro().subscribe(d => {
    //   this.documentos = d
    // });
    // this.modalRef?.hide();
  }
}
