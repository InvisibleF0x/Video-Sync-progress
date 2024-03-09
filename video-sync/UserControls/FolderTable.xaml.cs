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
using System.Collections.Generic;

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

        async void OnApplicationExit(object sender, ExitEventArgs e) // Delete the temporary directory and its contents
        {
            if (Directory.Exists(temporaryImagesDirectory))
            {
                Directory.Delete(temporaryImagesDirectory, true); 
            }
        }

        public void AddChildAdapter() //allows MainWindow To call AddChild
        {
            AddChild();
        }

        ///////////////////////////////////////////////////////////////////////// Generation of tracks from folder
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
        ///////////////////////////////////////////////////////////////////////// Track moving and removing logic
        List<string> uniqueFilePaths = new List<string>(); //List of IDs

        public void AddChild(TableElement tableElement)
        {
            if (tableElement.FilePath != null && !ContainsVideo(tableElement.FilePath))
            {
                TableElement clonedChild = tableElement.Clone();
                clonedChild.Nr = ListOfChildren.Items.Count + 1;
                ListOfChildren.Items.Add(clonedChild);
                uniqueFilePaths.Add(clonedChild.FilePath);
            }
        }

        public void RemoveChild(TableElement tableElement)
        {
            if (ListOfChildren.Items.Contains(tableElement))
            {
                uniqueFilePaths.Remove(tableElement.FilePath);
                ListOfChildren.Items.Remove(tableElement);
                Reenumerate();
            }
        }

        public void Reenumerate()
        {
            int i = 1;
            foreach (TableElement item in ListOfChildren.Items)
            {
                item.Nr = i;
                i++;
            }
        }

        bool ContainsVideo(string videoPath)
        {
            return uniqueFilePaths.Contains(videoPath);
        }
        ///////////////////////////////////////////////////////////////////////// Track Swapping Logic
        public void SwapUp(TableElement selectedItem)
        {
            int selectedIndex = ListOfChildren.Items.IndexOf(selectedItem);
            if (selectedIndex > 0 && selectedIndex < ListOfChildren.Items.Count)
            {
                ListOfChildren.Items.RemoveAt(selectedIndex);
                ListOfChildren.Items.Insert(selectedIndex - 1, selectedItem);
                Reenumerate();
            }
        }

        public void SwapDown(TableElement selectedItem)
        {
            int selectedIndex = ListOfChildren.Items.IndexOf(selectedItem);
            if (selectedIndex >= 0 && selectedIndex < ListOfChildren.Items.Count - 1)
            {
                var itemBelow = (TableElement)ListOfChildren.Items[selectedIndex + 1];
                ListOfChildren.Items.RemoveAt(selectedIndex + 1);
                ListOfChildren.Items.Insert(selectedIndex, itemBelow);
                Reenumerate();
            }
        }

    }
}