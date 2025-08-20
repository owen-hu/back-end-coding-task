using Claims.Core;

namespace Claims.ApiLayer;

public class ClaimDto
{
    /// <summary>
    /// ID: Ignored when adding
    /// </summary>
    public string? Id { get; set; }
    
    /// <summary>
    /// Must have a corresponding cover
    /// </summary>
    public string CoverId { get; set; }
    
    /// <summary>
    /// YYYY-MM-DD
    /// </summary>
    public DateOnly CreatedDate { get; set; }
    
    /// <summary>
    /// Name of Claim
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// e.g Fire, Collision etc.
    /// </summary>
    public ClaimType ClaimType { get; set; }
    
    /// <summary>
    /// Amount in USD? NOK? Who knows...
    /// </summary>
    public decimal DamageCost { get; set; }
}