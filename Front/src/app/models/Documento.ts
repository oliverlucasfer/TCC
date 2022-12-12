export const categorias = [
  'Resumo',
  'Artigo',
  'Monografia',
  'Dissertacao',
  'Tese',
  'Projeto',
  'Livro',
] as const;

export interface Documento {
  id: number;
  titulo: string;
  autor: string;
  area: string;
  categoria: number;
  documentoURL: string;
  palavrasChave: string;
  resumo: string;
  ano: string;
}
