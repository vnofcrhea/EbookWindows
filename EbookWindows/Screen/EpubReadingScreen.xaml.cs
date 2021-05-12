using EbookWindows.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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


namespace EbookWindows.Screen
{
    /// <summary>
    /// Interaction logic for EpubReadingScreen.xaml
    /// </summary>
    public partial class EpubReadingScreen : UserControl
    {

        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        private TimeSpan SpanTime;

        private static readonly myEpubReader myEpub = myEpubReader.getInstance();
        private static List<string> tableContentLink = new List<string>();
        private static List<string> tableContentTitle = new List<string>();
        private static List<string> menuItems = new List<string>();
        private static int currentPage = 0;



        public EpubReadingScreen()
        {
            InitializeComponent();  
        }

        public void ReadFile(string filePath)
        {
            myEpub.ReadFile(filePath);

            currentPage = 0;
            tableContentLink = myEpubReader._tableContentLink;
            menuItems = myEpubReader._menuItems;
            tableContentTitle = myEpubReader._tableContentTitle;

            TableContentComboBox.Items.Clear();
            foreach (string title in tableContentTitle)
            {
                TableContentComboBox.Items.Add(title);
            }
            TableContentComboBox.SelectedIndex = 0;

            EpubWebBrowser.Address = menuItems[0];
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
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.ComboBox comboBox = (System.Windows.Controls.ComboBox)sender;
            if (comboBox.SelectedItem != null)
            {
                string uri = tableContentLink[comboBox.SelectedIndex];
                currentPage = getIndex(menuItems, uri);


                if (currentPage == 0)
                {
                    PreButton.IsEnabled = false;
                    NextButton.IsEnabled = true;
                }
                else if (currentPage == menuItems.Count - 1)
                {

                    NextButton.IsEnabled = false;
                    PreButton.IsEnabled = true;
                }
                else
                {

                    NextButton.IsEnabled = true;
                    PreButton.IsEnabled = true;
                }

                EpubWebBrowser.Address = menuItems[currentPage];
            }
            
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < menuItems.Count - 1)
            {
                currentPage++;
                PreButton.IsEnabled = true;
                NextButton.IsEnabled = true;
            }
            else
            {
                NextButton.IsEnabled = false;
                PreButton.IsEnabled = true;
            }
            
            string uri = menuItems[currentPage];
            int currentChapter = getIndex(tableContentLink, uri);
            TableContentComboBox.SelectedIndex = currentChapter;
            EpubWebBrowser.Address = uri;
        }
        private void PreButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage >= 1)
            {
                currentPage--;
                PreButton.IsEnabled = true;
                NextButton.IsEnabled = true;
            }
            else
            {
                PreButton.IsEnabled = false;
                NextButton.IsEnabled = true;
            }
            //update table content

            string uri = menuItems[currentPage];
            int currentChapter = getIndex(tableContentLink, uri);
            TableContentComboBox.SelectedIndex = currentChapter;
            EpubWebBrowser.Address = uri;
        }
        private void FirstButton_Click(object sender, RoutedEventArgs e)
        {
            currentPage = 0;
            NextButton.IsEnabled = true;
            PreButton.IsEnabled = false;
            string uri = menuItems[currentPage];
            //update table content
            int currentChapter = getIndex(tableContentLink, uri);
            TableContentComboBox.SelectedIndex = currentChapter;
            EpubWebBrowser.Address = uri;
        }
        private void LastButton_Click(object sender, RoutedEventArgs e)
        {
            currentPage = menuItems.Count - 1;
            NextButton.IsEnabled = false;
            PreButton.IsEnabled = true;
            string uri = menuItems[currentPage];
            //update table content
            //int currentChapter = tableContentLink.IndexOf(menuItems[currentPage]);
            //if (currentChapter != -1)
            //    TableContentComboBox.SelectedIndex = currentChapter;
            int currentChapter = getIndex(tableContentLink, uri);
            TableContentComboBox.SelectedIndex = currentChapter;
            EpubWebBrowser.Address = uri;
        }

        public static int getIndex(List<string> list, string s)
        {
            string sub;
            for(int i =0;i<list.Count;i++)
            {
                sub = list[i].Substring(list[i].LastIndexOf("\\"));
                if (sub == s.Substring(s.LastIndexOf("\\")))
                    return i;
            }
            return 0;
        }

        private void ReadingHomeButton_Click(object sender, RoutedEventArgs e)
        {
            // GET FILE PATH, NAME
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();

            if(Path.GetExtension(openFileDialog.FileName) ==".epub")
            {
                //Call reading offline epub sceen
                
                ReadFile(openFileDialog.FileName);

            }
            else
            {
                MessageBox.Show("Invalid file! Please try another file.", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
