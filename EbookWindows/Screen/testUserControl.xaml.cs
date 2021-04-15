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

namespace EbookWindows.Screen
{
    /// <summary>
    /// Interaction logic for testUserControl.xaml
    /// </summary>
    public partial class testUserControl : Window
    {
        public testUserControl()
        {
            InitializeComponent();
        }

        private void openBTN_Clicked(object sender, RoutedEventArgs e)
        {
            pdfReader.Children.Clear();
            Pdf_ReadingScreen pdfReaderScreen = new Pdf_ReadingScreen("C:\\Users\\Bi\\Downloads\\Documents\\EBook\\NapBien.pdf");
            pdfReader.Children.Add(pdfReaderScreen);

        }
    }
}
