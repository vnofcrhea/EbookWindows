using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookWindows.Model
{
    public class Book_Short
    {
        
        public string Title { get; set; }
        public string img_dir { get; set; }
        public string book_dir { get; set; }

    }
    public class Book_Short_TreeView
    {
        public Book_Short_TreeView()
        {
            this.Items = new List<Book_Short>(); //Using For TreeView
        }
        public string Title { get; set; }
        public string book_dir { get; set; }
        public List<Book_Short> Items { get; set; }

    }
}
