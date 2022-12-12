import { HttpClient, HttpParams } from '@angular/common/http';
import { take, map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Documento } from '../models/Documento';
import { PaginatedResult } from '../models/Pagination';

@Injectable({
  providedIn: 'root',
})
export class DocumentoService {
  baseURL = 'http://localhost:5000/api/documentos';

  constructor(private http: HttpClient) {}

  public getDocumentos(
    page?: number,
    itemsPerPage?: number,
    term?: string,
    categoria?: number
  ): Observable<PaginatedResult<Documento[]>> {
    const paginatedResult: PaginatedResult<Documento[]> = new PaginatedResult<
      Documento[]
    >();

    let params = new HttpParams();

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page.toString());
      params = params.append('pageSize', itemsPerPage.toString());
    }

    if (term != null && term != '') params = params.append('term', term);
    if (categoria != null) params = params.append('categoria', categoria);

    return this.http
      .get<Documento[]>(this.baseURL, { observe: 'response', params })
      .pipe(
        take(1),
        map((response) => {
          paginatedResult.result = response.body;
          if (response.headers.has('Pagination')) {
            paginatedResult.pagination = JSON.parse(
              response.headers.get('Pagination')
            );
          }

          return paginatedResult;
        })
      );
  }

  public getDocumentosByCategoria(
    categoria: number,
    page?: number,
    itemsPerPage?: number,
    term?: string
  ): Observable<PaginatedResult<Documento[]>> {
    const paginatedResult: PaginatedResult<Documento[]> = new PaginatedResult<
      Documento[]
    >();

    let params = new HttpParams();

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page.toString());
      params = params.append('pageSize', itemsPerPage.toString());
    }

    if (term != null && term != '') params = params.append('term', term);
    return this.http
      .get<Documento[]>(`${this.baseURL}/categoria`, {
        observe: 'response',
        params,
      })
      .pipe(
        take(1),
        map((response) => {
          paginatedResult.result = response.body;
          if (response.headers.has('Pagination')) {
            paginatedResult.pagination = JSON.parse(
              response.headers.get('Pagination')
            );
          }

          return paginatedResult;
        })
      );
  }

  public getDocumentoById(id: number): Observable<Documento> {
    return this.http.get<Documento>(`${this.baseURL}/${id}`);
  }

  public postDocumento(documento: Documento): Observable<Documento> {
    return this.http.post<Documento>(this.baseURL, documento);
  }

  postUpload(documentoId: number, file: File): Observable<Documento> {
    const fileToUpload = file[0] as File;
    const formData = new FormData();
    formData.append('file', fileToUpload);

    return this.http
      .post<Documento>(
        `${this.baseURL}/upload-documento/${documentoId}`,
        formData
      )
      .pipe(take(1));
  }

  public putDocumento(id: number, documento: Documento): Observable<Documento> {
    return this.http.put<Documento>(`${this.baseURL}/${id}`, documento);
  }

  public deleteDocumento(id: number): Observable<any> {
    return this.http.delete(`${this.baseURL}/${id}`);
  }
}
