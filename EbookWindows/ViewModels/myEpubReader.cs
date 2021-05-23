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
        private static string stylesheetFileName = "stylesheet.css";
        private static EpubBook epubBook;
        //Variable name for epub paths
        private static string _filePath;
        private static string _fileName;
        private static string _tempPath;
        private static string _baseMenuXmlDiretory;
        private static string _stylesheetPath;
        //Variable name for read epub 
        public static List<string> menuItems = new List<string>();
        public static Dictionary<string, string> tableContent = new Dictionary<string, string>();
        public static Dictionary<string, string> bookmarks = new Dictionary<string, string>();
        //Variable for change font
        public static List<string> fontFamilys = new List<string>();


        #region Singleton
        private static myEpubReader instance;

        private myEpubReader()
        {
            fontFamilys.Add("Time New Roman");
            fontFamilys.Add("Arial");
            fontFamilys.Add("Courier");
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

        #region support funtions
        private static void getEpubFileName()
        {
            // GET FILE PATH, NAME
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Ebook Files( *.epub, *.EPUB)| *.epub; *.EPUB";
            openFileDialog.ShowDialog();

            _filePath = openFileDialog.FileName;
            if (_filePath != "") //check not choose file
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
                    MessageBox.Show("Invalid epub file! Please choose another file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        private static void unZipFile()
        {
            if (!Directory.Exists(_tempPath))
            {
                ZipFile.ExtractToDirectory(_filePath, _tempPath);
                //add bookmark file
                File.Create((_tempPath + "\\" + _baseMenuXmlDiretory + "\\" + bookmarkFileName));
                //set initial font and background
                setInitialStylesheet();
            }
        }

        private static void setInitialStylesheet()
        {
            Dictionary<string, EpubTextContentFile> cssFiles = epubBook.Content.Css;
            string s = getStylesheetFileName();
            if (s != null)
            {
                _stylesheetPath = Path.Combine(_tempPath, _baseMenuXmlDiretory, s);
                string cssContent = cssFiles[s].Content;
                cssContent += "\n/*Offline epub reader custom*/" +
                    "\nbody { " +
                    "\n padding: 0 1em 1em 1em; " +
                    "\n Background-color: white;" +
                    "\n Color: black;" +
                    "\n }";
                writeAllFile(_stylesheetPath, cssContent);
            }
        }

        private static string getStylesheetFileName()
        {
            Dictionary<string, EpubTextContentFile> cssFiles = epubBook.Content.Css;
            List<string> keys = new List<string>(cssFiles.Keys);
            return isContaintKey(stylesheetFileName, keys);           
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
        public static string isContaintKey(string key, List<string> keyList)
        {
            string s;
            for (int i = 0; i < keyList.Count; i++)
            {
                s = GetPath(keyList[i]);
                if (key == s.Substring(s.LastIndexOf("\\") + 1))
                    return keyList[i];
            }
            return null;
        }

        #endregion

        #region file
        private static string readAllFile(string filePath)
        {
            string s = null;
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        s += line +"\n";
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return s;
        }
        private static void writeAllFile(string filePath, string s)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    sw.WriteLine(s);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

       

        #endregion

        #region Open and readfile funtion
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

                try 
                { 
                    epubBook = EpubReader.ReadBook(_filePath);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Invalid epub file! Please choose another file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                _baseMenuXmlDiretory = epubBook.Schema.ContentDirectoryPath;

                unZipFile();
                Clear();

                //get stylesheet path
                if (_stylesheetPath == null)
                    _stylesheetPath = Path.Combine(_tempPath, _baseMenuXmlDiretory, getStylesheetFileName());
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
                        s.LastIndexOf(Path.GetExtension(s)) - s.LastIndexOf("\\") - 1 //length
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
            updateBookmarkToFile(Path.Combine(_tempPath, _baseMenuXmlDiretory, bookmarkFileName));
            return 1;
        }
        public static int deleteBookmark(string chapterLink)
        {
            if (!bookmarks.ContainsKey(chapterLink)) {
                return 0;
            }
            bookmarks.Remove(chapterLink);
            updateBookmarkToFile(Path.Combine(_tempPath, _baseMenuXmlDiretory, bookmarkFileName));
            return 1;
        }

        private static void updateBookmarkToFile(string filePath)
        {
            List<string> keyList = new List<string>(bookmarks.Keys);
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    foreach (string key in keyList)
                    {
                        sw.WriteLine(key + "\n" + bookmarks[key]);
                    }
                    sw.Close();
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
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        bookmarks.Add(line, sr.ReadLine());
                    }
                    sr.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region color and font
        public static void changeBackgroundColor(string newColor)
        {
            //get current style
            string currentStyle = readAllFile(_stylesheetPath);
            string newStyle = currentStyle.Substring(currentStyle.IndexOf("/*Offline epub reader custom*/"));
            newColor = newColor.Remove(1, 2);
            //change new color
            StringBuilder s = new StringBuilder(newStyle);
            int currentIndex = newStyle.IndexOf("Background-color");
            if (currentIndex >= 0) //check custom background is added yet
            {
                //delete old color
                s.Remove(currentIndex,
                    newStyle.IndexOf(";", currentIndex + 1) - currentIndex);
                s.Insert(currentIndex, "Background-color: " + newColor);
            }
            else
            {
                //add new color if not exists
                currentIndex = newStyle.IndexOf("}") - 1;
                s.Insert(currentIndex, "Background-color: " + newColor + "\n ");
            }
            currentStyle = currentStyle.Substring(0, currentStyle.IndexOf("/*Offline epub reader custom*/")) 
                + s.ToString();
            writeAllFile(_stylesheetPath, currentStyle);
        }

        public static void changeForegroundColor(string newColor)
        {
            //get current style
            string currentStyle = readAllFile(_stylesheetPath);
            string newStyle = currentStyle.Substring(currentStyle.IndexOf("/*Offline epub reader custom*/"));
            newColor = newColor.Remove(1, 2);
            //change new color
            StringBuilder s = new StringBuilder(newStyle);
            int currentIndex = newStyle.IndexOf("Color");
            if (currentIndex >= 0) //check custom background is added yet
            {
                //delete old color
                s.Remove(currentIndex,
                    newStyle.IndexOf(";", currentIndex + 1) - currentIndex);
                //insert new one               
                s.Insert(currentIndex, "Color: " + newColor);
            }
            else
            {
                //add new color if not exists
                currentIndex = newStyle.IndexOf("}") - 1;
                s.Insert(currentIndex, "Color: " + newColor + "\n ");
            }
            currentStyle = currentStyle.Substring(0, currentStyle.IndexOf("/*Offline epub reader custom*/"))
                + s.ToString();
            writeAllFile(_stylesheetPath, currentStyle);
        }

        public static void changeFontFamily(string fontFamily)
        {
            //get current style
            string currentStyle = readAllFile(_stylesheetPath);
            string newStyle = currentStyle.Substring(currentStyle.IndexOf("/*Offline epub reader custom*/"));
            //change new color
            StringBuilder s = new StringBuilder(newStyle);
            int currentIndex = newStyle.IndexOf("Font-family: ");
            if (currentIndex >= 0) //check custom background is added yet
            {
                //delete old color
                s.Remove(currentIndex,
                    newStyle.IndexOf(";", currentIndex + 1) - currentIndex);
                s.Insert(currentIndex, "Font-family: " + fontFamily);
            }
            else
            {
                //add new color if not exists
                currentIndex = newStyle.IndexOf("}") - 1;
                s.Insert(currentIndex, "Font-family: " + fontFamily + ";\n ");
            }
            currentStyle = currentStyle.Substring(0, currentStyle.IndexOf("/*Offline epub reader custom*/"))
                + s.ToString();
            writeAllFile(_stylesheetPath, currentStyle);
        }
        #endregion
    }
}
