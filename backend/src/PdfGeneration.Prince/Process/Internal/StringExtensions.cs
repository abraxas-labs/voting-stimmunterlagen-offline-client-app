namespace PdfGeneration.Prince.Process.Internal;

using System;

internal static class StringExtensions
{
    private const char DoubleQuote = '"';   // (char)34
    private const char Backslash = '\\';    // (char)92

    public static string EscapeCommandLineArgument(this string argument)
    {
        if (argument == null)
            throw new ArgumentNullException(nameof(argument));

        return argument.EscapeDoubleQuotes().EscapeEndingBackslashes();
    }

    public static string EscapeDoubleQuotes(this string argument)
    {
        if (argument == null)
            throw new ArgumentNullException(nameof(argument));

        // shortcut
        if (string.IsNullOrWhiteSpace(argument))
            return argument;

        // start at the end
        for (var pos = argument.Length - 1; pos >= 0; pos--)
        {
            if (argument[pos] == DoubleQuote)
            {
                //if there is a double quote in the arg string
                //find number of backslashes preceding the double quote ( " )
                var backslashesBeforeDoublequote = argument.CountConsecutiveBackslashes(pos - 1);

                var rightSubstring = argument.Substring(pos + 1);
                var leftSubstring = argument.Substring(0, (pos - backslashesBeforeDoublequote));

                // double counted backslashes, and add escaped double quote ( \" )
                var middleSubstring = new string(Backslash, (backslashesBeforeDoublequote * 2) + 1) + DoubleQuote;

                return leftSubstring.EscapeDoubleQuotes() + middleSubstring + rightSubstring;
            }
        }

        //no double quote found, return string itself
        return argument;
    }

    public static string EscapeEndingBackslashes(this string argument)
    {
        if (argument == null)
            throw new ArgumentNullException(nameof(argument));

        var backslashesToEscape = argument.CountConsecutiveBackslashes();

        return argument + new string(Backslash, backslashesToEscape);
    }

    public static int CountConsecutiveBackslashes(this string argument, int startIndex = int.MaxValue, int stopIndex = 0)
    {
        if (argument == null)
            throw new ArgumentNullException(nameof(argument));

        if (startIndex == int.MaxValue)
        {
            startIndex = argument.Length - 1;
        }

        var backslashCount = 0;

        for (var pos = startIndex; pos >= stopIndex; pos--)
        {
            if (argument[pos] != Backslash)
                break;

            backslashCount++;
        }

        return backslashCount;
    }
}
