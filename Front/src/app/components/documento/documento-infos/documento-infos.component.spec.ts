import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentoInfosComponent } from './documento-infos.component';

describe('DocumentoInfosComponent', () => {
  let component: DocumentoInfosComponent;
  let fixture: ComponentFixture<DocumentoInfosComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DocumentoInfosComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DocumentoInfosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
