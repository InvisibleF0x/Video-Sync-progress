using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using video_sync.UserControls;

namespace video_sync
{
    /// <summary>
    /// Logika interakcji dla klasy PlayerWindow.xaml
    /// </summary>
    public partial class PlayerWindow : Window
    {
        private MainWindow MainWindowInstance;
        private FolderTable folderTableRInstance;           //to allow for the existance and synchornisation of the side menu
        public FolderTable folderTableInPlayer = new FolderTable();
        private bool isPlaying = false;
        private bool isUserInteraction = true;
        private int currentIndex = 0;
        private double volumeCach;
        private DispatcherTimer? timer;
        private DispatcherTimer? mouseTimer;
        public PlayerWindow(MainWindow mainWindowInstance)
        {
            InitializeComponent();
            InitializeTimer();
            MainWindowInstance = mainWindowInstance;
            folderTableRInstance = mainWindowInstance.folderTableR;
            folderTableInPlayer = folderTableRInstance.Clone();
            TableElement firstItem = (TableElement)MainWindowInstance.folderTableR.ListOfChildren.Items[currentIndex];
            Player.Source = new Uri(firstItem.FilePath);
            Closing += OnWindowExit;
            InitializeMouseTimer();
            FillGrid();
        }

        void FillGrid()
        {
            FolderTableContainer.Children.Add(folderTableInPlayer);
            Grid.SetRow(folderTableInPlayer, 1);
            Grid.SetColumn(folderTableInPlayer, 1);
        }

        private void OnWindowExit(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            mouseTimer.Stop();
            Player.Source = null;
        }

        ////////////////////////////////////////////////////////////////////////////////// timer and time slider logic
        private bool IsUserInteraction()
        {
            return isUserInteraction;
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (Player.NaturalDuration.HasTimeSpan)
            {
                isUserInteraction = false;
                TimeSpan currentTime = Player.Position;
                TimeSpan totalTime = Player.NaturalDuration.TimeSpan;
                TimeSlider.Maximum = (int)Player.NaturalDuration.TimeSpan.TotalMilliseconds / 500;
                TimeSlider.Value = currentTime.TotalMilliseconds / 500;
                Timer.Text = $"{currentTime:mm\\:ss}/{totalTime:mm\\:ss}";
                isUserInteraction = true;
            }
        }

        private void TimeSliderDragStart(object sender, DragStartedEventArgs e)
        {
            Player.Pause();
            isPlaying = false;
            isUserInteraction = true;
        }

        private void TimeSliderChangeValue(object sender, EventArgs e)
        {
            if (IsUserInteraction())
            {
                Player.Pause();
                double newPositionMilliseconds = TimeSlider.Value * 500;
                TimeSpan newPosition = TimeSpan.FromMilliseconds(newPositionMilliseconds);
                Player.Position = newPosition;
                TimeSpan currentTime = newPosition;
                TimeSpan totalTime = Player.NaturalDuration.TimeSpan;
                Timer.Text = $"{currentTime:mm\\:ss}/{totalTime:mm\\:ss}";
                Player.Play();
                isPlaying = true;
            }
        }
        // mouse timer
        private void InitializeMouseTimer()
        {
            mouseTimer = new DispatcherTimer();
            mouseTimer.Interval = TimeSpan.FromSeconds(5); // Set the interval to 5 seconds
            mouseTimer.Tick += MouseTimer_Tick;
            MouseMove += PlayerWindow_MouseMove;
            mouseTimer.Start();
        }

        private void PlayerWindow_MouseMove(object sender, MouseEventArgs e) //if mouse moved
        {
            RowDefinition rowDefinition = MainGrid.RowDefinitions[1];
            rowDefinition.Height = new GridLength(40);
            Mouse.OverrideCursor = null;
            mouseTimer.Stop();
            mouseTimer.Start();
        }

        private void MouseTimer_Tick(object? sender, EventArgs e)  // if mouse not moved for 5s
        {
            RowDefinition rowDefinition = MainGrid.RowDefinitions[1];
            rowDefinition.Height = new GridLength(0);
            Mouse.OverrideCursor = Cursors.None;
        }
    /////////////////////////////////////////////////////////////////////////////////// Play, pause, ect

    private bool IsPlaying()
        {
            return isPlaying;
        }

