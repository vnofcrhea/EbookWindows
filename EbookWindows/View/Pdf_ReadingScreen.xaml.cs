using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;
using Apitron.PDF.Rasterizer.Navigation;
using Apitron.PDF.Rasterizer.Security;
using EbookWindows.View;
using EbookWindows.ViewModels;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Threading;
using Page = Apitron.PDF.Rasterizer.Page;
using Rectangle = Apitron.PDF.Rasterizer.Rectangle;

namespace EbookWindows.Screen
{
    /// <summary>
    /// Interaction logic for pdfDemo.xaml
    /// </summary>
    public partial class Pdf_ReadingScreen : UserControl
    {
        #region Attributes
        private int bottom = 0;

        public FileStream file;
        private double zoomValue { get; set; }

        private delegate void SetImageSourceDelegate(byte[] source, IList<Link> links, int width, int height);

        public pdfDocument document = null; //Apatron.PDF.Rasterrizer

        private Task task;

        private int GlobalScale { get; set; }  = 2 ;

        private Rectangle destinationRectangle;

        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        private TimeSpan SpanTime;

        private PdfLoadedDocument pdfFileBookmark; //Syncfusion

        private bool changeFile = false;

        private int recentLocation;

        private string filePath;

        private ObservableCollection<PdfBookmark> bookmarkList = null;
        private ObservableCollection<Bookmark> toc = null;
        #endregion

        public Pdf_ReadingScreen()
        {
            zoomValue = 0.8;
            
            InitializeComponent();
            zoomLabel.Content = $"{zoomValue * 100}%";
            bookmarkList = new ObservableCollection<PdfBookmark>();
            toc = new ObservableCollection<Bookmark>();
        }  

        /// <summary>
        /// Load data with a file path
        /// </summary>
        /// <param name="filePath">the file path need to read</param>
        public bool ReadFilePdf(string filePath, int location) //Load data here
        {//WithoutPassword
            bool hasPassword = false;
            this.document = new pdfDocument();
            this.DataContext = this.document;
            this.document.PropertyChanged += DocumentOnPropertyChanged;
            this.filePath = filePath;
            recentLocation = location;
            file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
          
            try
            {
               
                Document document = new Document(file);
                (this.document).Document = document;
                pdfFileBookmark = new PdfLoadedDocument(filePath);              
            }
            catch (Apitron.PDF.Rasterizer.ErrorHandling.DocumentLoadException)
            {
                file.Close();
                hasPassword = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Sorry something went wrong in Reader+ can't open this file", "Error", MessageBoxButton.OK,MessageBoxImage.Error);
                return false;
            }

            if (hasPassword)
            {
                //A file has password
                var passwordScreen = new PasswordScreen();
                passwordScreen.EnterPasswordEvent += checkPassword;
                passwordScreen.Owner = App.Current.MainWindow as WindowScreen;
                passwordScreen.ShowDialog();

            }
            if (document.Document == null)
            {
                return false;
            }
            ClassifyBookmarks();
            bookmarkListView.ItemsSource = bookmarkList;
            TOCTree.ItemsSource = toc;
            return true;
        }
       
        /// <summary>
        /// Check: Is password string valid?
        /// </summary>
        /// <param name="passwordStr">Password</param>
        /// <returns>true: valid; false: invalid</returns>
        private bool checkPassword(string passwordStr)
        {
            
            try
            {
                file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                Document document = new Document(file, passwordStr);
                (this.document).Document = document;
                pdfFileBookmark = new PdfLoadedDocument(filePath, passwordStr);
   
                bookmarkListView.ItemsSource = pdfFileBookmark.Bookmarks;
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid password. Please enter password again", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void ClassifyBookmarks()
        {

            for (int i = 0; i < pdfFileBookmark.Bookmarks.Count; i++)
            {
              
                if (pdfFileBookmark.Bookmarks[i].Destination != null && pdfFileBookmark.Bookmarks[i].Title.IndexOf("Page ") == 0)
                {
                    bookmarkList.Add(pdfFileBookmark.Bookmarks[i]);

                }
                else
                {
                    toc.Add(document.Document.Bookmarks.Children[i]);
                }

            }
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
            try
            {
                Page currentPage = this.document.Page;
                int desiredWidth = (int)currentPage.Width * GlobalScale;
                int desiredHeight = (int)currentPage.Height * GlobalScale;
                byte[] image = currentPage.RenderAsBytes(desiredWidth, desiredHeight, new RenderingSettings());
                IList<Link> links = currentPage.Links;
                SetImageSourceDelegate del = this.SetImageSource; //add event into delegate
                this.Dispatcher.Invoke(del, image, links, desiredWidth, desiredHeight); //execute SetImageSource
            }
            catch (Exception)
            {

            }
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
                //this.PageScroller.ScrollToTop();
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
            if(link.DestinationUri != null)
            {
                string linkStr = link.DestinationUri.ToString();
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to continue connecting \n{linkStr}", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    Process.Start(new ProcessStartInfo(linkStr));
                    routedEventArgs.Handled = true;
                }
                else if (result == MessageBoxResult.No)
                {
                    //nothing
                }
            }
            else
            {
                this.document.Document.Navigator.GoToLink(link);

                this.destinationRectangle = link.GetDestinationRectangle((int)(this.document.Page.Width * this.GlobalScale), (int)(this.document.Page.Height * this.GlobalScale), null);

            }

        }

        /// <summary>
        /// On zoom 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnZoomChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.UpdateImageZoom(); //loading: Minium & Value
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
                this.MouseMove += StackPanel_MouseMove;
            }
            else
            {
                ((sender as Button).Content as MaterialDesignThemes.Wpf.PackIcon).Kind = MaterialDesignThemes.Wpf.PackIconKind.EyeOff;
                BottomPanelTool.Visibility = Visibility.Collapsed;
                TopPanelTool.Visibility = Visibility.Collapsed;
                this.MouseMove -= StackPanel_MouseMove;
            }
        }

