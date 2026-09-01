export const CATEGORIA_LABELS = [
  'Resumo',
  'Artigo',
  'Monografia',
  'Dissertação',
  'Tese',
  'Livro',
  'Projeto',
] as const;

export const CATEGORIA_CORES = [
  '#2780e3', // Resumo
  '#3fb618', // Artigo
  '#8f4fd1', // Monografia
  '#e8590c', // Dissertação
  '#d6336c', // Tese
  '#20c997', // Livro
  '#fd7e14', // Projeto
] as const;

export function corCategoria(categoria: number): string {
  return CATEGORIA_CORES[categoria] || '#2780e3';
}

export function labelCategoria(categoria: number): string {
  return CATEGORIA_LABELS[categoria] || 'Documento';
}
