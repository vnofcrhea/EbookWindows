using EbookWindows.Model;
using EbookWindows.ViewModels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
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
        public async void OpenDetailScreen(Book_Short x)
        {
            StartLoading();
            await Task.Run(()=>detailScreen.LoadData(x));
            MainGrid.Visibility = Visibility.Collapsed;
            detailScreen.Visibility = Visibility.Visible;
            EndLoading();
        }
        public async void OpenDetailScreen(string url)
        {
            StartLoading();
            await Task.Run(() => detailScreen.LoadData(url));
            MainGrid.Visibility = Visibility.Collapsed;
            detailScreen.Visibility = Visibility.Visible;
            EndLoading();
        }

        public async void OpenComicReadingScreen()
        {
            await Task.Run(()=>comicReadingScreen.LoadData());
            MainGrid.Visibility = Visibility.Collapsed;
            detailScreen.Visibility = Visibility.Collapsed;
            comicReadingScreen.Visibility = Visibility.Visible;
        }
        public void LoadShelf()
        {
            BookTextShelf.LoadBook_ShortPanel();
        }
        /// <summary>
        /// return home screen when press homebutton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReturnHome_Click(object sender, RoutedEventArgs e)
        {
            if(MainGrid.Visibility != Visibility.Visible)
            {
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
                if (pdfReadingScreen.LoadData(filePath, location))
                {
                    MainGrid.Visibility = Visibility.Collapsed;
                    epubReadingScreen.Visibility = Visibility.Collapsed;
                    pdfReadingScreen.Visibility = Visibility.Visible;
                    AddRecentFileList(index, filePath, "Icon\\pdf.png");
                }
            }
            else if (fileExtension.Equals(".epub"))
            {
                if (epubReadingScreen.ReadFile(filePath))
                {
                    MainGrid.Visibility = Visibility.Collapsed;
                    pdfReadingScreen.Visibility = Visibility.Collapsed;
                    epubReadingScreen.Visibility = Visibility.Visible;
                    AddRecentFileList(index, filePath, "Icon\\epub.png");
                }
                //else do nothing
            }
            else
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
            if(pdfReadingScreen.Visibility == Visibility)
            {
                pdfReadingScreen.SaveFilePdf();
                int location = pdfReadingScreen.PageConboBox.SelectedIndex;
                recentFileUserControl.UpdateLocationOfFile(location);
            }
            recentFileUserControl.SaveRecentFileList();
        }

        private void LeftHeader_Click(object sender, RoutedEventArgs e)
        {
            if (LeftHeaderColumn.Width.Value == 0)
                LeftHeaderColumn.Width = new GridLength(250, GridUnitType.Pixel);
            else
                LeftHeaderColumn.Width = new GridLength(0, GridUnitType.Pixel);
        }

        private void SolidColorBrush_ColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {

        }

        public void LoadTreeViewList()
        {
            App.Global.Book_TreeView.Clear();
            Book_Short_TreeView ChildTree = new Book_Short_TreeView() { Title = "BOOK_CONTENTS" };
            ChildTree.Items.AddRange(App.Global.Book_Short_ViewModel.Book_Short.ToList());
            App.Global.Book_TreeView.Add(ChildTree);
            TreeView_BookList.ItemsSource = App.Global.Book_TreeView;
        }
    }
}
      