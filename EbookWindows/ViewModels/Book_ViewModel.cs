using EbookWindows.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
    }
}
