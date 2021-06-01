using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EbookWindows.ViewModels
{
    class RecentFile
    {
        public string fileName { get; set; }
        public string filePath { get; set; }
        public string fileIcon { get; set; }
        public string recentLocation { get; set; }
        public RecentFile() { }
        public RecentFile(string fileName, string filePath, string fileIcon)
        {
            this.fileName = fileName;
            this.filePath = filePath;
            this.fileIcon = fileIcon;
            this.recentLocation = "0";
        }

        public RecentFile(string fileName, string filePath, string fileIcon, string recentLocal)
        {
            this.fileName = fileName;
            this.filePath = filePath;
            this.fileIcon = fileIcon;
            this.recentLocation = recentLocal;
        }
    }   
}
