using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EbookWindows.Model;
using Newtonsoft.Json;

namespace EbookWindows.ViewModels
{
    public class Chapter_ViewModel
    {
        private IList<Chapter> _Chapter;
        private Chapter _Current_Chapter;
        private Chapter_Content _Current_Chapter_Content;
        public Chapter_ViewModel()
        {
            _Chapter = new List<Chapter>();
            _Current_Chapter_Content = new Chapter_Content();
            _Current_Chapter = new Chapter();
        }

        public IList<Chapter> Chapter
        {
            get { return _Chapter; }
            set { _Chapter = value; }
        }
        public Chapter_Content Current_Chapter_Content
        {
            get { return _Current_Chapter_Content; }
            set { _Current_Chapter_Content = value; }
        }
        public Chapter Current_Chapter
        {
            get { return _Current_Chapter; }
            set { _Current_Chapter = value; }
        }

        public List<Chapter> Get_Chapter_List()
        {
            return _Chapter.ToList();
        }

        public void Load_ChapterList()
        {
            _Chapter.Clear();
            var sum = App.Global.Book_ViewModel.bookTotalChapter;
            for (int i = 0; i < sum; i++)
            {
                _Chapter.Add(new Chapter { Title = App.Global.Book_ViewModel.chapter_name[i], link = App.Global.Book_ViewModel.chapter_link[i] });
            }
        }

        public void Load_Content()
        {
            var index = App.Global.Book_ViewModel.chapter_link.FindIndex(e => e.Contains(_Current_Chapter.link));
            var contents_dir = App.Global.Book_Directory + "\\content";
            var chapter_dir = App.Global.Book_Directory + "\\content\\" + index + ".json";
            if (File.Exists(chapter_dir))
            {
                Console.WriteLine(1);
                using (StreamReader file = File.OpenText(chapter_dir))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    _Current_Chapter_Content = (Chapter_Content)serializer.Deserialize(file, typeof(Chapter_Content));
                }
            }
            else
            {
                int count = 0;
                // Console.WriteLine(App.Global.Book_Directory + "\\content\\" + index + ".json");
                while (true)
                {
                    try
                    {
                        var json = new WebClient().DownloadString(App.Global.API_URL_Primary + "/api/chapters?url=" + _Current_Chapter.link);
                        _Current_Chapter_Content = JsonConvert.DeserializeObject<Chapter_Content>(json);
                        if(!Directory.Exists(contents_dir))
                        {
                            Directory.CreateDirectory(contents_dir);
                        }    
                        File.WriteAllText(chapter_dir, json);
                        App.Global.Book_ViewModel.Downloaded_Chapters_index.Add(index);
                        return;
                    }
                    catch (Exception e)
                    {
                        count++;
                        if (count > 3)
                        {
                            _Current_Chapter_Content = new Chapter_Content() {  content=""};
                            MessageBox.Show("Error while processing, please try again later!");
                            return;
                        }
                    }
                }
            }
        }
    }
}
