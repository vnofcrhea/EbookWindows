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

namespace EbookWindows.View
{
    /// <summary>
    /// Interaction logic for PasswordScreen.xaml
    /// </summary>
    public partial class PasswordScreen : Window
    {
        public delegate bool PasswordHandle(string passwordStr);
        public event PasswordHandle EnterPasswordEvent;

        public PasswordScreen()
        {
            InitializeComponent();
        }

        private void DragStart(object sender, MouseEventArgs e)
        {

        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CancleBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// Send password from PasswordScreen to Pdf_ReadingScreen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            string passwordStr = PasswordBox.Password.Trim();
            if (string.IsNullOrEmpty(passwordStr))
            {
                MessageBox.Show("Invalid password. Please enter password again","",MessageBoxButton.OK,MessageBoxImage.Warning);
            }
            else
            {
                bool result = false; //
                if (EnterPasswordEvent != null)
                {
                    result = EnterPasswordEvent(passwordStr);
                }
                else
                {
                    this.Close();
                }
                if (result)
                {
                    this.Close();
                }
            }
        }
    }
}
