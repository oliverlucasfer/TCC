import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { categorias, Documento } from 'src/app/models/Documento';
import { DocumentoService } from 'src/app/services/documento.service';
import { DownloadService } from 'src/app/services/download.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-documento-infos',
  templateUrl: './documento-infos.component.html',
  styleUrls: ['./documento-infos.component.scss'],
})
export class DocumentoInfosComponent implements OnInit {
  documento!: Documento;
  documentoId!: number;
  documentoURL!: string;
  categorias = categorias;

  constructor(
    private activatedRouter: ActivatedRoute,
    private documentoService: DocumentoService,
    private service: DownloadService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.carregarDocumento();
  }

  public carregarDocumento() {
    this.documentoId = +this.activatedRouter.snapshot.paramMap.get('id');

    if (this.documentoId !== null && this.documentoId !== 0) {
      this.documentoService.getDocumentoById(this.documentoId).subscribe(
        (documento: Documento) => {
          this.documento = documento;
          if (this.documento.documentoURL !== '') {
            this.documentoURL =
              // environment.apiURL +
              // 'resources/pdfs/' +
              this.documento.documentoURL;
          }
        },
        (error: any) => {
          console.error(error);
        }
      );
    }
  }

  redirectTo(id: number) {
    this.router.navigate([`documento/editar/${id}`]);
  }

  download() {
    this.service.downloadPdf(this.documentoURL).subscribe((res) => {
      let url = this.documentoURL;
      let a = document.createElement('a');
      a.href = url;
      a.download = this.documentoURL;
      a.click();
      window.URL.revokeObjectURL(url);
      a.remove();
    });
  }
}
