# Building Kindle Clipboard Cleaner

## Prerequisites

Install .NET 10 SDK (latest preview):

**Via winget:**
```bash
winget install Microsoft.DotNet.SDK.Preview
```

**Via browser:** [.NET 10 SDK Download](https://dotnet.microsoft.com/download/dotnet/10.0)

## Building

### Production Build (Recommended)

Creates a self-contained executable with .NET 10 runtime bundled:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishDir=dist
```

**Output:** `dist\KindleClipboardCleaner.exe` (~124 MB)

**Advantages:**
- No .NET runtime required on target machine
- Single file deployment
- Works on any Windows 10+ machine

### Framework-Dependent Build

Smaller executable that requires .NET 10 runtime on target machine:

```bash
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -p:PublishDir=dist
```

**Output:** `dist\KindleClipboardCleaner.exe` (~1-2 MB)

**Use when:**
- Target machine has .NET 10 runtime installed
- Minimizing file size is important

### Development Build

For debugging and development:

```bash
dotnet build -c Debug
```

**Output:** `bin\Debug\net10.0-windows\KindleClipboardCleaner.exe`

## Testing

Run the test suite:

```bash
cd KindleClipboardCleaner.Tests
dotnet test
```

Run specific test:

```bash
dotnet test --filter "FullyQualifiedName~InlineCitation"
```

## Clean Build

Remove all build artifacts:

```bash
dotnet clean
```

## Build Configuration

The project is configured in [KindleClipboardCleaner.csproj](KindleClipboardCleaner.csproj):

- **Target Framework:** net10.0-windows
- **Output Type:** WinExe (Windows GUI application)
- **Platform:** x64 only
- **Single File:** Enabled for Release builds
- **Ready to Run:** Enabled for Release builds (faster startup)

## Deployment

After building, deploy `dist\KindleClipboardCleaner.exe` to the target machine. The executable is fully self-contained and requires no additional files or installation.

Users can:
1. Run the .exe file directly
2. Enable auto-startup via the tray menu
3. The application will create its own registry entries for startup management
