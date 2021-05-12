using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using EbookWindows.ViewModels;
using Newtonsoft.Json;

namespace EbookWindows.Screen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ComicReadingScreen : UserControl
    {
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        private TimeSpan SpanTime;
        Root_Reading item = new Root_Reading();
        public ComicReadingScreen()
        {
            InitializeComponent();
        }
        public void LoadData(string url,int type)
        {
            var json = new WebClient().DownloadString("http://127.0.0.1:5000/api/v2/books/details?url="+url);
            item = JsonConvert.DeserializeObject<Root_Reading>(json);
            Content_Box.Text = item.content;
        }
        private void StackPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!dispatcherTimer.IsEnabled)
            {
                dispatcherTimer.IsEnabled = true;
                return;
            }
            BottomPanelTool.Visibility = Visibility.Visible;
            TopPanelTool.Visibility = Visibility.Visible;
            if (dispatcherTimer.IsEnabled)
            {
                dispatcherTimer.Interval = new TimeSpan(0, 0, 2); ;
                dispatcherTimer.Start();
                dispatcherTimer.Tick += new EventHandler(OnTimedEvent);
            }
        }
        private void OnTimedEvent(object source, EventArgs e)
        {
            dispatcherTimer.Stop();
            SpanTime = new TimeSpan();
            SpanTime = SpanTime.Add(dispatcherTimer.Interval);
            this.Dispatcher.Invoke(() =>
            {
                BottomPanelTool.Visibility = Visibility.Hidden;
                TopPanelTool.Visibility = Visibility.Hidden;
            });
        }

        private void ShowHideToolButton_Click(object sender, RoutedEventArgs e)
        {
            BottomPanelTool.Visibility = Visibility.Collapsed;
            TopPanelTool.Visibility = Visibility.Collapsed;
            dispatcherTimer.Stop();
            var data = ((sender as Button).Content as MaterialDesignThemes.Wpf.PackIcon);
            if (data.Kind.ToString().Equals("Hide"))
            {
                ((sender as Button).Content as MaterialDesignThemes.Wpf.PackIcon).Kind = MaterialDesignThemes.Wpf.PackIconKind.Eye;
                BottomPanelTool.Visibility = Visibility.Visible;
                TopPanelTool.Visibility = Visibility.Visible;
                MainGrid.MouseMove += new MouseEventHandler(StackPanel_MouseMove);
            }
            else
            {
                ((sender as Button).Content as MaterialDesignThemes.Wpf.PackIcon).Kind = MaterialDesignThemes.Wpf.PackIconKind.EyeOff;
                BottomPanelTool.Visibility = Visibility.Collapsed;
                TopPanelTool.Visibility = Visibility.Collapsed;
                MainGrid.MouseMove -= new MouseEventHandler(StackPanel_MouseMove);
            }
        }

        private void OnCopy(object sender, ExecutedRoutedEventArgs e)
        {

        }
    }
}
