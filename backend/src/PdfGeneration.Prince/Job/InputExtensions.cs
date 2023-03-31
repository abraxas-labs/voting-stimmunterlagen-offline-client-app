namespace PdfGeneration.Prince.Job;

using System;

public static class InputExtensions
{
    public static void EnsureBaseUrl(this Input input, string baseUrl)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));
        if (string.IsNullOrEmpty(baseUrl))
            throw new ArgumentNullException(nameof(baseUrl));

        if (string.IsNullOrEmpty(input.BaseUrl))
        {
            input.BaseUrl = baseUrl;
        }
    }
}
