using FluentValidation.Results;

namespace ERPLite.Services.Helpers
{
    public static class ValidationResultExtensions
    {
        public static string ToErrorString(this ValidationResult result)
        {
            return string.Join("; ", result.Errors.Select(e => e.ErrorMessage));
        }
    }
}
