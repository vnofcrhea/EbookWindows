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
        /// <summary>
        /// Loading data after RecentFileUserControl()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadData(object sender, RoutedEventArgs e)
        {
            RecentFile_ViewModel recentFileDao = new RecentFile_ViewModel();
            recentFileList = recentFileDao.GetAll();
            MappingDataFromListToView(minItems);
            recentFileListView.ItemsSource = viewingList;
        }

        /// <summary>
        /// Open a file from device
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="filePath"></param>
        /// <param name="fileIcon"></param>
        /// <returns></returns>
        public bool BrowserANewFile(string fileName, string filePath, string fileIcon)
        {
            int index = IsFilePathExist(filePath);
            //filepath is not exist in recentFileList
            if (index == -1)
            {
                RecentFile temp = new RecentFile(fileName, filePath, fileIcon);
                if (viewingList.Count() == minItems && viewBtn.Content.Equals(viewMore))
                {
                    viewingList.RemoveAt(minItems - 1);
                }
                else if (viewingList.Count() == maxItems && viewBtn.Content.Equals(viewLess))
                {
                    viewingList.RemoveAt(maxItems - 1);
                }
                else
                { //do nothing
                }
                viewingList.Insert(0, temp);
                recentFileList.Insert(0, temp);
                return true;
            }
            //filepath is exist in recentFileList
            else
            {
                //swap recentfileList[index]
                recentFileList.Insert(0, recentFileList[index]);
                recentFileList.RemoveAt(index + 1);
                //Loading data in recentFileListView again
                if (viewBtn.Content.Equals(viewMore))
                {
                    MappingDataFromListToView(minItems);
                }
                else if (viewBtn.Content.Equals(viewLess))
                {
                    MappingDataFromListToView(maxItems);
                }
                return true;
            }
            //return false;
        }

        /// <summary>
        /// Select a file in recentFileListView
        /// </summary>
        /// <param name="index">index of file was selected</param>
        /// <returns></returns>
        public bool openAFileInRecentFileList(int index)
        {
            viewingList.Insert(0, viewingList[index]);
            viewingList.RemoveAt(index + 1);
            recentFileList.Insert(0, recentFileList[index]);
            recentFileList.RemoveAt(index + 1);
            return true;
        }

        public int GetRecentLocationOfFile(string filePath)
        {
            int location = 0;
            int index = IsFilePathExist(filePath);
            if (index != -1) 
            {
                Int32.TryParse(recentFileList[index].recentLocation, out location);
            }
            return location;
        }

        /// <summary>
        /// Check: filepath is exist in  recentlistList
        /// </summary>
        /// <param name="newfilePath"></param>
        /// <returns>-1: if the filepath isn't exist; index of filepath exist in recentList</returns>
        private int IsFilePathExist(string newfilePath)
        {
            int amount = recentFileList.Count();
            if (recentFileList.Count() > 10)
            {
                amount = 10;
            }
            for (int i = 0; i < amount; ++i)
            {
                if (newfilePath.Equals(recentFileList[i].filePath))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Mapping data from recentFileList to viewingList with amount
        /// </summary>
        /// <param name="amount"></param>
        private void MappingDataFromListToView(int amount)
        {
            viewingList.Clear();
            foreach (var i in recentFileList.Take(amount).ToList())
            {
                viewingList.Add(i);
            }

        }

        public void UpdateLocationOfFile(int location)
        {
            recentFileList[0].recentLocation = location.ToString();

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
            if (index > -1)
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
                        home.filePathChanged(viewingList[index].filePath, index );
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

                recentFileListView.SelectedIndex = -1;
            }

        }

        //Write File
        public void SaveRecentFileList()
        {
            RecentFile_ViewModel recentFileDao = new RecentFile_ViewModel();
            recentFileDao.WriteNewRecentFileData(recentFileList);
        }


    }
}