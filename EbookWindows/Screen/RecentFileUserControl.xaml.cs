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

        public bool BrowserANewFile(string fileName, string filePath, string fileIcon)
        {
            RecentFile temp = new RecentFile(fileName, filePath, fileIcon);
            if(viewingList.Count() == minItems && viewBtn.Content.Equals(viewMore))
            {
                viewingList.RemoveAt(minItems-1);           
            } else if(viewingList.Count() == maxItems && viewBtn.Content.Equals(viewLess))
            {
                viewingList.RemoveAt(maxItems - 1);
            }
            else{ //do nothing
                  } 
            viewingList.Insert(0, temp);
            recentFileList.Insert(0, temp);
            return true;
        }

        public bool openAFileInRecentFileList(int index)
        {
            viewingList.Insert(0, viewingList[index]);
            viewingList.RemoveAt(index + 1);
            recentFileList.Insert(0, recentFileList[index]);
            recentFileList.RemoveAt(index + 1);
            return true;
        }

        private void LoadData(object sender, RoutedEventArgs e)
        {
            RecentFileDao recentFileDao = new RecentFileDao();
            recentFileList = recentFileDao.GetAll();
             MappingDataFromListToView(minItems);
            recentFileListView.ItemsSource = viewingList;
        }


        private void MappingDataFromListToView(int amount)
        {
            viewingList.Clear();
            foreach (var i in recentFileList.Take(amount).ToList())
            {
                viewingList.Add(i);
            }
            
        }

        private void viewBtn_CLick(object sender, RoutedEventArgs e)
        {
            if (viewBtn.Content.Equals(viewMore))
            {
                MappingDataFromListToView(maxItems);
                viewBtn.Content = viewLess;
            }
            else
            {
                MappingDataFromListToView(minItems);
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
                        home.filePathChanged(viewingList[index].filePath, index);
                        //openAFileInRecentFileList(index);
                    }
                    else if (viewingList[index].fileIcon.Equals(epub))
                    {
                        home.filePathChanged(viewingList[index].filePath, index);
                       // openAFileInRecentFileList(index);
                    }
                    else
                    {
                        //do nothing
                    }
                }
            }
           
        }

        //Write File
        public void SaveRecentFileList()
        {
            RecentFileDao recentFileDao = new RecentFileDao();
            recentFileDao.WriteNewRecentFileData(recentFileList);
        }


    }
}

