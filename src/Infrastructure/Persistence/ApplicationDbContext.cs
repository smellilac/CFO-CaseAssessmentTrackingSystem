﻿using System.Reflection;
using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Candidates;
using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Identity;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;

namespace Cfo.Cats.Infrastructure.Persistence;

#nullable disable
public class ApplicationDbContext
    : IdentityDbContext<
        ApplicationUser,
        ApplicationRole,
        int,
        ApplicationUserClaim,
        ApplicationUserRole,
        ApplicationUserLogin,
        ApplicationRoleClaim,
        ApplicationUserToken
    >,
        IApplicationDbContext,
        IDataProtectionKeyContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Tenant> Tenants => Set<Tenant>();

    public DbSet<AuditTrail> AuditTrails => Set<AuditTrail>();

    public DbSet<Contract> Contracts => Set<Contract>();

    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<KeyValue> KeyValues => Set<KeyValue>();
    public DbSet<Candidate> Candidates => Set<Candidate>();

    public DbSet<Location> Locations => Set<Location>();

    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.ApplyGlobalFilters<ISoftDelete>(s => s.Deleted == null);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured) { }
    }
}