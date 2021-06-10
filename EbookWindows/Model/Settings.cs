using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
namespace EbookWindows.Model
{
    public class Settings
    {
        public BaseTheme BaseTheme { get; set; } //(Light/Dark)
        public Color PrimaryColor { get; set; }
        public Color SecondaryColor { get; set; }
        public Settings()
        {
            
        }    
    }
}
