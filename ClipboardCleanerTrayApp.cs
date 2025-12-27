using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;

namespace KindleClipboardCleaner;

public partial class ClipboardCleanerTrayApp : IDisposable
{
    // Configuration Constants
    private const int ClipboardCheckIntervalMs = 500;
    private const string AppName = "Kindle Clipboard Cleaner";
    private const string RegistryKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    private const string RegistryValueName = "KindleClipboardCleaner";
    private const int IconSize = 64;

    // UI Color Scheme - Based on Dieter Rams principles: functional, minimal, honest
    private static readonly Color PrimaryColor = Color.FromArgb(46, 125, 50);      // Calm green
    private static readonly Color AccentColor = Color.FromArgb(255, 193, 7);       // Subtle gold
    private static readonly Color BackgroundColor = Color.White;
    private static readonly Color TextColor = Color.FromArgb(33, 33, 33);          // High contrast

    private readonly NotifyIcon _trayIcon;
    private readonly System.Windows.Forms.Timer _clipboardTimer;
    private readonly object _clipboardLock = new object();
    private Bitmap? _iconBitmap;
    private IntPtr _iconHandle;
    private string _lastClipboard = string.Empty;
    private int _citationsCleaned = 0;
    private bool _disposed = false;

    // Optimized regex patterns with separate matching for different citation formats
    // Using proven patterns but consolidated into maintainable array
    private static readonly Regex[] KindlePatterns = new[]
    {
        // Double newline patterns (most common)
        new Regex(@"(?:\r?\n){2,}[^\r\n]+\. [^\r\n]+\(p\. \d+\)\. [^\r\n]+\. Kindle Edition\.\s*$",
            RegexOptions.Multiline | RegexOptions.Compiled),
        new Regex(@"(?:\r?\n){2,}[^\r\n]+\. [^\r\n]+\. [^\r\n]+\. Kindle Edition\.\s*$",
            RegexOptions.Multiline | RegexOptions.Compiled),

        // Single newline patterns
        new Regex(@"(?:\r?\n)[^\r\n]+\. [^\r\n]+\(p\. \d+\)\. [^\r\n]+\. Kindle Edition\.\s*$",
            RegexOptions.Multiline | RegexOptions.Compiled),
        new Regex(@"(?:\r?\n)[^\r\n]+\. [^\r\n]+\. [^\r\n]+\. Kindle Edition\.\s*$",
            RegexOptions.Multiline | RegexOptions.Compiled),

        // Inline citation patterns (after sentence ending: ". " + Author name)
        new Regex(@"(?<=\.)\s+[A-Z][a-z]+,\s[^\r\n]+\. [^\r\n]+\(p\. \d+\)\. [^\r\n]+\. Kindle Edition\.\s*$",
            RegexOptions.Multiline | RegexOptions.Compiled),
        new Regex(@"(?<=\.)\s+[A-Z][a-z]+,\s[^\r\n]+\. [^\r\n]+\. [^\r\n]+\. Kindle Edition\.\s*$",
            RegexOptions.Multiline | RegexOptions.Compiled)
    };

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyIcon(IntPtr hIcon);

    public ClipboardCleanerTrayApp()
    {
        // Create system tray icon
        _trayIcon = new NotifyIcon
        {
            Icon = LoadIcon(),
            Visible = true,
            Text = AppName
        };

        // Create context menu with improved design
        _trayIcon.ContextMenuStrip = CreateContextMenu();

        // Start clipboard monitoring timer
        _clipboardTimer = new System.Windows.Forms.Timer
        {
            Interval = ClipboardCheckIntervalMs
        };
        _clipboardTimer.Tick += (sender, e) => MonitorClipboard();
        _clipboardTimer.Start();
    }

    private ContextMenuStrip CreateContextMenu()
    {
        var contextMenu = new ContextMenuStrip
        {
            // Dieter Rams principle: Clean, unobtrusive design
            ShowImageMargin = false,
            RenderMode = ToolStripRenderMode.System
        };

        // Header with application name and statistics
        var systemFont = SystemFonts.MenuFont ?? SystemFonts.DefaultFont;
        var headerItem = new ToolStripMenuItem($"{AppName}\n{GetCleanedCountText()}")
        {
            Enabled = false,
            Font = new Font(systemFont.FontFamily, systemFont.Size, FontStyle.Bold)
        };
        contextMenu.Items.Add(headerItem);
        contextMenu.Items.Add(new ToolStripSeparator());

        // Startup toggle with clear labeling
        var startupItem = new ToolStripMenuItem("Run at Startup", null, OnToggleStartup)
        {
            Checked = IsInStartup(),
            CheckOnClick = false // Manual control for better feedback
        };
        contextMenu.Items.Add(startupItem);
        contextMenu.Items.Add(new ToolStripSeparator());

        // Exit option
        var exitItem = new ToolStripMenuItem("Exit", null, OnExit);
        contextMenu.Items.Add(exitItem);

        return contextMenu;
    }

    private string GetCleanedCountText()
    {
        return _citationsCleaned switch
        {
            0 => "No citations cleaned yet",
            1 => "1 citation cleaned",
            _ => $"{_citationsCleaned} citations cleaned"
        };
    }

