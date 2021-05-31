using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;
using Apitron.PDF.Rasterizer.Navigation;
using EbookWindows.ViewModels;
//using Spire.Pdf;
//using Spire.Pdf.Bookmarks;
//using Spire.Pdf.General;
using System;
using System.Collections.Generic;
//using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Shapes;
using Page = Apitron.PDF.Rasterizer.Page;
using Rectangle = Apitron.PDF.Rasterizer.Rectangle;

namespace EbookWindows.Screen
{
    /// <summary>
    /// Interaction logic for pdfDemo.xaml
    /// </summary>
    public partial class Pdf_ReadingScreen : UserControl
    {
        //private int currentPage { get; set; }
        //private ObservableCollection<int> bookmarks { get; set; }
        private int bottom = 0;

        public FileStream file;
        private double zoomValue { get; set; }

        private delegate void SetImageSourceDelegate(byte[] source, IList<Link> links, int width, int height);

        public pdfDocumentViewModel document = null;

        private Task task;

        private int GlobalScale = 2;

        private Rectangle destinationRectangle;

        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        private TimeSpan SpanTime;

        //PdfDocument pdf = new PdfDocument();
        //string tempFile;

        public Pdf_ReadingScreen()
        {
            zoomValue = 0.8;
            InitializeComponent();
            this.document = new pdfDocumentViewModel();
            this.DataContext = this.document;
            this.document.PropertyChanged += DocumentOnPropertyChanged;
        }

        /// <summary>
        /// Load data with a file path
        /// </summary>
        /// <param name="filePath">the file path need to read</param>
        public void LoadData(string filePath, int location) //Load data here
        {
            //tempFile = filePath;
            file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            Document document = new Document(file);
            (this.document).Document = document;
            if(location != 0)
            {
                document.Navigator.GoToPage(location);
            }
           // pdf.LoadFromFile(filePath);
            //bookmarkListView.ItemsSource = pdf.Bookmarks;
        }

