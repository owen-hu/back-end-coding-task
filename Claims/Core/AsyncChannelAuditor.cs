using System.Threading.Channels;
using Claims.DataLayer.Auditing;

namespace Claims.Core;

/// <summary>
/// Simulates an async auditor that passes to a Service-Bus Queue.
/// </summary>
/// <remarks>NOt ana attempt to create a valid Queue architecture. Should be good enough for Demo purposes</remarks>
public class AsyncChannelAuditor : IAuditor
{
    private readonly Auditor _innerAuditor;
    private readonly Channel<(string,string)> _claimsChannel;
    private readonly Channel<(string,string)> _coversChannel;

        
    public AsyncChannelAuditor(AuditContext auditContext, ILogger<AsyncChannelAuditor> logger)
    {
        _innerAuditor = new Auditor(auditContext);
            
        _claimsChannel = Channel.CreateUnbounded<(string, string)>();
        _coversChannel = Channel.CreateUnbounded<(string, string)>();
        // Start immediate consumer in background
        _ = Task.Run(async () =>
        {
            await foreach (var msg in _claimsChannel.Reader.ReadAllAsync())
            {
                logger.LogInformation($"Claims: Received: {msg.Item1}:::{msg.Item2}");
                _innerAuditor.AuditClaim(msg.Item1, msg.Item2);
            }
        });
            
        // Start immediate consumer in background
        _ = Task.Run(async () =>
        {
            await foreach (var msg in _coversChannel.Reader.ReadAllAsync())
            {
                logger.LogInformation($"Covers: Received: {msg.Item1}:::{msg.Item2}");
                _innerAuditor.AuditCover(msg.Item1, msg.Item2);
            }
        });
    }
        
    public void AuditClaim(string id, string httpRequestType)
    {
        _claimsChannel.Writer.WriteAsync((id,httpRequestType)).AsTask().Wait();
    }

    public void AuditCover(string id, string httpRequestType)
    {
        _coversChannel.Writer.WriteAsync((id,httpRequestType)).AsTask().Wait();
    }
}