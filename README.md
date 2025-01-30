# MakeVB

MakeVB is a .NET MAUI application designed to automate the build process for Visual Basic 6 (VB6) projects. This app allows users to specify the path to a VB6 project file (.vbp) and a target binary directory, then initiates the build process using the VB6 compiler. The application was created as an exercise in learning and exploring the capabilities of .NET MAUI.

## Features

- Specify the path to a VB6 project file (.vbp).
- Specify the target binary directory for the build output.
- Automatically find the relative binary directory based on the project file path.
- Validate the existence of the specified project file and target directory.
- Initiate the build process and display the output of the build process.
- Check if the expected output file (.dll or .exe) exists and is newer than the project file.

## Getting Started

### Prerequisites

- .NET 8 SDK
- Visual Studio 2022 with .NET MAUI workload installed
- VB6 compiler (vb6.exe)

### Usage

1. Build and run the application in Visual Studio 2022.
2. On the main page, specify the path to the VB6 project file (.vbp) and the target binary directory.
3. Click the button to initiate the build process.
4. The application will display the output of the build process and indicate whether the build succeeded or failed.

### Automatic Build on Startup

The application checks if the specified VB6 project file and target binary directory exist on startup. If both are present, the build process is automatically initiated.

## Learning Objectives

This application was created as an exercise in learning .NET MAUI. The key learning objectives include:

- Understanding the basics of .NET MAUI and its project structure.
- Implementing UI elements and handling user input in .NET MAUI.
- Working with file and directory operations in C#.
- Executing external processes and capturing their output.
- Validating user input and providing feedback through the UI.

## Contributing

Contributions are welcome! If you have any suggestions or improvements, please create a pull request or open an issue.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Special thanks to the .NET MAUI team for creating a powerful and flexible framework for building cross-platform applications.

---