    private Icon LoadIcon()
    {
        // Load icon from file or create fallback
        try
        {
            // Try to load from icon.ico file in the same directory as the executable
            var iconPath = Path.Combine(AppContext.BaseDirectory, "icon.ico");
            if (File.Exists(iconPath))
            {
                return new Icon(iconPath);
            }

            // Try to extract from application's embedded icon
            var icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            if (icon != null)
            {
                return icon;
            }
        }
        catch
        {
            // Fall through to create programmatic icon
        }

        // Fallback: Create programmatic icon (Dieter Rams principle: As little design as possible)
        return CreateProgrammaticIcon();
    }

    private Icon CreateProgrammaticIcon()
    {
        _iconBitmap = new Bitmap(IconSize, IconSize);

        using (var g = Graphics.FromImage(_iconBitmap))
        {
            g.Clear(BackgroundColor);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Draw book with improved proportions
            using (var brush = new SolidBrush(PrimaryColor))
            using (var pen = new Pen(TextColor, 2))
            {
                g.FillRectangle(brush, 12, 15, 40, 35);
                g.DrawRectangle(pen, 12, 15, 40, 35);

                // Book spine - vertical line for depth
                using (var whitePen = new Pen(BackgroundColor, 2))
                {
                    g.DrawLine(whitePen, 32, 15, 32, 50);
                }
            }

            // Subtle sparkle effect - indicates "cleaning" action
            using (var brush = new SolidBrush(AccentColor))
            {
                g.FillEllipse(brush, 40, 8, 8, 8);
                g.FillEllipse(brush, 8, 35, 6, 6);
            }
        }

        // Properly manage icon handle to prevent memory leak
        _iconHandle = _iconBitmap.GetHicon();
        return Icon.FromHandle(_iconHandle);
    }

    private void MonitorClipboard()
    {
        // Thread-safe clipboard monitoring with optimized early exit checks
        lock (_clipboardLock)
        {
            try
            {
                if (!Clipboard.ContainsText())
                    return;

                var currentClipboard = Clipboard.GetText();

                // Early exit: null, empty, or unchanged
                if (string.IsNullOrWhiteSpace(currentClipboard) || currentClipboard == _lastClipboard)
                    return;

                // Early exit: no Kindle citation marker
                if (!currentClipboard.Contains("Kindle Edition", StringComparison.OrdinalIgnoreCase))
                {
                    _lastClipboard = currentClipboard;
                    return;
                }

                var cleaned = CleanKindleCitation(currentClipboard);

                if (cleaned != currentClipboard)
                {
                    Clipboard.SetText(cleaned);
                    _citationsCleaned++;
                    UpdateTrayMenu();
                    _lastClipboard = cleaned;
                }
                else
                {
                    _lastClipboard = currentClipboard;
                }
            }
            catch (Exception)
            {
                // Silently handle clipboard access errors (expected in some scenarios)
                // Clipboard can be locked by other processes or unavailable
            }
        }
    }

    private string CleanKindleCitation(string text)
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

    private void UpdateTrayMenu()
    {
        if (_trayIcon.ContextMenuStrip?.InvokeRequired == true)
        {
            _trayIcon.ContextMenuStrip.Invoke(() => UpdateTrayMenu());
            return;
        }

        if (_trayIcon.ContextMenuStrip?.Items[0] is ToolStripMenuItem headerItem)
        {
            headerItem.Text = $"{AppName}\n{GetCleanedCountText()}";
        }
    }

    private bool IsInStartup()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, false);
            return key?.GetValue(RegistryValueName) != null;
        }
        catch
        {
            return false;
        }
    }

    private void OnToggleStartup(object? sender, EventArgs e)
    {
        try
        {
            var exePath = Application.ExecutablePath;
            using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, true);

            if (key == null)
            {
                ShowErrorMessage("Cannot access Windows startup settings.", "Startup Configuration Error");
                return;
            }

            bool wasInStartup = IsInStartup();

            if (wasInStartup)
            {
                key.DeleteValue(RegistryValueName, false);
                ShowInfoMessage("Removed from Windows startup.", "Startup Disabled");
            }
            else
            {
                key.SetValue(RegistryValueName, $"\"{exePath}\"");
                ShowInfoMessage("The app will now start automatically when you log in.", "Startup Enabled");
            }

            // Update menu checkbox
            if (sender is ToolStripMenuItem menuItem)
            {
                menuItem.Checked = IsInStartup();
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"Failed to modify startup settings: {ex.Message}", "Error");
        }
    }

    private void ShowInfoMessage(string message, string title)
    {
        MessageBox.Show(
            message,
            title,
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );
    }

    private void ShowErrorMessage(string message, string title)
    {
        MessageBox.Show(
            message,
            title,
            MessageBoxButtons.OK,
            MessageBoxIcon.Error
        );
    }

    private void OnExit(object? sender, EventArgs e)
    {
        Application.Exit();
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _clipboardTimer?.Stop();
        _clipboardTimer?.Dispose();
        _trayIcon?.Dispose();

        // Properly dispose of icon resources to prevent memory leak
        if (_iconHandle != IntPtr.Zero)
        {
            DestroyIcon(_iconHandle);
            _iconHandle = IntPtr.Zero;
        }

        _iconBitmap?.Dispose();
        _iconBitmap = null;

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
