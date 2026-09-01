using System;

namespace Api.Application.Models
{
    public class ApiException : Exception
    {
        public int Status { get; set; }
        public string Title { get; set; }

        public ApiException(string message, int status = 400, string title = "Erro de aplicação")
            : base(message)
        {
            Status = status;
            Title = title;
        }
    }
}