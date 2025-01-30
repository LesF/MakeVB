namespace MakeVB
{
    public partial class AppShell : Shell
    {
        public AppShell(string sourceProjectFile, string targetBinDirectory)
        {
            InitializeComponent();

            // Create an instance of MainPage with the command line arguments
            var mainPage = new MainPage(sourceProjectFile, targetBinDirectory);
            // Items.Add(new ShellContent { Content = mainPage });

            // Create a new ShellContent and set its Content to the MainPage instance
            var shellContent = new ShellContent
            {
                Content = mainPage,
                Route = "MainPage"
            };

            // Add the ShellContent to the Shell
            Items.Add(shellContent);
        }

        public AppShell() : this(string.Empty, string.Empty) { }

    }
}
