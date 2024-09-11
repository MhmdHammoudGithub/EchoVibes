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
using EchoVibe.Backend.Classes;
using EchoVibe.Backend.Services;
using EchoVibe.View.App_Windows;
using EchoVibe.View.Pages;




namespace EchoVibe.View.Control_User
{

    public partial class User_Block : UserControl, INotifyPropertyChanged
    {

        private Visibility _searchGridVisibility;
        private Visibility _requestGridVisibility;
        private Visibility _myFriendGridVisibility;
        private Visibility _MyProfileGridVisibility;
        private Visibility _profileGridVisibility;

        public Visibility ProfileGridVisibility
        {
            get { return _profileGridVisibility; }
            set { _profileGridVisibility = value; OnPropertyChanged(); }
        }
        public Visibility SearchGridVisibility
        {
            get { return _searchGridVisibility; }
            set { _searchGridVisibility = value; OnPropertyChanged(); }
        }

        public Visibility RequestGridVisibility
        {
            get { return _requestGridVisibility; }
            set { _requestGridVisibility = value; OnPropertyChanged(); }
        }

        public Visibility MyFriendGridVisibility
        {
            get { return _myFriendGridVisibility; }
            set { _myFriendGridVisibility = value; OnPropertyChanged(); }
        }

        public Visibility MyProfileGridVisibility
        {
            get { return _MyProfileGridVisibility; }
            set { _MyProfileGridVisibility = value; OnPropertyChanged(); }
        }


        private Visibility _addFriendVisibility;
        private Visibility _alreadyFriendSearchGridVisibility;
        private Visibility _requestSentVisibility;
        private Visibility _requestAnswerVisibility;
        private Visibility _addFriendInProfileVisibility;
        private Visibility _alreadyFriendProfileGridVisibility;
        private Visibility _requestSentInProfileVisibility;
        private Visibility _requestAnswerInProfileVisibility;


        public Visibility AlreadyFriendSearchGridVisibility
        {
            get { return _alreadyFriendSearchGridVisibility; }
            set { _alreadyFriendSearchGridVisibility = value; OnPropertyChanged(); }
        }

        public Visibility AddFriendVisibility
        {
            get { return _addFriendVisibility; }
            set { _addFriendVisibility = value; OnPropertyChanged(); }
        }

        public Visibility RequestSentVisibility
        {
            get { return _requestSentVisibility; }
            set { _requestSentVisibility = value; OnPropertyChanged(); }
        }

        public Visibility RequestAnswerVisibility
        {
            get { return _requestAnswerVisibility; }
            set { _requestAnswerVisibility = value; OnPropertyChanged(); }
        }
        public Visibility AddFriendInProfileVisibility
        {
            get { return _addFriendInProfileVisibility; }
            set { _addFriendInProfileVisibility = value; OnPropertyChanged(); }
        }

        public Visibility AlreadyFriendProfileGridVisibility
        {
            get { return _alreadyFriendProfileGridVisibility; }
            set { _alreadyFriendProfileGridVisibility = value; OnPropertyChanged(); }
        }

        public Visibility RequestSentInProfileVisibility
        {
            get { return _requestSentInProfileVisibility; }
            set { _requestSentInProfileVisibility = value; OnPropertyChanged(); }
        }

        public Visibility RequestAnswerInProfileVisibility
        {
            get { return _requestAnswerInProfileVisibility; }
            set { _requestAnswerInProfileVisibility = value; OnPropertyChanged(); }
        }


        public EventHandler User_Name_Button_Event_Clicked;

        public event PropertyChangedEventHandler PropertyChanged = null;

        private List<Grid> column2Grids;

        public User Owner { get; set; } //the one who owns the profile which we will access by taping his name
        public User Actor { get; set; }


