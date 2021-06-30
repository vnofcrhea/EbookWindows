using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookWindows.ViewModels
{
    public class RecentFile_ViewModel
    {
        private List<RecentFile> _List_Recent_Files = new List<RecentFile>();
        private const string pdf = "Icon\\pdf.png";
        private const string epub = "Icon\\epub.png";
        public List<RecentFile> Recent_File
        {
            get { return _List_Recent_Files; }
            set { _List_Recent_Files = value; }
        }


        public RecentFile_ViewModel()
        {
            _List_Recent_Files.Clear();
            try
            {
                if (File.Exists(App.Global.Directory_Folder + "\\data\\RecentFiles.json"))
                {
                    using (StreamReader file = File.OpenText(App.Global.Directory_Folder + "\\data\\RecentFiles.json"))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        _List_Recent_Files = (List<RecentFile>)serializer.Deserialize(file, typeof(List<RecentFile>));
                    }
                }
            }
            catch
            {
            }
        }
        public void Save_File()
        {
            var path_data = App.Global.Directory_Folder + "\\data";
            #region create path. 
            if (!Directory.Exists(path_data))
            {
                Directory.CreateDirectory(path_data);
            }
            var i = _List_Recent_Files.Count;
            if (i > 10)
            {
                _List_Recent_Files.RemoveRange(10, i - 10);
            }
            #endregion
            File.WriteAllText(path_data + "\\" + "RecentFiles.json", JsonConvert.SerializeObject(_List_Recent_Files));
        }    
       
    }
}
