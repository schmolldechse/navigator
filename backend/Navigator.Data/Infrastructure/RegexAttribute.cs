using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Navigator.Data.Infrastructure;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class RegexAttribute : ValidationAttribute
{
    private static readonly TimeSpan Timeout = TimeSpan.FromMilliseconds(50);

    private readonly string _pattern;

    public RegexAttribute(string pattern)
    {
        _pattern = pattern;
    }

    /// <summary>
    /// Message returned when the value does not match the allowed pattern.
    /// Usage: [Regex("...", Exception = "...")]
    /// </summary>
    public string? Exception { get; init; }

    public int MaxLength { get; init; } = 64;

    public bool AllowNullOrWhiteSpace { get; init; } = true;

    /// <summary>
    /// Checks whether the submitted value itself is a valid regex.
    /// </summary>
    public bool ValidateRegexSyntax { get; init; } = true;

    /// <summary>
    /// Rejects regexes that match the empty string, e.g. ".*", "^.*", "a*", "^(MEX|RE)*".
    /// </summary>
    public bool RejectEmptyMatches { get; init; } = true;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null) return ValidationResult.Success;
        if (value is not string input) return Fail(validationContext, $"{validationContext.DisplayName} must be a string.");

        input = input.Trim();
        if (string.IsNullOrWhiteSpace(input)) return AllowNullOrWhiteSpace
                ? ValidationResult.Success
                : Fail(validationContext, $"{validationContext.DisplayName} must not be empty.");

        if (input.Length > MaxLength) return Fail(validationContext, $"{validationContext.DisplayName} must be at most {MaxLength} characters long.");

        if (!IsAllowedInput(input)) return Fail(validationContext, $"{validationContext.DisplayName} contains unsupported regex characters.");

        if (ValidateRegexSyntax)
        {
            var syntaxValidation = ValidateRegex(input, validationContext);
            if (syntaxValidation is not null) return syntaxValidation;
        }

        return ValidationResult.Success;
    }

    private bool IsAllowedInput(string input)
    {
        try
        {
            return Regex.IsMatch(input, _pattern, RegexOptions.CultureInvariant, Timeout);
        }
        catch (ArgumentException exception)
        {
            throw new InvalidOperationException($"The allowed pattern configured on {nameof(RegexAttribute)} is invalid: {_pattern}", exception);
        }
    }

    private ValidationResult? ValidateRegex(string input, ValidationContext validationContext)
    {
        try
        {
            var regex = new Regex(input, RegexOptions.CultureInvariant, Timeout);
            if (RejectEmptyMatches && regex.IsMatch(string.Empty)) return Fail(validationContext, $"{validationContext.DisplayName} must not match an empty value.");

            return null;
        }
        catch (ArgumentException exception)
        {
            return Fail(validationContext, $"{validationContext.DisplayName} is not a valid regular expression: {exception.Message}");
        }
        catch (RegexMatchTimeoutException exception)
        {
            return Fail(validationContext, $"{validationContext.DisplayName} took too long to validate.");
        }
    }

    private static ValidationResult Fail(ValidationContext validationContext, string message)
    {
        var memberNames = validationContext.MemberName is null
            ? null
            : new[] { validationContext.MemberName };
        return new ValidationResult(message, memberNames);
    }
}
