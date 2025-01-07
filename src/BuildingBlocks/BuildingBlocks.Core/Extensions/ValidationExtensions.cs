using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using BuildingBlocks.Core.Exception.Types;

namespace BuildingBlocks.Core.Extensions;

/// <summary>
/// Provides extension methods for validating arguments.
/// </summary>
public static partial class ValidationExtensions
{
    private static readonly HashSet<string> _allowedCurrency = ["USD", "EUR"];

    /// <summary>
    /// Ensures that an argument is not null.
    /// </summary>
    /// <typeparam name="T">The type of the argument.</typeparam>
    /// <param name="argument">The argument to validate.</param>
    /// <param name="argumentName">The name of the argument.</param>
    /// <returns>The validated argument.</returns>
    /// <exception cref="ValidationException">Thrown if the argument is null.</exception>
    public static T NotBeNull<T>(
        [NotNull] this T? argument,
        [CallerArgumentExpression("argument")] string? argumentName = null)
    {
        if (argument == null)
        {
            throw new ValidationException(message: $"{argumentName} cannot be null or empty.");
        }

        return argument;
    }

    /// <summary>
    /// Ensures that an argument is not null, throwing a specified exception if it is.
    /// </summary>
    /// <typeparam name="T">The type of the argument.</typeparam>
    /// <param name="argument">The argument to validate.</param>
    /// <param name="exception">The exception to throw if the argument is null.</param>
    /// <returns>The validated argument.</returns>
    /// <exception cref="System.Exception">Thrown if the argument is null.</exception>
    public static T NotBeNull<T>([NotNull] this T? argument, System.Exception exception)
    {
        if (argument == null)
        {
            throw exception;
        }

        return argument;
    }

    /// <summary>
    /// Ensures that a string argument is not invalid or empty.
    /// </summary>
    /// <param name="argument">The argument to validate.</param>
    /// <param name="argumentName">The name of the argument.</param>
    /// <returns>The validated argument.</returns>
    /// <exception cref="ValidationException">Thrown if the argument is empty.</exception>
    public static string NotBeInvalid(
        this string argument,
        [CallerArgumentExpression("argument")] string? argumentName = null)
    {
        if (argument.Length == 0)
        {
            throw new ValidationException($"{argumentName} cannot be null or empty.");
        }

        return argument;
    }

    /// <summary>
    /// Ensures that a string argument is not null or empty.
    /// </summary>
    /// <param name="argument">The argument to validate.</param>
    /// <param name="argumentName">The name of the argument.</param>
    /// <returns>The validated argument.</returns>
    /// <exception cref="ValidationException">Thrown if the argument is null or empty.</exception>
    public static string NotBeEmptyOrNull(
        [NotNull] this string? argument,
        [CallerArgumentExpression("argument")] string? argumentName = null)
    {
        if (string.IsNullOrEmpty(argument))
        {
            throw new ValidationException($"{argumentName} cannot be null or empty.");
        }

        return argument;
    }

