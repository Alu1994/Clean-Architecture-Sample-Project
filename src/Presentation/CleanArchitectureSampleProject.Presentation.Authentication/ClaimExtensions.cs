using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;

namespace CleanArchitectureSampleProject.Presentation.Authentication;

public static class ClaimExtensions
{
    private const string prefix = "claim";

    public static IEnumerable<Claim> ToClaims(this UserResourceView view)
    {
        var resouce = view.ResourceName.ToLowerAsSpan();
        var canread = view.CanRead.ToLowerValue();
        var canwrite = view.CanWrite.ToLowerValue();
        var candelete = view.CanDelete.ToLowerValue();

        var r1 = new Claim($"{resouce}{nameof(canread)}{prefix}", $"{canread}");
        var r2 = new Claim($"{resouce}{nameof(canwrite)}{prefix}", $"{canwrite}");
        var r3 = new Claim($"{resouce}{nameof(candelete)}{prefix}", $"{candelete}");

        yield return r1;
        yield return r2;
        yield return r3;
    }

    private static string ToLowerValue(this bool value)
    {
        return value switch
        {
            true => "true",
            false => "false"
        };
    }

    private static string ToLowerAsSpan(this string readOnlySpan)
    {
        // here I am using 'stackalloc' because I know that the content will not be big
        Span<char> span = stackalloc char[readOnlySpan.Length];
        readOnlySpan.AsSpan().CopyTo(span);

        // Process the span in-place
        for (int i = 0, len = span.Length; i < len; i++)
        {
            char c = span[i];
            // Only transform if the character is uppercase for better performance
            if (c >= 'A' && c <= 'Z')
                span[i] = (char)(c + 32); // ASCII adjustment for lowercase
        }

        return new string(span);
    }
}