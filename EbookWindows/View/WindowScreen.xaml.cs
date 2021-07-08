using EbookWindows.Model;
using EbookWindows.ViewModels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace EbookWindows.Screen
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    /// }

    public class TodoItem
    {
        public string Title { get; set; }
        public int Completion { get; set; }
    }
    public partial class WindowScreen : Window
    {
        public WindowScreen()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            App.ApplyTheme();
            LoadTreeViewList();
            //TreeView_BookList.Items[]
            //lbTodoList.ItemsSource = items;
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static Point GetMousePosition()
        {
            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            return new Point(w32Mouse.X, w32Mouse.Y);
        }
        private void DragStart(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (WindowState != WindowState.Normal)
                {   
                    double percentHorizontal = e.GetPosition(this).X / ActualWidth;
                    double targetHorizontal = RestoreBounds.Width * percentHorizontal;

                    double percentVertical = e.GetPosition(this).Y / ActualHeight;
                    double targetVertical = RestoreBounds.Height * percentVertical;

                    var lMousePosition = GetMousePosition();
                    if (Left<0)
                        Left = lMousePosition.X - targetHorizontal;
                    else
                        Left = lMousePosition.X - targetHorizontal;
                    if (Top < 0)
                        Top = lMousePosition.Y - targetVertical;
                    else
                        Top = lMousePosition.Y - targetVertical;
                    WindowState = WindowState.Normal;
                }
                this.DragMove();
            }
        }
        private void Maximize(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }
        private void Minimize(object sender, RoutedEventArgs e)
        {

            this.WindowState = WindowState.Minimized;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DragStart1(object sender, QueryContinueDragEventArgs e)
        {

        }
        public void OpenDetailScreen()
        {
            detailScreen.LoadPaging(detailScreen.page_index);
            comicReadingScreen.Visibility = Visibility.Collapsed;
            detailScreen.Visibility = Visibility.Visible;
        }
        public async void OpenDetailScreen(Book_Short x)
        {
            StartLoading();
            var xa = await Task.Run(()=> detailScreen.LoadData(x));
            if (xa)
            {
                MainGrid.Visibility = Visibility.Collapsed;
                detailScreen.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("An error occurred while processing, please try again.");
            }

            EndLoading();
        }
        
        public async void OpenDetailScreen(string url)
        {
            StartLoading();
            var x= await Task.Run(() => detailScreen.LoadData(url));
            if (x)
            {
                App.Global.Book_Short_ViewModel.LoadListBookShort();
                LoadShelf();
                LoadTreeViewList();
                MainGrid.Visibility = Visibility.Collapsed;
                detailScreen.Visibility = Visibility.Visible;
            }    
            else
            {
                MessageBox.Show("The path is invalid or not supported. ");
            }    
            EndLoading();
        }

        public async void OpenComicReadingScreen()
        {
            await Task.Run(()=>comicReadingScreen.LoadData());
            MainGrid.Visibility = Visibility.Collapsed;
            detailScreen.Visibility = Visibility.Collapsed;
            comicReadingScreen.Visibility = Visibility.Visible;
        }
        public async void LoadShelf()
        {
            await Task.Run(()=> BookTextShelf.LoadBook_ShortPanel());
        }
        /// <summary>
        /// return home screen when press homebutton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReturnHome_Click(object sender, RoutedEventArgs e)
        {

            LoadShelf();
            MainGrid.Visibility = Visibility.Visible;
                if (detailScreen.Visibility == Visibility.Visible)
                {
                    detailScreen.Visibility = Visibility.Collapsed;
                    
                    return;
                }
                else if (pdfReadingScreen.Visibility == Visibility.Visible)
                {
                    pdfReadingScreen.homeBtn_Click(sender,e);
                    return;
                }
                else if (epubReadingScreen.Visibility == Visibility.Visible)
                {
                    epubReadingScreen.Visibility = Visibility.Collapsed;
                    return;
                }
                else if(comicReadingScreen.Visibility == Visibility.Visible)
                {
                    comicReadingScreen.Visibility = Visibility.Collapsed;
                    return;
                }
                else
                {
                    //do nothing
                }


        }

        /// <summary>
        /// Back to home when press return home button of reading screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ReturnFromReadingScreen_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Visibility = Visibility.Visible;
            pdfReadingScreen.Visibility = Visibility.Collapsed;
            epubReadingScreen.Visibility = Visibility.Collapsed;
            comicReadingScreen.Visibility = Visibility.Collapsed;
           
        }


        /// <summary>
        /// Home load reading screen when open a file from file path
        /// </summary>
        /// <param name="filePath">path of file</param>
        /// <param name="index">-1: if press browser button; index: index in recent file ListView</param>
        public void filePathChanged(string filePath, int index)
        {
            
            string fileExtension = Path.GetExtension(filePath);
            if (fileExtension.Equals(".pdf"))
            {
                int location = recentFileUserControl.GetRecentLocationOfFile(filePath);
                AddRecentFileList(index, filePath, "\\Icon\\pdf.png");
                if (pdfReadingScreen.ReadFilePdf(filePath, location))
                {
                    MainGrid.Visibility = Visibility.Collapsed;
                    epubReadingScreen.Visibility = Visibility.Collapsed;
                    pdfReadingScreen.Visibility = Visibility.Visible;
                   
                }     
            } else if (fileExtension.Equals(".epub"))
            {
                epubReadingScreen.ReadFile(filePath);
                MainGrid.Visibility = Visibility.Collapsed;
                pdfReadingScreen.Visibility = Visibility.Collapsed;
                epubReadingScreen.Visibility = Visibility.Visible;
                AddRecentFileList(index, filePath, "\\Icon\\epub.png");
            }else
            {
                //nothing
            }

          
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"> if press browser button; index: index in recent file ListView</param></param>
        /// <param name="filePath"></param>
        /// <param name="fileIcon">Name of icon file</param>
        private void AddRecentFileList(int index,string filePath, string fileIcon)
        {
            string fileName = Path.GetFileName(filePath);
            
            if (index < 0) //index = -1
            {
                recentFileUserControl.BrowserANewFile(fileName, filePath, fileIcon);
            }
            else //index >= 0
            {
                recentFileUserControl.openAFileInRecentFileList(index);
            }
        }

        public void addMoreBookBtn_Click(object sender, RoutedEventArgs e)
        {
            var popupEbookScreen = new PopupEbookScreen();
            popupEbookScreen.BrowserEvent += filePathChanged;
            popupEbookScreen.Owner = this;
            popupEbookScreen.ShowDialog();
        }
        public void StartLoading()
        {
                LoadingGrid.Visibility = Visibility.Visible;
        }
        public void EndLoading()
        {
                LoadingGrid.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// event before app closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (pdfReadingScreen.Visibility == Visibility.Visible)
            {
                pdfReadingScreen.SaveFilePdf();
                //int location = pdfReadingScreen.PageConboBox.SelectedIndex;
                //recentFileUserControl.UpdateLocationOfFile(location);
            }

            //Saving Setting before close
            App.Global.RecentFile_ViewModel.Save_File();
            App.Global.Settings_ViewModel.SaveSetting();
        }

        private void LeftHeader_Click(object sender, RoutedEventArgs e)
        {
            if (LeftHeaderColumn.Width.Value == 50)
            {
                BookListGrid.Visibility = Visibility.Visible;
                LeftHeaderColumn.Width = new GridLength(250, GridUnitType.Pixel);
            }
            else
            {
                BookListGrid.Visibility = Visibility.Collapsed;
                LeftHeaderColumn.Width = new GridLength(50, GridUnitType.Pixel);
            }
        }

        private void SolidColorBrush_ColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {

        }

        public async void LoadTreeViewList()
        {
            await Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
            {
                App.Global.Book_TreeView.Clear();
                //Book_Short_TreeView ChildTree = new Book_Short_TreeView() { Title = "BOOK_CONTENTS" };
                //ChildTree.Items.AddRange(App.Global.Book_Short_ViewModel.Book_Short);
                //App.Global.Book_TreeView.Add(ChildTree);
                TreeView_BookList.ItemsSource = App.Global.Book_Short_ViewModel.Book_Short;
                TreeView_BookList.Items.Refresh();
            });
            });
            
            
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsGrid.Visibility = Visibility.Visible;
            App.Global.Settings_ViewModel.Notification = "";
        }
        public void SettingClose()
        {
            SettingsGrid.Visibility = Visibility.Collapsed;
        }

        private void Book_Click(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            
        }
        public void ModifyFullscreenMode() // Wait fix
        {
            if (App.Global.isFullScreen)
            {
                this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
                RowHeaderSize.Height = new GridLength(30);
            }
            else
            {
                this.MaxHeight = double.PositiveInfinity;
                RowHeaderSize.Height = new GridLength(0);
            }
            App.Global.isFullScreen = !App.Global.isFullScreen;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutGrid.Visibility = Visibility.Visible;
        }
        public void AboutClose()
        {
            AboutGrid.Visibility = Visibility.Collapsed;
        }
        private void Book_Click(object sender, SelectionChangedEventArgs e)
        {
            if (!((sender as ListView).SelectedItem is Book_Short item))
                return;
            else if ((sender as ListView).SelectedIndex < 0)
                return;
            else
            {
                OpenDetailScreen(item);
                //var x = (sender as TreeView).ItemContainerGenerator.ContainerFromIndex

                if (pdfReadingScreen.Visibility == Visibility.Visible)
                {
                    pdfReadingScreen.homeBtn_Click(sender, e);
                    return;
                }
                else if (epubReadingScreen.Visibility == Visibility.Visible)
                {
                    epubReadingScreen.Visibility = Visibility.Collapsed;
                    return;
                }
                else if (comicReadingScreen.Visibility == Visibility.Visible)
                {
                    comicReadingScreen.Visibility = Visibility.Collapsed;
                    return;
                }
                else
                {
                    //do nothing
                }
                (sender as ListView).SelectedIndex = -1;
            }
        }
    }
}
      