using EchoVibe.Backend.Classes;
using EchoVibe.Backend.Services;
using EchoVibe.View.Control_User;
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

namespace EchoVibe.View.Pages
{

    public partial class Home_Page : Page
    {
        public User TheUser { get; set; } // TheUser is the owner of the account currently active, he will
                                          // be the actor of any actions that will take place on his account

        

        private Post_Block_ScrollViewer_For_HomePage TheScrollViewer;

        public Home_Page(User user)
        {
            InitializeComponent();
            this.TheUser = user;// this will be the actor later on

            this.TheScrollViewer = new Post_Block_ScrollViewer_For_HomePage(this.TheUser);

            Post_Blocks_Grid.Children.Add(TheScrollViewer);
            
        }

    }
}
