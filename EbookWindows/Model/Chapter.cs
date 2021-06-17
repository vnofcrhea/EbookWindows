using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookWindows.Model
{
    public class Chapter
    {

        public Chapter()
        {
            this.Items = new ObservableCollection<Chapter>();
        }

        public string Title { get; set; }
        public string link { get; set; }
        public bool isReaded { get; set; }
        public bool isDownloaded { get; set; }
        public ObservableCollection<Chapter> Items { get; set; }
    }    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
}
