using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookWindows.Model;
using System.IO;
using MaterialDesignThemes.Wpf;
using System.Windows.Media;
using MaterialDesignColors;
using MaterialDesignColors.ColorManipulation;
using Newtonsoft.Json;

namespace EbookWindows.ViewModels
{
    public class Settings_ViewModel
    {
        private Settings _settings = new Settings();
        private readonly PaletteHelper _paletteHelper = new PaletteHelper();
        public Settings_ViewModel()
        {
            try
            {
                if (File.Exists(App.Global.Directory_Folder + "\\data\\Settings.json"))
                {
                    using (StreamReader file = File.OpenText(App.Global.Directory_Folder + "\\data\\Settings.json"))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        Settings = (Settings)serializer.Deserialize(file, typeof(Settings));
                    }
                }
                else
                {
                    _settings.BaseTheme = BaseTheme.Dark;
                    _settings.PrimaryColor = Color.FromRgb(0, 200, 255);
                    _settings.SecondaryColor = Color.FromRgb(50, 205, 50);
                }
            }
            catch
            {
                _settings.BaseTheme = BaseTheme.Dark;
                _settings.PrimaryColor = Color.FromRgb(0, 200, 255);
                _settings.SecondaryColor = Color.FromRgb(50, 205, 50);
            }
        }
        public Settings Settings
        {
            get { return _settings; }
            set { _settings = value;}
        }
        
        public void ApplySetting()
        {
            ApplyBaseTheme();
            ApplyPrimaryColor();
            ApplySecondaryColor();
            //(App.Current.Resources.MergedDictionaries[0] as CustomColorTheme).BaseTheme = _settings.BaseTheme;
            //(App.Current.Resources.MergedDictionaries[0] as CustomColorTheme).PrimaryColor = _settings.PrimaryColor;
            //(App.Current.Resources.MergedDictionaries[0] as CustomColorTheme).SecondaryColor = _settings.SecondaryColor;
        }
        public void SaveSetting()
        {
            var path_data = App.Global.Directory_Folder + "\\data";
            #region create path. 
            if (!Directory.Exists(path_data))
            {
                Directory.CreateDirectory(path_data);
            }
            #endregion
            File.WriteAllText(path_data + "\\" + "Settings.json", JsonConvert.SerializeObject(_settings));
            //Save new settings to folder. 
        }
        public BaseTheme BaseTheme
        {
            get { return _settings.BaseTheme; }
            set { if (_settings.BaseTheme != value) { _settings.BaseTheme = value; ApplyBaseTheme(); } }
        }
        public Color PrimaryColor
        {
            get { return _settings.PrimaryColor; }
            set { if (_settings.PrimaryColor != value) { _settings.PrimaryColor = value; ApplyPrimaryColor(); } }
        }
        public Color SecondaryColor
        {
            get { return _settings.SecondaryColor; }
            set { if (_settings.SecondaryColor != value) { _settings.SecondaryColor = value; ApplySecondaryColor(); } }
        }
        public bool IsDarkTheme
        {
            get { if (_settings.BaseTheme == BaseTheme.Dark) return true; else return false; }
            set { if (value) BaseTheme = BaseTheme.Dark; else BaseTheme = BaseTheme.Light; }
        }
        
        private void ApplyBaseTheme()
        {
            ITheme theme = _paletteHelper.GetTheme();
            theme.SetBaseTheme(_settings.BaseTheme.GetBaseTheme());
            if (_settings.BaseTheme == BaseTheme.Light)
                theme.Paper = Color.FromRgb(246, 244, 236);
            _paletteHelper.SetTheme(theme);
        }
        private void ApplyPrimaryColor()
        {
            ITheme theme = _paletteHelper.GetTheme();
            theme.PrimaryLight = new ColorPair(_settings.PrimaryColor.Lighten());
            theme.PrimaryMid = new ColorPair(_settings.PrimaryColor);
            theme.PrimaryDark = new ColorPair(_settings.PrimaryColor.Darken());
            _paletteHelper.SetTheme(theme);
        }
        private void ApplySecondaryColor()
        {
            ITheme theme = _paletteHelper.GetTheme();
            theme.PrimaryLight = new ColorPair(_settings.PrimaryColor.Lighten());
            theme.PrimaryMid = new ColorPair(_settings.PrimaryColor);
            theme.PrimaryDark = new ColorPair(_settings.PrimaryColor.Darken());
            _paletteHelper.SetTheme(theme);
        }
    }
}