        private void DocumentOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "Page")
            {
                task = new Task(UpdatePageView);
                task.Start();
            }
        }
        private void UpdatePageView()
        {
            Page currentPage = this.document.Page;
            int desiredWidth = (int)currentPage.Width * GlobalScale;
            int desiredHeight = (int)currentPage.Height * GlobalScale;
            byte[] image = currentPage.RenderAsBytes(desiredWidth, desiredHeight, new RenderingSettings());
            IList<Link> links = currentPage.Links;

            SetImageSourceDelegate del = this.SetImageSource; //gán sự kiện vào hàm delegate
            this.Dispatcher.Invoke(del, image, links, desiredWidth, desiredHeight); //thực hiện hàm SetImageSource
        }

        private void SetImageSource(byte[] image, IList<Link> links, int width, int height)
        {
            BitmapSource source = BitmapSource.Create(width, height, 72, 72, PixelFormats.Bgr32, null, image, 4 * width);
            this.PageImage.Source = source;
            this.PageImage.Width = width;
            this.PageImage.Height = height;

            this.PageCanvas.Width = width;
            this.PageCanvas.Height = height;

            for (int i = 1; i < this.PageCanvas.Children.Count; i++)
            {
                Button rectangle = (Button)this.PageCanvas.Children[i];
                rectangle.Click -= this.OnLinkClick;
            }

            this.PageCanvas.Children.RemoveRange(1, this.PageCanvas.Children.Count);

            foreach (Link link in links)
            {
                Apitron.PDF.Rasterizer.Rectangle location = link.GetLocationRectangle(width, height, null);
                Button rectangle = new Button();
                rectangle.Opacity = 0;
                rectangle.Cursor = Cursors.Hand;
                rectangle.Width = location.Right - location.Left;
                rectangle.Height = location.Top - location.Bottom;
                Canvas.SetLeft(rectangle, location.Left);
                Canvas.SetBottom(rectangle, location.Bottom);
                rectangle.Click += this.OnLinkClick;
                rectangle.DataContext = link;
                this.PageCanvas.Children.Add(rectangle);
            }

            this.UpdateImageZoom();
            this.UpdateViewLocation(this.destinationRectangle);
            
            //file.Close();
            ////Ý tưởng bookmark 1
            //Document doc = document.Document;
            //DocumentNavigator navigator = doc == null ? null : doc.Navigator;
            //navigator.Move(2, Origin.Begin);
        }
        private void OnBookmarkSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //Bookmark bookmark = new Bookmark();


            Bookmark newValue = (Bookmark)e.NewValue;
            Page currentPage = this.document.Page;
            //document.Bookmark.Children.
           
            if (newValue != null)
            {
                //document.Document.Navigator.GoToPage(8);
                document.Document.Navigator.GoToBookmark(newValue);
                this.destinationRectangle = newValue.GetDestinationRectangle((int)(this.document.Page.Width * this.GlobalScale), (int)(this.document.Page.Height * this.GlobalScale), null);
            }
        }


        /// <summary>
        /// Updates the view location when choose a bookmark
        /// </summary>
        /// <param name="destinationInfo">The destination info.</param>
        private void UpdateViewLocation(Apitron.PDF.Rasterizer.Rectangle destinationInfo)
        {
            if (destinationInfo == null)
            {
                //this.pageScroller.ScrollToTop();
                this.PageScroller.ScrollToVerticalOffset(0.1);
                return;
            }
            double value = zoomValue;
            double scale = value;

            double horizontalScale = this.PageScroller.ViewportWidth / this.PageImage.ActualWidth;
            double verticalScale = this.PageScroller.ViewportHeight / this.PageImage.ActualHeight;

            if (destinationInfo.Bottom != 0 && destinationInfo.Right != this.PageImage.ActualWidth)
            {
                double expectedHScale = this.PageScroller.ViewportWidth / destinationInfo.Height;
                double expectidVScale = this.PageScroller.ViewportHeight / destinationInfo.Width;
                horizontalScale = expectedHScale;
                verticalScale = expectidVScale;

                scale = Math.Min(verticalScale, horizontalScale);

            }

            this.PageScroller.ScrollToHorizontalOffset(destinationInfo.Left * scale);
            this.PageScroller.ScrollToVerticalOffset((this.PageImage.ActualHeight - destinationInfo.Top) * scale);

        }

        /// <summary>
        /// Click link in a page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private void OnLinkClick(object sender, RoutedEventArgs routedEventArgs)
        {
            Link link = (Link)((Button)sender).DataContext;
            this.document.Document.Navigator.GoToLink(link);

            this.destinationRectangle = link.GetDestinationRectangle((int)(this.document.Page.Width * this.GlobalScale), (int)(this.document.Page.Height * this.GlobalScale), null);
        }

        /// <summary>
        /// On zoom 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnZoomChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.UpdateImageZoom(); //Khởi động chạy 2 lần: Thay đổi ở Minium và Value
        }

        /// <summary>
        /// Imagezoom was changed
        /// </summary>
        private void UpdateImageZoom()
        {

            Canvas imageContainer = this.PageCanvas;
            if (imageContainer != null && zoomLabel.Content != null && this.document != null)
            {
                double scale = zoomValue; //lấy giá trị zoom
                imageContainer.LayoutTransform = new ScaleTransform(scale, scale);
            }
        }


        //Nhat develop for reading screen
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


        private void ZoomInBtn_Click(object sender, RoutedEventArgs e)
        {
            if (zoomValue < 2)
            {
                zoomValue += 0.1;
                this.UpdateImageZoom();
                zoomLabel.Content = $"{zoomValue * 100}%";
            }
        }

        private void ZoomOutBtn_Click(object sender, RoutedEventArgs e)
        {
            if (zoomValue > 0.2)
            {
                zoomValue -= 0.1;
                this.UpdateImageZoom();
                zoomLabel.Content = $"{zoomValue * 100}%";
            }
        }

        private void OnNavigationButtonClick(object sender, RoutedEventArgs e)
        {
            Button source = (Button)e.Source;
            Document doc = document.Document;
            DocumentNavigator navigator = doc == null ? null : doc.Navigator;
            if (doc == null || navigator == null)
            {
                return;
            }
            switch ((string)source.CommandParameter)
            {
                case "Next":
                    navigator.MoveForward();
                    break;
                case "Prev":
                    navigator.MoveBackward();
                    break;
                case "First":
                    navigator.Move(0, Origin.Begin);
                    //navigator.Move(2, Origin.Begin); => 3rd page
                    break;
                case "Last":
                    navigator.Move(0, Origin.End);
                    break;
                default:
                    return;
            }
            this.destinationRectangle = null;
        }

        public void homeBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowScreen home = (WindowScreen)Window.GetWindow(this);
            zoomValue = 0.8;
            zoomLabel.Content = $"{zoomValue * 100}%";
            PageImage.Source = null;
            bottom = 0;
            int location = PageConboBox.SelectedIndex;
            home.recentFileUserControl.UpdateLocationOfFile(location);
            file.Close();

            //FileStream newSt = new FileStream(tempFile, FileMode.Open, FileAccess.Write);
            //pdf.SaveToFile(filePath);
            //pdf.SaveToStream(newSt);S
            //pdf.SaveToFile(tempFile);
            //pdf.Close();
            //newSt.Close();


            home.ReturnFromReadingScreen_Click(sender, e);
        }

        private void bookmarkBtn_Click(object sender, RoutedEventArgs e)
        {
            int pageIndex = PageConboBox.SelectedIndex;
            //MessageBox.Show(pageIndex.ToString());
            //pdf.Bookmarks.Add(string.Format($"Page {pageIndex+1}"));
            //PdfBookmark bookmark = pdf.Bookmarks.Insert(2, "New Chapter 3");

            //Set the page and location of the bookmark
            bookmarkListView.Items.Refresh();
        }
        private void bookmarkListBtn_Click(object sender, RoutedEventArgs e)
        {
            bookmarkBorder.Visibility = Visibility.Visible;           
        }


        private void closeBookmarkBtn_Click(object sender, RoutedEventArgs e)
        {
            bookmarkBorder.Visibility = Visibility.Collapsed;
        }

        private void bookmarkDeleteButtons_Click(object sender, RoutedEventArgs e)
        {
            //bookmarkListView.Items.Refresh();
        }

        private void bookmarkListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void PageScroller_Changed(object sender, ScrollChangedEventArgs e)
        {
            if (PageImage.Source != null)
            {
                if (PageScroller.VerticalOffset == PageScroller.ScrollableHeight)
                {
                    if (bottom == 0)
                    {
                        bottom = 1;
                        this.PageScroller.ScrollToVerticalOffset(PageScroller.ScrollableHeight - 0.1);
                    }

                    else
                    {
                        document.Document.Navigator.MoveForward();
                        bottom = 0;
                    }

                    //MessageBox.Show("lan 2");
                }
                else if (PageScroller.VerticalOffset == 0)
                {
                    document.Document.Navigator.MoveBackward();
                }
            }


        }

       

        //private void Arrow_KeyDown(object sender, KeyEventArgs e)
        //{
        //document.Document.Navigator.MoveForward();
        //    if(e.Key == Key.Right)
        //    {
        //        document.Document.Navigator.MoveForward();
        //    }
        //}
    }
}
