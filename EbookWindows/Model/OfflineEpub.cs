using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersOne.Epub;

namespace EbookWindows.Model
{
    public class OfflineEpub
    {
        #region initial variables
        //Initial file/folder names
        public readonly string library = "Library"; 
        public readonly string bookmarkFileName = "bookmarks.txt";
        public readonly string stylesheetFileName = "stylesheet.css";
        public readonly string statusFileName = "status.txt";
        #endregion

        //Variables  for epub paths
        public  string filePath { get; set; }
        public  string fileName { get; set; }
        public  string tempPath { get; set; }
        public  string baseMenuXmlDiretory { get; set; }
        public  string stylesheetPath { get; set; }
        public  string readingStatusPath { get; set; }

        public List<string> fontFamilys = new List<string>();

        #region Singleton
        private static OfflineEpub instance;
        private OfflineEpub()
        {
            fontFamilys.Add("Time New Roman");
            fontFamilys.Add("Arial");
            fontFamilys.Add("Roboto");
            fontFamilys.Add("Tahoma");
        }

        public static OfflineEpub GetInstance()
        {
            if (instance == null)
                instance = new OfflineEpub();
            return instance;
        }
        #endregion


    }
}
