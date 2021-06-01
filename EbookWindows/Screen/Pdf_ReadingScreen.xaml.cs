using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;
using Apitron.PDF.Rasterizer.Navigation;
using EbookWindows.ViewModels;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;
using System;
using System.Collections.Generic;
 using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
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

        private PdfLoadedDocument pdfFile;

        private bool changeFile  = false;
        private int recentLocation;
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
            recentLocation = location;
            try
            {
                file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                Document document = new Document(file);
                (this.document).Document = document;
            }catch(Exception e)
            {
                MessageBox.Show("This file cann't open! Please choose other file!");
            }        
            pdfFile = new PdfLoadedDocument(filePath);
            bookmarkListView.ItemsSource = pdfFile.Bookmarks;
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
            if (recentLocation != 0)
            {
                document.Document.Navigator.GoToPage(recentLocation);
                recentLocation = -1;
            }
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
                    break;
                case "Last":
                    navigator.Move(0, Origin.End);
                    break;
                default:
                    return;
            }
            this.destinationRectangle = null;
        }

      
        private void bookmarkBtn_Click(object sender, RoutedEventArgs e)
        {
            changeFile = true;
            int pageIndex = PageConboBox.SelectedIndex;
            //Creates document bookmarks.
            PdfBookmark bookmark = pdfFile.Bookmarks.Add($"Page {pageIndex + 1}");
            // Sets the destination page.
            bookmark.Destination = new PdfDestination(pdfFile.Pages[pageIndex]);
            //Set the page and location of the bookmark
            bookmark.Destination.Location = new PointF(0, 0);
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
            changeFile = true;
            var item = (sender as FrameworkElement).DataContext;
            int index = bookmarkListView.Items.IndexOf(item);
            pdfFile.Bookmarks.RemoveAt(index);
            bookmarkListView.Items.Refresh();
        }

        private int GetPageIndexFormTitle(string title)
        {
            int result = 0;
            string[] stringList = title.Split(' ');
            if(stringList.Count() == 2 && stringList[0].Equals("Page"))
            {
                Int32.TryParse(stringList[1], out result);
                result--;
            }
            else
            {
                result = -1;
            }
            return result;
        }

        private void bookmarkListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {  
            int index = bookmarkListView.SelectedIndex;
            if (index != -1)
            {
                int pageIndex = GetPageIndexFormTitle(pdfFile.Bookmarks[index].Title);
                if(pageIndex == -1)
                {
                    pageIndex = pdfFile.Bookmarks[index].Destination.PageIndex;
                }
                document.Document.Navigator.GoToPage(pageIndex);
                bookmarkListView.SelectedIndex = -1;
            }           
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
                }
                else if (PageScroller.VerticalOffset == 0)
                {
                    document.Document.Navigator.MoveBackward();
                }
            }
        }

        public void homeBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowScreen home = (WindowScreen)Window.GetWindow(this);
            zoomValue = 0.8;
            zoomLabel.Content = $"{zoomValue * 100}%";
            PageImage.Source = null;
            bottom = 0;
            bookmarkBorder.Visibility = Visibility.Collapsed;
            int location = PageConboBox.SelectedIndex;
            home.recentFileUserControl.UpdateLocationOfFile(location);
            file.Close();
            if (changeFile)
            {
                pdfFile.Save();
                changeFile = false;
            }
            pdfFile.Close(true);
            home.ReturnFromReadingScreen_Click(sender, e);
        }

        public void SaveFilePdf()
        {
            file.Close();
            if (changeFile)
            {
                pdfFile.Save();
                pdfFile.Close(true);
            }
        }
    }
}
