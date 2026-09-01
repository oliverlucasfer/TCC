import { Component, OnInit, TemplateRef } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { DocumentoService } from 'src/app/services/documento.service';
import { areas, Documento } from 'src/app/models/Documento';
import { environment } from 'src/environments/environment';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { AccountService } from 'src/app/services/account.service';
import { ToastService } from 'src/app/services/toast.service';
import { take } from 'rxjs/operators';

@Component({
    selector: 'app-criar-editar',
    templateUrl: './criar-editar.component.html',
    styleUrls: ['./criar-editar.component.scss'],
    standalone: false
})
export class CriarEditarComponent implements OnInit {
  documento = {} as Documento;
  documentoId!: number;
  documentoURL!: string;
  form!: UntypedFormGroup;
  estadoSalvar = 'post';
  file!: File;
  mostrar = false;
  change = false;
  modalRef?: BsModalRef;
  areas = areas;
  podeEditar = false;

  get f(): any {
    return this.form.controls;
  }

  constructor(
    private fb: UntypedFormBuilder,
    private activatedRouter: ActivatedRoute,
    private router: Router,
    private documentoService: DocumentoService,
    private modalService: BsModalService,
    private accountService: AccountService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.carregarDocumento();
    this.validation();
    this.accountService.currentUser$.pipe(take(1)).subscribe((user) => {
      const tipo = user ? user.tipo : undefined;
      this.podeEditar = tipo === 'Administrador';
    });
  }

  setChange() {
    this.change = true;
  }

  public carregarDocumento() {
    this.documentoId = Number(this.activatedRouter.snapshot.paramMap.get('id'));

    if (this.documentoId > 0) {
      this.estadoSalvar = 'put';
      this.mostrar = true;

      this.documentoService.getDocumentoById(this.documentoId).subscribe(
        (documento: Documento) => {
          this.documento = { ...documento };
          this.form.patchValue(this.documento);
          if (this.documento.documentoURL !== '') {
            this.documentoURL =
              environment.apiURL +
              'resources/pdfs/' +
              this.documento.documentoURL;
          }
        },
        (error: any) => {
          this.toastService.error('Erro ao carregar documento.');
        }
      );
    }
  }

  public validation(): void {
    this.form = this.fb.group({
      titulo: [
        '',
        [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(150),
        ],
      ],
      autor: [
        '',
        [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(70),
        ],
      ],
      area: ['', [Validators.required]],
      categoria: ['', Validators.required],
      palavrasChave: ['', Validators.required],
      resumo: ['', Validators.required],
      ano: ['', Validators.required],
      documentoURL: [''],
    });
  }

  public resetForm(): void {
    this.form.reset();
  }

  public salvarAlteracao(event: any, template: TemplateRef<any>): void {
    if (this.estadoSalvar == 'post') {
      this.documento = { ...this.form.value, categoria: Number(this.form.value.categoria) };
      this.documentoService.postDocumento(this.documento).subscribe(
        (documento: Documento) => {
          this.documentoId = documento.id;
          event.stopPropagation();
          this.modalRef = this.modalService.show(template, {
            class: 'modal-sm',
          });
        },
        (error: any) => {
          this.toastService.error('Erro ao criar documento.');
        }
      );
    } else {
      this.documento = {
        id: this.documento.id,
        documentoText: this.documento.documentoText,
        ...this.form.value,
        categoria: Number(this.form.value.categoria),
      };
      this.documentoService
        .putDocumento(this.documento.id, this.documento)
        .subscribe(
          () => {
            event.stopPropagation();
            this.modalRef = this.modalService.show(template, {
              class: 'modal-sm',
            });
          },
          (error: any) => {
            this.toastService.error('Erro ao atualizar documento.');
          }
        );
    }
  }

  onFileChange(ev: any): void {
    const reader = new FileReader();
    reader.onload = (event: any) => (this.documentoURL = event.target.result);

    this.file = ev.target.files;
    reader.readAsDataURL(this.file[0]);

    this.uploadDocumento();
  }

  uploadDocumento(): void {
    this.documentoService.postUpload(this.documentoId, this.file).subscribe(
      () => {
        this.carregarDocumento();
        this.modalRef?.hide();
        this.router.navigate(['lista']);
      },
      (error: any) => {
        this.toastService.error('Erro no upload do arquivo.');
      }
    );
  }

  close() {
    this.modalRef?.hide();
    this.router.navigate(['lista']);
  }

  excluir() {
    this.documentoService.deleteDocumento(this.documentoId).subscribe(
      (result: any) => {
        this.toastService.success('Documento excluído.');
        this.router.navigate(['lista']);
      },
      (error) => this.toastService.error('Erro ao excluir documento.')
    );
  }
}
