using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;
using FFmpeg.NET;
using FFmpeg.NET.Events;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace video_sync.UserControls
{
    public partial class FolderTable : UserControl
    {
        private readonly Engine ffmpegEngine;
        private string temporaryImagesDirectory;

        public FolderTable()
        {
            InitializeComponent();
            temporaryImagesDirectory = Path.Combine(Path.GetTempPath(), "video_sync_temp_images");
            Directory.CreateDirectory(temporaryImagesDirectory);
            ffmpegEngine = new Engine(@"../../../ffmpeg/bin/ffmpeg.exe"); // Initialize FFmpeg engine
            Application.Current.Exit += OnApplicationExit;
        }

        async void OnApplicationExit(object sender, ExitEventArgs e)
        { 
            if (Directory.Exists(temporaryImagesDirectory))
            {
                Directory.Delete(temporaryImagesDirectory, true); // Delete the temporary directory and its contents
            }
        }

        public void AddChildAdapter()
        {
            AddChild();
        }

        public void AddChild(TableElement tableElement)
        {
            tableElement.Nr = ListOfChildren.Items.Count + 1;
            ListOfChildren.Items.Add(tableElement);
        }

        public void RemoveChild(TableElement tableElement)
        {
            ListOfChildren.Items.Remove(tableElement);
        }

        async void AddChild()
        {
            string directoryPath = @"../../../Videos"; // path to videos
            string[] videoFiles = Directory.GetFiles(directoryPath, "*.mp4"); // allowed extensions
            foreach (string videoFile in videoFiles)
            {
                string previewImagePath = await ExtractPreviewImageAsync(videoFile);
                string duration = await GetVideoDurationAsync(videoFile);
                TableElement tableElement = new TableElement(videoFile, ListOfChildren.Items.Count + 1, Path.GetFileNameWithoutExtension(videoFile), duration, previewImagePath, true);
                ListOfChildren.Items.Add(tableElement);
            }
        }

        async Task<string> ExtractPreviewImageAsync(string videoFile)
        {
            var inputFile = new FFmpeg.NET.InputFile(videoFile);
            var outputFile = new FFmpeg.NET.OutputFile(GetTemporaryImagePath(videoFile));

            var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(1) };

            var cancellationToken = System.Threading.CancellationToken.None; // Create a CancellationToken

            ffmpegEngine.Progress += (sender, e) => Console.WriteLine($"Progress: {e.ProcessedDuration}%");

            await ffmpegEngine.GetThumbnailAsync(inputFile, outputFile, options, cancellationToken);
            return outputFile.FileInfo.FullName;
        }

        async Task<string> GetVideoDurationAsync(string videoFile)
        {
            string duration = string.Empty;

            var processStartInfo = new ProcessStartInfo
            {
                FileName = @"../../../ffmpeg/bin/ffmpeg.exe", // Specify the path to the FFmpeg binary
                Arguments = $"-i \"{videoFile}\" -hide_banner",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processStartInfo))
            {
                process.WaitForExit();

                string output = process.StandardError.ReadToEnd();
                string durationLine = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                           .FirstOrDefault(line => line.Contains("Duration"));

                if (!string.IsNullOrEmpty(durationLine))
                {
                    string[] parts = durationLine.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    duration = parts.FirstOrDefault(part => part.Contains("Duration")).Trim().Substring(10);

                    int millisecondsIndex = duration.IndexOf('.');
                    if (millisecondsIndex != -1)
                    {
                        duration = duration.Substring(0, millisecondsIndex);
                    }
                }
            }

            return duration;
        }

        string GetTemporaryImagePath(string videoFile)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), "video_sync_temp_images");
            Directory.CreateDirectory(tempPath); // Ensure the temporary folder exists
            return Path.Combine(tempPath, Path.ChangeExtension(Path.GetFileName(videoFile), ".jpg"));
        }

    }
}
