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
        public static Global Global { get; set; } = new Global(); //Dynamic Global Data Use for all Windows,UserControl
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
