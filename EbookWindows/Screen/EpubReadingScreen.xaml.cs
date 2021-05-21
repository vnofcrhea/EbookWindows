﻿using EbookWindows.ViewModels;
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

        private static int currentPage = 0;
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
        public void ReadFile(string filePath)
        {
            myEpubReader.ReadFile(filePath);

            currentPage = 0;

            //TableContentComboBox.Items.Clear();
            //foreach (string title in myEpubReader.tableContentTitle)
            //{
            //    TableContentComboBox.Items.Add(title);
            //}
            //TableContentComboBox.SelectedIndex = 0;

            TableContentComboBox.ItemsSource = myEpubReader.tableContent;
            TableContentComboBox.Items.Refresh();
            TableContentComboBox.SelectedIndex = 0;

            bookmarkListview.ItemsSource = myEpubReader.bookmarks;
            bookmarkListview.Items.Refresh();           

            epubWebBrowser.Address = myEpubReader.menuItems[0];
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

                currentPage = getIndex(myEpubReader.menuItems, uri);

                if (currentPage == 0)
                {
                    PreButton.IsEnabled = false;
                    NextButton.IsEnabled = true;
                }
                else if (currentPage == myEpubReader.menuItems.Count - 1)
                {

                    NextButton.IsEnabled = false;
                    PreButton.IsEnabled = true;
                }
                else
                {

                    NextButton.IsEnabled = true;
                    PreButton.IsEnabled = true;
                }

                epubWebBrowser.Address = myEpubReader.menuItems[currentPage];
            }

        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < myEpubReader.menuItems.Count - 1)
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

            string uri = myEpubReader.menuItems[currentPage];
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
            string uri = myEpubReader.menuItems[currentPage];
            TableContentComboBox.SelectedIndex = currentPage;
            epubWebBrowser.Address = uri;
        }
        private void FirstButton_Click(object sender, RoutedEventArgs e)
        {
            currentPage = 0;
            NextButton.IsEnabled = true;
            PreButton.IsEnabled = false;
            string uri = myEpubReader.menuItems[currentPage];
            //update table content
            TableContentComboBox.SelectedIndex = currentPage;
            epubWebBrowser.Address = uri;
        }
        private void LastButton_Click(object sender, RoutedEventArgs e)
        {
            currentPage = myEpubReader.menuItems.Count - 1;
            NextButton.IsEnabled = false;
            PreButton.IsEnabled = true;
            string uri = myEpubReader.menuItems[currentPage];
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

            //if (myEpub.ReadFile()) //file not ""
            //{
            //    currentPage = 0;

            //    TableContentComboBox.Items.Clear();
            //    foreach (string title in myEpubReader.tableContentTitle)
            //    {
            //        TableContentComboBox.Items.Add(title);
            //    }
            //    TableContentComboBox.SelectedIndex = 0;

            //    bookmarkListview.ItemsSource = myEpubReader.tableContentTitle;
            //    bookmarkListview.Items.Refresh();

            //    epubWebBrowser.Address = myEpubReader.menuItems[0];
            //}
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
            if (myEpubReader.addBookmark(myEpubReader.menuItems[currentPage]) == 1)
                bookmarkListview.Items.Refresh();
            
                //do nothing
        }

        private void bookmarkDeleteButtons_Click(object sender, RoutedEventArgs e)
        {
            string chapterLink;
            Button delete = (Button)sender;
            chapterLink = delete.DataContext.GetType().GetProperty("Key").GetValue(delete.DataContext, null).ToString();
            if (myEpubReader.deleteBookmark(chapterLink) == 1)
                bookmarkListview.Items.Refresh();

            //do nothing

        }

        #endregion

        private void bookmarkListview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string chapterLink;
            ListView list = (ListView)sender;
            if (list.SelectedItem != null)
            {
                chapterLink = list.SelectedItem.GetType().GetProperty("Key").GetValue(list.SelectedItem, null).ToString();
                currentPage = getIndex(myEpubReader.menuItems, chapterLink);
                TableContentComboBox.SelectedIndex = currentPage;
                epubWebBrowser.Address = chapterLink;
            }

        }
    }
}