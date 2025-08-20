namespace Claims.DataLayer.Claims;

/// <summary>
/// Repository wrapper: Use this in Service Layer instead of calling the Contexts
/// directly to avoid hard-to-mock Linq-to-SQL infecting the business logic.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetItemsAsync();
    Task<T?> GetItemAsync(string id);

    Task AddItemAsync(T item);

    Task DeleteItemAsync(T item);
}