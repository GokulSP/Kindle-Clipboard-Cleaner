# Kindle Clipboard Cleaner Tests

Comprehensive test suite for validating citation removal patterns.

## Running Tests

```bash
dotnet test
```

## Test Coverage

The test suite validates all supported Kindle citation formats:

### Pattern Categories

1. **Double Newline Citations** - Citations separated by blank lines
2. **Single Newline Citations** - Citations on new line without blank line
3. **Inline Citations** - Citations on same line as content text
4. **Page Number Variations** - With and without page numbers
5. **Multiple Authors** - Citations with multiple author names
6. **Edge Cases** - Trailing whitespace, Unix newlines

### Test Cases

- `CleanKindleCitation_WithPageNumber_RemovesCitation`
- `CleanKindleCitation_WithoutPageNumber_RemovesCitation`
- `CleanKindleCitation_SingleNewline_RemovesCitation`
- `CleanKindleCitation_SingleNewlineNoPage_RemovesCitation`
- `CleanKindleCitation_NoMatch_ReturnsOriginal`
- `CleanKindleCitation_EmptyString_ReturnsEmpty`
- `CleanKindleCitation_NullString_ReturnsNull`
- `CleanKindleCitation_TrailingSpaces_RemovesCitation`
- `CleanKindleCitation_InlineCitation_RemovesCitation`
- `CleanKindleCitation_UnixNewlines_RemovesCitation`
- `CleanKindleCitation_MultipleAuthors_RemovesCitation`
- `CleanKindleCitation_UserExactExample_WithDoubleNewline_RemovesCitation`
- `CleanKindleCitation_UserExactExample_WithSingleNewline_RemovesCitation`
- `CleanKindleCitation_UserExactExample_NoNewline_RemovesCitation`
- `CleanKindleCitation_UserActualFailingText_RemovesCitation`

## Framework

- **Test Framework:** xUnit
- **Target Framework:** .NET 10
- **Pattern Matching:** 6 regex patterns with lookbehind assertions

## Adding New Tests

To add a new test case:

1. Create a new `[Fact]` method
2. Set up input text with citation
3. Define expected output (text without citation)
4. Call `CleanKindleCitation(input)`
5. Assert the result matches expected output

Example:

```csharp
[Fact]
public void CleanKindleCitation_YourTestCase_RemovesCitation()
{
    var input = "Your text here.\r\n\rAuthor. Book. Kindle Edition.";
    var expected = "Your text here.";

    var result = CleanKindleCitation(input);

    Assert.Equal(expected, result);
}
```
