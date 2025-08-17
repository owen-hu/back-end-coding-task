﻿using Microsoft.EntityFrameworkCore;

namespace Claims.DataLayer.Auditing;
public class AuditContext : DbContext
{
    public AuditContext(DbContextOptions<AuditContext> options) : base(options)
    {
    }
    public DbSet<ClaimAudit> ClaimAudits { get; set; }
    public DbSet<CoverAudit> CoverAudits { get; set; }
}
