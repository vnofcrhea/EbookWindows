using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VersOne.Epub;

namespace EbookWindows.ViewModels
{
    class myEpubReader
    {
        private static string library = "Library"; //Library folder name
        private static string bookmarkFileName = "bookmarks.txt";
        private static EpubBook epubBook;
        //Variable name for epub paths
        private static string _filePath;
        private static string _fileName;
        private static string _tempPath;
        private static string _baseMenuXmlDiretory;

        //Variable name for read epub 
        public static List<string> menuItems = new List<string>();
       
        public static Dictionary<string, string> tableContent = new Dictionary<string, string>();
     
        public static Dictionary<string, string> bookmarks = new Dictionary<string, string>();

        #region Singleton
        private static myEpubReader instance;

        private myEpubReader()
        {
     
        }

  
        public static myEpubReader getInstance()
        {
            if (instance == null)
            {
                instance = new myEpubReader();
            }
            return instance;
        }
        #endregion

        #region Open and readfile funtion
        private bool ReadFile()
        {
            getEpubFileName();

            if (_filePath != "")
            {
                epubBook = EpubReader.ReadBook(_filePath);
                _baseMenuXmlDiretory = epubBook.Schema.ContentDirectoryPath;

                unZipFile();
                Clear();
                                
                //get menu
                foreach (EpubContentFile epubContent in epubBook.ReadingOrder)
                {
                    menuItems.Add(GetPath(epubContent.FileName));
                }
                //get table of contents
                getTableContent(epubBook.Navigation);
                setTableContent();
                //get bookmarks
                //getBookmarkFromFile(_tempPath + "\\" + bookmarkFileName);
            }
            else return false;
            return true;
        }
        public static bool ReadFile(string filePath)
        {
            //getEpubFileName();

            if (File.Exists(filePath))
            {
                _filePath = filePath;
                _fileName = Path.GetFileNameWithoutExtension(_filePath);

                if (!Directory.Exists(library))
                {
                    Directory.CreateDirectory(library);
                }
                _tempPath = Path.Combine(library, _fileName);

                epubBook = EpubReader.ReadBook(_filePath);
                _baseMenuXmlDiretory = epubBook.Schema.ContentDirectoryPath;

                unZipFile();
                Clear();
                                
                //get menu
                foreach (EpubContentFile epubContent in epubBook.ReadingOrder)
                {
                    menuItems.Add(GetPath(epubContent.FileName));
                }
                //get table of contents
                getTableContent(epubBook.Navigation);
                setTableContent();
                //get bookmarks
                getBookmarkFromFile(Path.GetFullPath(Path.Combine(_tempPath, _baseMenuXmlDiretory, bookmarkFileName)));
            }
            else return false;
            return true;
        }
        #endregion

        #region support funtions
        private static void getEpubFileName()
        {
            // GET FILE PATH, NAME
            OpenFileDialog openFileDialog = new OpenFileDialog();          
            openFileDialog.Filter = "Ebook Files( *.epub, *.EPUB)| *.epub; *.EPUB";
            openFileDialog.ShowDialog();
            
            _filePath = openFileDialog.FileName;
            if (_filePath!="") //check not choose file
            {
                if (Path.GetExtension(_filePath).ToLower().Equals(".epub"))//check file type
                {
                    _fileName = Path.GetFileNameWithoutExtension(_filePath);

                    if (!Directory.Exists(library))
                    {
                        Directory.CreateDirectory(library);
                    }
                    _tempPath = Path.Combine(library, _fileName);
                }
                else
                {
                    MessageBox.Show("Invalid epub file! Please choose another file.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                }
            }
            
        }
        private static void unZipFile()
        {
            if (!Directory.Exists(_tempPath))
            {
                ZipFile.ExtractToDirectory(_filePath, _tempPath);
                //add bookmark file
                File.Create((_tempPath + "\\"+ _baseMenuXmlDiretory + "\\" + bookmarkFileName));
            }
        }           
        public static void Clear()
        {
            //CLEAR TABLE CONTENTS, MENU, BOOKMARK
            menuItems.Clear();            
            bookmarks.Clear();
            tableContent.Clear();
        }
        private static string GetPath(string link)
        {
            return String.Format("file:///{0}", Path.GetFullPath(Path.Combine(_tempPath, _baseMenuXmlDiretory, link)));
        }
        public static int getIndex(List<string> list, string s)
        {
            s = GetPath(s).Substring(GetPath(s).LastIndexOf("\\"));
            string sub;
            for (int i = 0; i < list.Count; i++)
            {
                sub = list[i].Substring(list[i].LastIndexOf("\\"));
                if (sub == s)
                    return i;
            }
            return -1;
        }

        #endregion

        #region table of contents
        private static void getTableContent(List<EpubNavigationItem> navigation)
        {
            // Enumerating chapters
            foreach (EpubNavigationItem chapter in navigation)
            {
                if (chapter.Link != null)
                {                  
                    string link = menuItems[getIndex(menuItems, chapter.Link.ContentFileName)];
                    tableContent.Add(link, chapter.Title);                    
                }
                if (chapter.NestedItems.Count > 0)
                {
                    // Nested chapters
                    getTableContent(chapter.NestedItems);
                }
            }
        }
        private static void setTableContent()
        {
            Dictionary<string, string> realTablecontent = new Dictionary<string, string>();
            foreach (string s in menuItems)
            {
                if (tableContent.ContainsKey(s))
                {
                    realTablecontent.Add(s, tableContent[s]);
                }
                else
                {
                    realTablecontent.Add(s, 
                        s.Substring(s.LastIndexOf("\\") + 1, //start index (+1/-1 to remove "\\")
                        s.LastIndexOf(Path.GetExtension(s)) - s.LastIndexOf("\\") -1 //length
                        ));
                }
            }
            tableContent.Clear();
            tableContent = realTablecontent;
        }
        #endregion

        #region bookmark
        public static int addBookmark(string chapterLink)
        {
            if (bookmarks.ContainsKey(chapterLink))
                return 0; //chapter already bookmarked
            bookmarks.Add(chapterLink, tableContent[chapterLink]);
            updateBookmarkToFile((_tempPath + "\\"+ _baseMenuXmlDiretory + "\\" + bookmarkFileName));
            return 1;
        }
        public static int deleteBookmark(string chapterLink)
        {
            if (!bookmarks.ContainsKey(chapterLink)){
                return 0;
            }
            bookmarks.Remove(chapterLink);
            updateBookmarkToFile((_tempPath + "\\" + _baseMenuXmlDiretory + "\\" + bookmarkFileName));
            return 1;
        }

        private static void updateBookmarkToFile(string filePath)
        {
            List<string> keyList = new List<string>(bookmarks.Keys);
            try
            {               
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    foreach(string key in keyList)
                    {
                        sw.WriteLine(key + "\n" + bookmarks[key]);
                    }

                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine(e.Message);
            }
        }

        private static void getBookmarkFromFile(string filePath)
        {
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;

                    // Read and display lines from the file until 
                    // the end of the file is reached. 
                    while ((line = sr.ReadLine()) != null)
                    {
                        bookmarks.Add(line, sr.ReadLine());
                        //Console.WriteLine(line);
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine(e.Message);
            }
        }
        #endregion



    }
}
