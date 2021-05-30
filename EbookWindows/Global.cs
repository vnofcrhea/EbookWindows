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
        public string base_url { get; set; }
        public string book_dir { get; set; }
        public Chapter chapter { get; set; }
        public Root Items { get; set; }
        public List<ShelfTag> shelfTag { get; set; }
        public string Directory_Folder { get; set; }
        public Global()
        {
            base_url = "https://flask-web-scraping.herokuapp.com";
            book_dir = null;
            shelfTag = new List<ShelfTag>();
            Directory_Folder =  System.IO.Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString();
        }
    }
}
