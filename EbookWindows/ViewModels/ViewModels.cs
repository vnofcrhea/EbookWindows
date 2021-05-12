using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookWindows.ViewModels
{
    public class Chapter
    {
        public Chapter()
        {
            this.Items = new ObservableCollection<Chapter>();
        }

        public string Title { get; set; }
        public string link { get; set; }

        public ObservableCollection<Chapter> Items { get; set; }
    }    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Root
    {
        public string img_url { get; set; }
        public string book_name { get; set; }
        public string book_intro { get; set; }
        public string book_author { get; set; }
        public List<string> chapter_name { get; set; }
        public List<string> chapter_link { get; set; }
        public List<string> season_name { get; set; }
        public List<int> season_index { get; set; }
    }
    public class Root_Reading
    {
        public string book_title { get; set; }
        public string chapter_title { get; set; }
        public string content { get; set; }
    }
}