        private void Play(object sender, EventArgs e)
        {
            if (!IsPlaying())
            {
                isPlaying = true;
                Player.Play();
                playImg.Source = new BitmapImage(new Uri(@"../../../Icons/pause.ico", UriKind.Relative));
            }
            else
            {
                isPlaying = false;
                Player.Pause();
                playImg.Source = new BitmapImage(new Uri(@"../../../Icons/play.ico", UriKind.Relative));
            }
        }

        private string FilePathGetter()
        {
            TableElement currentItem = (TableElement)folderTableRInstance.ListOfChildren.Items[currentIndex];
            return currentItem.FilePath;
        }

        private void Next(object sender, EventArgs e)
        {
            if (currentIndex < folderTableRInstance.ListOfChildren.Items.Count - 1)
            {
                currentIndex++;
                Player.Source = new Uri(FilePathGetter());
                isPlaying = false;
                Play(this, e);
            }
            else
            {
                MessageBox.Show("No more videos in the Playlist", "Warning");
            }
        }

        private void Previous(object sender, EventArgs e)
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                Player.Source = new Uri(FilePathGetter());
                isPlaying = false;
                Play(this, e);
            }
            else
            {
                MessageBox.Show("No previous videos exist", "Warning");
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////// Volume Control

        private void VolumeButtonClick(object sender, EventArgs e)
        {
            double volume = VolumeSlider.Value;
            if (volume > 0) {
                volumeCach = volume;
                VolumeSlider.Value = 0;
                UpdateVolumeImg();
            }
            else
            {
                VolumeSlider.Value = volumeCach;
                UpdateVolumeImg();
            }
        }

        private void VolumeChange(object sender, EventArgs e)
        {
            Player.Volume = (double)VolumeSlider.Value;
            UpdateVolumeImg();
        }

        private void UpdateVolumeImg()
        {
            double volume = (double)VolumeSlider.Value;
            if (volume == 0)
            {
                volumeImg.Source = new BitmapImage(new Uri(@"../../../Icons/mute.ico", UriKind.Relative));
            }
            else if (volume < 0.5)
            {
                volumeImg.Source = new BitmapImage(new Uri(@"../../../Icons/vlow.ico", UriKind.Relative));
            }
            else if (volume > 0.5)
            {
                volumeImg.Source = new BitmapImage(new Uri(@"../../../Icons/vhigh.ico", UriKind.Relative));
            }
        }

        //////////////////////////////////////////////////////////////////// Right side tracklist logic

        void SwapUp(object sender, RoutedEventArgs e)
        {
            folderTableRInstance.ListOfChildren.SelectedIndex = folderTableInPlayer.ListOfChildren.SelectedIndex;
            folderTableInPlayer.SwapUp((TableElement)folderTableInPlayer.ListOfChildren.SelectedItem);
            MainWindowInstance.SwapUp(sender, e, this);
        }
        void SwapDown(object sender, RoutedEventArgs e)
        {
            folderTableRInstance.ListOfChildren.SelectedIndex = folderTableInPlayer.ListOfChildren.SelectedIndex;
            folderTableInPlayer.SwapDown((TableElement)folderTableInPlayer.ListOfChildren.SelectedItem);
            MainWindowInstance.SwapDown(sender, e, this);
        }
        void FolderTableContainerHide(object sender, RoutedEventArgs e)
        {
            ColumnDefinition columnDefinition = MainGrid.ColumnDefinitions[1];
            columnDefinition.Width = new GridLength(0, GridUnitType.Star);
        }

        void FolderTableContainerRestore(object sender, RoutedEventArgs e)
        {
            OverlayCanvas.Opacity = 0;
            ColumnDefinition columnDefinition = MainGrid.ColumnDefinitions[1];
            columnDefinition.Width = new GridLength(400, GridUnitType.Pixel);
        }

        void OverlayMouseEnter(object sender, MouseEventArgs e)
        {
            if(MainGrid.ColumnDefinitions[1].Width == new GridLength(0, GridUnitType.Star))
            {
                OverlayCanvas.Opacity = 0.5;
            }
        }
        void OverlayMouseLeave(object sender, MouseEventArgs e)
        {
            OverlayCanvas.Opacity = 0;
        }
    }
}
