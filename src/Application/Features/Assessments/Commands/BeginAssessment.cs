using System.Text.Json.Serialization;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Assessments.Caching;
using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Education;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.HealthAndAdditiction;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Housing;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Money;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.ThoughtsAndBehaviours;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.WellbeingAndMentalHealth;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Assessments;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Assessments.Commands;

public static class BeginAssessment
{
    [RequestAuthorize(Policy = PolicyNames.AllowEnrol)]
    public class Command : ICacheInvalidatorRequest<Result<Guid>>
    {
        public required string ParticipantId { get; set; }
        
        //TODO: this could be done at a per participant level
        public string[] CacheKeys => [ AssessmentsCacheKey.GetAllCacheKey ];
        public CancellationTokenSource? SharedExpiryTokenSource 
            => AssessmentsCacheKey.SharedExpiryTokenSource();
    }

    public class Handler : IRequestHandler<Command, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public Handler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            Assessment assessment = new Assessment()
            {
                Id = Guid.NewGuid(),
                ParticipantId = request.ParticipantId,
                Pathways =
                [
                    new WorkingPathway(),
                    new HousingPathway(),
                    new MoneyPathway(),
                    new EducationPathway(),
                    new HealthAndAddictionPathway(),
                    new RelationshipsPathway(),
                    new ThoughtsAndBehavioursPathway(),
                    new WellbeingAndMentalHealthPathway(),
                ]
            };

            string json = JsonConvert.SerializeObject(assessment, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            
            ParticipantAssessment pa = ParticipantAssessment.Create(assessment.Id, request.ParticipantId, assessmentJson: json, _currentUserService.TenantId!);
            foreach (var pathway in assessment.Pathways)
            {
                pa.SetPathwayScore(pathway.Title, -1);
            }

            _unitOfWork.DbContext.ParticipantAssessments.Add(pa);
            return await Result<Guid>.SuccessAsync(assessment.Id);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.ParticipantId)
                .MinimumLength(9)
                .MaximumLength(9);
        }
    }

}
