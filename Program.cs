using System;
using System.Windows.Forms;

namespace KindleClipboardCleaner;

static class Program
{
    [STAThread]
    static void Main()
    {
        // Check if required .NET runtime is installed
        if (!RuntimeChecker.CheckRuntime())
        {
            return;
        }

        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        using (var trayApp = new ClipboardCleanerTrayApp())
        {
            Application.Run();
        }
    }
}
