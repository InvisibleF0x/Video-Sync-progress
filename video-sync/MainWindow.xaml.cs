using System;
using System.Collections.Generic;
using System.Linq;
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
            if (selectedElement != null && !ContainsVideo(selectedElement.FilePath))
            {
                TableElement clonedChild = selectedElement.Clone();
                folderTableR.AddChild(clonedChild);
                uniqueFilePaths.Add(clonedChild.FilePath);
            }
        }

        void TrackRemove(object sender, RoutedEventArgs e)
        {
            TableElement selectedElement = (TableElement)folderTableR.ListOfChildren.SelectedItem;

            // Check if the selected item belongs to folderTableR
            if (folderTableR.ListOfChildren.Items.Contains(selectedElement))
            {
                uniqueFilePaths.Remove(selectedElement.FilePath);
                folderTableR.RemoveChild(selectedElement);
                int i = 1;
                foreach (TableElement item in folderTableR.ListOfChildren.Items)
                {
                    item.Nr = i;
                    i++;
                }
            }
        }

            List<string> uniqueFilePaths = new List<string>();
        public bool ContainsVideo(string videoPath)
        {
            return uniqueFilePaths.Contains(videoPath);
        }

    }
}