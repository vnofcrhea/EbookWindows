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

        public App()
        {
            Global.Settings_ViewModel = new Settings_ViewModel();
            //ApplyTheme();
            Global.Book_Short_ViewModel.LoadListBookShort();
        }
        public static bool isDarkMode()
        {
            if ((App.Current.Resources.MergedDictionaries[0] as CustomColorTheme).BaseTheme == BaseTheme.Dark)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void ChangeBaseTheme(BaseTheme x)
        {
            (App.Current.Resources.MergedDictionaries[0] as CustomColorTheme).BaseTheme = x;
            //(App.Current.Resources.MergedDictionaries[0] as CustomColorTheme).PrimaryColor = System.Windows.Media.Color.FromRgb(00,200,255);
        }
        public static void ApplyTheme()
        {
            App.Global.Settings_ViewModel.ApplySetting();
        }
    }
    
}
