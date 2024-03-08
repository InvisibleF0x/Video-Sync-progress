using FFmpeg.NET.Enums;
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

namespace video_sync.UserControls
{
    /// <summary>
    /// Logika interakcji dla klasy TableElement.xaml
    /// </summary>
    public partial class TableElement : UserControl
    {
        public TableElement()                   //testing constructor
        {
            InitializeComponent();
        }
        public TableElement(string filePath,int nr, string title, string duration, string preview, bool isSynced)
        {
            FilePath = filePath;
            Nr = nr;
            TitleOfMovie = title;
            Duration = duration;
            Preview = preview;
            IsSynced = isSynced;
            InitializeComponent();
            if (IsSynced)
            {
                IsSyncDisplay.Text = "Y";
            }
            else
            {
                IsSyncDisplay.Text = "N";
            }
        }

        public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register("FilePath", typeof(string), typeof(TableElement), new PropertyMetadata(null));
        public static readonly DependencyProperty NrProperty = DependencyProperty.Register("Nr", typeof(int), typeof(TableElement), new PropertyMetadata(0));
        public static readonly DependencyProperty TitleOfMovieProperty = DependencyProperty.Register("TitleOfMovie", typeof(string), typeof(TableElement), new PropertyMetadata(null));
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(string), typeof(TableElement), new PropertyMetadata(null));
        public static readonly DependencyProperty PreviewProperty = DependencyProperty.Register("Preview", typeof(string), typeof(TableElement), new PropertyMetadata(null));
        public static readonly DependencyProperty IsSyncedProperty = DependencyProperty.Register("Title", typeof(bool), typeof(TableElement), new PropertyMetadata(false));

        public string TitleOfMovie
        {
            get { return (string)GetValue(TitleOfMovieProperty); }
            set { SetValue(TitleOfMovieProperty, value); }
        }
        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }
        public string Duration
        {
            get { return (string)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }
        public string Preview
        {
            get { return (string)GetValue(PreviewProperty); }
            set { SetValue(PreviewProperty, value); }
        }
        public bool IsSynced
        {
            get { return (bool)GetValue(IsSyncedProperty); }
            set { SetValue(IsSyncedProperty, value); }
        }
        public int Nr
        {
            get { return (int)GetValue(NrProperty); }
            set { SetValue(NrProperty, value); }
        }

        public TableElement Clone()
        {
            TableElement copiedElement = new TableElement(this.FilePath, this.Nr, this.TitleOfMovie, this.Duration, this.Preview, this.IsSynced);
            return copiedElement;
        }
    }
}