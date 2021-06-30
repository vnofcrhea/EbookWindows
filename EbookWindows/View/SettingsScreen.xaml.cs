using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EbookWindows.Model;
namespace EbookWindows.Screen
{
    /// <summary>
    /// Interaction logic for SettingsScreen.xaml
    /// </summary>
    public partial class SettingsScreen : UserControl
    {
        public SettingsScreen()
        {
            InitializeComponent();
            DataContext = App.Global.Settings_ViewModel;
        }
        private void CloseSetting_Click(object sender, RoutedEventArgs e)
        {
            (App.Current.MainWindow as WindowScreen).SettingClose();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            App.Global.Settings_ViewModel.GetUserCredential();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            App.Global.Settings_ViewModel.RevokeToken();
        }

        private void UploadSyncData_Click(object sender, RoutedEventArgs e)
        {
            App.Global.Settings_ViewModel.UploadSyncData();
        }

        private void DownloadSyncData_Click(object sender, RoutedEventArgs e)
        {
            App.Global.Settings_ViewModel.DownLoadSyncData();

        }
    }
    public class LoginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool && (bool)value)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return parameter;
        }
    }
    public class LoggedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool && (bool)value)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return parameter;
        }
    }
}
