# Building Kindle Clipboard Cleaner

## Prerequisites

Install .NET 10 SDK (latest preview):

**Via winget:**

```bash
winget install Microsoft.DotNet.SDK.Preview
```

**Via browser:** [.NET 10 SDK Download](https://dotnet.microsoft.com/download/dotnet/10.0)

## Building

### Development Build (Fast, Cached)

For quick iteration during development:

```bash
dotnet build
```

**Characteristics:**

- Uses incremental compilation
- Uses compiler cache
- Typically 1-2 seconds for small changes
- **Output:** `bin\Debug\net10.0-windows\KindleClipboardCleaner.exe`
- Requires .NET 10 Runtime installed

### Production Build (Recommended - Tiny!)

Framework-dependent build with automatic runtime check:

```bash
dotnet publish -c Release
```

**Output:** `dist\KindleClipboardCleaner.exe` (535 KB / 0.52 MB)

**Advantages:**

- **99.6% smaller** than bundled runtime (0.52 MB vs 122 MB)
- Single file deployment
- Automatically checks for .NET 10 runtime on startup
- Prompts user to download runtime if missing with direct download link
- Full optimization enabled
- Much faster to distribute and download

**Requirements:**

- Target machine needs .NET 10 Desktop Runtime
- Application handles runtime check automatically
- One-time runtime install (~50 MB) works for all .NET apps

### Alternative: Self-Contained Build

If you prefer bundling the runtime (no installation required):

```bash
dotnet publish -c Release --self-contained true
```

**Output:** `dist\KindleClipboardCleaner.exe` (~50 MB)

**Trade-offs:**

- **Pros:** No runtime installation required
- **Cons:** Much larger file (50 MB vs 2-5 MB)
- **Note:** Windows Forms cannot use IL trimming, so 50 MB is the minimum for self-contained

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
