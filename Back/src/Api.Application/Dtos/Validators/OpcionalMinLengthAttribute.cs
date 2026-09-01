using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Application.Dtos.Validators
{
    /// <summary>
    /// Valida tamanho mínimo apenas quando o valor não é nulo nem vazio.
    /// Útil para senha opcional (perfil que não troca senha).
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OpcionalMinLengthAttribute : ValidationAttribute
    {
        private readonly int _minLength;

        public OpcionalMinLengthAttribute(int minLength)
        {
            _minLength = minLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            var text = value.ToString();
            if (string.IsNullOrWhiteSpace(text)) return ValidationResult.Success;
            if (text.Length < _minLength)
                return new ValidationResult(ErrorMessage ?? $"O campo deve ter no mínimo {_minLength} caracteres.");
            return ValidationResult.Success;
        }
    }
}