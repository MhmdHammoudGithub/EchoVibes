using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EchoVibe.Backend.Classes;
using EchoVibe.View.Custom_Events;


namespace EchoVibe.View.Control_User
{
    public partial class Left_Panel : UserControl
    {
        public User TheUser { get; set; }// TheUser is the owner of the account currently active, he will
                                         // be the actor of any actions that will take place on his account
        public Left_Panel(User theUser)
        {
            InitializeComponent();
            DataContext = this;
            this.TheUser = theUser;
            Home_Button.IsChecked = true;
            User_Name_TextBlock.Text = TheUser.FullName;
        }



       

        public event EventHandler<ToggleButtonEventArgs> PageToggleButtonEventChecked;

        private void Page_Toggle_Button_Checked(object sender, RoutedEventArgs e)
        {
            var clickedToggleButton = (ToggleButton)sender;
            
            // Uncheck all ToggleButtons
            foreach (var toggleButton in new List<ToggleButton> { Home_Button, MyProfile_Button, Search_Button })
            {

                if (toggleButton != sender && toggleButton.IsChecked == true)
                {
                    toggleButton.IsChecked = false;
                }
            }

            // Trigger the event to notify the parent window about the checked ToggleButton
            PageToggleButtonEventChecked?.Invoke(this, new ToggleButtonEventArgs((ToggleButton)sender));

        }




        public event EventHandler MyFriendsToggleButtonEventClicked;
        private void Friends_ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            MyFriendsToggleButtonEventClicked?.Invoke(this, EventArgs.Empty);

        }

        public event EventHandler SearchButtonEventClicked;

        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            SearchButtonEventClicked?.Invoke(this, EventArgs.Empty);
        }


        public event EventHandler CreatePostEventClicked;
        private void Create_Post_Button_Click(object sender, RoutedEventArgs e)
        {
            CreatePostEventClicked?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler LogOutEventClicked;
        private void Logout_Button_Click(object sender, RoutedEventArgs e)
        {
            LogOutEventClicked?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler RefreshButtonEventClicked;
        private void Refresh_Button_Click(object sender, RoutedEventArgs e)
        {
            RefreshButtonEventClicked?.Invoke(this, EventArgs.Empty);
        }

       
    }


}
