using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class AddConsent
{
    [RequestAuthorize(Policy = PolicyNames.AllowEnrol)]
    public class Command : IRequest<Result<string>>
    {
        [Description("Participant Id")]
        public required string ParticipantId { get; set; }
        
        [Description("Consent Date")]
        public DateTime? ConsentDate { get; set; }
        
        public UploadRequest? UploadRequest { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IUploadService uploadService) : IRequestHandler<Command, Result<string>>
    {
        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            // get the participant
            var participant = await unitOfWork.DbContext.Participants.FindAsync(request.ParticipantId!, cancellationToken);

            if (participant == null)
            {
                throw new NotFoundException("Cannot find participant", request.ParticipantId);
            }

            var document = Document.Create(request.UploadRequest!.FileName,
                $"Consent form for {request.ParticipantId}",
                DocumentType.PDF);
            
            var result = await uploadService.UploadAsync($"{request.ParticipantId}/consent", request.UploadRequest!);

            document.SetURL(result);

            participant.AddConsent(request.ConsentDate!.Value, document.Id);
            
            unitOfWork.DbContext.Documents.Add(document);
            return result;
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            
            RuleFor(c => c.ParticipantId)
                .NotNull()
                .MinimumLength(9)
                .MaximumLength(9)
                .WithMessage("Invalid Participant Id")
                .MustAsync(MustExist)
                .WithMessage("Participant does not exist");
            
            RuleFor(v => v.UploadRequest)
                .NotNull();
            
        }
        
        private async Task<bool> MustExist(string identifier, CancellationToken cancellationToken) 
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == identifier, cancellationToken);
        
    }
}
