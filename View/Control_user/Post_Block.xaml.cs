using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
using EchoVibe.Backend.Classes;
using EchoVibe.Backend.Services;
using EchoVibe.View.App_Windows;
using EchoVibe.View.Control_User;
using System.ComponentModel;
using System.Windows.Media.Animation;
using System.Runtime.CompilerServices;
using EchoVibe.View.Control_User.ScrollViewers;
using EchoVibe.View.Pages;

namespace EchoVibe.View.Control_User
{
    public partial class Post_Block : UserControl, INotifyPropertyChanged
    {
        public Post ThePost { get; set; }
        public User PostCreator { get; set; }
        public User Actor { get; set; }

        private Comment_Block_ScrollViewer TheCommentsScrollViewer;
        private User_Block_ScrollViewer_For_PostLikes TheLikesScrollViewer;


        private bool isCommentsVisible;
        public bool IsCommentsVisible
        {
            get { return isCommentsVisible; }
            set
            {
                if (isCommentsVisible != value)
                {
                    isCommentsVisible = value;
                    OnPropertyChanged(nameof(IsCommentsVisible));
                }
            }
        }

        private bool isLikesVisible;
        public bool IsLikesVisible
        {
            get { return isLikesVisible; }
            set
            {
                if (isLikesVisible != value)
                {
                    isLikesVisible = value;
                    OnPropertyChanged();
                }
            }
        }
        private string number_Of_Comments_Text;
        public string Number_Of_Comments_Text
        {
            get { return number_Of_Comments_Text; }
            set
            {
                    number_Of_Comments_Text = value;
                    OnPropertyChanged();
            }
        }
        private Visibility number_Of_Likes_Content;
        public Visibility Number_Of_Likes_Content
        {
            get { return number_Of_Likes_Content; }
            set { number_Of_Likes_Content = value; OnPropertyChanged(); }
        }

        private Visibility user_Name_Button_Content;

        public Visibility User_Name_Button_Content
        {
            get { return user_Name_Button_Content; }
            set { user_Name_Button_Content = value; OnPropertyChanged(); }
        }


        public event PropertyChangedEventHandler PropertyChanged = null;



        public Post_Block(Post post, User actor, User postCreator)
        {
            InitializeComponent();
            DataContext = this;
            this.ThePost = post;
            this.PostCreator = postCreator;
            this.Actor = actor;
            if(this.PostCreator.UserId != this.Actor.UserId)
                Delete_Post.Visibility = Visibility.Collapsed; 
            else
                Delete_Post.Visibility = Visibility.Visible;
            



            Comment_Box.EnterPressed += Comment_Box_EnterPressed;
            Showing_Post_Block();
        }

        private void Comment_Box_EnterPressed(object sender, EventArgs e)
        {
            string content = Comment_Box.Text.Trim();
            UserService.CreateComment(this.ThePost.PostId, this.Actor.UserId, content);
            this.TheCommentsScrollViewer = new Comment_Block_ScrollViewer(this.Actor, this.ThePost);
            Post_Comments_Grid.Children.Add(this.TheCommentsScrollViewer);
            Comment_Box.Text = "";
            Number_of_Comments.Text = Convert.ToString(PostService.GetPostNumberOfComments(this.ThePost.PostId)) + " Comments";

        }

        private void Showing_Post_Block()
        {
            Poster_Name.Content = this.PostCreator.FullName;
            Post_Content.Text = this.ThePost.Content;
            PostDate.Text = this.ThePost.DateOfPost.ToString();
            int decision = PostLikeService.SearchLikeBasedOnPostAndUserIds(this.ThePost.PostId, this.Actor.UserId);
            if (decision != 0)
                Like_Button.Foreground = Brushes.Blue;

            else
                Like_Button.Foreground = Brushes.Black;
            Number_of_Likes.Content = Convert.ToString(this.ThePost.LikeCounter) + " Likes";
            Number_of_Comments.Text = Convert.ToString(this.ThePost.CommentCounter) + " Comments";
        }


        private void Like_Button_Click(object sender, RoutedEventArgs e)
        {
            
            PostService.ToggleLike(this.ThePost.PostId,this.Actor.UserId, PostService.GetPostNumberOfLikes(this.ThePost.PostId));


            int decision = PostLikeService.SearchLikeBasedOnPostAndUserIds(this.ThePost.PostId, this.Actor.UserId);


            if (decision != 0)
                Like_Button.Foreground = Brushes.Blue;

            else
                Like_Button.Foreground = Brushes.Black;
            this.TheLikesScrollViewer = new User_Block_ScrollViewer_For_PostLikes(this.Actor, this.ThePost);
            Post_Likes_Grid.Children.Add(this.TheLikesScrollViewer);
            Number_of_Likes.Content = Convert.ToString(PostService.GetPostNumberOfLikes(this.ThePost.PostId)) + " Likes";

        }


        private void Comment_Button_Click(object sender, RoutedEventArgs e)
        {
            this.TheCommentsScrollViewer = new Comment_Block_ScrollViewer(this.Actor, this.ThePost);
            Post_Comments_Grid.Children.Add(this.TheCommentsScrollViewer);
            if (IsLikesVisible)
            { 
                PostLikes.Visibility = Visibility.Collapsed;
                Number_of_Likes.IsChecked = false;
            }
            if (IsCommentsVisible) PostComments.Visibility = Visibility.Visible;
            else PostComments.Visibility = Visibility.Collapsed;
            Number_of_Comments.Text = Convert.ToString(PostService.GetPostNumberOfComments(this.ThePost.PostId)) + " Comments";

        }
        private void Number_of_Likes_Click(object sender, RoutedEventArgs e)
        {
            this.TheLikesScrollViewer = new User_Block_ScrollViewer_For_PostLikes(this.Actor, this.ThePost);
            Post_Likes_Grid.Children.Add(this.TheLikesScrollViewer);
            if (IsCommentsVisible)
            {
                PostComments.Visibility = Visibility.Collapsed;
                Comment_Button.IsChecked = false;
            }
            if (IsLikesVisible) PostLikes.Visibility = Visibility.Visible;
            else PostLikes.Visibility = Visibility.Collapsed;
            Number_of_Likes.Content = Convert.ToString(PostService.GetPostNumberOfLikes(this.ThePost.PostId)) + " Likes";

        }


        private void Delete_Post_Click(object sender, RoutedEventArgs e)
        {
            UserService.DeletePost(this.ThePost.PostId);
            if (Window.GetWindow(this) is Home_Window home_Window)
                home_Window.LeftPanel_RefreshButtonEventClicked();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Poster_Name_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is Home_Window homeWindow)
            {
                if(Actor.UserId == PostCreator.UserId)
                    homeWindow.Main_Frame.Content = new MyProfile_Page(Actor);
                else
                    homeWindow.Main_Frame.Content = new Profile_Page(Actor, PostCreator);
            }
        }


        
       
    }
}