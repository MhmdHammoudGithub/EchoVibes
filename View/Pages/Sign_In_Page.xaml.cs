using EchoVibe.Backend.Classes;
using EchoVibe.Backend.Services;
using EchoVibe.View.App_Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using EchoVibe.View.Control_User.ScrollViewers;


namespace EchoVibe.View.Pages
{

    public partial class Sign_In_Page : Page
    {
        public Sign_In_Page()
        {
            InitializeComponent();
        }
        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            string inputProblems = null;

            Username_TextBox.Text = Username_TextBox.Text.Trim();

            if (string.IsNullOrEmpty(Username_TextBox.Text))
                inputProblems += "You must enter username\n";

            if (string.IsNullOrEmpty(PasswordBox.Password))
                inputProblems += "You must enter a password\n";


            else if (PasswordBox.Password.Length < 8)
                inputProblems += "The password must consist of a combination of at least 8 characters or numbers\n";

            if (inputProblems != null)
            {
                MessageBox.Show(inputProblems);
                return;
            }
            string fullName = Username_TextBox.Text;
            string password = PasswordBox.Password;
            bool isCaseSensitive = true;
            List<int> ids = UserService.SearchUserBasedOnFullName(fullName, isCaseSensitive);


            User thisUser = null;
            int primaryKey = 0;
            foreach (int id in ids)
            {
                User user = UserService.FetchUser(id);
                string userPassword = user.Password;
                userPassword = EncryptService.Decrypt(userPassword);
                if (password == userPassword)
                {
                    primaryKey = id;
                    thisUser = user;
                    break;
                }
            }


            if (thisUser == null)
            {
                MessageBox.Show("No match found!\nEnter the correct credentials ");
                return;
            }
            else
            {

                Home_Window newHomeWindow = new Home_Window(thisUser);
                newHomeWindow.Show();

                if (Window.GetWindow(this) is Window mainWindow)
                    mainWindow.Close();
                return;
            }

        }


        private void Create_Account_Button_MouseEnter(object sender, MouseEventArgs e)
        {
            Create_Account_Button.Background = Brushes.Transparent;
        }


        private void Create_Account_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is Entry_Window mainWindow)
                mainWindow.Main_Frame.Content = new Sign_Up_Page();
            return;
        }
        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is Entry_Window mainWindow)
                mainWindow.Close();
        }


    }
}
