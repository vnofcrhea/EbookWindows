using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EbookWindows.Model
{
    public class Settings
    {
        public BaseTheme BaseTheme { get; set; } //(Light/Dark)
        public Color PrimaryColor { get; set; }
        public Color SecondaryColor { get; set; }
        public string AccountName { get; set; }
        public bool IsLogged { get; set; }
        public Settings()
        {
            
        }    
    
    }   
    public class GoogleUserProfile
    {
        public string id { get; set; }
        public string name { get; set; }
        public string given_name { get; set; }
        public string picture { get; set; }
        public string locale { get; set; }
    }
}
