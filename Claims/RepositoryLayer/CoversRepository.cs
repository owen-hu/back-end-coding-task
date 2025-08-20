using Claims.DataLayer.Claims;
using Microsoft.EntityFrameworkCore;

namespace Claims.RepositoryLayer;

public class CoversRepository: IRepository<Cover>
{
    private readonly ClaimsContext _context;
    
    public CoversRepository(ClaimsContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Cover>> GetItemsAsync()
    {
        return await _context.Covers.ToListAsync();
    }

    public async Task<Cover?> GetItemAsync(string id)
    {
        return await _context.Covers
            .Where(cover => cover.Id == id)
            .SingleOrDefaultAsync();
    }

    public async Task AddItemAsync(Cover item)
    {
        _context.Covers.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteItemAsync(Cover item)
    {
        _context.Covers.Remove(item);
        await _context.SaveChangesAsync();
    }
}