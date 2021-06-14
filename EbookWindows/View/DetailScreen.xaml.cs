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
using EbookWindows.Model;

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

        

        public void LoadData(Book_Short item) //Load data offine here
        {
            App.Global.Book_ViewModel.LoadData(item);
            page_numbers = App.Global.Book_ViewModel.bookTotalChapter / chapter_limit + 1;
            page_index = 1;
            this.Dispatcher.Invoke(() =>
            {//Gán DataContext được 
                bookAuthor.Text = App.Global.Book_ViewModel.book_author;
                bookTotalChapter.Text = App.Global.Book_ViewModel.bookTotalChapter.ToString();
                bookDec.Text = App.Global.Book_ViewModel.book_intro;
                bookName.Text = App.Global.Book_ViewModel.book_name;
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(item.book_dir + "\\img.jpg");
                image.EndInit();
                bookImg.Source = image;
                LoadPaging(page_index);
                PagePanelReload();
            });
        }
        #region Loading Data Online 

        public bool LoadData(string url) //Load data online here
        {
            if(!App.Global.Book_ViewModel.LoadData(url))
                return false;
            #region //Xác định số trang
            page_numbers = App.Global.Book_ViewModel.bookTotalChapter / chapter_limit + 1;
            page_index = 1;
            #endregion
            this.Dispatcher.Invoke(() =>
            {//Gán DataContext được 
                bookAuthor.Text = App.Global.Book_ViewModel.book_author;
                bookTotalChapter.Text = App.Global.Book_ViewModel.bookTotalChapter.ToString();
                bookDec.Text = App.Global.Book_ViewModel.book_intro;
                bookName.Text = App.Global.Book_ViewModel.book_name;
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(App.Global.Book_ViewModel.img_url);
                image.EndInit();
                bookImg.Source = image;
                LoadPaging(page_index);
                PagePanelReload();
            });
            return true;
        }



        #endregion
        #region LOADING PAGING
        public void LoadPaging(int page)
        {
            lvDataBinding.Items.Clear();
            int count = 0;
            int index_start = (page - 1) * chapter_limit;
            int check_count = 0;
            foreach (var item in App.Global.Book_ViewModel.season_name)
            {
                if (App.Global.Book_ViewModel.season_index[count] < index_start)
                {
                    if (count < App.Global.Book_ViewModel.season_name.Count - 1)
                    {
                        if (App.Global.Book_ViewModel.season_index[count + 1] > index_start)
                        {
                            Chapter childItem1 = new Chapter() { Title = item };
                            if (count < App.Global.Book_ViewModel.season_index.Count - 1)
                            {
                                for (int i = index_start; i < App.Global.Book_ViewModel.season_index[count + 1]; i++)
                                {
                                    childItem1.Items.Add(new Chapter { Title = App.Global.Book_ViewModel.chapter_name[i], link = App.Global.Book_ViewModel.chapter_link[i],isReaded = App.Global.Book_ViewModel.Bookmark_Chapters.Contains(i) });
                                    check_count++;
                                    if (check_count >= chapter_limit)
                                        break;
                                }
                            }
                            else
                            {
                                for (int i = index_start; i < App.Global.Book_ViewModel.chapter_name.Count; i++)
                                {
                                    childItem1.Items.Add(new Chapter { Title = App.Global.Book_ViewModel.chapter_name[i], link = App.Global.Book_ViewModel.chapter_link[i], isReaded = App.Global.Book_ViewModel.Bookmark_Chapters.Contains(i) });
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
                        for (int i = index_start; i < App.Global.Book_ViewModel.chapter_name.Count; i++)
                        {
                            childItem1.Items.Add(new Chapter { Title = App.Global.Book_ViewModel.chapter_name[i], link = App.Global.Book_ViewModel.chapter_link[i], isReaded = App.Global.Book_ViewModel.Bookmark_Chapters.Contains(i) });
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
                if (count < App.Global.Book_ViewModel.season_index.Count - 1)
                {
                    for (int i = App.Global.Book_ViewModel.season_index[count]; i < App.Global.Book_ViewModel.season_index[count + 1]; i++)
                    {
                        childItem.Items.Add(new Chapter { Title = App.Global.Book_ViewModel.chapter_name[i], link = App.Global.Book_ViewModel.chapter_link[i], isReaded = App.Global.Book_ViewModel.Bookmark_Chapters.Contains(i) });
                        check_count++;
                        if (check_count >= chapter_limit)
                            break;
                    }
                }
                else
                {
                    for (int i = App.Global.Book_ViewModel.season_index[count]; i < App.Global.Book_ViewModel.chapter_name.Count; i++)
                    {
                        childItem.Items.Add(new Chapter { Title = App.Global.Book_ViewModel.chapter_name[i], link = App.Global.Book_ViewModel.chapter_link[i], isReaded = App.Global.Book_ViewModel.Bookmark_Chapters.Contains(i) });

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
            if (page_index == 1)
            {
                firstpagebtn.IsEnabled = false;
                prepagebtn.IsEnabled = false;
                if (page_numbers == 1)
                {
                    nextpagebtn.IsEnabled = false;
                    lastpagebtn.IsEnabled = false;
                    return;
                }
                nextpagebtn.IsEnabled = true;
                lastpagebtn.IsEnabled = true;
            }
            else if (page_index == page_numbers)
            {
                nextpagebtn.IsEnabled = false;
                lastpagebtn.IsEnabled = false;
                firstpagebtn.IsEnabled = true;
                prepagebtn.IsEnabled = true;
            }
            else
            {
                nextpagebtn.IsEnabled = true;
                lastpagebtn.IsEnabled = true;
                firstpagebtn.IsEnabled = true;
                prepagebtn.IsEnabled = true;
            }
        }

        private void Select_Click(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!((sender as TreeView).SelectedItem is Chapter item))
                return;
            if (item.Items.Count > 0)
                return;
            else
            {
                App.Global.Chapter_ViewModel.Current_Chapter = item;
                if (!item.isReaded)
                {
                    ((sender as TreeView).SelectedItem as Chapter).isReaded = true;
                    (sender as TreeView).Items.Refresh();
                    App.Global.Book_ViewModel.Update_ChapterOpened(item);
                }
                WindowScreen win = (WindowScreen)Window.GetWindow(this);
                win.OpenComicReadingScreen();
            }
        }

        private async void AddToLibrary_Click(object sender, RoutedEventArgs e)
        {
            #region old code
            //var path_data = App.Global.Directory_Folder + "\\data\\book" + "\\" + App.Global.Items.source + "\\" + App.Global.Items.book_id;
            //#region create path. 
            //if (!Directory.Exists(path_data))
            //{
            //    Directory.CreateDirectory(path_data);
            //}
            //#endregion
            //File.WriteAllText(path_data + "\\" + "detail.json", JsonConvert.SerializeObject(App.Global.Items));
            //using (WebClient client = new WebClient())
            //{
            //    await Task.Run(()=> { client.DownloadFile(new Uri(App.Global.Items.img_url), path_data + "\\" + "img.jpg"); });
            //};
            #endregion
            await Task.Run(() => App.Global.Book_ViewModel.AddToLibrary());
            (App.Current.MainWindow as WindowScreen).LoadShelf();
            (App.Current.MainWindow as WindowScreen).LoadTreeViewList();
        }
        private async void DeleteBook_Click(object sender, RoutedEventArgs e)
        {
            //WindowScreen win = (WindowScreen)Window.GetWindow(this);
            //Remove book out of Book_Short.

            //Reload List
            await Task.Run(() => { App.Global.Book_Short_ViewModel.Book_Short.RemoveAll(x => x.book_dir == App.Global.Book_Directory); });
            //(App.Current.MainWindow as WindowScreen).TreeView_BookList.Items.Clear();
            //Task.Run(() => (App.Current.MainWindow as WindowScreen).LoadShelf());

            (App.Current.MainWindow as WindowScreen).LoadTreeViewList();
            (App.Current.MainWindow as WindowScreen).LoadShelf();
            //Remove List
            try
            {
                bookImg.Source = null;
                if (Directory.Exists(App.Global.Book_Directory));
                Directory.Delete(App.Global.Book_Directory, true);
            }
            catch (Exception i)
            {

            }
             (App.Current.MainWindow as WindowScreen).MainGrid.Visibility = Visibility.Visible;
            (App.Current.MainWindow as WindowScreen).detailScreen.Visibility = Visibility.Collapsed;
        }
        private async void DownloadContent_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => App.Global.Book_ViewModel.Download_Content());
        }
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            page_index++;
            LoadPaging(page_index);
            PagePanelReload();
        }
        private void LastPage_Click(object sender, RoutedEventArgs e)
        {
            page_index = page_numbers;
            LoadPaging(page_index);
            PagePanelReload();
        }
        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            page_index--;
            LoadPaging(page_index);
            PagePanelReload();
        }
        private void FirstPage_Click(object sender, RoutedEventArgs e)
        {
            page_index = 1;
            LoadPaging(page_index);
            PagePanelReload();
        }
        private void ReadFirstChapter_Click(object sender, RoutedEventArgs e)
        {
            App.Global.Chapter_ViewModel.Current_Chapter = new Chapter() { link = App.Global.Book_ViewModel.Book.chapter_link[0] };
            WindowScreen win = (WindowScreen)Window.GetWindow(this);
            win.OpenComicReadingScreen();
        }

        private void ReadLastChapter_Click(object sender, RoutedEventArgs e)
        {
            App.Global.Chapter_ViewModel.Current_Chapter = new Chapter() { link = App.Global.Book_ViewModel.Book.chapter_link.Last() };
            WindowScreen win = (WindowScreen)Window.GetWindow(this);
            win.OpenComicReadingScreen();
        }

        
    }
}
