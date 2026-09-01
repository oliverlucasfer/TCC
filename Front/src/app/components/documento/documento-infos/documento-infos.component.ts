import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { areas, Documento } from 'src/app/models/Documento';
import { CATEGORIA_LABELS } from 'src/app/shared/categorias';
import { AccountService } from 'src/app/services/account.service';
import { DocumentoService } from 'src/app/services/documento.service';
import { environment } from 'src/environments/environment';
import { take } from 'rxjs/operators';

@Component({
    selector: 'app-documento-infos',
    templateUrl: './documento-infos.component.html',
    styleUrls: ['./documento-infos.component.scss'],
    standalone: false
})
export class DocumentoInfosComponent implements OnInit {
  documento = {} as Documento;
  documentoId!: number;
  documentoURL!: string;
  downloadUrl = '';
  previewUrl = '';
  categorias = CATEGORIA_LABELS;
  areas = areas;
  tipo: string;
  podeEditar = false;

  constructor(
    private activatedRouter: ActivatedRoute,
    private documentoService: DocumentoService,
    private accountService: AccountService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.carregarDocumento();
    this.accountService.currentUser$.pipe(take(1)).subscribe((user) => {
      this.tipo = user ? user.tipo : undefined;
      this.podeEditar = this.tipo === 'Administrador' || this.tipo === 'UsuarioAvancado';
    });
  }

  public carregarDocumento() {
    this.documentoId = +this.activatedRouter.snapshot.paramMap.get('id');

    if (this.documentoId !== null && this.documentoId !== 0) {
      this.downloadUrl = `${environment.apiURL}api/documentos/${this.documentoId}/download`;
      this.previewUrl = `${environment.apiURL}api/documentos/${this.documentoId}/download?inline=true`;
      this.documentoService.getDocumentoById(this.documentoId).subscribe(
        (documento: Documento) => {
          this.documento = documento;
          if (this.documento.documentoURL !== '') {
            this.documentoURL = this.documento.documentoURL;
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
}
