using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EbookWindows.Screen
{
    /// <summary>
    /// Interaction logic for DetailScreen.xaml
    /// </summary>
    public partial class DetailScreen : UserControl
    {
        public Root items = new Root();
        public DetailScreen()
        {
            InitializeComponent(); 
            
        }

        public void LoadData() //Load data here
        {
        }
        public void LoadData(int i) //Load data here

        {
            if (i == 1)
            {
                var json = new WebClient().DownloadString("http://127.0.0.1:5000/api/v1/books?url=https://truyen.tangthuvien.vn/doc-truyen/phong-cuong-tam-ly-su");
                items = JsonConvert.DeserializeObject<Root>(json);
                lvDataBinding.ItemsSource = items.chapter_name;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }

    public class User
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public string Mail { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Root
    {
        public string img_url { get; set; }
        public string book_info { get; set; }
        public List<string> chapter_name { get; set; }
        public List<string> link { get; set; }
        public List<string> season { get; set; }
        public List<int> season_index { get; set; }
    }
}
