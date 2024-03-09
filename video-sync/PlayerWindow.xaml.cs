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
using System.Windows.Shapes;
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
        public PlayerWindow(MainWindow mainWindowInstance)
        {
            InitializeComponent();
            MainWindowInstance = mainWindowInstance;
            folderTableRInstance = mainWindowInstance.folderTableR;
        }

        void demobutton(object sender, EventArgs e)
        {
            TableElement firstItem = (TableElement)MainWindowInstance.folderTableR.ListOfChildren.Items[0];
            Player.Source = new Uri(firstItem.FilePath);
            Player.Play();
        }
    }
}
