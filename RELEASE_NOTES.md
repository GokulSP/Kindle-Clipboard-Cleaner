# Release Notes

## Version 2.0.0

### What's New

- **Fixed STA Thread Issue** - Resolved clipboard access errors by using Windows Forms Timer instead of Threading Timer
- **Enhanced Citation Detection** - Added support for inline citations (citations on same line as text)
- **Comprehensive Test Suite** - 15 unit tests covering all citation formats
- **Simplified Codebase** - Removed debug mode and console output for cleaner production code
- **Professional Documentation** - Updated README, BUILD, and PROJECT_SUMMARY with clear instructions

### Supported Citation Formats

The application now handles all known Kindle citation formats:

1. **Double newline citations** - Citation separated by blank line
2. **Single newline citations** - Citation on new line
3. **Inline citations** - Citation on same line as content text
4. **With/without page numbers** - Both formats supported
5. **Multiple authors** - Handles multiple author names

### Technical Improvements

- Changed from `System.Threading.Timer` to `System.Windows.Forms.Timer` for proper STA thread clipboard access
- Removed unnecessary console mode and debug logging
- Cleaned up P/Invoke code by removing unused NativeMethods
- Added comprehensive .gitignore for cleaner repository
- All patterns use compiled regex for optimal performance

### Testing

All 15 tests pass successfully:
- Pattern matching validation
- Edge case handling
- Real-world citation examples
- User-reported issue verification

### Build Information

- **Build Date:** 2025-12-28
- **Framework:** .NET 10 Preview
- **Size:** 124 MB (self-contained)
- **Platform:** Windows 10+ (x64)

### Files Included

```
KindleClipboardCleaner.exe  - Main application
README.md                   - User documentation
BUILD.md                    - Build instructions
PROJECT_SUMMARY.md          - Technical overview
```

### Known Limitations

- Windows only (requires Windows Forms)
- x64 architecture only
- Requires Windows 10 or later

### Usage

1. Run `KindleClipboardCleaner.exe`
2. Application starts in system tray (green book icon)
3. Copy text from Kindle app
4. Citations are automatically removed from clipboard
5. Right-click tray icon for statistics and startup management

### Contributors

Gokul SP - Original author and maintainer

### License

Free to use and modify.
