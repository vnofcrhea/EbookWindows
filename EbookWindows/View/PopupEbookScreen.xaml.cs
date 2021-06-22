using EbookWindows.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Design.Behavior;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static EbookWindows.Screen.WindowScreen;
using Path = System.IO.Path;

namespace EbookWindows.Screen
{
    /// <summary>
    /// Interaction logic for BrowserLinkScreen.xaml
    /// </summary>
    public partial class PopupEbookScreen : Window
    {
        public delegate void BrowserHandler(string filePath, int index);
        public event BrowserHandler BrowserEvent;



        public PopupEbookScreen()
        {
            InitializeComponent();
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
                    if (Left < 0)
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

        public static Point GetMousePosition()
        {
            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void go_Click(object sender, RoutedEventArgs e)
        {

        }

        private void browseBtn_CLick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Ebook Files(*.pdf, *.PDF, *.epub, *.EPUB)| *.pdf; *.PDF;*.epub; *.EPUB";
            bool? dialogResult = openFileDialog.ShowDialog(this);
            if (dialogResult.Value)
            {
                string filePath = openFileDialog.FileName;
                this.Hide();
                BrowserEvent?.Invoke(filePath,-1);
                this.Close();

            }


        }

        public void CloseMethod(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OpenviaLink_Click(object sender, RoutedEventArgs e)
        {
            WindowScreen x = Application.Current.MainWindow as WindowScreen;
            int index = App.Global.Book_Short_ViewModel.Book_Short.FindIndex(a => a.book_link == linkTextBox.Text);
            if (index >= 0)
            {
                x.OpenDetailScreen(App.Global.Book_Short_ViewModel.Book_Short[index]);
            }
            else
            {
                x.OpenDetailScreen(linkTextBox.Text);
            }

            Close();
        }
    }
}
