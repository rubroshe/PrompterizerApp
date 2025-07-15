using Microsoft.Maui.Storage;

namespace PrompterizerApp;

public partial class MainPage : ContentPage
{
    string[] fileNames = new[] { "JamesPrompterizer.txt" };

    public MainPage()
    {
        InitializeComponent();

        // Copy embedded files to AppDataDirectory on first launch
        string[] fileNames = new[] { "JamesPrompterizer.txt" };

        foreach (var file in fileNames)
        {
            var source = Path.Combine(AppContext.BaseDirectory, "Files", file);
            var dest = Path.Combine(FileSystem.AppDataDirectory, file);

            if (!File.Exists(dest))
            {
                File.Copy(source, dest);
            }
        }

        filePicker.ItemsSource = fileNames;
    }

    private async void OnDownloadClicked(object sender, EventArgs e)
    {
        string? selectedFile = filePicker.SelectedItem as string;

        if (string.IsNullOrEmpty(selectedFile))
        {
            await DisplayAlert("Error", "Please select a file.", "OK");
            return;
        }

        string sourcePath = Path.Combine(FileSystem.AppDataDirectory, selectedFile);



        if (!File.Exists(sourcePath))
        {
            await DisplayAlert("Error", "File not found.", "OK");
            return;
        }

        // Save to Desktop
        string destinationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), selectedFile);

        using var input = File.OpenRead(sourcePath);
        using var output = File.Create(destinationPath);
        await input.CopyToAsync(output);

        await DisplayAlert("Success", $"Saved to Desktop:\n{destinationPath}", "OK");
    }
}
