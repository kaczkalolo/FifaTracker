namespace FifaTracker.WebApi.Requests;

public record AddUserRequest(Guid UserId, bool GenerateMissingMatches);
