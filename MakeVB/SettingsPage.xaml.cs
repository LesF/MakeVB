using Microsoft.Maui.Controls;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Microsoft.Maui.ApplicationModel;

namespace MakeVB
{
    public partial class SettingsPage : ContentPage
    {
        private static readonly FilePickerFileType ExeFileType = new FilePickerFileType(
            new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                    { DevicePlatform.WinUI, new[] { ".exe" } },
                    { DevicePlatform.MacCatalyst, new[] { "com.microsoft.windows-executable" } },
                    { DevicePlatform.iOS, new[] { "com.microsoft.windows-executable" } },
                    { DevicePlatform.Android, new[] { "application/vnd.microsoft.portable-executable" } }
            });

        public SettingsPage()
        {
            InitializeComponent();
            LoadSettings();

            // if PathToVBExe.Text does not contain a value, look for the default VB executable
            if (string.IsNullOrEmpty(PathToVBExe.Text))
            {
                // C:\Program Files (x86)\Microsoft Visual Studio\VB98\VB6.exe  is where mine is located
                var vbExe = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft Visual Studio", "VB98", "VB6.exe");
                if (File.Exists(vbExe))
                {
                    PathToVBExe.Text = vbExe;
                }
            }
        }

        private async void OnFindExeClicked(object sender, EventArgs e)
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select VB Executable",
                FileTypes = ExeFileType
            });

            if (result != null)
            {
                PathToVBExe.Text = result.FullPath;
            }
        }

        private async void OnSelectDirClicked(object sender, EventArgs e)
        {
#if WINDOWS
            var picker = new FolderPicker();
            picker.SuggestedStartLocation = PickerLocationId.Desktop;
            picker.FileTypeFilter.Add("*");

            var hwnd = ((MauiWinUIWindow)Application.Current.Windows[0].Handler.PlatformView).WindowHandle;
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var result = await picker.PickSingleFolderAsync();
            if (result != null)
            {
                PathToBuildDir.Text = result.Path;
            }
#else
                // Implement folder picker for other platforms if needed
                await DisplayAlert("Not Supported", "Folder picking is not supported on this platform.", "OK");
#endif
        }

        private async void OnOkClicked(object sender, EventArgs e)
        {
            // Validate that both text fields have values
            if (string.IsNullOrWhiteSpace(PathToVBExe.Text))
            {
                await DisplayAlert("Validation Error", "Please specify the path to the VB executable.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(PathToBuildDir.Text))
            {
                await DisplayAlert("Validation Error", "Please specify the path to the temporary build directory.", "OK");
                return;
            }

            // Validate that the .exe file exists
            if (!File.Exists(PathToVBExe.Text))
            {
                await DisplayAlert("Validation Error", "The specified VB executable file does not exist.", "OK");
                return;
            }

            // Validate that the directory exists
            if (!Directory.Exists(PathToBuildDir.Text))
            {
                await DisplayAlert("Validation Error", "The specified temporary build directory does not exist.", "OK");
                return;
            }

            // Save settings
            Preferences.Set("PathToVBExe", PathToVBExe.Text);
            Preferences.Set("PathToBuildDir", PathToBuildDir.Text);
            await Navigation.PopAsync();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void LoadSettings()
        {
            PathToVBExe.Text = Preferences.Get("PathToVBExe", string.Empty);
            PathToBuildDir.Text = Preferences.Get("PathToBuildDir", string.Empty);
        }
    }
}
