using EchoVibe.Backend.Classes;
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
using EchoVibe.Backend.Services;


namespace EchoVibe.View.Pages
{
    public partial class Search_Result_Page : Page
    {
        public User TheUser { get; set; }// TheUser is the owner of the account currently active, he will
                                         // be the actor of any actions that will take place on his account


        private User_Block_ScrollViewer_For_Profiles TheScrollViewer;

        public Search_Result_Page(User user)
        {
            InitializeComponent();
            this.TheUser = user;

            this.Searched_UserName.EnterPressed += Do_The_Search;
            this.Searched_UserName.SearchButton_Event_Clicked += Do_The_Search;

            
        }

        private void Do_The_Search(object sender, EventArgs e)
        {
            string nameToSearch = this.Searched_UserName.Text.Trim();
            UserService.SearchUserBasedOnFullName(nameToSearch, false);

            this.TheScrollViewer = new User_Block_ScrollViewer_For_Profiles(this.TheUser, nameToSearch);
            Grid.SetRow(this.TheScrollViewer, 1);
            Search_Result_Page_Grid.Children.Add(this.TheScrollViewer);
        }
    }
}
