﻿using Cfo.Cats.Application.Features.Tenants.Caching;
using Cfo.Cats.Application.Features.Tenants.DTOs;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Tenants.Commands.AddEdit;

public class AddEditTenantCommand : ICacheInvalidatorRequest<Result<string>>
{
    [Description("Tenant Id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Description("Tenant Name")]
    public string? Name { get; set; }

    [Description("Description")]
    public string? Description { get; set; }

    public string CacheKey => TenantCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource =>
        TenantCacheKey.SharedExpiryTokenSource();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<TenantDto, AddEditTenantCommand>(MemberList.None);
            CreateMap<AddEditTenantCommand, Tenant>(MemberList.None);
        }
    }
}