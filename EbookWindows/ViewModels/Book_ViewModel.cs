using EbookWindows.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EbookWindows.ViewModels
{
    public class Book_ViewModel
    {
        private Book _Book;
        public Book_ViewModel()
        {
            _Book = new Book();
        }
        public void LoadData(Book_Short item) //Load data offine here
        {
            App.Global.Book_Directory = item.book_dir;
            using (StreamReader file = File.OpenText(item.book_dir + "\\detail.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                _Book = (Book)serializer.Deserialize(file, typeof(Book));
            }
        }

        public void LoadData(string url) //Load data online here
        {
            var json = new WebClient().DownloadString(App.Global.API_URL_Primary + "/api/books?url=" + url);
            _Book = JsonConvert.DeserializeObject<Book>(json);
        }

        public void Download_Content()
        {
            var path_data = App.Global.Directory_Folder + "\\data\\book" + "\\" + _Book.source + "\\" + _Book.book_id + "\\content";
            if (!Directory.Exists(path_data))
            {
                Directory.CreateDirectory(path_data);
            }
            Parallel.ForEach(_Book.chapter_link, item =>
            {

                var priviousePrio = Thread.CurrentThread.Priority;
                // Set your desired priority
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;

                Download_Content_OneChaper(item);

                //Reset priviouse priority of the TPL Thread
                Thread.CurrentThread.Priority = priviousePrio;
            });
            List<Task> TaskList = new List<Task>();
            //Console.WriteLine("EndInit");
        }

        public void AddToLibrary()
        {

            var path_data = App.Global.Directory_Folder + "\\data\\book" + "\\" + _Book.source + "\\" + _Book.book_id;
            #region create path. 
            if (!Directory.Exists(path_data))
            {
                Directory.CreateDirectory(path_data);
            }
            #endregion
            File.WriteAllText(path_data + "\\" + "detail.json", JsonConvert.SerializeObject(_Book));
            using (WebClient client = new WebClient())
            {
                Task.Run(() => { client.DownloadFile(new Uri(_Book.img_url), path_data + "\\" + "img.jpg"); });
            };
            App.Global.Book_Short_ViewModel.LoadListBookShort();
        }



        private void Download_Content_OneChaper(string item)
        {
            var count = _Book.chapter_link.FindIndex(x => x.Contains(item));
            var path_data = App.Global.Directory_Folder + "\\data\\book" + "\\" + _Book.source + "\\" + _Book.book_id + "\\content";
            if (File.Exists(path_data + "\\" + count + ".json"))
                return;
            while (true)
            {
                try
                {
                    var json = new WebClient().DownloadString(App.Global.API_URL_Primary + "/api/chapters?url=" + item);
                    File.WriteAllText(path_data + "\\" + count + ".json", json);
                    return;
                }
                catch
                {
                    // Catch but do nothing
                }
            }
        }


        #region Function getdata
        public Book Book
        {
            get { return _Book; }
            set { _Book = value; }
        }
        public string book_author
        {
            get { return _Book.book_author; }
            set { _Book.book_author = value; }
        }
        public int bookTotalChapter
        {
            get { return _Book.chapter_link.Count; }
        }
        public string book_intro
        {
            get { return _Book.book_intro; }
            set { _Book.book_intro = value; }
        }
        public string book_name
        {
            get { return _Book.book_name; }
            set { _Book.book_name = value; }
        }
        public string img_url
        {
            get { return _Book.img_url; }
            set { _Book.img_url = value; }
        }
        public List<string> season_name
        {
            get { return _Book.season_name; }
            set { _Book.season_name = value; }
        }
        public List<int> season_index
        {
            get { return _Book.season_index; }
            set { _Book.season_index = value; }
        }
        public List<string> chapter_name
        {
            get { return _Book.chapter_name; }
            set { _Book.chapter_name = value; }
        }
        public List<string> chapter_link
        {
            get { return _Book.chapter_link; }
            set { _Book.chapter_link = value; }
        }

        public void RemoveOutOfLibrary()
        {
            try
            {
                //Directory.Delete(App.Global.Book_Directory); //xóa thư mục.
                //App.Global.Book_Short_ViewModel.Book_Short.RemoveAt(0);

                App.Global.Book_Short_ViewModel.Book_Short.RemoveAll(e => e.book_dir == App.Global.Book_Directory );
                App.Global.Book_TreeView[0].Items.RemoveAll(e => e.book_dir == App.Global.Book_Directory);
            }
            catch
            {
                // do nothing
            }
            
        }
        #endregion

    }
}
