import { Component, OnInit, TemplateRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { DocumentoService } from 'src/app/services/documento.service';
import { areas, Documento } from 'src/app/models/Documento';
import { environment } from 'src/environments/environment';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'app-criar-editar',
  templateUrl: './criar-editar.component.html',
  styleUrls: ['./criar-editar.component.scss'],
})
export class CriarEditarComponent implements OnInit {
  documento = {} as Documento;
  documentoId!: number;
  documentoURL!: string;
  form!: FormGroup;
  estadoSalvar = 'post';
  file!: File;
  mostrar = false;
  change = false;
  modalRef?: BsModalRef;
  areas = areas;

  get f(): any {
    return this.form.controls;
  }

  constructor(
    private fb: FormBuilder,
    private activatedRouter: ActivatedRoute,
    private router: Router,
    private documentoService: DocumentoService,
    private modalService: BsModalService
  ) {}

  ngOnInit(): void {
    this.carregarDocumento();
    this.validation();
  }

  setChange() {
    this.change = true;
  }

  public carregarDocumento() {
    this.documentoId = +this.activatedRouter.snapshot.paramMap.get('id');

    if (this.documentoId !== null && this.documentoId !== 0) {
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
          console.error(error);
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
      this.documento = { ...this.form.value };
      this.documentoService.postDocumento(this.documento).subscribe(
        () => {
          event.stopPropagation();
          this.modalRef = this.modalService.show(template, {
            class: 'modal-sm',
          });
        },
        (error: any) => {
          console.error(error);
        }
      );
    } else {
      this.documento = {
        id: this.documento.id,
        documentoText: this.documento.documentoText,
        ...this.form.value,
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
            console.error(error);
          }
        );
    }
  }

  onFileChange(ev: any): void {
    const reader = new FileReader();
    if (this.estadoSalvar == 'post') {
      this.documentoService.getDocumento().subscribe((d) => {
        reader.onload = (event: any) =>
          (this.documentoURL = event.target.result);

        this.file = ev.target.files;
        reader.readAsDataURL(this.file[0]);

        this.uploadDocumento(d.id);
      });
    } else {
      reader.onload = (event: any) => (this.documentoURL = event.target.result);

      this.file = ev.target.files;
      reader.readAsDataURL(this.file[0]);

      this.uploadDocumento();
    }
  }

  uploadDocumento(id?: number): void {
    if (this.estadoSalvar == 'post') {
      this.documentoId = id;
    }
    this.documentoService.postUpload(this.documentoId, this.file).subscribe(
      () => {
        this.carregarDocumento();
        this.modalRef?.hide();
        this.router.navigate(['lista']);
      },
      (error: any) => {
        console.error(error);
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
        this.router.navigate(['lista']);
      },
      (error) => console.log(error)
    );
  }
}
