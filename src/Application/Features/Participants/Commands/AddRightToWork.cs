using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class AddRightToWork
{
    [RequestAuthorize(Policy = PolicyNames.AllowEnrol)]
    public class Command : IRequest<Result<string>>
    {
        [Description("Participant Id")]
        public required string ParticipantId { get; set; }
        
        [Description("Valid From")]
        public DateTime? ValidFrom { get; set; }
        
        [Description("Valid To")]
        public DateTime? ValidTo { get; set; }
        
        public UploadRequest? UploadRequest { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IUploadService uploadService)
        : IRequestHandler<Command, Result<string>>
    {

        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            var participant = await unitOfWork.DbContext.Participants.FindAsync(request.ParticipantId);
            
            if(participant == null)
            {
                throw new NotFoundException("Cannot find participant", request.ParticipantId);
            }

            var document = Document.Create(request.UploadRequest!.FileName,
                $"Right to work evidence for {request.ParticipantId}",
                DocumentType.PDF);
            
            
            var result = await uploadService.UploadAsync($"{request.ParticipantId}/rtw", request.UploadRequest!);
            document.SetURL(result);

            participant.AddRightToWork(request.ValidFrom!.Value, request.ValidTo!.Value, document.Id);

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
