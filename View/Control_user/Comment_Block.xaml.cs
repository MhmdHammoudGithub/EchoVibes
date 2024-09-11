using EchoVibe.Backend.Classes;
using EchoVibe.Backend.Services;
using EchoVibe.View.App_Windows;
using EchoVibe.View.Control_User.ScrollViewers;
using EchoVibe.View.Pages;
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
using System.Xml.Linq;

namespace EchoVibe.View.Control_User
{
    
    public partial class Comment_Block : UserControl
    {

        public Comment TheComment { get; set; }
        public User CommentCreator{ get; set; }
        public User Actor { get; set; }

        private User_Block_ScrollViewer_For_CommentLikes TheLikesScrollViewer;


        private bool isLikesVisible;
        public bool IsLikesVisible
        {
            get { return isLikesVisible; }
            set
            {
                if (isLikesVisible != value)
                {
                    isLikesVisible = value;
                    OnPropertyChanged(nameof(IsLikesVisible));
                }
            }
        }
        
        private Visibility number_Of_Likes_Content;
        public Visibility Number_Of_Likes_Content
        {
            get { return number_Of_Likes_Content; }
            set { number_Of_Likes_Content = value; OnPropertyChanged(); }
        }


        public Comment_Block(Comment comment, User actor, User commentCreator)
        {
            InitializeComponent();
            DataContext = this;
            this.TheComment = comment;
            this.CommentCreator = commentCreator;
            this.Actor = actor;
            if (this.CommentCreator.UserId != this.Actor.UserId)
                Delete_Comment_Button.Visibility = Visibility.Collapsed;
            else
                Delete_Comment_Button.Visibility = Visibility.Visible;
            

            Showing_Comment_Block();
        }

        public event PropertyChangedEventHandler PropertyChanged = null;
        private void OnPropertyChanged([CallerMemberName] string propertyname = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }


        private void Showing_Comment_Block()
        {
            Commentator_Name_Button.Content = this.CommentCreator.FullName;
            Comment_Content.Text = this.TheComment.Content;
            Comment_Date.Text = this.TheComment.DateOfComment.ToString();
            Number_of_Likes.Content = Convert.ToString(this.TheComment.LikeCounter) + " Likes";
            int decision = CommentLikeService.SearchLikeBasedOnCommentAndUserIds(this.TheComment.CommentId, this.Actor.UserId);

            if (decision != 0)
                Like_Button.Foreground = Brushes.Blue;

            else
                Like_Button.Foreground = Brushes.Black;
        }

        private void Like_Button_Click(object sender, RoutedEventArgs e)
        {
            CommentService.ToggleLike(this.TheComment.CommentId,this.Actor.UserId, CommentService.GetCommentNumberOfLikes(this.TheComment.CommentId));
            int decision = CommentLikeService.SearchLikeBasedOnCommentAndUserIds(this.TheComment.CommentId, this.Actor.UserId);

            if (decision != 0)
                Like_Button.Foreground = Brushes.Blue;

            else
                Like_Button.Foreground = Brushes.Black;
            this.TheLikesScrollViewer = new User_Block_ScrollViewer_For_CommentLikes(this.Actor, this.TheComment);
            Comment_Likes_Grid.Children.Add(this.TheLikesScrollViewer);
            Number_of_Likes.Content = Convert.ToString(CommentService.GetCommentNumberOfLikes(this.TheComment.CommentId)) + " Likes";


        }

        private void Number_of_Likes_Click(object sender, RoutedEventArgs e)
        {
            this.TheLikesScrollViewer = new User_Block_ScrollViewer_For_CommentLikes(this.Actor, this.TheComment);
            Comment_Likes_Grid.Children.Add(this.TheLikesScrollViewer);
            Number_of_Likes.Content = Convert.ToString(CommentService.GetCommentNumberOfLikes(this.TheComment.CommentId)) + " Likes";
            if (IsLikesVisible)
                CommentLikes.Visibility = Visibility.Visible;
            else
                CommentLikes.Visibility = Visibility.Collapsed;

        }

        private void Commentator_Name_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is Home_Window homeWindow)
            {
                if (Actor.UserId == CommentCreator.UserId)
                    homeWindow.Main_Frame.Content = new MyProfile_Page(Actor);
                else
                    homeWindow.Main_Frame.Content = new Profile_Page(Actor, CommentCreator);
            }
        }

        private void Delete_Comment_Button_Click(object sender, RoutedEventArgs e)
        {
            UserService.DeleteComment(this.TheComment.CommentId);
        }
    }
}
