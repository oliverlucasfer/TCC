export const categorias = [
  'Resumo',
  'Artigo',
  'Monografia',
  'Dissertacao',
  'Tese',
  'Projeto',
  'Livro',
] as const;

export const areas = [
  'Engenharia',
  'Saúde',
  'Direito',
  'Robótica',
  'Administração',
  'Contabilidade',
  'Desenvolvimento',
];

export interface Documento {
  id: number;
  titulo: string;
  autor: string;
  area: string;
  categoria: number;
  documentoURL: string;
  documentoText: string;
  palavrasChave: string;
  resumo: string;
  ano: string;
}
