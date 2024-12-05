namespace CleanArchitectureSampleProject.CrossCuttingConcerns;

public static class StringExtensions
{    
    private const string True = "true";
    private const string False = "false";

    public static string ToLowerValue(this bool value)
    {
        return value switch
        {
            true => True,
            false => False
        };
    }

    public static string ToLowerAsSpan(this string readOnlySpan)
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