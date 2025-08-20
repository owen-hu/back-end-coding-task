using System.Globalization;

namespace Claims.Core;

/// <summary>
/// Base Validation interface.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IValidator<T>
{
    /// <summary>
    /// Error on validation failure
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    /// <exception cref="ValidationException">If validation fails</exception>
    Task ValidateAsync(T item);
}