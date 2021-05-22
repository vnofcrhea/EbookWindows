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
using System.IO;
using System.ComponentModel;

namespace EbookWindows.Screen
{
    /// <summary>
    /// Interaction logic for DetailScreen.xaml
    /// </summary>
    public partial class DetailScreen : UserControl
    {

        public Chapter chapters = new Chapter();
        public int page_numbers = 0;
        public int page_index = 0;
        public int chapter_limit = 50;
        public DetailScreen()
        {
            InitializeComponent();

        }

        public void LoadData(ShelfTag item) //Load data offine here
        {
            App.book_dir = item.book_dir;
            using (StreamReader file = File.OpenText(item.book_dir + "\\detail.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                App.Items = (Root)serializer.Deserialize(file, typeof(Root));
            }
            page_numbers = App.Items.chapter_name.Count / chapter_limit + 1;
            page_index = 1;
            this.Dispatcher.Invoke(() =>
            {
                LoadPaging(page_index);
                PagePanelReload();
            });
        }
        #region Loading Data Online 
        public void LoadData(string url) //Load data online here
        {
                var json = new WebClient().DownloadString(App.base_url + "/api/books?url="+url);
                App.Items = JsonConvert.DeserializeObject<Root>(json);
                #region //Xác định số trang
                page_numbers = App.Items.chapter_name.Count / chapter_limit + 1;
                page_index = 1;
                #endregion
                this.Dispatcher.Invoke(() =>
                {
                    LoadPaging(page_index);
                    PagePanelReload();
                });
            
        }
        #endregion
        #region LOADING PAGING
        public void LoadPaging(int page)
        {

            lvDataBinding.Items.Clear();
            int count = 0;
            int index_start = (page - 1) * chapter_limit;
            int check_count = 0;
            foreach (var item in App.Items.season_name)
            {
                if (App.Items.season_index[count] < index_start)
                {
                    if (count < App.Items.season_name.Count - 1)
                    {
                        if (App.Items.season_index[count + 1] > index_start)
                        {
                            Chapter childItem1 = new Chapter() { Title = item };
                            if (count < App.Items.season_index.Count - 1)
                            {
                                for (int i = index_start; i < App.Items.season_index[count + 1]; i++)
                                {
                                    childItem1.Items.Add(new Chapter { Title = App.Items.chapter_name[i], link = App.Items.chapter_link[i] });
                                    check_count++;
                                    if (check_count >= chapter_limit)
                                        break;
                                }
                            }
                            else
                            {
                                for (int i = index_start; i < App.Items.chapter_name.Count; i++)
                                {
                                    childItem1.Items.Add(new Chapter { Title = App.Items.chapter_name[i], link = App.Items.chapter_link[i] });
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
                        for (int i = index_start; i < App.Items.chapter_name.Count; i++)
                        {
                            childItem1.Items.Add(new Chapter { Title = App.Items.chapter_name[i], link = App.Items.chapter_link[i] });
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
                if (count < App.Items.season_index.Count - 1)
                {
                    for (int i = App.Items.season_index[count]; i < App.Items.season_index[count + 1]; i++)
                    {
                        childItem.Items.Add(new Chapter { Title = App.Items.chapter_name[i], link = App.Items.chapter_link[i] });
                        check_count++;
                        if (check_count >= chapter_limit)
                            break;
                    }
                }
                else
                {
                    for (int i = App.Items.season_index[count]; i < App.Items.chapter_name.Count; i++)
                    {
                        childItem.Items.Add(new Chapter { Title = App.Items.chapter_name[i], link = App.Items.chapter_link[i] });

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
        #endregion
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
            int i;
            if (page_numbers <= 5)
            {
                i = 0;
            }
            else
            {
                i = page_index - 2;
                if (i <= 1)
                    i = 1;
                if (i > page_numbers - 5)
                {
                    i = i - 4 + (page_numbers - i);
                }
            }
            for (int j = i; j <= page_numbers; j++)
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
                App.chapter = item;
                WindowScreen win = (WindowScreen)Window.GetWindow(this);
                win.OpenComicReadingScreen();
            }
        }

        private async void AddToLibrary_Click(object sender, RoutedEventArgs e)
        {

            var path_data = App.path + "\\data\\book" + "\\" + App.Items.source + "\\" + App.Items.book_id;
            #region create path. 
            if (!Directory.Exists(path_data))
            {
                Directory.CreateDirectory(path_data);
            }
            #endregion
            File.WriteAllText(path_data + "\\" + "detail.json", JsonConvert.SerializeObject(App.Items));
            using (WebClient client = new WebClient())
            {
                await Task.Run(()=> { client.DownloadFile(new Uri(App.Items.img_url), path_data + "\\" + "img.jpg"); });
            };
            WindowScreen win = (WindowScreen)Window.GetWindow(this);
            win.LoadShelf();
        }
        public void Download_Content()
        {
            var path_data = App.path + "\\data\\book" + "\\" + App.Items.source + "\\" + App.Items.book_id + "\\content";
            if (!Directory.Exists(path_data))
            {
                Directory.CreateDirectory(path_data);
            }


            Parallel.ForEach(App.Items.chapter_link, new ParallelOptions { MaxDegreeOfParallelism = -1 }, getjsonstring);
            List<Task> TaskList = new List<Task>();
            //foreach (var url in App.Items.chapter_link)
            //{
            //     Task LastTask = new Task(() => {getjsonstring(url, count); });
            //    TaskList.Add(LastTask);
            //    count++;
            //}

            //Task.WaitAll(TaskList.ToArray());
            Console.WriteLine("EndInit");
        }
        public static void getjsonstring(string item)
        { 
            var json = new WebClient().DownloadString(App.base_url + "/api/chapters?url=" + item);
            var count = App.Items.chapter_link.FindIndex(x => x.Contains(item));
            var path_data = App.path + "\\data\\book" + "\\" + App.Items.source + "\\" + App.Items.book_id + "\\content";
            File.WriteAllText(path_data + "\\" + count +".json", json);
            Console.WriteLine(count);
        }

        private void DownloadContent_Click(object sender, RoutedEventArgs e)
        {
            Download_Content();
        }
    }
}
