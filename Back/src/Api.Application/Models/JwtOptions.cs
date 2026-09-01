namespace Api.Application.Models
{
    public class JwtOptions
    {
        public const string SectionName = "Jwt";
        public string TokenKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = "ProDocs";
        public string Audience { get; set; } = "ProDocsClient";
        public int ExpirationHours { get; set; } = 12;
    }
}