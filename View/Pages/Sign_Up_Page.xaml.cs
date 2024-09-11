using EchoVibe.Backend.Classes;
using EchoVibe.Backend.Services;
using EchoVibe.View.App_Windows;
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
using EchoVibe.View.Control_User.ScrollViewers;
using System.Threading;


namespace EchoVibe.View.Pages
{
    
    public partial class Sign_Up_Page : Page
    {
        public Sign_Up_Page()
        {
            InitializeComponent();

            for (int year = 2023; year >= 1900; year--)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = year,
                    FontSize = 20
                };

                Year_Box.Items.Add(item);
            }
            for (int month = 1; month <= 12; month++)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = month,
                    FontSize = 20
                };

                Month_Box.Items.Add(item);
            }
            for (int day = 1; day <= 31; day++)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = day,
                    FontSize = 20
                };

                Day_Box.Items.Add(item);
            }

        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is Entry_Window mainWindow)
                mainWindow.Close();
        }


        
        private void Create_Button_Click(object sender, RoutedEventArgs e)
        {
            string inputProblems = null;
            First_Name_Box.Text = First_Name_Box.Text.Trim();
            Last_Name_Box.Text = Last_Name_Box.Text.Trim();

            if (string.IsNullOrEmpty(First_Name_Box.Text))
                inputProblems += "A valid first name is required!\n";

            if (string.IsNullOrEmpty(Last_Name_Box.Text))
                inputProblems += "A valid last name is required!\n";

            if (Password_Box.Password.Length < 8 || string.IsNullOrEmpty(Password_Box.Password))
                inputProblems += "The password must consist of at least of a combination of 8 characters or numbers!\n";

            if (Password_Box.Password != Confirm_pass.Password)
                inputProblems += "The two passwords don't match!\n";

            if (string.IsNullOrEmpty(Day_Box.Text.Trim()))
                inputProblems += "Day is required!\n";

            if (string.IsNullOrEmpty(Month_Box.Text.Trim()))
                inputProblems += "Month is required!\n";

            if (string.IsNullOrEmpty(Year_Box.Text.Trim()))
                inputProblems += "Year is required!\n";

            if (string.IsNullOrEmpty(Gender_Box.Text))
                inputProblems += "Gender is required!\n";

            if (CheckBox.IsChecked == false)
                inputProblems += "To proceed, you must agree to the terms and conditions!\n";


            if (inputProblems != null)
            {
                MessageBox.Show(inputProblems);
                return;
            }




            string fullName = First_Name_Box.Text + " " + Last_Name_Box.Text;
            string password = Password_Box.Password;

            DateTime dateOfBirth = new DateTime(Convert.ToInt32(Year_Box.Text), Convert.ToInt32(Month_Box.Text), Convert.ToInt32(Day_Box.Text));
            dateOfBirth = dateOfBirth.Date;
            char gender = Convert.ToChar(Gender_Box.Text);


            bool isCaseSensitive = true;
            List<int> ids = UserService.SearchUserBasedOnFullName(fullName, isCaseSensitive);

            int primaryKey = 0;
            foreach (int id in ids)
            {
                User user = UserService.FetchUser(id);
                string userPassword = user.Password;
                userPassword = EncryptService.Decrypt(userPassword);
                if (password == userPassword)
                {
                    primaryKey = id;
                    break;
                }
            }


            if (primaryKey != 0)
            {
                MessageBox.Show("Account already exists!\nEnter a unique name and password combination");
                

            }
            else
            {
                string EncryptedPassword = EncryptService.Encrypt(password);
                User myUser = new User(fullName, EncryptedPassword, dateOfBirth, gender);

                Home_Window newHomeWindow = new Home_Window(myUser);

                newHomeWindow.Show();
                if (Window.GetWindow(this) is Entry_Window mainWindow)
                    mainWindow.Close();
            }

        }


        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is Entry_Window mainWindow)
                mainWindow.Main_Frame.Content = new Sign_In_Page();
        }
    }
}
