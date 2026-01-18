using System.Text.RegularExpressions;
using Xunit;

namespace KindleClipboardCleaner.Tests;

public class CitationCleanerTests
{
    // Optimized regex patterns with separate matching for different citation formats
    // Using proven patterns but consolidated into maintainable array
    // Note: Custom uploaded books may not have publisher info
    private static readonly Regex[] KindlePatterns = new[]
    {
        // Double newline patterns (most common)
        // With location and optional publisher
        new Regex(@"(?:\r?\n){2,}[^\r\n]+\. [^\r\n]+\((?:pp?\.|Kindle Locations?) \d+(?:-\d+)?\)\. (?:[^\r\n]+\. )?Kindle Edition\.\s*$",
            RegexOptions.Multiline | RegexOptions.Compiled),
        new Regex(@"(?:\r?\n){2,}[^\r\n]+\. [^\r\n]+\. [^\r\n]*\. Kindle Edition\.\s*$",
            RegexOptions.Multiline | RegexOptions.Compiled),

        // Single newline patterns
        new Regex(@"(?:\r?\n)[^\r\n]+\. [^\r\n]+\((?:pp?\.|Kindle Locations?) \d+(?:-\d+)?\)\. (?:[^\r\n]+\. )?Kindle Edition\.\s*$",
            RegexOptions.Multiline | RegexOptions.Compiled),
        new Regex(@"(?:\r?\n)[^\r\n]+\. [^\r\n]+\. [^\r\n]*\. Kindle Edition\.\s*$",
            RegexOptions.Multiline | RegexOptions.Compiled),

        // Inline citation patterns (after sentence ending: ". " + Author name)
        new Regex(@"(?<=\.)\s+[A-Z][a-z]+,\s[^\r\n]+\. [^\r\n]+\((?:pp?\.|Kindle Locations?) \d+(?:-\d+)?\)\. (?:[^\r\n]+\. )?Kindle Edition\.\s*$",
            RegexOptions.Multiline | RegexOptions.Compiled),
        new Regex(@"(?<=\.)\s+[A-Z][a-z]+,\s[^\r\n]+\. [^\r\n]+\. [^\r\n]*\. Kindle Edition\.\s*$",
            RegexOptions.Multiline | RegexOptions.Compiled)
    };

    private string? CleanKindleCitation(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        // Try each pattern in order (from most specific to least specific)
        foreach (var pattern in KindlePatterns)
        {
            var cleaned = pattern.Replace(text, string.Empty).TrimEnd();
            if (cleaned != text)
                return cleaned;
        }

        return text;
    }

