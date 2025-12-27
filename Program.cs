using System;
using System.Windows.Forms;

namespace KindleClipboardCleaner;

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        using (var trayApp = new ClipboardCleanerTrayApp())
        {
            Application.Run();
        }
    }
}
