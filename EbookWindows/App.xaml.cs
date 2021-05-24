using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using EbookWindows.ViewModels;
using MaterialDesignThemes.Wpf;
namespace EbookWindows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string base_url = "https://flask-web-scraping.herokuapp.com";
        public static string book_dir = null;
        public static Chapter chapter;
        public static Root Items;
        public static string path = System.IO.Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString();
        public static void ChangeBaseTheme(BaseTheme x)
        {
                (App.Current.Resources.MergedDictionaries[0] as BundledTheme).BaseTheme = x;
        }
        public static bool isDarkMode()
        {
            if ((App.Current.Resources.MergedDictionaries[0] as BundledTheme).BaseTheme == BaseTheme.Dark)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
