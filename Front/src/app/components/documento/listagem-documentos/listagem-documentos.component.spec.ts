import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListagemDocumentosComponent } from './listagem-documentos.component';

describe('ListagemDocumentosComponent', () => {
  let component: ListagemDocumentosComponent;
  let fixture: ComponentFixture<ListagemDocumentosComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ListagemDocumentosComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ListagemDocumentosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
