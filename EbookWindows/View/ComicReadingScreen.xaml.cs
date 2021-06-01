﻿using System;
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
using MaterialDesignThemes.Wpf;
using EbookWindows.Model;
namespace EbookWindows.Screen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ComicReadingScreen : UserControl
    {
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        private TimeSpan SpanTime;
        public bool isOnline = true;
        public double Scaling_Rate = 1;

        public ComicReadingScreen()
        {
            InitializeComponent();
            DarkModeChecker.IsChecked = App.isDarkMode();
        }
        #region Execute
        public void LoadData()
        {
            this.Dispatcher.Invoke(() =>
            {
                App.Global.Chapter_ViewModel.Load_ChapterList();
                Load_DataContext(); // Binding DataContext
                Chapter_List.SelectedValue = App.Global.Chapter_ViewModel.Current_Chapter.link;
                WindowScreen win = (WindowScreen)Window.GetWindow(this);
                Content_Box.MaxWidth = (win.ActualWidth - win.LeftHeader.ActualWidth);
            });
            
        }
        public void Load_DataContext() //Load/Reload DataContext
        {
            this.Dispatcher.Invoke(() =>
            {
                this.DataContext = App.Global.Chapter_ViewModel;
            });
        }
        public void Load_Content(Chapter chapter)
        {
            App.Global.Chapter_ViewModel.Current_Chapter = chapter;
            App.Global.Chapter_ViewModel.Load_Content();
            this.Dispatcher.Invoke(() =>
            {
                Content_Box.Text = App.Global.Chapter_ViewModel.Current_Chapter_Content.content;
                scrollContent_Box.ScrollToVerticalOffset(0);
            });
        }
        
        public void WriteRecentChapter()
        {
            // var chapter_dir = App.book_dir + "\\content\\" + index + ".json";
        }
        #endregion
        private async void Chapter_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!((sender as ComboBox).SelectedItem is Chapter item))
                return;
            WindowScreen win = (WindowScreen)Window.GetWindow(this);
            win.StartLoading();
            await Task.Run(() => Load_Content(item));
            win.EndLoading();
        }
        #region Event
        private void StackPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!dispatcherTimer.IsEnabled)
            {
                dispatcherTimer.IsEnabled = true;
                dispatcherTimer.Interval = new TimeSpan(0, 0, 2); ;
                dispatcherTimer.Start();
                dispatcherTimer.Tick += new EventHandler(OnTimedEvent);
                return;
            }
            BottomPanelTool.Visibility = Visibility.Visible;
            TopPanelTool.Visibility = Visibility.Visible;
            scrollContent_Box.Margin = new Thickness(0, 0, 0, 42);
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
                scrollContent_Box.Margin = new Thickness(0, 0, 0, 0);
            });
        }
        private void ShowHideToolButton_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            var data = ((sender as Button).Content as MaterialDesignThemes.Wpf.PackIcon);
            if (data.Kind.ToString().Equals("Hide"))
            {
                ((sender as Button).Content as MaterialDesignThemes.Wpf.PackIcon).Kind = MaterialDesignThemes.Wpf.PackIconKind.Eye;
                BottomPanelTool.Visibility = Visibility.Visible;
                TopPanelTool.Visibility = Visibility.Visible;
                dispatcherTimer.Start();
                this.MouseMove += StackPanel_MouseMove;
                scrollContent_Box.Margin = new Thickness(0, 0, 0, 42);
            }
            else
            {
                ((sender as Button).Content as MaterialDesignThemes.Wpf.PackIcon).Kind = MaterialDesignThemes.Wpf.PackIconKind.EyeOff;
                BottomPanelTool.Visibility = Visibility.Collapsed;
                TopPanelTool.Visibility = Visibility.Collapsed;
                dispatcherTimer.Stop();
                scrollContent_Box.Margin = new Thickness(0, 0, 0, 0);

                this.MouseMove -= StackPanel_MouseMove;
            }
        }
        private void OnCopy(object sender, ExecutedRoutedEventArgs e)
        {

        }
        public void UpdatePageView()
        {
            this.Dispatcher.Invoke(() =>
            {
                if (this.ActualWidth == 0)
                {
                    WindowScreen win = (WindowScreen)Window.GetWindow(this);
                    Content_Box.MaxWidth = (win.ActualWidth - win.LeftHeader.ActualWidth) * 0.75;
                }
                else
                    Content_Box.MaxWidth = this.ActualWidth * 0.75;
            });
        }
        
        
        
        public void Content_Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePageView();
        }
        private void PreviousChapter_Click(object sender, RoutedEventArgs e)
        {
            var x = (sender as Button);
            if (--Chapter_List.SelectedIndex == 0)
                x.IsEnabled = false;
            else { x.IsEnabled = true; }
        }
        private void NextChapter_Click(object sender, RoutedEventArgs e)
        {
            var x = (sender as Button);
            if (Chapter_List.SelectedIndex++ == Chapter_List.Items.Count - 1)
                x.IsEnabled = false;
            else { x.IsEnabled = true; }
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            var x = (sender as Button);
            Scaling_Rate += 0.25;
            ZoomOut_Button.IsEnabled = true;
            if (Scaling_Rate == 2)
            {
                x.IsEnabled = false;
            }
            else
            {
                x.IsEnabled = true;
            }
            Content_Box_Scaling.ScaleX = Scaling_Rate;
            Content_Box_Scaling.ScaleY = Scaling_Rate;
            zoomTextbox.Text = (Scaling_Rate * 100).ToString() + '%';
        }


        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            var x = (sender as Button);
            Scaling_Rate -= 0.25;
            ZoomIn_Button.IsEnabled = true;
            if (Scaling_Rate == 0.25)
            {
                x.IsEnabled = false;
            }
            else
            {
                x.IsEnabled = true;
            }
            Content_Box_Scaling.ScaleX = Scaling_Rate;
            Content_Box_Scaling.ScaleY = Scaling_Rate;
            zoomTextbox.Text = (Scaling_Rate * 100).ToString() + '%';
        }
        #endregion
        #region Persionality
        private void ScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Content_Box.Width = (sender as ScrollViewer).ActualWidth;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Content_Box.FontSize = (sender as Slider).Value;
        }

        private void Font_Changed(object sender, SelectionChangedEventArgs e)
        {
            Content_Box.FontFamily = new FontFamily(((sender as ComboBox).SelectedValue as ComboBoxItem).Content.ToString());
        }

        private void DarkMode_Enable(object sender, RoutedEventArgs e)
        {
            App.ChangeBaseTheme(BaseTheme.Dark);
        }

        private void DarkMode_Disable(object sender, RoutedEventArgs e)
        {
            App.ChangeBaseTheme(BaseTheme.Light);
        }
        #endregion
    }
}