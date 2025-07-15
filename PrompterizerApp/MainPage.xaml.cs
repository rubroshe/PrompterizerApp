using Microsoft.Maui.Storage;
using System.Net.Http.Json;

namespace PrompterizerApp;

public partial class MainPage : ContentPage
{
    string[] fileNames = new[] { "JamesPrompterizer.txt" };

    public MainPage()
    {
        InitializeComponent();
        Loaded += async (s, e) => await LoadFileListFromGitHub();


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

        try
        {
            using var http = new HttpClient();
            var fileUrl = $"https://raw.githubusercontent.com/rubroshe/prompterizer-files/main/{selectedFile}";
            var fileBytes = await http.GetByteArrayAsync(fileUrl);

            string destinationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), selectedFile);
            await File.WriteAllBytesAsync(destinationPath, fileBytes);

            await DisplayAlert("Success", $"Saved to Desktop:\n{destinationPath}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Download Failed", ex.Message, "OK");
        }
    }


    private async Task LoadFileListFromGitHub()
    {
        try
        {
            using var http = new HttpClient();

            var fileNames = await http.GetFromJsonAsync<string[]>(
                "https://raw.githubusercontent.com/rubroshe/prompterizer-files/main/index.json");

            if (fileNames == null || fileNames.Length == 0)
            {
                await DisplayAlert("No Files Found", "The file list is empty or unavailable.", "OK");
                return;
            }

            filePicker.ItemsSource = fileNames;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error Loading Files", ex.Message, "OK");
        }
    }

}
