using MediatR;

namespace FifaTracker.Application.Sessions.Commands.GenerateMoreMatches;

public class GenerateMoreMatchesCommand : IRequest<int>
{
    public Guid SessionId { get; set; }
    public int TargetCount { get; set; } = 5;
}
