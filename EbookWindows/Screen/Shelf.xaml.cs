using EbookWindows.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EbookWindows.Screen
{
    /// <summary>
    /// Interaction logic for RecentBook.xaml
    /// </summary>
    public partial class Shelf : UserControl
    {
        public string ShelfTitle = "RECENT BOOKS";
        public Shelf()
        {
            InitializeComponent();
            LoadDataBookShelf();
        }
        public void LoadDataBookShelf()
        {
            List<ShelfTag> shelfTags = new List<ShelfTag>();
            var path_data = App.path + "\\data\\book";
            string[] subdirectoryEntries = Directory.GetDirectories(path_data);
            foreach (var item in subdirectoryEntries)
            {
                var sub1 = Directory.GetDirectories(item);
                foreach (var item1 in sub1)
                {
                    using (StreamReader file = File.OpenText(item1 + "\\detail.json"))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        Root root = (Root)serializer.Deserialize(file, typeof(Root));
                        shelfTags.Add(new ShelfTag() { Title = root.book_name, img_dir = item1 + "\\img.jpg",book_dir = item1});
                    }
                }
            }
            lbTodoList.ItemsSource = shelfTags;


        }    
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScrollList.ScrollToHorizontalOffset(ScrollList.HorizontalOffset + 150);
            
        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            ScrollList.ScrollToHorizontalOffset(ScrollList.HorizontalOffset - 150);
        }

        private void DetailScreen_Click(object sender, RoutedEventArgs e)
        {
            
            //WindowScreen win = (WindowScreen)Window.GetWindow(this);
            //win.OpenDetailScreen();
        }

        private void lbTodoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var x = (sender as ListView);
                if (x.SelectedIndex != -1)
            {
                WindowScreen win = (WindowScreen)Window.GetWindow(this);
                win.OpenDetailScreen(x.SelectedItem as ShelfTag);
                lbTodoList.SelectedIndex = -1;
            }
        }

        
    }
}
