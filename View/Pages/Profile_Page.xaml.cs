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
using EchoVibe.View.Control_User;
using EchoVibe.Backend.Services;


namespace EchoVibe.View.Pages
{
    public partial class Profile_Page : Page
    {
        public User TheUser { get; set; }// TheUser is the owner of the account currently active, he will
                                         // be the actor of any actions that will take place on his account
        public User TheOwner { get; set; }// TheOwner is the owner of the profile on the profilePage
        private User_Block TheProfileUserBlock { get; set; }

        private Post_Block_ScrollViewer_For_A_ProfilePage TheScrollViewer {  get; set; }

        public Profile_Page(User user,User owner)
        {
            InitializeComponent();
            this.TheUser = user;
            this.TheOwner = owner;
            this.TheProfileUserBlock = new User_Block(this.TheOwner,this.TheUser);

            this.TheScrollViewer = new Post_Block_ScrollViewer_For_A_ProfilePage(this.TheUser, this.TheOwner);
            this.TheProfileUserBlock.SetGridVisibility("ProfileGrid");
            string status = FriendService.GetFriendStatusUserRelativeUserBlock(this.TheOwner.UserId, this.TheUser.UserId);
            this.TheProfileUserBlock.SetSearchOrProfileGridVisibility(status);
            Grid.SetRow(this.TheProfileUserBlock, 0);
            Grid.SetRow(this.TheScrollViewer, 1);

            Profile_Page_Grid.Children.Add(this.TheProfileUserBlock);
            Profile_Page_Grid.Children.Add(this.TheScrollViewer);
        }
    }
}
