using Claims.DataLayer.Claims;
using Microsoft.EntityFrameworkCore;

namespace Claims.RepositoryLayer;

public class ClaimsRepository: IRepository<Claim>
{
    private readonly ClaimsContext _context;
    public ClaimsRepository(ClaimsContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Claim>> GetItemsAsync()
    {
        return await _context.Claims.ToListAsync();
    }

    public async Task<Claim?> GetItemAsync(string id)
    {
        return await _context.Claims
            .Where(claim => claim.Id == id)
            .SingleOrDefaultAsync();
    }

    public async Task AddItemAsync(Claim item)
    {
        _context.Claims.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteItemAsync(Claim item)
    {
        _context.Claims.Remove(item);
        await _context.SaveChangesAsync();
    }
}