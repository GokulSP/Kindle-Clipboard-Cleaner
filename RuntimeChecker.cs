using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;

namespace KindleClipboardCleaner;

internal static class RuntimeChecker
{
    private const int RequiredMajorVersion = 10;
    private const string DownloadUrl = "https://dotnet.microsoft.com/download/dotnet/10.0";

    /// <summary>
    /// Checks if the required .NET runtime is installed.
    /// Returns true if runtime is present, false otherwise.
    /// </summary>
    public static bool CheckRuntime()
    {
        var currentVersion = Environment.Version;

        // Check if we're running on .NET 10 or higher
        if (currentVersion.Major >= RequiredMajorVersion)
        {
            return true;
        }

        // Runtime not found - show error dialog
        ShowRuntimeMissingDialog();
        return false;
    }

    private static void ShowRuntimeMissingDialog()
    {
        var result = MessageBox.Show(
            $"This application requires .NET {RequiredMajorVersion}.0 Desktop Runtime or higher.\n\n" +
            $"Current version: .NET {Environment.Version.Major}.{Environment.Version.Minor}\n" +
            $"Required version: .NET {RequiredMajorVersion}.0 or higher\n\n" +
            "Would you like to download and install the required runtime now?",
            "Missing Runtime - Kindle Clipboard Cleaner",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
        );

        if (result == DialogResult.Yes)
        {
            OpenDownloadPage();
        }
    }

    private static void OpenDownloadPage()
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = DownloadUrl,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not open download page automatically.\n\n" +
                $"Please visit: {DownloadUrl}\n\n" +
                $"Error: {ex.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }
}
