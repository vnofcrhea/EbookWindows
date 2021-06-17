using EbookWindows.Model;
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
    public class OfflineEpub_ViewModel
    {
        private static EpubBook epubBook;
        public static OfflineEpub epub = OfflineEpub.GetInstance();

        #region share variables
        //Variables for read epub 
        public static List<string> menuItems = new List<string>();
        public static Dictionary<string, string> tableContent = new Dictionary<string, string>();
        public static Dictionary<string, string> bookmarks = new Dictionary<string, string>();
        public static List<string> readingStatus = new List<string>();

        //Variables for font
        public static List<string> fontFamilys = epub.fontFamilys;
        #endregion

        #region Open and readfile funtion
        public static bool ReadFile(string filePath)
        {
            if (File.Exists(filePath)) //check if any file is chosen
            {               
                //Start read new file
                epub.filePath = filePath;
                epub.fileName = Path.GetFileNameWithoutExtension(epub.filePath);
                //Create library folder if not have yet
                if (!Directory.Exists(epub.library))
                {
                    Directory.CreateDirectory(epub.library);
                }
                epub.tempPath = Path.Combine(epub.library, epub.fileName);
                //Check valid epub type
                try
                {
                    epubBook = EpubReader.ReadBook(epub.filePath);
                }
                catch
                {
                    MessageBox.Show("Invalid epub file! Please choose another file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }                
                //Set up new epub file folder: stylesheet, bookmark, status
                epub.baseMenuXmlDiretory = Path.Combine(epub.tempPath, epubBook.Schema.ContentDirectoryPath);
                SetupNewEpub();
                Clear();
                //get stylesheet, status path
                if (epub.stylesheetPath == null)
                {
                    epub.stylesheetPath = Path.Combine(epub.baseMenuXmlDiretory, getStylesheetFileName());
                    epub.readingStatusPath = Path.Combine(epub.baseMenuXmlDiretory, epub.statusFileName);
                }                  
                //get menu
                foreach (EpubContentFile epubContent in epubBook.ReadingOrder)
                {
                    menuItems.Add(GetPath(epubContent.FileName));
                }
                //get table of contents
                getTableContent(epubBook.Navigation);
                setTableContent();
                //get epub.bookmarks
                getBookmarkFromFile(Path.GetFullPath(Path.Combine(epub.baseMenuXmlDiretory, epub.bookmarkFileName)));
                //get reading status
                getReadingStatus();
            }
            else return false;
            return true;
        }
        #endregion

        #region support funtions
        private static void SetupNewEpub()
        {
            if (!Directory.Exists(epub.tempPath))
            {
                //unzip file
                ZipFile.ExtractToDirectory(epub.filePath, epub.tempPath);
                //add bookmark file
                var fileStream = File.Create((epub.baseMenuXmlDiretory + "\\" + epub.bookmarkFileName));
                fileStream.Close();
                //  initial reading status file
                setInitialReadingStatus();
                //set initial Stylesheet file - create new if not exists
                setInitialStylesheet();                  
            }
        }

        private static void setInitialReadingStatus()
        {
            epub.readingStatusPath = (epub.baseMenuXmlDiretory + "\\" + epub.statusFileName);
            var fileStream = File.Create(epub.readingStatusPath);
            fileStream.Close();
            string firstStatus = "0\nTime New Roman";
            writeAllFile(epub.readingStatusPath, firstStatus);
        }

        private static void setInitialStylesheet()
        {
            Dictionary<string, EpubTextContentFile> cssFiles = epubBook.Content.Css;
            string s = getStylesheetFileName();
            if (s != null) //StylesheetFile exists
            {
                //update stylesheetPath
                epub.stylesheetPath = Path.Combine(epub.baseMenuXmlDiretory, s);
                string cssContent = cssFiles[s].Content;
                cssContent = "/*Offline epub reader custom*/" +
                    "\nbody { " +
                    "\n padding: 0 1em 1em 1em; " +
                    "\n Background-color: white;" +
                    "\n Color: black;" +
                    "\n Font-family: Time New Roman;" +
                    "\n }" +
                    "\n" + cssContent;
                writeAllFile(epub.stylesheetPath, cssContent);

            }
            else //stylesheet File not exists
            {
                string directoryName = Path.GetDirectoryName(cssFiles.ElementAt(0).Key);
                //update stylesheetPath
                epub.stylesheetPath = epub.baseMenuXmlDiretory + "\\" + directoryName + "\\" + epub.stylesheetFileName;
                //make new sylesheet file,
                var fileStream = File.Create(epub.stylesheetPath);
                fileStream.Close();
                //update new stylesheet file
                string cssContent = "/*Offline epub reader custom*/" +
                    "\nbody { " +
                    "\n padding: 0 1em 1em 1em; " +
                    "\n Background-color: white;" +
                    "\n Color: black;" +
                    "\n Font-family: Time New Roman;" +
                    "\n }\n";
                writeAllFile(epub.stylesheetPath, cssContent);
                //import that file to other css file "@import "stylesheet.css";
                foreach (var temp in cssFiles)
                {
                    cssContent = temp.Value.Content.ToString();
                    cssContent = "import " + epub.stylesheetFileName + "\n" + cssContent;
                    writeAllFile(Path.Combine(epub.baseMenuXmlDiretory, temp.Value.FileName.ToString()), cssContent);
                }
            }
        }

        private static string getStylesheetFileName()
        {
            Dictionary<string, EpubTextContentFile> cssFiles = epubBook.Content.Css;
            List<string> keys = new List<string>(cssFiles.Keys);
            return isContaintKey(epub.stylesheetFileName, keys);
        }

        public static void Clear()
        {
            //CLEAR TABLE CONTENTS, MENU, BOOKMARK
            menuItems.Clear();
            bookmarks.Clear();
            tableContent.Clear();
            readingStatus.Clear();
        }
        private static string GetPath(string link)
        {
            return String.Format("file:///{0}", Path.GetFullPath(Path.Combine(epub.baseMenuXmlDiretory, link)));
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
                        s += line + "\n";
                    }
                    sr.Close();
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
                    sw.Close();
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
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
            Dictionary<string, string> realtableContent = new Dictionary<string, string>();
            foreach (string s in menuItems)
            {
                if (tableContent.ContainsKey(s))
                {
                    realtableContent.Add(s, tableContent[s]);
                }
                else
                {
                    realtableContent.Add(s,
                        s.Substring(s.LastIndexOf("\\") + 1, //start index (+1/-1 to remove "\\")
                        s.LastIndexOf(Path.GetExtension(s)) - s.LastIndexOf("\\") - 1 //length
                        ));
                }
            }
            tableContent.Clear();
            tableContent = realtableContent;
        }
        #endregion

        #region bookmark
        public static int addBookmark(string chapterLink)
        {
            if (bookmarks.ContainsKey(chapterLink))
                return 0; //chapter already bookmarked
            bookmarks.Add(chapterLink, tableContent[chapterLink]);
            updateBookmarkToFile(Path.Combine(epub.baseMenuXmlDiretory, epub.bookmarkFileName));
            return 1;
        }
        public static int deleteBookmark(string chapterLink)
        {
            if (!bookmarks.ContainsKey(chapterLink))
            {
                return 0;
            }
            bookmarks.Remove(chapterLink);
            updateBookmarkToFile(Path.Combine(epub.baseMenuXmlDiretory, epub.bookmarkFileName));
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

        #region reading status
        private static bool getReadingStatus()
        {
            try
            {
                using (StreamReader sr = new StreamReader(epub.readingStatusPath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        readingStatus.Add(line);
                    }
                    sr.Close();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

        public static bool updateReadingStatus(int chapter, string fontFamily)
        {
            try
            {
                //update status list
                readingStatus[0] = chapter.ToString();
                readingStatus[1] = fontFamily;
                 //update file
                using (StreamWriter sw = new StreamWriter(epub.readingStatusPath))
                {
                    foreach (string s in readingStatus)
                        sw.WriteLine(s);
                    sw.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

        #endregion

        #region color and font
        public static void changeBackgroundColor(string newColor)
        {
            //get current style
            string currentStyle = readAllFile(epub.stylesheetPath);
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
            writeAllFile(epub.stylesheetPath, currentStyle);
        }

        public static void changeForegroundColor(string newColor)
        {
            //get current style
            string currentStyle = readAllFile(epub.stylesheetPath);
            string newStyle = currentStyle.Substring(currentStyle.IndexOf("/*Offline epub reader custom*/"));
            newColor = newColor.Remove(1, 2);
            //change new color
            StringBuilder s = new StringBuilder(newStyle);
            int currentIndex = newStyle.IndexOf("Color");
            if (currentIndex >= 0) //check custom foreground is added yet
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
            writeAllFile(epub.stylesheetPath, currentStyle);
        }

        public static void changeFontFamily(string fontFamily)
        {
            //get current style
            string currentStyle = readAllFile(epub.stylesheetPath);
            string newStyle = currentStyle.Substring(currentStyle.IndexOf("/*Offline epub reader custom*/"));
            //change new color
            StringBuilder s = new StringBuilder(newStyle);
            int currentIndex = newStyle.IndexOf("Font-family: ");
            if (currentIndex >= 0) //check custom font-family is added yet
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
            writeAllFile(epub.stylesheetPath, currentStyle);
        }
        #endregion
    }
}