    [Fact]
    public void CleanKindleCitation_WithPageNumber_RemovesCitation()
    {
        var input = "Here's some interesting text from a book.\r\n\r\nAuthor, Name. Book Title (p. 42). Publisher. Kindle Edition.";
        var expected = "Here's some interesting text from a book.";

        var result = CleanKindleCitation(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CleanKindleCitation_WithoutPageNumber_RemovesCitation()
    {
        var input = "Some text here.\r\n\r\nAuthor Name. Book Title. Publisher. Kindle Edition.";
        var expected = "Some text here.";

        var result = CleanKindleCitation(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CleanKindleCitation_SingleNewline_RemovesCitation()
    {
        var input = "Text from book.\r\nAuthor, Name. Book Title (p. 123). Publisher. Kindle Edition.";
        var expected = "Text from book.";

        var result = CleanKindleCitation(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CleanKindleCitation_RealWorldExample_RemovesCitation()
    {
        var input = "If the completed word is not correct, keep triggering the command until the correct name appears. It's surprisingly accurate. It works on any type of file, in any language. And it works for any token, even if you're typing a comment.\r\n\r\nBoswell, Dustin; Foucher, Trevor. The Art of Readable Code: Simple and Practical Techniques for Writing Better Code (p. 42). O'Reilly Media. Kindle Edition.";
        var expected = "If the completed word is not correct, keep triggering the command until the correct name appears. It's surprisingly accurate. It works on any type of file, in any language. And it works for any token, even if you're typing a comment.";

        var result = CleanKindleCitation(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CleanKindleCitation_InlineCitation_RemovesCitation()
    {
        // NEW TEST - Citation on same line after text
        var input = "Trigger the word-completion command (see below). If the completed word is not correct, keep triggering the command until the correct name appears. It's surprisingly accurate. It works on any type of file, in any language. And it works for any token, even if you're typing a comment. Boswell, Dustin; Foucher, Trevor. The Art of Readable Code: Simple and Practical Techniques for Writing Better Code (p. 42). O'Reilly Media. Kindle Edition.";
        var expected = "Trigger the word-completion command (see below). If the completed word is not correct, keep triggering the command until the correct name appears. It's surprisingly accurate. It works on any type of file, in any language. And it works for any token, even if you're typing a comment.";

        var result = CleanKindleCitation(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CleanKindleCitation_WithTrailingSpaces_RemovesCitation()
    {
        var input = "Some text.\r\n\r\nAuthor. Title (p. 99). Publisher. Kindle Edition.   ";
        var expected = "Some text.";

        var result = CleanKindleCitation(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CleanKindleCitation_NoKindleEdition_ReturnsOriginal()
    {
        var input = "Just some regular text without any citation.";
        var expected = "Just some regular text without any citation.";

        var result = CleanKindleCitation(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CleanKindleCitation_EmptyString_ReturnsEmpty()
    {
        var input = "";
        var expected = "";

        var result = CleanKindleCitation(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CleanKindleCitation_NullString_ReturnsNull()
    {
        string? input = null;

        var result = CleanKindleCitation(input);

        Assert.Null(result);
    }

    [Fact]
    public void CleanKindleCitation_UnixNewlines_RemovesCitation()
    {
        var input = "Text from book.\n\nAuthor. Title (p. 50). Publisher. Kindle Edition.";
        var expected = "Text from book.";

        var result = CleanKindleCitation(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CleanKindleCitation_MultipleAuthors_RemovesCitation()
    {
        var input = "Great content here.\r\n\r\nSmith, John; Doe, Jane; Johnson, Bob. The Complete Guide (p. 200). Big Publisher. Kindle Edition.";
        var expected = "Great content here.";

        var result = CleanKindleCitation(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CleanKindleCitation_UserExactExample_WithDoubleNewline_RemovesCitation()
    {
        // User's exact failing example with double newline
        var input = "appears. It's surprisingly accurate. It works on any type of file, in any language. And it works for any token, even if you're typing a comment.\r\n\r\nBoswell, Dustin; Foucher, Trevor. The Art of Readable Code: Simple and Practical Techniques for Writing Better Code (p. 42). O'Reilly Media. Kindle Edition.";
        var expected = "appears. It's surprisingly accurate. It works on any type of file, in any language. And it works for any token, even if you're typing a comment.";

        var result = CleanKindleCitation(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CleanKindleCitation_UserExactExample_WithSingleNewline_RemovesCitation()
    {
        // User's exact failing example with single newline
        var input = "appears. It's surprisingly accurate. It works on any type of file, in any language. And it works for any token, even if you're typing a comment.\r\nBoswell, Dustin; Foucher, Trevor. The Art of Readable Code: Simple and Practical Techniques for Writing Better Code (p. 42). O'Reilly Media. Kindle Edition.";
        var expected = "appears. It's surprisingly accurate. It works on any type of file, in any language. And it works for any token, even if you're typing a comment.";

        var result = CleanKindleCitation(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CleanKindleCitation_UserExactExample_NoNewline_RemovesCitation()
    {
        // User's exact failing example with no newline (inline)
        var input = "appears. It's surprisingly accurate. It works on any type of file, in any language. And it works for any token, even if you're typing a comment. Boswell, Dustin; Foucher, Trevor. The Art of Readable Code: Simple and Practical Techniques for Writing Better Code (p. 42). O'Reilly Media. Kindle Edition.";
        var expected = "appears. It's surprisingly accurate. It works on any type of file, in any language. And it works for any token, even if you're typing a comment.";

        var result = CleanKindleCitation(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CleanKindleCitation_UserActualFailingText_RemovesCitation()
    {
        // The ACTUAL text user just provided that's failing
        var input = "completed word is not correct, keep triggering the command until the correct name appears. It's surprisingly accurate. It works on any type of file, in any language. And it works for any token, even if you're typing a comment.\r\n\r\nBoswell, Dustin; Foucher, Trevor. The Art of Readable Code: Simple and Practical Techniques for Writing Better Code (p. 42). O'Reilly Media. Kindle Edition.";
        var expected = "completed word is not correct, keep triggering the command until the correct name appears. It's surprisingly accurate. It works on any type of file, in any language. And it works for any token, even if you're typing a comment.";

        var result = CleanKindleCitation(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CleanKindleCitation_PageRange_RemovesCitation()
    {
        // Citations with page ranges (pp. 90-91) instead of single page (p. 42)
        var input = "6.22 Fan Chi asked about wisdom. The Master said, \"Work for what is appropriate and right in human relationships; show respect to the gods and spirits while keeping them at a distance—this can be called wisdom.\"\r\n\r\nConfucius. The Analects (Penguin Classics) (pp. 90-91). Penguin Publishing Group. Kindle Edition.";
        var expected = "6.22 Fan Chi asked about wisdom. The Master said, \"Work for what is appropriate and right in human relationships; show respect to the gods and spirits while keeping them at a distance—this can be called wisdom.\"";

        var result = CleanKindleCitation(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CleanKindleCitation_PageRange_SingleNewline_RemovesCitation()
    {
        // Citations with page ranges (pp.) and single newline - testing reported issue
        var input = "6.22 Fan Chi asked about wisdom. The Master said, \"Work for what is appropriate and right in human relationships; show respect to the gods and spirits while keeping them at a distance - this can be called wisdom.\"\r\nConfucius. The Analects (Penguin Classics) (pp. 90-91). Penguin Publishing Group. Kindle Edition.";
        var expected = "6.22 Fan Chi asked about wisdom. The Master said, \"Work for what is appropriate and right in human relationships; show respect to the gods and spirits while keeping them at a distance - this can be called wisdom.\"";

        var result = CleanKindleCitation(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CleanKindleCitation_SingleWordBeforeCitation_RemovesCitation()
    {
        // Text ending with a single word before citation (like "friends")
        var input = "friends\r\n\r\nMark Michaelis. Essential C# 12.0 (Kindle Location 37). Kindle Edition.";
        var expected = "friends";

        var result = CleanKindleCitation(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CleanKindleCitation_KindleLocationsPlural_RemovesCitation()
    {
        // Citations with "Kindle Locations" (plural) for ranges
        var input = "code base like this running smoothly. Thanks especially for making the EssentialCSharp.com website a reality.\r\n\r\nMark Michaelis. Essential C# 12.0 (Kindle Locations 38-39). Kindle Edition.";
        var expected = "code base like this running smoothly. Thanks especially for making the EssentialCSharp.com website a reality.";

        var result = CleanKindleCitation(input);

        Assert.Equal(expected, result);
    }
}
