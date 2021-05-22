using EbookWindows.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private BindingList<RecentFile> recentFileList;
        private BindingList<RecentFile> viewingList = new BindingList<RecentFile>();
        private string viewMore = "View more";
        private string viewLess = "View less";
        private const int minItems = 5;
        private const int maxItems = 10;
        public RecentFileUserControl()
        {

            InitializeComponent();

        }

        private void LoadData(object sender, RoutedEventArgs e)
        {
            RecentFileDao recentFileDao = new RecentFileDao();
            recentFileList = recentFileDao.GetAll();
            foreach (var i in recentFileList.Take(5).ToList())
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
                foreach (var i in recentFileList)
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


    }
}

