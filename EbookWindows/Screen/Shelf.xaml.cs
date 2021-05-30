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
        public string ShelfTitle = "ONLINE BOOKS";

        public Shelf()
        {
            InitializeComponent();
            LoadDataBookShelf();
            ShelftitleBox.Text = ShelfTitle;
        }
        public void LoadDataBookShelf()
        {
            var path_data = App.Global.Directory_Folder + "\\data\\book";

            if (!Directory.Exists(path_data))
            {
                Directory.CreateDirectory(path_data);
            }
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
                        App.Global.shelfTag.Add(new ShelfTag() { Title = root.book_name, img_dir = item1 + "\\img.jpg", book_dir = item1 });
                    }
                }
            }
            this.Dispatcher.Invoke(() => {
                lbTodoList.ItemsSource = App.Global.shelfTag;
            });



        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //ScrollList.ScrollToHorizontalOffset(ScrollList.HorizontalOffset + 150);

        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            //ScrollList.ScrollToHorizontalOffset(ScrollList.HorizontalOffset - 150);
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

        private void lbTodoList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (lbTodoList.Items.Count == 0)
                return;
            var list = lbTodoList;
            var t = (sender as Grid).ActualWidth;
            double x = (double)(list.ItemContainerStyle.Setters[0] as Setter).Value;
            double y = (double)(list.ItemContainerStyle.Setters[1] as Setter).Value;
            Style style = new Style(typeof(ListViewItem), (Style)Application.Current.FindResource("MaterialDesignListBoxItem"));
            if (t >= 1250)
            {
                if (x != 250)
                {
                    style.Setters.Add(new Setter(Control.WidthProperty, 250d));
                    style.Setters.Add(new Setter(Control.HeightProperty, 400d));
                    style.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(5)));
                    lbTodoList.ItemContainerStyle = style;
                }
            }
            else if (t >= 1000)
            {
                if (x != 200)
                {
                    style.Setters.Add(new Setter(Control.WidthProperty, 200d));
                    style.Setters.Add(new Setter(Control.HeightProperty, 360d));
                    style.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(5)));
                    style.Setters.Add(new Setter(Control.MarginProperty, new Thickness(0, 5, 0, 5)));
                    lbTodoList.ItemContainerStyle = style;
                }
            }
            
            else if (t >= 750)
            {
                if (x != 150)
                {
                    style.Setters.Add(new Setter(Control.WidthProperty, 150d));
                    style.Setters.Add(new Setter(Control.HeightProperty, 240d));
                    style.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(5)));
                    style.Setters.Add(new Setter(Control.MarginProperty, new Thickness(0, 5, 0, 5)));
                    lbTodoList.ItemContainerStyle = style;
                }
            }
            else if (t < 750)
            {
                if (x != 100)
                {
                    style.Setters.Add(new Setter(Control.WidthProperty, 100d));
                    style.Setters.Add(new Setter(Control.HeightProperty, 160d));
                    style.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(5)));
                    style.Setters.Add(new Setter(Control.MarginProperty, new Thickness(0,5,0,5)));
                    lbTodoList.ItemContainerStyle = style;
                }
            }
        }
    } 
}
