using EbookWindows.ViewModels;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EbookWindows.Screen
{
    /// <summary>
    /// Interaction logic for RecentFileUserControl.xaml
    /// </summary>
    public partial class RecentFileUserControl : UserControl
    {
        private BindingList<RecentFile> recentFileList = new BindingList<RecentFile>();
        private BindingList<RecentFile> viewingList = new BindingList<RecentFile>();
        private string viewMore = "View more";
        private string viewLess = "View less";
        private const int minItems = 5;
        private const int maxItems = 10;
        private const string pdf = "Icon\\pdf.png";
        private const string epub = "Icon\\epub.png";
        public RecentFileUserControl()
        {

            InitializeComponent();

        }

      

        private void LoadData(object sender, RoutedEventArgs e)
        {
            RecentFileDao recentFileDao = new RecentFileDao();
            recentFileList = recentFileDao.GetAll();
            foreach (var i in recentFileList.Take(minItems).ToList())
            {
                viewingList.Add(i);
            }
            recentFileListView.ItemsSource = viewingList;
        }


        private void viewBtn_CLick(object sender, RoutedEventArgs e)
        {
            if (viewBtn.Content.Equals(viewMore))
            {       
                viewingList.Clear();
                foreach (var i in recentFileList.Take(maxItems).ToList())
                {
                    viewingList.Add(i);
                }
                viewBtn.Content = viewLess;
            }
            else
            {
                viewingList.Clear();
                foreach (var i in recentFileList.Take(minItems).ToList())
                {

                    viewingList.Add(i);
                }
                viewBtn.Content = viewMore;
            }

        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;
            int index = recentFileListView.Items.IndexOf(item);
            //MessageBox.Show(index.ToString());
            viewingList.RemoveAt(index);
            recentFileList.RemoveAt(index);

        }

        private void recentFileListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = recentFileListView.SelectedIndex;
            //MessageBox.Show(index.ToString());
            if(index > -1)
            {
                if (!File.Exists(viewingList[index].filePath))
                {
                    MessageBox.Show($"The file does not exist\n{viewingList[index].filePath}", "Problem Occurred", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    WindowScreen home = (WindowScreen)Window.GetWindow(this);
                    if (viewingList[index].fileIcon.Equals(pdf))
                    {
                        home.filePathChanged(viewingList[index].filePath);
                        //openAFileInRecentFileList(index);
                    }
                    else if (viewingList[index].fileIcon.Equals(epub))
                    {
                        home.filePathChanged(viewingList[index].filePath);
                       // openAFileInRecentFileList(index);
                    }
                    else
                    {
                        //do nothing
                    }
                }
            }
           
        }
    }
}

