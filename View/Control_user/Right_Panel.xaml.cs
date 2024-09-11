using EchoVibe.Backend.Classes;
using EchoVibe.Backend.Services;
using EchoVibe.View.Control_User.ScrollViewers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace EchoVibe.View.Control_User
{
    public partial class Right_Panel : UserControl
    {
        public User TheUser { get; set; }
        private User_Block_ScrollViewer_For_Friends TheFriendsScrollViewer;
        private User_Block_ScrollViewer_For_PotentialFriends ThePotentialFriendsScrollViewer;
        public Right_Panel(User user)
        {
            InitializeComponent();
            TheUser = user;
            this.TheFriendsScrollViewer = new User_Block_ScrollViewer_For_Friends(this.TheUser);
            this.ThePotentialFriendsScrollViewer = new User_Block_ScrollViewer_For_PotentialFriends(this.TheUser);
            Grid.SetRow(ThePotentialFriendsScrollViewer, 1);
            Grid.SetRow(TheFriendsScrollViewer, 1);
            My_Friends_Grid.Children.Add(this.TheFriendsScrollViewer);
            My_Potential_Friends_Grid.Children.Add(this.ThePotentialFriendsScrollViewer);
        }

        public void Refresh_Button_Click(object sender, RoutedEventArgs e)
        {
            My_Friends_Grid.Children.Remove(this.TheFriendsScrollViewer);
            My_Potential_Friends_Grid.Children.Remove(this.ThePotentialFriendsScrollViewer);


            this.TheFriendsScrollViewer = new User_Block_ScrollViewer_For_Friends(this.TheUser);
            this.ThePotentialFriendsScrollViewer = new User_Block_ScrollViewer_For_PotentialFriends(this.TheUser);
            Grid.SetRow(ThePotentialFriendsScrollViewer, 1);
            Grid.SetRow(TheFriendsScrollViewer, 1);
            My_Friends_Grid.Children.Add(this.TheFriendsScrollViewer);
            My_Potential_Friends_Grid.Children.Add(this.ThePotentialFriendsScrollViewer);
        }

    }
}