        private void ZoomInBtn_Click(object sender, RoutedEventArgs e)
        {
            ZoomOutBtn.IsEnabled = true;
            zoomValue += 0.1;
            if (zoomValue > 2)
            {
                ZoomInBtn.IsEnabled = false;
                           
            }

            this.UpdateImageZoom();
            zoomLabel.Content = $"{zoomValue * 100}%";
        }

        private void ZoomOutBtn_Click(object sender, RoutedEventArgs e)
        {
            ZoomInBtn.IsEnabled = true;
            zoomValue -= 0.1;
            if (zoomValue < 0.2)
            {
                
                ZoomOutBtn.IsEnabled = false;
            }

            this.UpdateImageZoom();
            zoomLabel.Content = $"{zoomValue * 100}%";
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
            changeFile = true; //should fix
            int pageIndex = PageConboBox.SelectedIndex;
            //Creates document bookmarks.
            PdfBookmark bookmark = pdfFileBookmark.Bookmarks.Add($"Page {pageIndex + 1}");
            // Sets the destination page.
            bookmark.Destination = new PdfDestination(pdfFileBookmark.Pages[pageIndex]);
            //Set the page and location of the bookmark
            bookmark.Destination.Location = new PointF(0, 0);
            bookmarkList.Add(bookmark);
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
            changeFile = true; //should fix
           var item = (sender as FrameworkElement).DataContext;
           int index = bookmarkListView.Items.IndexOf(item);
           string stringtitle = bookmarkList[index].Title;
           int count = -1;
           foreach(PdfBookmark itembookmark in pdfFileBookmark.Bookmarks)
           {
                count++;
                if(itembookmark is PdfBookmark && (itembookmark as PdfBookmark).Title.Equals(stringtitle))
                {
                    pdfFileBookmark.Bookmarks.RemoveAt(count);
                    bookmarkList.RemoveAt(index);
                    return;
                }    
           }    
           
        }

        private int GetPageIndexFormTitle(string title)
        {
            int result = 0;
            string[] stringList = title.Split(' ');
            if (stringList.Count() == 2 && stringList[0].Equals("Page"))
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

                int pageIndex = GetPageIndexFormTitle(bookmarkList[index].Title);
                if (pageIndex == -1)
                {
                    pageIndex = bookmarkList[index].Destination.PageIndex;
                }
                document.Document.Navigator.GoToPage(pageIndex);

            }
        }
    
        private int top = 0;
        private void PageScroller_Changed(object sender, ScrollChangedEventArgs e)
        {
            if (PageImage.Source != null)
            {
                if (PageScroller.VerticalOffset == PageScroller.ScrollableHeight)
                {
                    top = 0;
                    if (bottom < 3)
                    {
                        bottom++;
                        this.PageScroller.ScrollToVerticalOffset(PageScroller.ScrollableHeight - 0.01);
                        return;
                    }
                    else if(PageScroller.ScrollableHeight != 0)
                    {
                        document.Document.Navigator.MoveForward();
                        bottom = 0;
                    }
                }
                else if (PageScroller.VerticalOffset == 0)
                {
                    bottom = 0;
                    if (top < 3)
                    {
                        top++;
                        this.PageScroller.ScrollToVerticalOffset(0.01);
                        return;
                    }    
                    document.Document.Navigator.MoveBackward();
                    this.PageScroller.ScrollToVerticalOffset(0.01);
                    top = 0;
                    
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
            //hide TOC & bookmark
            bookmarkBorder.Visibility = Visibility.Collapsed;
            TOCBorder.Visibility = Visibility.Collapsed;
            //clear TOC & bookmark
            bookmarkList.Clear();           
            toc.Clear();
            file.Close();
            if (changeFile)
            {
                try
                {
                    pdfFileBookmark.Save();
                }
                catch (Exception)
                {
                    MessageBox.Show("Cannot save changes because the file is open in another program", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                changeFile = false;
            }
            if(pdfFileBookmark != null)
            {
                pdfFileBookmark.Close(true);
            }
         
            home.ReturnFromReadingScreen_Click(sender, e);
        }

        public void SaveFilePdf()
        {
            file.Close();
            if (changeFile)
            {
                try
                {
                    pdfFileBookmark.Save();
                }
                catch (Exception)
                {
                    MessageBox.Show("Cannot save changes because the file is open in another program", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                
            }
            pdfFileBookmark.Close(true);
        }

        private void keyDown_Test(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                document.Document.Navigator.MoveForward();
            } else if(e.Key == Key.Left){
                document.Document.Navigator.MoveBackward();
            }
            
        }

        private void PageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = (sender as ComboBox).SelectedIndex;
            App.Global.RecentFile_ViewModel.Recent_File[0].recentLocation = index;
        }

        private void TOCSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Bookmark newValue = (Bookmark)e.NewValue;
            if (newValue != null)
            {
                document.Document.Navigator.GoToBookmark(newValue);
                this.destinationRectangle = newValue.GetDestinationRectangle((int)(this.document.Page.Width * this.GlobalScale), (int)(this.document.Page.Height * this.GlobalScale), null);
            }
        }
        
      
        private void ShowContentOfTable_Click(object sender, RoutedEventArgs e)
        {
            TOCBorder.Visibility = Visibility.Visible;
        }

        private void closeTOCBtn_Click(object sender, RoutedEventArgs e)
        {
            TOCBorder.Visibility = Visibility.Collapsed;
        }
    }
   
}