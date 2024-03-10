using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using video_sync.UserControls;

namespace video_sync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public FolderTable folderTableL = new FolderTable();
        public FolderTable folderTableR = new FolderTable();
        public PlayerWindow? playerWindow;
        private bool isPlayerWindowOpen = false;
        public MainWindow()
        {
            InitializeComponent();
            FillGrid();
            folderTableL.AddChildAdapter(); //View every video in the folder on the left table
            Closing += OnMainWindowClosing;
        }

        void OnMainWindowClosing(object? sender, System.ComponentModel.CancelEventArgs e) // Delete the temporary directory and its contents
        {
            if (isPlayerWindowOpen)
            {
                playerWindow.Close();
            }
        }

        void FillGrid()
        {
            mainGrid.Children.Add(folderTableL);
            mainGrid.Children.Add(folderTableR);
            Grid.SetRow(folderTableL, 1);
            Grid.SetColumn(folderTableL, 1);
            Grid.SetRow(folderTableR, 1);
            Grid.SetColumn(folderTableR, 3);
        }

        void TrackAdd(object sender, RoutedEventArgs e)
        {
            if (!isPlayerWindowOpen)
            {
                TableElement selectedElement = (TableElement)folderTableL.ListOfChildren.SelectedItem;
                folderTableR.AddChild(selectedElement);
            }
            else
            {
                MessageBox.Show("To edit playlist close the player, or use the buttons inside of it");
            }
        }

        void TrackRemove(object sender, RoutedEventArgs e)
        {
            if (!isPlayerWindowOpen)
            {
                folderTableR.RemoveChild((TableElement)folderTableR.ListOfChildren.SelectedItem);
            }
            else
            {
                MessageBox.Show("To edit playlist close the player, or use the buttons inside of it");
            }
        }

        public void SwapUp(object sender, RoutedEventArgs e)
        {
            SwapUp(sender, e, this);
        }

        public void SwapUp(object sender, RoutedEventArgs e, Window? callingWindow = null)
        {
            if (!isPlayerWindowOpen || callingWindow == playerWindow)
            {
                folderTableR.SwapUp((TableElement)folderTableR.ListOfChildren.SelectedItem);
            }
            else
            {
                MessageBox.Show("To edit playlist close the player, or use the buttons inside of it");
            }
        }
        public void SwapDown(object sender, RoutedEventArgs e)
        {
            SwapDown(sender, e, this);
        }

        public void SwapDown(object sender, RoutedEventArgs e, Window? callingWindow = null)
        {
            if (!isPlayerWindowOpen || callingWindow == playerWindow)
            {
                folderTableR.SwapDown((TableElement)folderTableR.ListOfChildren.SelectedItem);
            }
            else
            {
                if (callingWindow == playerWindow)
                {
                    folderTableR.ListOfChildren.SelectedIndex = playerWindow.folderTableInPlayer.ListOfChildren.SelectedIndex;
                }
                MessageBox.Show("To edit playlist close the player, or use the buttons inside of it");
            }
        }

        void Play(object sender, RoutedEventArgs e)
        {
            if (!isPlayerWindowOpen)
            {
                if (folderTableR.ListOfChildren.HasItems)
                {
                    playerWindow = new PlayerWindow(this);
                    playerWindow.Closed += (s, args) => isPlayerWindowOpen = false;
                    playerWindow.Show();
                    isPlayerWindowOpen = true;
                }
                else
                {
                    MessageBox.Show("Playlist cannot be empty !");
                }
            }
            else
            {
                MessageBox.Show("Player Window is already open !");
            }
        }

        void OpenVideoFolder(object sender, RoutedEventArgs e)
        {
            string executablePath = AppDomain.CurrentDomain.BaseDirectory;
            string folderPath = Path.GetFullPath(Path.Combine(executablePath, @"..\..\..\Videos"));
            Process.Start("explorer.exe",folderPath);
        }
    }
}