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
    }
}
