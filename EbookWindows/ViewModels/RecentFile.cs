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
            var strFileName = App.path + @"\\data\\recentfile.txt";

            if (!File.Exists(strFileName))
            {

                FileStream fileStream = new FileStream(strFileName, FileMode.Create);
            }
            else
            {
                String[] filePathList;
                filePathList = ReadFile(strFileName);
                if(filePathList.Length != 0)
                {
                    list = LoadListFromData(filePathList);
                }
            }
            //var list = new BindingList<RecentFile>()
            //    {
            //        new RecentFile() { fileName="Hoàng Tử Bé", filePath="akisnkbdsac b", fileIcon="Icon/epub.png"},
            //        new RecentFile() { fileName="Lâu Đài Quỷ", filePath="akisnkbdsac b", fileIcon="Icon/epub.png"},
            //        new RecentFile() { fileName="Chuyện Mèo Con Dạy Hải Âu Biết Bay Mèo Con Dạy Hải Âu Biết Bay", filePath="akisnkbdsac b", fileIcon="Icon/pdf.png"},
            //        new RecentFile() { fileName="Số Đỏ", filePath="akisnkbdsac b", fileIcon="Icon/epub.png"},
            //        new RecentFile() { fileName="Chuyện Ngày Mưa", filePath="akisnkbdsac b", fileIcon="Icon/epub.png"},
            //        new RecentFile() { fileName="Hoàng Tử Bé", filePath="akisnkbdsac b", fileIcon="Icon/epub.png"},
            //        new RecentFile() { fileName="Lâu Đài Quỷ", filePath="akisnkbdsac b", fileIcon="Icon/epub.png"},
            //        new RecentFile() { fileName="Chuyện Mèo Con Dạy Hải Âu Biết Bay", filePath="akisnkbdsac b", fileIcon="Icon/pdf.png"},
            //        new RecentFile() { fileName="Số Đỏ", filePath="akisnkbdsac b", fileIcon="Icon/epub.png"},
            //        new RecentFile() { fileName="Chuyện Ngày Mưa", filePath="akisnkbdsac b", fileIcon="Icon/epub.png"},
            //};

            return list;
        }

        private BindingList<RecentFile> LoadListFromData(string[] filePathList)
        {
            BindingList<RecentFile> recentFile = new BindingList<RecentFile>();
            int numberOfFilePath = filePathList.Length / 3;
            if (numberOfFilePath > 10)
            {
                for(int i = 0; i< filePathList.Length; i = i + 3)
                {
                    if (File.Exists(filePathList[i + 1]))
                    {
                        RecentFile temp = new RecentFile();
                        temp.fileName = filePathList[i];
                        temp.filePath = filePathList[i + 1];
                        temp.fileIcon = filePathList[i + 2];
                        recentFile.Add(temp);
                    }            
                }
            }
            else
            {
                for (int i = 0; i < filePathList.Length; i=i+3)
                {
                    if (File.Exists(filePathList[i + 1]))
                    {
                        RecentFile temp = new RecentFile();
                        temp.fileName = filePathList[i];
                        temp.filePath = filePathList[i + 1];
                        temp.fileIcon = filePathList[i + 2];
                        recentFile.Add(temp);
                    }
                }
            }
            return recentFile;
            //throw new NotImplementedException();
        }

        private String[] ReadFile(string strFileName)
        {
            StreamReader sr = new StreamReader(strFileName);
            String[] list;
            list = File.ReadAllLines(strFileName);
            return list;
            //throw new NotImplementedException();
        }
    }
}
