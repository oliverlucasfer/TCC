import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { areas, categorias, Documento } from 'src/app/models/Documento';
import { AccountService } from 'src/app/services/account.service';
import { DocumentoService } from 'src/app/services/documento.service';

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
  areas = areas;
  tipo: any;

  constructor(
    private activatedRouter: ActivatedRoute,
    private documentoService: DocumentoService,
    private accountService: AccountService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.carregarDocumento();
    this.accountService.getUser().subscribe((u) => {
      var user = JSON.parse(localStorage.getItem('user'));
      for (let index = 0; index < u.length; index++) {
        if (user.userName == u[index].userName) {
          this.tipo = u[index].tipo;
        }
      }
    });
  }

  public carregarDocumento() {
    this.documentoId = +this.activatedRouter.snapshot.paramMap.get('id');

    if (this.documentoId !== null && this.documentoId !== 0) {
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
