using Claims.Core;

namespace Claims.ApiLayer;

public class ClaimDto
{
    public string? Id { get; set; }
    public string CoverId { get; set; }
    public DateOnly CreatedDate { get; set; }
    public string Name { get; set; }
    
    public ClaimType ClaimType { get; set; }
    public decimal DamageCost { get; set; }
}