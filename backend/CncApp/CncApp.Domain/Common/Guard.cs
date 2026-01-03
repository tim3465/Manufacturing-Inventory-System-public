namespace CncApp.Domain.Common;

/// <summary>
/// Static helper class for enforcing domain invariants.
/// Guards protect against invalid state by throwing <see cref="DomainException"/> when constraints are violated.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Throws a <see cref="DomainException"/> if the specified value is null, empty, or whitespace.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter being validated (used in error message).</param>
    /// <exception cref="DomainException">Thrown when the value is null, empty, or whitespace.</exception>
    public static void AgainstNullOrWhiteSpace(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{parameterName} cannot be null, empty, or whitespace.");
        }
    }

    /// <summary>
    /// Throws a <see cref="DomainException"/> if the specified string value exceeds the maximum length.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="maxLength">The maximum allowed length.</param>
    /// <param name="parameterName">The name of the parameter being validated (used in error message).</param>
    /// <exception cref="DomainException">Thrown when the value exceeds the maximum length.</exception>
    public static void AgainstMaxLength(string? value, int maxLength, string parameterName)
    {
        if (value != null && value.Length > maxLength)
        {
            throw new DomainException($"{parameterName} cannot exceed {maxLength} characters. Actual length: {value.Length}.");
        }
    }
}

