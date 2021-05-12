using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersOne.Epub;

namespace EbookWindows.ViewModels
{
    class myEpubReader
    {
        private static string _library = "Library"; //Library folder name
        private static EpubBook epubBook;
        //Variable name for epub paths
        private static string _filePath;
        private static string _fileName;
        private static string _tempPath;
        private static string _baseMenuXmlDiretory;

        //Variable name for read epub 
        public static List<string> _tableContentLink = new List<string>();
        public static List<string> _tableContentTitle = new List<string>();
        public static List<string> _menuItems = new List<string>();
        //public static int _currentPage;
            
        //Singleton
        private static myEpubReader instance;

        private myEpubReader()
        {
     
        }

        public myEpubReader(string filePath)
        {
            getInstance();
            ReadFile(filePath);
        }

        public static myEpubReader getInstance()
        {
            if (instance == null)
            {
                instance = new myEpubReader();
            }
            return instance;
        }

        //Open and readfile funtion
        public bool ReadFile()
        {
            getEpubFileName();

            if (_filePath != "")
            {

                unZipFile();
                Clear();

                epubBook = EpubReader.ReadBook(_filePath);
                _baseMenuXmlDiretory = epubBook.Schema.ContentDirectoryPath;
                //get menu
                foreach (EpubContentFile epubContent in epubBook.ReadingOrder)
                {
                    _menuItems.Add(GetPath(epubContent.FileName));
                }
                //get table of contents
                getTableContent(epubBook.Navigation);
            }
            else return false;
            return true;
        }
        public bool ReadFile(string filePath)
        {
            //getEpubFileName();

            if (File.Exists(filePath))
            {
                _filePath = filePath;
                _fileName = Path.GetFileNameWithoutExtension(_filePath);

                if (!Directory.Exists(_library))
                {
                    Directory.CreateDirectory(_library);
                }
                _tempPath = Path.Combine(_library, _fileName);

                unZipFile();
                Clear();

                epubBook = EpubReader.ReadBook(_filePath);
                _baseMenuXmlDiretory = epubBook.Schema.ContentDirectoryPath;
                //get menu
                foreach (EpubContentFile epubContent in epubBook.ReadingOrder)
                {
                    _menuItems.Add(GetPath(epubContent.FileName));
                }
                //get table of contents
                getTableContent(epubBook.Navigation);
            }
            else return false;
            return true;
        }

        //Funtions for read file
        private void getEpubFileName()
        {
            // GET FILE PATH, NAME
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();

            _filePath = openFileDialog.FileName;
            _fileName = Path.GetFileNameWithoutExtension(_filePath);

            if (!Directory.Exists(_library))
            {
                Directory.CreateDirectory(_library);
            }
            _tempPath = Path.Combine(_library, _fileName);
        }

        private void unZipFile()
        {
            if (!Directory.Exists(_tempPath))
            {
                ZipFile.ExtractToDirectory(_filePath, _tempPath);
                System.Windows.MessageBox.Show("New book has been added to library!");
            }
        }
            
        public void Clear()
        {
            //CLEAR TABLE CONTENTS, MENU
            _menuItems.Clear();
            _tableContentLink.Clear();
            _tableContentTitle.Clear();
            //TableContentCombobox.Items.Clear();
        }

        public void getTableContent(List<EpubNavigationItem> navigation)
        {
            // Enumerating chapters
            foreach (EpubNavigationItem chapter in navigation)
            {
                if (chapter.Link != null)
                {
                    // Link of chapter
                    _tableContentLink.Add(GetPath(chapter.Link.ContentFileName.Substring(3)));
                    _tableContentTitle.Add(chapter.Title);
                    //TableContentCombobox.Items.Add(chapter.Title);
                }
                if (chapter.NestedItems.Count > 0)
                {
                    // Nested chapters
                    getTableContent(chapter.NestedItems);
                }
            }
        }

        public string GetPath(string link)
        {
            return String.Format("file:///{0}", Path.GetFullPath(Path.Combine(_tempPath, _baseMenuXmlDiretory, link)));
        }

        
    }
}
