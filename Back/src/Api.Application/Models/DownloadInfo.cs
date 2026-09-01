namespace Api.Application.Models
{
    public class DownloadInfo
    {
        public string CaminhoArquivo { get; set; }
        public string NomeOriginal { get; set; }
        public bool ArquivoExiste { get; set; }

        public DownloadInfo(string caminhoArquivo, string nomeOriginal, bool arquivoExiste)
        {
            CaminhoArquivo = caminhoArquivo;
            NomeOriginal = nomeOriginal;
            ArquivoExiste = arquivoExiste;
        }
    }
}