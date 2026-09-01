import { TestBed } from '@angular/core/testing';
import { HttpClientModule } from '@angular/common/http';
import { DocumentoService } from './documento.service';

describe('Service: Documento', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientModule],
      providers: [DocumentoService]
    });
  });

  it('should ...', () => {
    const service: DocumentoService = TestBed.inject(DocumentoService);
    expect(service).toBeTruthy();
  });
});
