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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EbookWindows.Screen
{
    /// <summary>
    /// Interaction logic for RecentBook.xaml
    /// </summary>
    public partial class RecentBook : UserControl
    {
        public RecentBook()
        {
            InitializeComponent(); List<TodoItem> items = new List<TodoItem>();
            items.Add(new TodoItem() { Title = "Complete this WPF tutorial", Completion = 45 });
            items.Add(new TodoItem() { Title = "Learn C#", Completion = 80 });
            items.Add(new TodoItem() { Title = "Wash the car", Completion = 0 });
            items.Add(new TodoItem() { Title = "Wash the car", Completion = 0 });
            items.Add(new TodoItem() { Title = "Wash the car", Completion = 0 });
            items.Add(new TodoItem() { Title = "Wash the car", Completion = 0 });
            items.Add(new TodoItem() { Title = "Wash the car", Completion = 0 });
            items.Add(new TodoItem() { Title = "Wash the car", Completion = 0 });
            items.Add(new TodoItem() { Title = "Wash the car", Completion = 0 });
            items.Add(new TodoItem() { Title = "Wash the car", Completion = 0 });
            lbTodoList.ItemsSource = items;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScrollList.ScrollToHorizontalOffset(ScrollList.HorizontalOffset + 150);
        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            ScrollList.ScrollToHorizontalOffset(ScrollList.HorizontalOffset - 150);
        }
    }
}
