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

        private static int currentPage = -1;
        private static double currentZoom = 0;



        public EpubReadingScreen()
        {
            InitializeComponent();
        }

        private int getIndex(List<string> list, string s)
        {
            string sub;
            for (int i = 0; i < list.Count; i++)
            {
                sub = list[i].Substring(list[i].LastIndexOf("\\"));
                if (sub == s.Substring(s.LastIndexOf("\\")))
                    return i;
            }
            return -1;
        }
        public bool ReadFile(string filePath)
        {
            //update reading status before read new file
            if(currentPage!=-1)
                OfflineEpub_ViewModel.updateReadingStatus(currentPage, textFontComboBox.SelectedItem.ToString());
            if (OfflineEpub_ViewModel.ReadFile(filePath))
            {
               currentPage = int.Parse(OfflineEpub_ViewModel.readingStatus[0]);
                //Font family combobox
                textFontComboBox.ItemsSource = OfflineEpub_ViewModel.fontFamilys;
                //update table content
                TableContentComboBox.ItemsSource = OfflineEpub_ViewModel.tableContent;
                TableContentComboBox.Items.Refresh();
                TableContentComboBox.SelectedIndex = currentPage;
                //update boookmark list
                bookmarkListview.ItemsSource = OfflineEpub_ViewModel.bookmarks;
                bookmarkListview.Items.Refresh();
                //update reading status
                epubWebBrowser.Address = OfflineEpub_ViewModel.menuItems[currentPage];
                textFontComboBox.Text = OfflineEpub_ViewModel.readingStatus[1];

                return true;
            }
            return false;
        }

        #region show/hide tool bar
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
        #endregion

        #region chapters tool bar
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.ComboBox comboBox = (System.Windows.Controls.ComboBox)sender;
            if (comboBox.SelectedItem != null)
            {
                //get current key
                string uri = comboBox.SelectedItem.GetType().GetProperty("Key").GetValue(comboBox.SelectedItem, null).ToString();

                currentPage = getIndex(OfflineEpub_ViewModel.menuItems, uri);

                if (currentPage == 0)
                {
                    PreButton.IsEnabled = false;
                    NextButton.IsEnabled = true;
                }
                else if (currentPage == OfflineEpub_ViewModel.menuItems.Count - 1)
                {

                    NextButton.IsEnabled = false;
                    PreButton.IsEnabled = true;
                }
                else
                {

                    NextButton.IsEnabled = true;
                    PreButton.IsEnabled = true;
                }

                epubWebBrowser.Address = OfflineEpub_ViewModel.menuItems[currentPage];
            }

        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < OfflineEpub_ViewModel.menuItems.Count - 1)
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

            string uri = OfflineEpub_ViewModel.menuItems[currentPage];
            TableContentComboBox.SelectedIndex = currentPage;
            epubWebBrowser.Address = uri;
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
            string uri = OfflineEpub_ViewModel.menuItems[currentPage];
            TableContentComboBox.SelectedIndex = currentPage;
            epubWebBrowser.Address = uri;
        }
        private void FirstButton_Click(object sender, RoutedEventArgs e)
        {
            currentPage = 0;
            NextButton.IsEnabled = true;
            PreButton.IsEnabled = false;
            string uri = OfflineEpub_ViewModel.menuItems[currentPage];
            //update table content
            TableContentComboBox.SelectedIndex = currentPage;
            epubWebBrowser.Address = uri;
        }
        private void LastButton_Click(object sender, RoutedEventArgs e)
        {
            currentPage = OfflineEpub_ViewModel.menuItems.Count - 1;
            NextButton.IsEnabled = false;
            PreButton.IsEnabled = true;
            string uri = OfflineEpub_ViewModel.menuItems[currentPage];
            //update table content            
            TableContentComboBox.SelectedIndex = currentPage;
            epubWebBrowser.Address = uri;
        }
        #endregion

        #region Zoom buttons       
        private void zoomoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (--currentZoom > zoomSlider.Minimum)
            {
                zoomSlider.Value = currentZoom;
                epubWebBrowser.ZoomLevel = currentZoom;
                zoominButton.IsEnabled = true;
            }
            else
            {
                currentZoom = zoomSlider.Minimum;

                zoomSlider.Value = currentZoom;
                epubWebBrowser.ZoomLevel = currentZoom;
                zoomoutButton.IsEnabled = false;
                zoominButton.IsEnabled = true;
            }
        }
        private void zoominButton_Click(object sender, RoutedEventArgs e)
        {
            if (++currentZoom < zoomSlider.Maximum)
            {
                zoomSlider.Value = currentZoom;
                epubWebBrowser.ZoomLevel = currentZoom;
                zoominButton.IsEnabled = true;
            }
            else
            {
                currentZoom = zoomSlider.Maximum;

                zoomSlider.Value = currentZoom;
                epubWebBrowser.ZoomLevel = currentZoom;
                zoomoutButton.IsEnabled = true;
                zoominButton.IsEnabled = false;
            }
        }
        private void zoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            currentZoom = zoomSlider.Value;
            epubWebBrowser.ZoomLevel = currentZoom;
            if (currentZoom == zoomSlider.Minimum)
            {
                zoomoutButton.IsEnabled = false; ;
                zoominButton.IsEnabled = true;
            }
            else if (currentZoom == zoomSlider.Maximum)
            {
                zoomoutButton.IsEnabled = true;
                zoominButton.IsEnabled = false;
            }
            else
            {
                zoomoutButton.IsEnabled = true;
                zoominButton.IsEnabled = true;
            }
        }
        #endregion

        private void readingOtherButton_Click(object sender, RoutedEventArgs e)
        {
            WindowScreen windowScreen = (WindowScreen)Application.Current.MainWindow;
            windowScreen.addMoreBookBtn_Click(sender, e);
        }

        #region bookmark
        private void closeBookmarkButton_Click(object sender, RoutedEventArgs e)
        {
            bookmarkBorder.Visibility = Visibility.Collapsed;
        }
        private void openBookmarksButton_Click(object sender, RoutedEventArgs e)
        {
            if (bookmarkBorder.Visibility == Visibility.Visible)
            {
                bookmarkBorder.Visibility = Visibility.Collapsed;
            }
            else bookmarkBorder.Visibility = Visibility.Visible;
        }
        private void addBookmarkButton_Click(object sender, RoutedEventArgs e)
        {
            if (OfflineEpub_ViewModel.addBookmark(OfflineEpub_ViewModel.menuItems[currentPage]) == 1)
                bookmarkListview.Items.Refresh();
            else
            {
                MessageBox.Show("This chapter already bookmarked!", "Same Bookmark", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void bookmarkDeleteButtons_Click(object sender, RoutedEventArgs e)
        {
            string chapterLink;
            Button delete = (Button)sender;
            chapterLink = delete.DataContext.GetType().GetProperty("Key").GetValue(delete.DataContext, null).ToString();
            if (OfflineEpub_ViewModel.deleteBookmark(chapterLink) == 1)
                bookmarkListview.Items.Refresh();

            //do nothing

        }

        private void bookmarkListview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string chapterLink;
            ListView list = (ListView)sender;
            if (list.SelectedItem != null)
            {
                chapterLink = list.SelectedItem.GetType().GetProperty("Key").GetValue(list.SelectedItem, null).ToString();
                currentPage = getIndex(OfflineEpub_ViewModel.menuItems, chapterLink);
                TableContentComboBox.SelectedIndex = currentPage;
                epubWebBrowser.Address = chapterLink;
            }

        }


        #endregion


        private void changeBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string color = button.Background.ToString();
            OfflineEpub_ViewModel.changeBackgroundColor(color);
            //
            epubWebBrowser.Address = OfflineEpub_ViewModel.menuItems[currentPage];
        }

        private void changeForcegroundButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string color = button.Background.ToString();
            OfflineEpub_ViewModel.changeForegroundColor(color);
            //
            epubWebBrowser.Address = OfflineEpub_ViewModel.menuItems[currentPage];
        }

        private void textFontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.ComboBox comboBox = (System.Windows.Controls.ComboBox)sender;
            if (comboBox.SelectedItem != null)
            {
                //get the selected font
                string font = comboBox.SelectedItem.ToString();
                OfflineEpub_ViewModel.changeFontFamily(font);
                epubWebBrowser.Address = OfflineEpub_ViewModel.menuItems[currentPage];
            }
        }
    }
}