import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DocumentoService } from 'src/app/services/documento.service';
import { Documento } from 'src/app/models/Documento';
import { PaginatedResult } from 'src/app/models/Pagination';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss'],
    standalone: false
})
export class HomeComponent implements OnInit {
  recentes: Documento[] = [];

  constructor(private router: Router, private documentoService: DocumentoService) {}

  ngOnInit() {
    this.documentoService.getDocumentos(1, 3).subscribe(
      (paginated: PaginatedResult<Documento[]>) => {
        this.recentes = paginated.result || [];
      },
      () => {
        this.recentes = [];
      }
    );
  }

  redirectTo(link: string) {
    this.router.navigate([link]);
  }

  abrirDocumento(id: number) {
    this.router.navigate([`/documento/${id}`]);
  }

  public corCategoria(categoria: number): string {
    const cores = [
      '#2780e3', '#3fb618', '#8f4fd1', '#e8590c', '#d6336c', '#20c997', '#fd7e14',
    ];
    return cores[categoria] || '#2780e3';
  }
}
