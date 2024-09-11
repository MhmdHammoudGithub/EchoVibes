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


namespace EchoVibe.View.Pages
{
    public partial class MyProfile_Page : Page
    {
        public User TheUser { get; set; }// TheUser is the owner of the account currently active, he will
                                         // be the actor of any actions that will take place on his account

        private User_Block TheMyProfileUserBlock { get; set; }
        

        private Post_Block_ScrollViewer_For_A_ProfilePage TheScrollViewer {  get; set; }

        public MyProfile_Page(User user)
        {
            InitializeComponent();
            this.TheUser = user;// this will be the actor later on
            this.TheScrollViewer = new Post_Block_ScrollViewer_For_A_ProfilePage(this.TheUser, this.TheUser);

            this.TheMyProfileUserBlock = new User_Block(this.TheUser,this.TheUser);
            this.TheMyProfileUserBlock.SetGridVisibility("MyProfileGrid");
            Grid.SetRow(this.TheMyProfileUserBlock,0);
            Grid.SetRow(this.TheScrollViewer,1);

            My_Profile_Grid.Children.Add(this.TheMyProfileUserBlock);
            My_Profile_Grid.Children.Add(this.TheScrollViewer);
        }
    }
}
