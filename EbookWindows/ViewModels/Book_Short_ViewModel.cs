using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookWindows.Model;
using Newtonsoft.Json;

namespace EbookWindows.ViewModels
{
    public class Book_Short_ViewModel
    {
        private List<Book_Short> _List_Book_Short;
        public Book_Short_ViewModel()
        {
            _List_Book_Short = new List<Book_Short>();
        }
        public List<Book_Short> Book_Short
        {
            get { return _List_Book_Short; }
            set { _List_Book_Short = value; }
        }
        //Get List Book_Short
        public List<Book_Short> Get_Book_Short_List()
        {
            return _List_Book_Short.ToList();
        }    
        public void LoadListBookShort()
        {
            _List_Book_Short.Clear();
            var path_data = App.Global.Directory_Folder + "\\data\\book";

            if (!Directory.Exists(path_data))
            {
                Directory.CreateDirectory(path_data);
            }
            string[] subdirectoryEntries = Directory.GetDirectories(path_data);
            foreach (var item in subdirectoryEntries)
            {
                var sub1 = Directory.GetDirectories(item);
                foreach (var item1 in sub1)
                {
                    using (StreamReader file = File.OpenText(item1 + "\\detail.json"))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        Book root = (Book)serializer.Deserialize(file, typeof(Book));
                        _List_Book_Short.Add(new Book_Short() { Title = root.book_name, img_dir = item1 + "\\img.jpg", book_dir = item1 });

                    }
                }
            }
        }
    }
}
