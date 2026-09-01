import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { ModalModule } from 'ngx-bootstrap/modal';
import { ReactiveFormsModule } from '@angular/forms';

import { CriarEditarComponent } from './criar-editar.component';

describe('CriarEditarComponent', () => {
  let component: CriarEditarComponent;
  let fixture: ComponentFixture<CriarEditarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [CriarEditarComponent],
    imports: [RouterTestingModule, ModalModule.forRoot(), ReactiveFormsModule],
    providers: [provideHttpClient(withInterceptorsFromDi())]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CriarEditarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
