﻿using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetParticipantNotes
{
    [RequestAuthorize(Policy = PolicyNames.AuthorizedUser)]
    public class Query : IRequest<Result<ParticipantNoteDto[]>>
    {
        public required string ParticipantId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<ParticipantNoteDto[]>>
    {
        public async Task<Result<ParticipantNoteDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = unitOfWork.DbContext.Participants
                .Where(p => p.Id == request.ParticipantId)
                .SelectMany(p => p.Notes);

            var notes = await query.ProjectTo<ParticipantNoteDto>(mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken) ?? [];

            return await Result<ParticipantNoteDto[]>.SuccessAsync(notes);
        }
    }

}
