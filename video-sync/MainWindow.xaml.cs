using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
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
        public PlayerWindow playerWindow;
        public MainWindow()
        {
            InitializeComponent();
            FillGrid();
            folderTableL.AddChildAdapter(); //View every video in the folder on the left
        }

        void FillGrid()
        {
            // Add them to mainGrid
            mainGrid.Children.Add(folderTableL);
            mainGrid.Children.Add(folderTableR);

            // Set their positions in mainGrid
            Grid.SetRow(folderTableL, 1); // Second row
            Grid.SetColumn(folderTableL, 1); // First column
            Grid.SetRow(folderTableR, 1); // Second row
            Grid.SetColumn(folderTableR, 3); // Second column
        }

        void TrackAdd(object sender, RoutedEventArgs e)
        {
            TableElement selectedElement = (TableElement)folderTableL.ListOfChildren.SelectedItem;
            folderTableR.AddChild(selectedElement);
        }

        void TrackRemove(object sender, RoutedEventArgs e)
        {
            folderTableR.RemoveChild((TableElement)folderTableR.ListOfChildren.SelectedItem);
        }

        void SwapUp(object sender, RoutedEventArgs e)
        {
            folderTableR.SwapUp((TableElement)folderTableR.ListOfChildren.SelectedItem);
        }
        void SwapDown(object sender, RoutedEventArgs e)
        {
            folderTableR.SwapDown((TableElement)folderTableR.ListOfChildren.SelectedItem);
        }

        void Play(object sender, RoutedEventArgs e)
        {
            if (folderTableR.ListOfChildren.HasItems)
            {
                playerWindow = new PlayerWindow(this);
                playerWindow.Show();
            }
            else
            {
                MessageBox.Show("Playlist cannot be empty !");
            }
        }
    }
}