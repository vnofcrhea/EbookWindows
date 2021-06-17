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
            if (i >10)
            {
                _List_Recent_Files.RemoveRange(10, i - 10);
            }    
            #endregion
            File.WriteAllText(path_data + "\\" + "RecentFiles.json", JsonConvert.SerializeObject(_List_Recent_Files));
        }    
        //Get data từ file
        //public BindingList<RecentFile> GetAll()
        //{
        //    BindingList<RecentFile> list = new BindingList<RecentFile>();
        //    var strFileName = App.Global.Directory_Folder + @"\\data\\recentfile.txt";

        //    if (!File.Exists(strFileName))
        //    {
        //        FileStream fileStream = new FileStream(strFileName, FileMode.Create);
        //        fileStream.Close();
        //    }
        //    else
        //    {
        //        String[] recentFileData;
        //        recentFileData = ReadFile(strFileName);
        //        if (recentFileData.Length != 0)
        //        {
        //            list = LoadListFromData(recentFileData);
        //        }
        //    }
        //    return list;
        //}
        //
        //private BindingList<RecentFile> LoadListFromData(string[] recentFileData)
        //{
        //    BindingList<RecentFile> recentFileList = new BindingList<RecentFile>();
        //    int numberOfFilePath = recentFileData.Length / 4;
        //    if (numberOfFilePath != 0)
        //    {
        //        for (int i = 0; i < recentFileData.Length; i = i + 4)
        //        {
        //            if (File.Exists(recentFileData[i + 1]))
        //            {
        //                RecentFile temp = new RecentFile();
        //                temp.fileName = recentFileData[i];
        //                temp.filePath = recentFileData[i + 1];
        //                temp.fileIcon = recentFileData[i + 2];
        //                //temp.recentLocation = recentFileData[i + 3];
        //                recentFileList.Add(temp);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        //nothing
        //    }

        //    return recentFileList;
        //    //throw new NotImplementedException();
        //}
        //Lưu file json 
        //public bool WriteNewRecentFileData(BindingList<RecentFile> recentFileList)
        //{
        //    var strFileName = App.Global.Directory_Folder + @"\\data\\recentfile.txt";
        //    //if (File.Exists(strFileName))
        //    //{
        //    //    File.Delete(strFileName);
        //    //}
        //    StreamWriter fileStream = new StreamWriter(strFileName);
        //    int amount = recentFileList.Count();
        //    if (amount > 10)
        //    {
        //        amount = 10;
        //    }
        //    fileStream.Flush();
        //    for (int i = 0; i < amount; ++i)
        //    {
        //        fileStream.WriteLine(recentFileList[i].fileName);
        //        fileStream.WriteLine(recentFileList[i].filePath);
        //        fileStream.WriteLine(recentFileList[i].fileIcon);
        //        fileStream.WriteLine(recentFileList[i].recentLocation);
        //    }
        //    fileStream.Close();
        //    return true;

        //}

        //private String[] ReadFile(string strFileName)
        //{
        //    StreamReader sr = new StreamReader(strFileName);
        //    String[] list;
        //    list = File.ReadAllLines(strFileName);
        //    sr.Close();
        //    return list;
        //    //throw new NotImplementedException();
        //}
    }
}