        public User_Block(User owner, User actor)
        {
            InitializeComponent();
            DataContext = this;
            this.Owner = owner;
            this.Actor = actor;
            User_Name_Button.Content = Owner.FullName;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetGridVisibility(string selectedGrid)
        {
            column2Grids = new List<Grid> { SearchGrid, RequestGrid, MyProfileGrid, ProfileGrid };//For likescroll and friend write any text so everthing will be collapsed
            foreach (var grid in column2Grids)
            {
                grid.Visibility = (grid.Name == selectedGrid) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public void SetSearchOrProfileGridVisibility(string state)
        {
            // Loop through all FrameworkElements in SearchGrid and set visibility based on the selected element
            AddFriendVisibility = Visibility.Collapsed;
            AddFriendInProfileVisibility = Visibility.Collapsed;

            AlreadyFriendSearchGridVisibility = Visibility.Collapsed;
            AlreadyFriendProfileGridVisibility = Visibility.Collapsed;

            RequestSentVisibility = Visibility.Collapsed;
            RequestSentInProfileVisibility = Visibility.Collapsed;

            RequestAnswerVisibility = Visibility.Collapsed;
            RequestAnswerInProfileVisibility = Visibility.Collapsed;

            switch (state)
            {
                case "SendFriendRequest":
                    AddFriendVisibility = Visibility.Visible;
                    AddFriendInProfileVisibility = Visibility.Visible;
                    break;
                case "RequestAnswer":
                    RequestAnswerVisibility = Visibility.Visible;
                    RequestAnswerInProfileVisibility = Visibility.Visible;
                    break;
                case "RequestSent":
                    RequestSentVisibility = Visibility.Visible;
                    RequestSentInProfileVisibility = Visibility.Visible;
                    break;
                case "AlreadyFriend":
                    AlreadyFriendSearchGridVisibility = Visibility.Visible;
                    AlreadyFriendProfileGridVisibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private void User_Name_Button_Click(object sender, RoutedEventArgs e)
        {
            User_Name_Button_Event_Clicked?.Invoke(this, EventArgs.Empty);
            if (Window.GetWindow(this) is Home_Window homeWindow)
            {
                if(Actor.UserId == Owner.UserId)
                    homeWindow.Main_Frame.Content = new MyProfile_Page(Actor);
                else
                    homeWindow.Main_Frame.Content = new Profile_Page(Actor, Owner);
                homeWindow.Left_Panel_Instance.Search_Button.IsChecked = false;
            }
        }

        private void Add_Friend_In_Profile_Grid_Click(object sender, RoutedEventArgs e)
        {
            UserService.SendFriendRequest(this.Actor.UserId, this.Owner.UserId);
            SetSearchOrProfileGridVisibility("RequestSent");
        }

        private void Remove_Friend_In_Profile_Grid_Click(object sender, RoutedEventArgs e)
        {
            UserService.RemoveFriend(this.Actor.UserId, this.Owner.UserId);
            SetSearchOrProfileGridVisibility("SendFriendRequest");

        }




        private void Delete_Request_In_Profile_Grid_Click(object sender, RoutedEventArgs e)
        {
            UserService.FriendRequestDecision(this.Actor.UserId, this.Owner.UserId, "deleted");
            SetSearchOrProfileGridVisibility("SendFriendRequest");

        }

        private void Accept_Request_In_Profile_Grid_Click(object sender, RoutedEventArgs e)
        {
            UserService.FriendRequestDecision(this.Actor.UserId, this.Owner.UserId, "accept");
            SetSearchOrProfileGridVisibility("AlreadyFriend");

        }

        private void Decline_Request_In_Profile_Grid_Click(object sender, RoutedEventArgs e)
        {
            UserService.FriendRequestDecision(this.Actor.UserId, this.Owner.UserId, "reject");
            SetSearchOrProfileGridVisibility("SendFriendRequest");

        }

        private void Send_FriendRequest_In_Search_Grid_Click(object sender, RoutedEventArgs e)
        {
            UserService.SendFriendRequest(this.Actor.UserId, this.Owner.UserId);
            SetSearchOrProfileGridVisibility("RequestSent");

        }

        private void Delete_Request_In_Search_Grid_Click(object sender, RoutedEventArgs e)
        {
            UserService.FriendRequestDecision(this.Actor.UserId, this.Owner.UserId, "deleted");
            SetSearchOrProfileGridVisibility("SendFriendRequest");

        }

        private void Accept_Request_In_Search_Grid_Click(object sender, RoutedEventArgs e)
        {
            UserService.FriendRequestDecision(this.Actor.UserId, this.Owner.UserId, "accept");
            SetSearchOrProfileGridVisibility("AlreadyFriend");

        }

        private void Decline_Request_In_Search_Grid_Click(object sender, RoutedEventArgs e)
        {
            UserService.FriendRequestDecision(this.Actor.UserId, this.Owner.UserId, "reject");
            SetSearchOrProfileGridVisibility("SendFriendRequest");

        }

        private void Accept_Request_In_Request_Grid_Click(object sender, RoutedEventArgs e)
        {
            UserService.FriendRequestDecision(this.Actor.UserId, this.Owner.UserId, "accept");
            SetSearchOrProfileGridVisibility("AlreadyFriend");

        }

        private void Decline_Request_In_Request_Grid_Click(object sender, RoutedEventArgs e)
        {
            UserService.FriendRequestDecision(this.Actor.UserId, this.Owner.UserId, "reject");
            SetSearchOrProfileGridVisibility("SendFriendRequest");

        }

        private void Delete_Account_In_My_Profile_Grid_Click(object sender, RoutedEventArgs e)
        {
            UserService.DeleteAccount(this.Actor.UserId);

            Entry_Window newHomeWindow = new Entry_Window();
            newHomeWindow.Show();
            if (Window.GetWindow(this) is Home_Window mainWindow)
                mainWindow.Close();
        }


    }

}
