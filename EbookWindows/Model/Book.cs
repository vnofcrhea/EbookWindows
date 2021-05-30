using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookWindows.Model
{
    public class Book
    {
        public string source { get; set; }
        public string book_id { get; set; }
        public string img_url { get; set; }
        public string book_name { get; set; }
        public string book_intro { get; set; }
        public string book_author { get; set; }
        public List<string> chapter_name { get; set; }
        public List<string> chapter_link { get; set; }
        public List<string> season_name { get; set; }
        public List<int> season_index { get; set; }
    }
}
