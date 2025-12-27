# Kindle Clipboard Cleaner

A native Windows application that automatically removes Kindle citation notes from your clipboard when copying text from the Kindle app.

## Technology Stack

- **Language**: C# 12
- **Framework**: .NET 10.0 Windows Forms (Preview)
- **UI**: System Tray (NotifyIcon)
- **Clipboard**: Windows.Forms.Clipboard
- **Startup**: Windows Registry
- **Regex**: System.Text.RegularExpressions
- **Icon**: Multi-resolution ICO from [IconsDB.com](https://www.iconsdb.com/green-icons/book-icon.html)

## Project Structure

```
Kindle Copy Fix/
├── Program.cs                        # Entry point (~1 KB)
├── ClipboardCleanerTrayApp.cs       # Main application logic (~8 KB)
├── NativeMethods.cs                 # Windows API imports (~259 B)
├── KindleClipboardCleaner.csproj    # Project configuration (~819 B)
├── icon.ico                         # Application icon (265 KB)
├── dist/                            # Built executable
│   └── KindleClipboardCleaner.exe   # Ready to run (~124 MB)
├── README.md                        # User documentation
├── BUILD.md                         # Build instructions
└── PROJECT_SUMMARY.md               # This file
```

**Total source code:** ~10 KB across 3 C# files
**Clean and minimal** - No build scripts, just `dotnet publish`!

## Quick Start

### Install .NET 10 SDK

```bash
winget install Microsoft.DotNet.SDK.Preview
```

Or download from: https://dotnet.microsoft.com/download/dotnet/10.0

### Build

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishDir=dist
```

### Run

```bash
.\dist\KindleClipboardCleaner.exe
```

The app will start in your system tray with a green book icon 📗.

## Features

✅ **System Tray** - Runs silently in background
✅ **Professional Icon** - Green book icon (multi-resolution)
✅ **Auto-Startup Toggle** - Right-click tray icon → "Run at Startup"
✅ **Stats Tracking** - See how many citations cleaned
✅ **Low Memory Usage** - Only ~10-20 MB RAM
✅ **No Scripts Needed** - Everything built-in
✅ **Native Windows** - Fast C# application

## How It Works

The application monitors your clipboard continuously in the background and uses regex patterns to detect and remove Kindle citation lines automatically.

**Supported citation formats:**
- `Author. Title (p. 123). Publisher. Kindle Edition.`
- Citations without page numbers
- Citations with or without blank lines before them
- Trailing whitespace after "Kindle Edition."

**Example:**

**Before (copied from Kindle):**
```
Here's some interesting text from a book.

Author, Name. Book Title (p. 42). Publisher. Kindle Edition.
```

**After (automatically cleaned):**
```
Here's some interesting text from a book.
```

## Usage

1. **Start the app** - Double-click the exe or run from command line
2. **Copy from Kindle** - Citations are automatically removed
3. **Enable auto-start** - Right-click tray icon → "Run at Startup"
4. **View stats** - Right-click tray icon to see count
5. **Exit** - Right-click tray icon → "Exit"

## Debug Mode

For troubleshooting:
```bash
.\dist\KindleClipboardCleaner.exe --console
.\dist\KindleClipboardCleaner.exe --debug
```

This shows a console window with debug output.

## Architecture

### Program.cs
- Application entry point
- Command-line argument parsing
- Launches tray application

### ClipboardCleanerTrayApp.cs
- Main application logic
- Clipboard monitoring loop
- Citation removal regex patterns
- System tray icon management
- Startup registry management
- Statistics tracking

### NativeMethods.cs
- P/Invoke declarations
- Windows API calls for console allocation

## Performance

- **Memory Usage**: ~10-20 MB RAM
- **CPU Usage**: Minimal (monitors every 500ms)
- **Startup Time**: Instant
- **Executable Size**: ~124 MB (self-contained with .NET 10 runtime)

## Build Configuration

The project uses:
- **Self-contained deployment** - Includes .NET runtime
- **Single-file publish** - Everything in one .exe
- **ReadyToRun compilation** - Optimized native code
- **Release optimizations** - Maximum performance
- **No trimming** - Windows Forms doesn't support it

## License

Free to use and modify.

## Icon Attribution

Application icon from [IconsDB.com](https://www.iconsdb.com/green-icons/book-icon.html) - free for commercial and personal use.
