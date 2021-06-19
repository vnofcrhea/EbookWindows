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
        private List<RecentFile> viewingList = new List<RecentFile>();
        private string viewMore = "View more";
        private string viewLess = "View less";
        private const int minItems = 5;
        private const int maxItems = 10;
        private const string pdf = "\\Icon\\pdf.png";
        private const string epub = "\\Icon\\epub.png";
        public RecentFileUserControl()
        {

            InitializeComponent();
            recentFileListView.ItemsSource = viewingList;
            recentFileListView.Items.Refresh();
        }
        /// <summary>
        /// Loading data after RecentFileUserControl()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadData(object sender, RoutedEventArgs e)
        {
            MappingDataFromListToView(minItems);
            recentFileListView.ItemsSource = viewingList;
            recentFileListView.Items.Refresh();
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
                //viewingList.Insert(0, temp);
                App.Global.RecentFile_ViewModel.Recent_File.Insert(0, new RecentFile(fileName, filePath, fileIcon));
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
            //filepath is exist in recentFileList
            else
            {
                //swap recentfileList[index]
                App.Global.RecentFile_ViewModel.Recent_File.Insert(0, App.Global.RecentFile_ViewModel.Recent_File[index]);
                App.Global.RecentFile_ViewModel.Recent_File.RemoveAt(index + 1);
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
            App.Global.RecentFile_ViewModel.Recent_File.Insert(0, App.Global.RecentFile_ViewModel.Recent_File[index]);
            App.Global.RecentFile_ViewModel.Recent_File.RemoveAt(index + 1);
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

        public int GetRecentLocationOfFile(string filePath)
        {
            int location = 0;
            int index = IsFilePathExist(filePath);
            if (index != -1) 
            {
               location = App.Global.RecentFile_ViewModel.Recent_File[index].recentLocation;
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
            return App.Global.RecentFile_ViewModel.Recent_File.FindIndex(e => e.filePath == newfilePath);
        }
        
        /// <summary>
        /// Mapping data from recentFileList to viewingList with amount
        /// </summary>
        /// <param name="amount"></param>
        private void MappingDataFromListToView(int amount)
        {
            viewingList.Clear();
            viewingList.AddRange(App.Global.RecentFile_ViewModel.Recent_File.Take(amount).ToList());
            recentFileListView.Items.Refresh();
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
            var item = (sender as Button).DataContext;
            int index = recentFileListView.Items.IndexOf(item);

            //MessageBox.Show(index.ToString());
            viewingList.RemoveAt(index);
            recentFileListView.Items.Refresh();
            App.Global.RecentFile_ViewModel.Recent_File.RemoveAt(index);
            App.Global.RecentFile_ViewModel.Save_File();
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


    }
}