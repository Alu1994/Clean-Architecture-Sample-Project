using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;

namespace CleanArchitectureSampleProject.Presentation.Authentication.Setups;

public static class ClaimExtensions
{
    private const string Prefix = "claim";

    public static IEnumerable<Claim> ToClaims(this UserResourceView view)
    {
        var resouce = view.ResourceName.ToLowerAsSpan();
        var canread = view.CanRead.ToLowerValue();
        var canwrite = view.CanWrite.ToLowerValue();
        var candelete = view.CanDelete.ToLowerValue();

        var r1 = new Claim($"{resouce}{nameof(canread)}{Prefix}", $"{canread}");
        var r2 = new Claim($"{resouce}{nameof(canwrite)}{Prefix}", $"{canwrite}");
        var r3 = new Claim($"{resouce}{nameof(candelete)}{Prefix}", $"{candelete}");

        yield return r1;
        yield return r2;
        yield return r3;
    }
}