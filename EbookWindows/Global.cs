using EbookWindows.Model;
using EbookWindows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookWindows
{
    public class Global
    {
        public bool isFullScreen { get; set; }
        public string API_URL_Primary { get; set; }
        public string Book_Directory { get; set; }
        public List<Book_Short> List_Book_Short { get; set; }
        public Book_Short_ViewModel Book_Short_ViewModel { get; set; }
        public bool IsBookDownloaded { get; set; }
        public bool IsBookContentDownloaded { get; set; }
        public bool IsChapterDowwnloaded { get; set; }
        public Book_ViewModel Book_ViewModel { get; set; }
        public Chapter_ViewModel Chapter_ViewModel { get; set; }
        public Settings_ViewModel Settings_ViewModel { get; set; }
        public string Directory_Folder { get; set; }
        public List<Book_Short_TreeView> Book_TreeView { get; set; }
        public Global()
        {
            API_URL_Primary = "https://ebook-main-server.herokuapp.com";
            Book_Directory = null;
            List_Book_Short = new List<Book_Short>();
            Directory_Folder =  System.IO.Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString();
            Book_Short_ViewModel = new Book_Short_ViewModel();
            Book_ViewModel = new Book_ViewModel();
            Chapter_ViewModel = new Chapter_ViewModel();
            Book_TreeView = new List<Book_Short_TreeView>();
            isFullScreen = false;
        }
    }
}
