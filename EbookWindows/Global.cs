using EbookWindows.Model;
using EbookWindows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookWindows
{
    public partial class Global
    {
        public string API_URL_Primary { get; set; }
        public string Book_Directory { get; set; }
        public Chapter Chapter { get; set; }
        public Book Items { get; set; }
        public List<Book_Short> List_Book_Short { get; set; }
        public Book_Short_ViewModel Book_Short_ViewModel { get; set; }

        public Book_ViewModel Book_ViewModel { get; set; }
        public string Directory_Folder { get; set; }
        public Global()
        {
            API_URL_Primary = "https://flask-web-scraping.herokuapp.com";
            Book_Directory = null;
            List_Book_Short = new List<Book_Short>();
            Directory_Folder =  System.IO.Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString();
            Book_Short_ViewModel = new Book_Short_ViewModel();
            Book_ViewModel = new Book_ViewModel();
        }
    }
}
