﻿using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Application.Features.Tenants.Commands;

[RequestAuthorize(Roles = "Admin")]
public static class AddDomainCommand
{
    [RequestAuthorize(Policy = PolicyNames.SystemFunctionsWrite)]
    public class Command : IRequest<Result>
    {
        [Description("Tenant Id")]
        public string? TenantId { get; set; }

        [Description("Domain")]
        public string? Domain { get; set; }

        private class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Command, TenantDomain>(MemberList.None);
            }
        }
    }

    internal class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var tenant = await unitOfWork.DbContext.Tenants.FindAsync(request.TenantId);

            if (tenant is null)
            {
                throw new NotFoundException(nameof(Tenant), request.TenantId!);
            }

            var model = mapper.Map<TenantDomain>(request);

            tenant.AddDomain(model);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(v => v.Domain)
                .MaximumLength(255)
                .WithMessage("Domain must be no more than 255 characters")
                .NotEmpty()
                .WithMessage("Domain is required");

            /*
             * Match "domain" patterns, case sensitive. Examples:
             * @example.com
             * @test.example.com
             * @my-domain.com
            */
            RuleFor(v => v.Domain)
                .Matches(@"^@[a-z0-9]+(?:[-]?[a-z0-9]+)*(?:\.[a-z0-9]+(?:[-]?[a-z0-9]+)*)+$")
                .WithMessage("Must be in the format '@example.com'");

            RuleFor(v => v.TenantId)
                .NotEmpty()
                .WithMessage("Tenant Id is required");
        }
    }

}