    /// <summary>
    /// Ensures that a string argument is not null or whitespace.
    /// </summary>
    /// <param name="argument">The argument to validate.</param>
    /// <param name="argumentName">The name of the argument.</param>
    /// <returns>The validated argument.</returns>
    /// <exception cref="ValidationException">Thrown if the argument is null or whitespace.</exception>
    public static string NotBeNullOrWhiteSpace(
        [NotNull] this string? argument,
        [CallerArgumentExpression("argument")] string? argumentName = null)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            throw new ValidationException($"{argumentName} cannot be null or white space.");
        }

        return argument;
    }

    /// <summary>
    /// Ensures that a Guid argument is not invalid (empty).
    /// </summary>
    /// <param name="argument">The argument to validate.</param>
    /// <param name="argumentName">The name of the argument.</param>
    /// <returns>The validated argument.</returns>
    /// <exception cref="ValidationException">Thrown if the argument is empty.</exception>
    public static Guid NotBeInvalid(
        this Guid argument,
        [CallerArgumentExpression("argument")] string? argumentName = null)
    {
        if (argument == Guid.Empty)
        {
            throw new ValidationException($"{argumentName} cannot be empty.");
        }

        return argument;
    }

    /// <summary>
    /// Ensures that a nullable Guid argument is not null or invalid (empty).
    /// </summary>
    /// <param name="argument">The argument to validate.</param>
    /// <param name="argumentName">The name of the argument.</param>
    /// <returns>The validated argument.</returns>
    /// <exception cref="ValidationException">Thrown if the argument is null or empty.</exception>
    public static Guid NotBeInvalid(
        [NotNull] this Guid? argument,
        [CallerArgumentExpression("argument")] string? argumentName = null)
    {
        if (argument is null)
        {
            throw new ValidationException(message: $"{argumentName} cannot be null or empty.");
        }

        return argument.Value.NotBeInvalid();
    }

    /// <summary>
    /// Ensures that an integer argument is not negative or zero.
    /// </summary>
    /// <param name="argument">The argument to validate.</param>
    /// <param name="argumentName">The name of the argument.</param>
    /// <returns>The validated argument.</returns>
    /// <exception cref="ValidationException">Thrown if the argument is negative or zero.</exception>
    public static int NotBeNegativeOrZero(
        this int argument,
        [CallerArgumentExpression("argument")] string? argumentName = null)
    {
        if (argument <= 0)
        {
            throw new ValidationException($"{argumentName} cannot be zero.");
        }

        return argument;
    }

    /// <summary>
    /// Ensures that a nullable long argument is not null, negative, or zero.
    /// </summary>
    /// <param name="argument">The argument to validate.</param>
    /// <param name="argumentName">The name of the argument.</param>
    /// <returns>The validated argument.</returns>
    /// <exception cref="ValidationException">Thrown if the argument is null, negative, or zero.</exception>
    public static long NotBeNegativeOrZero(
        [NotNull] this long? argument,
        [CallerArgumentExpression("argument")] string? argumentName = null)
    {
        if (argument is null)
        {
            throw new ValidationException(message: $"{argumentName} cannot be null or empty.");
        }

        return argument.Value.NotBeNegativeOrZero();
    }

    /// <summary>
    /// Ensures that a long argument is not negative or zero.
    /// </summary>
    /// <param name="argument">The argument to validate.</param>
    /// <param name="argumentName">The name of the argument.</param>
    /// <returns>The validated argument.</returns>
    /// <exception cref="ValidationException">Thrown if the argument is negative or zero.</exception>
    public static long NotBeNegativeOrZero(
        this long argument,
        [CallerArgumentExpression("argument")] string? argumentName = null)
    {
        if (argument <= 0)
        {
            throw new ValidationException($"{argumentName} cannot be negative or zero.");
        }

        return argument;
    }

    /// <summary>
    /// Ensures that a nullable integer argument is not null, negative, or zero.
    /// </summary>
    /// <param name="argument">The argument to validate.</param>
    /// <param name="argumentName">The name of the argument.</param>
    /// <returns>The validated argument.</returns>
    /// <exception cref="ValidationException">Thrown if the argument is null, negative, or zero.</exception>
    public static int NotBeNegativeOrZero(
        [NotNull] this int? argument,
        [CallerArgumentExpression("argument")] string? argumentName = null)
    {
        if (argument is null)
        {
            throw new ValidationException(message: $"{argumentName} cannot be null or empty.");
        }

        return argument.Value.NotBeNegativeOrZero();
    }

    /// <summary>
    /// Ensures that a decimal argument is not negative or zero.
    /// </summary>
    /// <param name="argument">The argument to validate.</param>
    /// <param name="argumentName">The name of the argument.</param>
    /// <returns>The validated argument.</returns>
    /// <exception cref="ValidationException">Thrown if the argument is negative or zero.</exception>
    public static decimal NotBeNegativeOrZero(
        this decimal argument,
        [CallerArgumentExpression("argument")] string? argumentName = null)
    {
        if (argument <= 0)
        {
            throw new ValidationException($"{argumentName} cannot be negative or zero.");
        }

        return argument;
    }

    /// <summary>
    /// Ensures that a nullable decimal argument is not null, negative, or zero.
    /// </summary>
    /// <param name="argument">The argument to validate.</param>
    /// <param name="argumentName">The name of the argument.</param>
    /// <returns>The validated argument.</returns>
    /// <exception cref="ValidationException">Thrown if the argument is null, negative, or zero.</exception>
    public static decimal NotBeNegativeOrZero(
        [NotNull] this decimal? argument,
        [CallerArgumentExpression("argument")] string? argumentName = null)
    {
        if (argument is null)
        {
            throw new ValidationException(message: $"{argumentName} cannot be null or empty.");
        }

        return argument.Value.NotBeNegativeOrZero();
    }

    /// <summary>
    /// Ensures that a double argument is not negative or zero.
    /// </summary>
    /// <param name="argument">The argument to validate.</param>
    /// <param name="argumentName">The name of the argument.</param>
    /// <returns>The validated argument.</returns>
    /// <exception cref="ValidationException">Thrown if the argument is negative or zero.</exception>
    public static double NotBeNegativeOrZero(
        this double argument,
        [CallerArgumentExpression("argument")] string? argumentName = null)
    {
        if (argument <= 0)
        {
            throw new ValidationException($"{argumentName} cannot be zero.");
        }

        return argument;
    }

    /// <summary>
    /// Ensures that a nullable double argument is not null, negative, or zero.
    /// </summary>
    /// <param name="argument">The argument to validate.</param>
    /// <param name="argumentName">The name of the argument.</param>
    /// <returns>The validated argument.</returns>
    /// <exception cref="ValidationException">Thrown if the argument is null, negative, or zero.</exception>
    public static double NotBeNegativeOrZero(
        [NotNull] this double? argument,
        [CallerArgumentExpression("argument")] string? argumentName = null)
    {
        if (argument is null)
        {
            throw new ValidationException(message: $"{argumentName} cannot be null or empty.");
        }

        return argument.Value.NotBeNegativeOrZero();
    }

    /// <summary>
    /// Ensures that a string argument is a valid email address.
    /// </summary>
    /// <param name="email">The email address to validate.</param>
    /// <param name="argumentName">The name of the argument.</param>
    /// <returns>The validated email address.</returns>
    /// <exception cref="ValidationException">Thrown if the email address is invalid.</exception>
    public static string NotBeInvalidEmail(
        this string email,
        [CallerArgumentExpression("email")] string? argumentName = null)
    {
        // Use Regex to validate email format
        var regex = EmailRegex();
        if (!regex.IsMatch(email))
        {
            throw new ValidationException($"{argumentName} is not a valid email address.");
        }

        return email;
    }

    /// <summary>
    /// Ensures that a string argument is a valid phone number.
    /// </summary>
    /// <param name="phoneNumber">The phone number to validate.</param>
    /// <param name="argumentName">The name of the argument.</param>
    /// <returns>The validated phone number.</returns>
    /// <exception cref="ValidationException">Thrown if the phone number is invalid.</exception>
    public static string NotBeInvalidPhoneNumber(
        this string phoneNumber,
        [CallerArgumentExpression("phoneNumber")]
        string? argumentName = null)
    {
        var regex = PhoneNumberRegex();

        if (!regex.IsMatch(phoneNumber))
        {
            throw new ValidationException($"{argumentName} is not a valid phone number.");
        }

        return phoneNumber;
    }

    /// <summary>
    /// Ensures that a string argument is a valid mobile number.
    /// </summary>
    /// <param name="mobileNumber">The mobile number to validate.</param>
    /// <param name="argumentName">The name of the argument.</param>
    /// <returns>The validated mobile number.</returns>
    /// <exception cref="ValidationException">Thrown if the mobile number is invalid.</exception>
    public static string NotBeInvalidMobileNumber(
        this string mobileNumber,
        [CallerArgumentExpression("mobileNumber")]
        string? argumentName = null)
    {
        // Use Regex to validate mobile number format
        var regex = MobileNumberRegex();

        if (!regex.IsMatch(mobileNumber))
        {
            throw new ValidationException($"{argumentName} is not a valid mobile number.");
        }

        return mobileNumber;
    }

    /// <summary>
    /// Ensures that a string argument is a valid currency.
    /// </summary>
    /// <param name="currency">The currency to validate.</param>
    /// <param name="argumentName">The name of the argument.</param>
    /// <returns>The validated currency.</returns>
    /// <exception cref="ValidationException">Thrown if the currency is invalid.</exception>
    public static string NotBeInvalidCurrency(
        this string currency,
        [CallerArgumentExpression("currency")] string? argumentName = null)
    {
        currency = currency.ToUpperInvariant();
        if (!_allowedCurrency.Contains(currency))
        {
            throw new ValidationException($"{argumentName} is not a valid currency.");
        }

        return currency;
    }

    /// <summary>
    /// Ensures that an enum argument is not null or empty.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="enumValue">The enum value to validate.</param>
    /// <param name="argumentName">The name of the argument.</param>
    /// <returns>The validated enum value.</returns>
    /// <exception cref="ValidationException">Thrown if the enum value is null or empty.</exception>
    public static TEnum NotBeEmptyOrNull<TEnum>(
        [NotNull] this TEnum? enumValue,
        [CallerArgumentExpression("enumValue")]
        string argumentName = "")
        where TEnum : Enum
    {
        if (enumValue is null)
        {
            throw new ValidationException(message: $"{argumentName} cannot be null or empty.");
        }

        enumValue.NotBeInvalid();

        return enumValue;
    }

    /// <summary>
    /// Ensures that an enum argument is valid.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="enumValue">The enum value to validate.</param>
    /// <param name="argumentName">The name of the argument.</param>
    /// <returns>The validated enum value.</returns>
    /// <exception cref="ValidationException">Thrown if the enum value is not valid.</exception>
    public static TEnum NotBeInvalid<TEnum>(
        [NotNull] this TEnum enumValue,
        [CallerArgumentExpression("enumValue")]
        string? argumentName = null)
        where TEnum : Enum
    {
        enumValue.NotBeNull();

        // returns `true` if `enumValue` corresponds to one of the defined values in `TEnum`
        if (!Enum.IsDefined(typeof(TEnum), enumValue))
        {
            throw new ValidationException(
                $"The value of '{argumentName}' is not valid for enum of '{typeof(TEnum).Name}'.");
        }

        return enumValue;
    }

    /// <summary>
    /// Ensures that a DateTime argument is not the default value.
    /// </summary>
    /// <param name="dateTime">The date time to validate.</param>
    /// <param name="argumentName">The name of the argument.</param>
    /// <exception cref="ValidationException">Thrown if the date time is the default value.</exception>
    public static void NotBeInvalid(
        this DateTime dateTime,
        [CallerArgumentExpression("dateTime")] string? argumentName = null)
    {
        var isEmpty = dateTime == DateTime.MinValue;
        if (isEmpty)
        {
            throw new ValidationException(
                $"The value of '{argumentName}' cannot be the default value of '{dateTime}'.");
        }
    }

    #region Regex

    [GeneratedRegex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")]
    private static partial Regex EmailRegex();

    // Use Regex to validate phone number format
    // valid phones: +10---------- , (+10)----------
    [GeneratedRegex(@"^[+]?[(]?[+]?[0-9]{1,4}[)]?[-\s./0-9]{9,12}$")]
    private static partial Regex PhoneNumberRegex();

    [GeneratedRegex(@"^(?:\+|00)?(\(\d{1,3}\)|\d{1,3})?([1-9]\d{9})$")]
    private static partial Regex MobileNumberRegex();

    #endregion
}