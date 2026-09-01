import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ModalModule } from 'ngx-bootstrap/modal';
import { AppRoutingModule } from './app-routing.module';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { DocumentoService } from './services/documento.service';
import { authInterceptor } from './interceptors/interceptor';
import { DocumentoInfosComponent } from './components/documento/documento-infos/documento-infos.component';
import { CriarEditarComponent } from './components/documento/criar-editar/criar-editar.component';
import { ListagemDocumentosComponent } from './components/documento/listagem-documentos/listagem-documentos.component';
import { UserComponent } from './components/user/user.component';
import { LoginComponent } from './components/user/login/login.component';
import { RegistrationComponent } from './components/user/registration/registration.component';
import { PerfilComponent } from './components/user/perfil/perfil.component';
import { SidenavComponent } from './shared/sidenav/sidenav.component';
import { HeaderComponent } from './shared/header/header.component';
import { ToastComponent } from './shared/toast/toast.component';

@NgModule({ declarations: [
        AppComponent,
        HomeComponent,
        DocumentoInfosComponent,
        CriarEditarComponent,
        ListagemDocumentosComponent,
        UserComponent,
        LoginComponent,
        RegistrationComponent,
        PerfilComponent,
SidenavComponent,
        HeaderComponent,
        ToastComponent
    ],
    bootstrap: [AppComponent], imports: [FormsModule,
        ReactiveFormsModule,
        BrowserModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        ModalModule.forRoot(),
PaginationModule.forRoot()], providers: [
        DocumentoService,
        provideHttpClient(withInterceptors([authInterceptor])),
    ] })
export class AppModule {}
