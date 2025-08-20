namespace Claims.Core;

public interface IAuditor
{
    void AuditClaim(string id, string httpRequestType);
    void AuditCover(string id, string httpRequestType);
}