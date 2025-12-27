# Kindle Clipboard Cleaner

A lightweight Windows system tray application that automatically removes Kindle citation notes from your clipboard when copying text from the Kindle app.

## Features

- **System Tray Integration** - Runs silently in background with minimal resource usage
- **Automatic Detection** - Monitors clipboard and removes Kindle citations instantly
- **Multiple Citation Formats** - Handles inline citations, single/double newlines, with/without page numbers
- **Usage Statistics** - Track how many citations have been cleaned via tray menu
- **Auto-Startup Support** - Built-in toggle for Windows startup (no manual registry editing)
- **Zero Configuration** - Works out of the box, no settings required
- **Native Performance** - Built with C# and Windows Forms for optimal efficiency

## Installation

### Option 1: Download Release (Recommended)

1. Download `KindleClipboardCleaner.exe` from the latest release
2. Run the executable - it will appear in your system tray
3. Look for the green book icon in your system tray

### Option 2: Build from Source

**Prerequisites:**
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (preview)

Install via winget:
```bash
winget install Microsoft.DotNet.SDK.Preview
```

**Build:**
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishDir=dist
```

**Run:**
```bash
.\dist\KindleClipboardCleaner.exe
```

The application will start in your system tray with a green book icon.

## Usage

### Enable Auto-Startup

1. Right-click the system tray icon (green book)
2. Click "Run at Startup" to enable
3. The app will now start automatically when you log in

### View Statistics

Right-click the tray icon to see how many citations have been cleaned since startup.

### Exit Application

Right-click the tray icon and select "Exit".

## How It Works

The application monitors your Windows clipboard every 500ms. When it detects text containing "Kindle Edition", it applies regex patterns to identify and remove citation metadata.

### Supported Citation Formats

- **With page numbers:** `Author, Name. Book Title (p. 42). Publisher. Kindle Edition.`
- **Without page numbers:** `Author, Name. Book Title. Publisher. Kindle Edition.`
- **Inline citations:** Citations on the same line as content text
- **Newline separated:** Citations with single or double newlines before them
- **Multiple authors:** `Smith, John; Doe, Jane. Title. Publisher. Kindle Edition.`

### Example

**Before (copied from Kindle):**
```
It's surprisingly accurate. It works on any type of file, in any language.

Boswell, Dustin; Foucher, Trevor. The Art of Readable Code (p. 42). O'Reilly Media. Kindle Edition.
```

**After (automatic cleanup):**
```
It's surprisingly accurate. It works on any type of file, in any language.
```

## Development

### Project Structure

```
KindleClipboardCleaner/
├── Program.cs                    # Application entry point
├── ClipboardCleanerTrayApp.cs    # Main application logic and UI
├── KindleClipboardCleaner.csproj # Project configuration
├── icon.ico                      # Application icon
└── KindleClipboardCleaner.Tests/ # xUnit test suite
    ├── CitationCleanerTests.cs   # Pattern validation tests
    └── KindleClipboardCleaner.Tests.csproj
```

### Running Tests

```bash
cd KindleClipboardCleaner.Tests
dotnet test
```

The test suite includes 14 real-world citation examples covering all supported formats.

### Technical Details

- **Framework:** .NET 10 (Windows-specific)
- **UI Framework:** Windows Forms
- **Clipboard Monitoring:** Windows Forms Timer (500ms interval)
- **Pattern Matching:** 6 compiled regex patterns for optimal performance
- **Thread Model:** STA (Single-Threaded Apartment) for clipboard access

See [BUILD.md](BUILD.md) for detailed build instructions and [PROJECT_SUMMARY.md](PROJECT_SUMMARY.md) for architecture details.

## Troubleshooting

### Application doesn't start
- Ensure you're running Windows 10 or later
- Check that the executable isn't blocked (Right-click > Properties > Unblock)

### Citations not being removed
- Verify the text contains "Kindle Edition"
- Check the citation format matches one of the supported patterns
- Try copying the text again

### Remove from startup
Right-click the tray icon and uncheck "Run at Startup", or manually remove the registry entry at:
```
HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run\KindleClipboardCleaner
```

## Credits

### Icon Attribution
The application icon is derived from the [KDE Oxygen Icons](https://develop.kde.org/frameworks/oxygen-icons/) project.

**License:** GNU Lesser General Public License (LGPL)
**Original Authors:** KDE Visual Design Group
**Source:** https://develop.kde.org/frameworks/oxygen-icons/

The Oxygen Icon Theme is licensed under the GNU Lesser General Public License, which allows redistribution and modification. The icon has been adapted for use in this application while maintaining compliance with the LGPL terms.

## License

This application is free to use and modify. The application icon is subject to the LGPL license as noted in the Credits section above.
