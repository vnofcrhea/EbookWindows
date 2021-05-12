using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
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
using EbookWindows.ViewModels;
namespace EbookWindows.Screen
{
    /// <summary>
    /// Interaction logic for DetailScreen.xaml
    /// </summary>
    public partial class DetailScreen : UserControl
    {
        public Root items = new Root();
        public Chapter chapters = new Chapter();
        public int page_numbers = 0;
        public int page_index = 0;
        public int chapter_limit = 50;
        public DetailScreen()
        {
            InitializeComponent(); 
            
        }

        public void LoadData() //Load data here
        {
        }
        public void LoadData(int a) //Load data here

        {
            if (a == 1)
            {
                var json = new WebClient().DownloadString("http://127.0.0.1:5000/api/v2/books?url=https://truyen.tangthuvien.vn/doc-truyen/thai-at");
                items = JsonConvert.DeserializeObject<Root>(json);
                #region //Xác định số trang
                page_numbers = items.chapter_name.Count / chapter_limit + 1;
                page_index = 2;
                #endregion
                LoadPaging(page_index);
                PagePanelReload();
            }
        }
        public void LoadPaging(int page)
        {
            
            lvDataBinding.Items.Clear();
            int count = 0;
            int index_start = (page-1) * chapter_limit;
            int check_count = 0;
            foreach (var item in items.season_name)
            {
                if (items.season_index[count] < index_start)
                {
                    if (count < items.season_name.Count - 1)
                    {
                        if (items.season_index[count + 1] > index_start)
                        {
                            Chapter childItem1 = new Chapter() { Title = item };
                            if (count < items.season_index.Count - 1)
                            {
                                for (int i = index_start; i < items.season_index[count + 1]; i++)
                                {
                                    childItem1.Items.Add(new Chapter { Title = items.chapter_name[i], link = items.chapter_link[i] });
                                    check_count++;
                                    if (check_count >= chapter_limit)
                                        break;
                                }
                            }
                            else
                            {
                                for (int i = index_start; i < items.chapter_name.Count; i++)
                                {
                                    childItem1.Items.Add(new Chapter { Title = items.chapter_name[i], link = items.chapter_link[i] });
                                    check_count++;
                                    if (check_count >= chapter_limit)
                                        break;
                                }
                            }
                            lvDataBinding.Items.Add(childItem1);
                            count++;
                            if (check_count >= chapter_limit)
                                break;
                            continue;
                        }
                        count++;
                        continue;
                    }
                    else
                    {
                        Chapter childItem1 = new Chapter() { Title = item };
                        for (int i = index_start; i < items.chapter_name.Count; i++)
                        {
                            childItem1.Items.Add(new Chapter { Title = items.chapter_name[i], link = items.chapter_link[i] });
                            check_count++;
                            if (check_count >= chapter_limit)
                                break;
                        }
                        lvDataBinding.Items.Add(childItem1);
                        count++;
                        if (check_count >= chapter_limit)
                            break;
                        continue;
                    }
                }
                Chapter childItem = new Chapter() { Title = item };
                if (count < items.season_index.Count - 1)
                {
                    for (int i = items.season_index[count]; i < items.season_index[count + 1]; i++)
                    {
                        childItem.Items.Add(new Chapter { Title = items.chapter_name[i], link = items.chapter_link[i] });
                        check_count++;
                        if (check_count >= chapter_limit)
                            break;
                    }
                }
                else
                {
                    for (int i = items.season_index[count]; i < items.chapter_name.Count; i++)
                    {
                        childItem.Items.Add(new Chapter { Title = items.chapter_name[i], link = items.chapter_link[i] });

                        check_count++;
                        if (check_count >= chapter_limit)
                            break;
                    }
                }
                lvDataBinding.Items.Add(childItem);
                count++;
                if (check_count >= chapter_limit)
                    break;

            }


        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void PageIndex_Click(object sender, RoutedEventArgs e)
        {
            page_index = Int32.Parse((sender as Button).Content.ToString());
            LoadPaging(page_index);
            PagePanelReload();
        }
        public void PagePanelReload()
        {
            Page_Panel.Items.Clear();
            int count = 0;
            var i = page_index - 2;
            if (i <= 1)
                i = 1;
            if(i>page_numbers-5)
            {
                i = i - 4 + (page_numbers-i);
            }    
            for(int j=i; j<=page_numbers;j++)
            {
                if (count >= 5)
                    return;
                Page_Panel.Items.Add(j);
                count++;
            }    
        }

        private void Select_Click(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = (sender as TreeView).SelectedItem as Chapter;
            if (item == null) 
                return;
            if (item.Items.Count > 0)
                return;
            else
            {
                #region Open reading monitor online
                WindowScreen win = (WindowScreen)Window.GetWindow(this);
                win.OpenComicReadingScreen();

                #endregion
                #region Open reading monitor offine

                #endregion
            }
        }
    }

    
}
