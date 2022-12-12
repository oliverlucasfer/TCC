import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CriarEditarComponent } from './components/documento/criar-editar/criar-editar.component';
import { DocumentoInfosComponent } from './components/documento/documento-infos/documento-infos.component';
import { ListagemDocumentosComponent } from './components/documento/listagem-documentos/listagem-documentos.component';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/user/login/login.component';
import { PerfilComponent } from './components/user/perfil/perfil.component';
import { RegistrationComponent } from './components/user/registration/registration.component';
import { UserComponent } from './components/user/user.component';
import { AuthGuard } from './guard/auth.guard';

const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  {
    path: 'home',
    component: HomeComponent,
  },
  {
    path: 'user',
    component: UserComponent,
    children: [
      { path: 'login', component: LoginComponent },
      { path: 'perfil', component: PerfilComponent, canActivate: [AuthGuard] },
      { path: 'registration', component: RegistrationComponent },
    ],
  },
  {
    path: 'lista',
    component: ListagemDocumentosComponent,
  },
  {
    path: ':categoria',
    component: ListagemDocumentosComponent,
  },
  {
    path: 'documento/:id',
    component: DocumentoInfosComponent,
  },
  {
    path: 'document/novo',
    component: CriarEditarComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'documento/editar/:id',
    component: CriarEditarComponent,
    canActivate: [AuthGuard],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
