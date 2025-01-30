using Microsoft.UI.Xaml;
using Microsoft.Maui.Controls;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.Maui.Platform;
using Windows.Graphics;

namespace MakeVB
{
    public partial class App : Microsoft.Maui.Controls.Application
    {
        public string SourceProjectFile { get; private set; }
        public string TargetBinDirectory { get; private set; }

        public App()
        {
            InitializeComponent();
            ProcessCommandLineArgs();

            MainPage = new AppShell(SourceProjectFile ?? String.Empty, TargetBinDirectory ?? String.Empty);
        }

        private void ProcessCommandLineArgs()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                SourceProjectFile = args[1];

                if (args.Length > 2)
                {
                    TargetBinDirectory = args[2];
                }
            }
        }

        protected override Microsoft.Maui.Controls.Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

#if WINDOWS
            window.Created += (s, e) =>
            {
                var mauiContext = window.Handler.MauiContext;
                var nativeWindow = mauiContext.Services.GetService<Microsoft.UI.Xaml.Window>();
                var appWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(nativeWindow.GetWindowHandle()));
                appWindow.Resize(new SizeInt32(840, 600)); // Set the desired width and height
            };
#endif

            return window;
        }

    }
}
