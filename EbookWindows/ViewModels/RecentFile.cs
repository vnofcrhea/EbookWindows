using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookWindows.ViewModels
{
    class RecentFile
    {
        public string fileName { get; set; }
        public string filePath { get; set; }
        public string fileIcon { get; set; }
        public RecentFile() { }
        public RecentFile(string fileName, string filePath, string fileIcon)
        {
            this.fileName = fileName;
            this.filePath = filePath;
            this.fileIcon = fileIcon;
        }
    }

    class RecentFileDao
    {

        //ObservableCollection
        public BindingList<RecentFile> GetAll()
        {
            BindingList<RecentFile> list = new BindingList<RecentFile>();
            var strFileName = App.Global.Directory_Folder + @"\\data\\recentfile.txt";

            if (!File.Exists(strFileName))
            {

                FileStream fileStream = new FileStream(strFileName, FileMode.Create);
                fileStream.Close();
            }
            else
            {
                String[] recentFileData;
                recentFileData = ReadFile(strFileName);
                if (recentFileData.Length != 0)
                {
                    list = LoadListFromData(recentFileData);
                }
            }
            return list;
        }

        private BindingList<RecentFile> LoadListFromData(string[] recentFileData)
        {
            BindingList<RecentFile> recentFileList = new BindingList<RecentFile>();
            int numberOfFilePath = recentFileData.Length / 3;
            if (numberOfFilePath > 10)
            {
                for(int i = 0; i< recentFileData.Length; i = i + 3)
                {
                    if (File.Exists(recentFileData[i + 1]))
                    {
                        RecentFile temp = new RecentFile();
                        temp.fileName = recentFileData[i];
                        temp.filePath = recentFileData[i + 1];
                        temp.fileIcon = recentFileData[i + 2];
                        recentFileList.Add(temp);
                    }            
                }
            }
            else
            {
                for (int i = 0; i < recentFileData.Length; i=i+3)
                {
                    if (File.Exists(recentFileData[i + 1]))
                    {
                        RecentFile temp = new RecentFile();
                        temp.fileName = recentFileData[i];
                        temp.filePath = recentFileData[i + 1];
                        temp.fileIcon = recentFileData[i + 2];
                        recentFileList.Add(temp);
                    }
                }
            }
            return recentFileList;
            //throw new NotImplementedException();
        }

        public bool WriteNewRecentFileData(BindingList<RecentFile> recentFileList)
        {
            var strFileName = App.Global.Directory_Folder + @"\\data\\recentfile.txt";
            //if (File.Exists(strFileName))
            //{
            //    File.Delete(strFileName);
            //}
            StreamWriter fileStream = new StreamWriter(strFileName);
            int amount = recentFileList.Count();
            if(amount > 10)
            {
                amount = 10;
            }
            fileStream.Flush();
            for (int i = 0; i < amount; ++i)
            {
                fileStream.WriteLine(recentFileList[i].fileName);
                fileStream.WriteLine(recentFileList[i].filePath);
                fileStream.WriteLine(recentFileList[i].fileIcon);
            }
            fileStream.Close();
            return true;

        }

        private String[] ReadFile(string strFileName)
        {
            StreamReader sr = new StreamReader(strFileName);
            String[] list;
            list = File.ReadAllLines(strFileName);
            sr.Close();
            return list;
            //throw new NotImplementedException();
        }
    }
}
