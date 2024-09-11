using EchoVibe;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using EchoVibe.Backend.Classes;
using EchoVibe.Backend.Services;
using EchoVibe.View.App_Windows;
using EchoVibe.View.Control_User;
using EchoVibe.View.Pages;
using System.Windows.Controls.Primitives;
using EchoVibe.View.Custom_Events;

namespace EchoVibe.View.App_Windows
{
    public partial class Home_Window : Window
    {
        public User TheUser { get; set; }// TheUser is the owner of the account currently active, he will
                                         //  be the actor of any actions that will take place on his account
        public Left_Panel Left_Panel_Instance { get; set; }
        private Right_Panel Right_Panel_Instance { get; set; }

        public Home_Window(User user)
        {
            InitializeComponent();
            this.TheUser = user;
            Left_Panel_Instance = new Left_Panel(this.TheUser);
            Right_Panel_Instance = new Right_Panel(this.TheUser);
            Left_Panel_Instance.CreatePostEventClicked += LeftPanel_CreatePostEventClicked;
            Left_Panel_Instance.LogOutEventClicked += LeftPanel_LogOutEventClicked;
            Left_Panel_Instance.MyFriendsToggleButtonEventClicked += LeftPanel_MyFriendsToggleButtonEventClicked;
            Left_Panel_Instance.PageToggleButtonEventChecked += LeftPanel_PageToggleButtonEventChecked;
            Left_Panel_Instance.RefreshButtonEventClicked += LeftPanel_RefreshButtonEventClicked;
            SetActivePage(PageType.Home);

            // Set the Grid.Column property for the Left_Panel instance
            Grid.SetColumn(Left_Panel_Instance, 0);

            // Add the Left_Panel instance to the grid
            Home_Screen.Children.Add(Left_Panel_Instance);
            Right_Panel_Grid.Children.Add(Right_Panel_Instance);


        }


        private void LeftPanel_MyFriendsToggleButtonEventClicked(object sender, EventArgs e)
        {
            Right_Expander.IsExpanded = !Right_Expander.IsExpanded;
            Right_Panel_Instance.Refresh_Button_Click(null, null);

        }

        private void LeftPanel_LogOutEventClicked(object sender, EventArgs e)
        {
            Entry_Window entry_Window = new Entry_Window();
            entry_Window.Show();
            this.Close();
        }

        private void LeftPanel_CreatePostEventClicked(object sender, EventArgs e)
        {
            Window post_Window = new Create_Post_Window(this.TheUser);
            this.Opacity = 0.8;
            post_Window.Owner = this;
            post_Window.ShowDialog();
            this.Opacity = 1;

            switch (Main_Frame.Content)
            {
                case Home_Page _:
                    Main_Frame.Content = new Home_Page(TheUser);
                    break;
                case MyProfile_Page _:
                    Main_Frame.Content = new MyProfile_Page(TheUser);
                    break;
                case Search_Result_Page _:
                    Main_Frame.Content = new Search_Result_Page(TheUser);
                    break;
                default:
                    
                    break;
            }
        }

        public void LeftPanel_RefreshButtonEventClicked(object sender, EventArgs e)
        {
            switch (Main_Frame.Content)
            {
                case Home_Page _:
                    Main_Frame.Content = new Home_Page(TheUser);
                    break;
                case MyProfile_Page _:
                    Main_Frame.Content = new MyProfile_Page(TheUser);
                    break;
                case Search_Result_Page _:
                    Main_Frame.Content = new Search_Result_Page(TheUser);
                    break;
                default:
                    break;
            }



        }
        public void LeftPanel_RefreshButtonEventClicked()
        {
            switch (Main_Frame.Content)
            {
                case Home_Page _:
                    Main_Frame.Content = new Home_Page(TheUser);
                    break;
                case MyProfile_Page _:
                    Main_Frame.Content = new MyProfile_Page(TheUser);
                    break;
                case Search_Result_Page _:
                    Main_Frame.Content = new Search_Result_Page(TheUser);
                    break;
                default:
                    break;
            }



        }

        private void LeftPanel_PageToggleButtonEventChecked(object sender, ToggleButtonEventArgs e)
        {
            var clickedToggleButton = e.ToggledButton;

            // Set the active page based on the clicked ToggleButton
            if (clickedToggleButton.IsChecked == true)
            {
                SetActivePage(GetPageTypeFromToggleButton(clickedToggleButton));
            }
        }

        private void SetActivePage(PageType page)
        {
      
            switch (page)
            {
                case PageType.Home:
                    {
                        Main_Frame.Content = new Home_Page(TheUser);
                        break;
                    }
                case PageType.MyProfile:
                    {
                        Main_Frame.Content = new MyProfile_Page(TheUser);
                        break;
                    }

                case PageType.Search:
                    {
                        Main_Frame.Content = new Search_Result_Page(TheUser);
                        break;
                    }
            }
        }

        private PageType GetPageTypeFromToggleButton(ToggleButton toggleButton)
        {


            if (toggleButton == Left_Panel_Instance.Home_Button)
                return PageType.Home;
            else if (toggleButton == Left_Panel_Instance.MyProfile_Button)
                return PageType.MyProfile;
            else if (toggleButton == Left_Panel_Instance.Search_Button)
                return PageType.Search;

            // Default to Home if not recognized
            return PageType.Home;
        }

        private void Right_Expander_Expanded(object sender, RoutedEventArgs e)
        {
            Right_Panel_Instance.Refresh_Button_Click(null, null);

        }








    }
    public enum PageType
    {
        Home,
        MyProfile,
        Search
    }
}