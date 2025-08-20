using Claims.Core;
using Claims.DataLayer.Claims;

namespace Claims.ApiLayer;

public class CoverDto
{
    /// <summary>
    /// Id (do not use on create)
    /// </summary>
    public string? Id { get; set; }
    
    /// <summary>
    /// Start of the Cover Period
    /// </summary>
    public DateOnly StartDate { get; set; }
    
    /// <summary>
    /// End of the Cover Period
    /// </summary>
    public DateOnly EndDate { get; set; }
    
    /// <summary>
    /// e.g Collision
    /// </summary>
    public CoverType CoverType { get; set; }
    
    /// <summary>
    /// The calculated premium (do not use on create)
    /// </summary>
    public decimal? Premium { get; set; }
}