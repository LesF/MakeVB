using Microsoft.Maui.Controls;

namespace MakeVB
{
    public partial class MainPage : ContentPage
    {
        private string _pathToVBExe;
        private string _pathToBuildDir;
        private string _sourceProjectFile;
        private string _targetBinDirectory;

        public MainPage(string sourceProjectFile, string targetBinDirectory)
        {
            InitializeComponent();

            _sourceProjectFile = sourceProjectFile;
            _targetBinDirectory = targetBinDirectory;
        }

        public MainPage() : this(string.Empty, string.Empty) { }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadSettings();

            // Set the values of the Entry controls here
            PathToVBP.Text = _sourceProjectFile;
            if (String.IsNullOrEmpty(_targetBinDirectory) && !String.IsNullOrEmpty(_sourceProjectFile))
            {
                _targetBinDirectory = FindRelativeBin(Path.GetDirectoryName(_sourceProjectFile));
            }
            TargetBin.Text = _targetBinDirectory;

            /* TODO 
             * - Add a button to initiate the build process
             * - If a valid .vbp was provide in the command line, and the output dir is valid, automatically start the build process
             * - Get details of the expected .dll or .exe file in the target bin directory and display them
             *      * Might need to inspect content of the .vbp file to find the output name or suffix,
             *        or assume it will be the project name followed by .dll or .exe  (check if any of our projects have a different binary name)
             *      * Need to check the modified date to see if the file was updated
             */

        }

        private string FindRelativeBin(string? sourceDirectory)
        {
            /* NOTE: This is specific to HealthViews source repository structure.
             * For an IHM project, example:
             *      d:\Projects\HealthViewsGit\CWS\IHMessenger\ComponentsSource\VB6\Business\IHMStreamTXT\IHMStreamTXT.vbp
             * The relative bin directory is:
             *      d:\Projects\HealthViewsGit\CWS\IHMessenger\Components\Bin\
             */

            if (String.IsNullOrEmpty(sourceDirectory))
                return string.Empty;

            int index = sourceDirectory.LastIndexOf("ComponentsSource");
            if (index == -1)
                return string.Empty;

            string baseDir = sourceDirectory.Substring(0, index);
            string binDir = Path.Combine(baseDir, "Components", "Bin");
            if (Directory.Exists(binDir))
                return binDir;
            else
                return String.Empty;
        }

        private async void OnSettingsIconTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }

        private void LoadSettings()
        {
            _pathToVBExe = Preferences.Get("PathToVBExe", string.Empty);
            _pathToBuildDir = Preferences.Get("PathToBuildDir", string.Empty);
        }
    }

}
