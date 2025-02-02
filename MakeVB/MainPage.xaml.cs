using System.Diagnostics;
using System.Text;
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

            // Check if PathToVBP contains the name of an existing .vbp file
            // and TargetBin contains the name of an existing directory
            if (File.Exists(PathToVBP.Text) && Directory.Exists(TargetBin.Text))
            {
                // Initiate the build process
                _ = BuildProject(PathToVBP.Text, TargetBin.Text);
            }
        }

        private string FindRelativeBin(string? sourceDirectory)
        {
            /* NOTE: This is specific to HealthViews source repository structure.
             * For an IHM project, example:
             *      d:\...whatever...\IHMessenger\ComponentsSource\VB6\Business\IHMStreamTXT\IHMStreamTXT.vbp
             * The relative bin directory is:
             *      d:\...whatever...\IHMessenger\Components\Bin\
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

        private async void OnMakeVBProject(object sender, EventArgs e)
        {
            // Validate that both text fields have values
            if (string.IsNullOrWhiteSpace(PathToVBP.Text))
            {
                await DisplayAlert("Validation Error", "Please specify the path to the .vbp file.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(TargetBin.Text))
            {
                string tryBin = FindRelativeBin(Path.GetDirectoryName(PathToVBP.Text));
                if (String.IsNullOrEmpty(tryBin))
                {
                    await DisplayAlert("Validation Error", "Please specify the path to the target bin directory.", "OK");
                    return;
                }
                TargetBin.Text = tryBin;
            }
            // Validate that the .vbp file exists
            if (!File.Exists(PathToVBP.Text))
            {
                await DisplayAlert("Validation Error", "The specified .vbp file does not exist.", "OK");
                return;
            }
            // Validate that the directory exists
            if (!Directory.Exists(TargetBin.Text))
            {
                await DisplayAlert("Validation Error", "The specified target bin directory does not exist.", "OK");
                return;
            }

            // Save last-used arguments??  Maybe define a log file and store activity there.  TODO add log file to Settings page
            //  Preferences.Set("PathToVBP", PathToVBP.Text);
            //  Preferences.Set("TargetBin", TargetBin.Text);

            // Start the build process
            await BuildProject(PathToVBP.Text, TargetBin.Text);
        }

        private async Task BuildProject(string pathToVBP, string targetBin)
        {
            /* Notes on VB6.exe command line options:
             * (need to verify these are correct)
             *      /make: Compiles the specified project and creates an executable file.
             *      /out: Redirects output messages to a specified file.
             *      /d: Defines conditional compilation constants.
             *      /cmd: Passes command line arguments to the application being compiled.
             *      /m: Minimizes the Visual Basic window during the build process.
             *      /r: Specifies a resource file to include in the project.
             * For example, to compile a project and redirect output messages to a file, you might use:
             *      vb6.exe /make MyProject.vbp /out BuildLog.txt
             */

            string makeButtonText = MakeButton.Text;
            MakeButton.IsEnabled = false;
            MakeButton.Text = "Building...";

            StringBuilder outputText = new StringBuilder();
            FileInfo vbpFile = new FileInfo(pathToVBP);
            DateTime vbpModified = vbpFile.LastWriteTime;

            string[] outputNames = {
                Path.Combine(targetBin, Path.GetFileNameWithoutExtension(pathToVBP) + ".dll"),
                Path.Combine(targetBin, Path.GetFileNameWithoutExtension(pathToVBP) + ".exe")
            };

            string makeCommand = $"\"{_pathToVBExe}\" /make \"{pathToVBP}\"";

            // Display the output of the build process
            OutputLabel.Text = makeCommand + Environment.NewLine;

            // Start the process to execute the makeCommand
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"{makeCommand}\"",
                WorkingDirectory = targetBin,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process
            {
                StartInfo = processStartInfo
            };

            // VB6.exe MAKE command does not seem to output anything useful, there is no distinct success or failure message.
            // Will catch whatever it puts out and display it anyway.
            process.OutputDataReceived += (sender, args) => outputText.AppendLine(args.Data);
            process.ErrorDataReceived += (sender, args) => outputText.AppendLine(args.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();

            OutputLabel.Text += outputText.ToString() + Environment.NewLine;

            // Check if the expected output file exists was modified recently
            DateTime recentTime = DateTime.Now.AddSeconds(-20);
            string outputName = string.Empty;
            for (int fname = 0; fname < outputNames.Length; fname++)
            {
                if (File.Exists(outputNames[fname]))
                {
                    outputName = outputNames[fname];
                    if (File.GetLastWriteTime(outputName) > recentTime)
                    {
                        OutputLabel.Text += $"Success!!  Output file was updated: {outputName}" + Environment.NewLine;
                        break;
                    }
                    else
                    {
                        OutputLabel.Text += $"Output file was not updated: {outputName}" + Environment.NewLine;
                        break;
                    }
                }
            }
            if (string.IsNullOrEmpty(outputName))
            {
                OutputLabel.Text += "Build failed or no output file was generated." + Environment.NewLine;
            }

            MakeButton.Text = makeButtonText;
            MakeButton.IsEnabled = true;

            await Task.CompletedTask;
        }
    }
}
