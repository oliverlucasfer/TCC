import {
  Component,
  ElementRef,
  OnInit,
  TemplateRef,
  ViewChild,
} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { DocumentoService } from 'src/app/services/documento.service';
import { Documento } from 'src/app/models/Documento';
import { environment } from 'src/environments/environment';

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

  get f(): any {
    return this.form.controls;
  }

  constructor(
    private fb: FormBuilder,
    private activatedRouter: ActivatedRoute,
    private router: Router,
    private documentoService: DocumentoService
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
      area: [
        '',
        [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(50),
        ],
      ],
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

  public salvarAlteracao(): void {
    if (this.estadoSalvar == 'post') {
      this.documento = { ...this.form.value };
      this.documentoService.postDocumento(this.documento).subscribe(
        () => {
          this.router.navigate(['lista']);
        },
        (error: any) => {
          console.error(error);
        }
      );
    } else {
      this.documento = { id: this.documento.id, ...this.form.value };
      this.documentoService
        .putDocumento(this.documento.id, this.documento)
        .subscribe(
          () => {
            this.router.navigate(['lista']);
          },
          (error: any) => {
            console.error(error);
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

        this.router.navigate(['lista']);
      },
      (error: any) => {
        console.error(error);
      }
    );
  }
}
