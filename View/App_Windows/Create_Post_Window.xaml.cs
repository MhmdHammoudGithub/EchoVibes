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
using EchoVibe.Backend.Classes;
using EchoVibe.Backend.Services;
using EchoVibe.View.App_Windows;
using EchoVibe.View.Control_User;
using EchoVibe.View.Control_User.ScrollViewers;


namespace EchoVibe.View.App_Windows
{
    
    public partial class Create_Post_Window : Window
    {
        public User Poster { get; set; }
        public Create_Post_Window(User poster)
        {
            InitializeComponent();
            this.Poster = poster;
        }


        private void Send_Button_Click(object sender, RoutedEventArgs e)
        {
            string content = Content_TextBox.Text.Trim();
            UserService.CreatePost(Poster.UserId, content);
            this.Close();
            
        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            
        }
    }
}
