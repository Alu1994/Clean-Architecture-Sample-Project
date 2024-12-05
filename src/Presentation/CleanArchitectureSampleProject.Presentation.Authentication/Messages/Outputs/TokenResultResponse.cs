namespace CleanArchitectureSampleProject.Presentation.Authentication.Messages.Outputs;

internal record TokenResultResponse(string AccessToken, DateTime ValidFrom, DateTime ValidTo, string Issuer, byte ExpiresInMinutes);