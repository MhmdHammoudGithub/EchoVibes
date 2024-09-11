using EchoVibe.Backend.Classes;
using EchoVibe.Backend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace EchoVibe.View.Control_User.ScrollViewers
{

    public partial class User_Block_ScrollViewer_For_Profiles : UserControl
    {
        public User TheUser { get; set; }// this will be the actor
        public string FullName { get; set; }
        public List<User_Block> User_Blocks { get; set; }

        public StackPanel TheUser_Block_StackPanel { get; set; }
        public ScrollViewer TheUser_Block_ScrollViewer { get; set; }

        public User_Block_ScrollViewer_For_Profiles(User user, string fullname)
        {
            InitializeComponent();
            this.TheUser = user;
            this.FullName = fullname;
            this.User_Blocks = new List<User_Block>();
            this.TheUser_Block_StackPanel = User_Block_StackPanel_In_The_ScrollViewer_In_XAML;
            this.TheUser_Block_ScrollViewer = User_Block_Scroll_Viewer_In_XAML;
            LoadUserBlocks();
        }



        private void LoadUserBlocks()
        {
            int countBeforeAdding = User_Blocks.Count();


            List<User_Block> list2 = BlocksService.SelectNewestRowsForProfileBlock(countBeforeAdding, this.FullName, this.TheUser);

            User_Blocks = User_Blocks.Concat(list2).ToList();
            int countAfterAdding = User_Blocks.Count();

            for (int i = countBeforeAdding; i < countAfterAdding; i++)
            {
                User_Block thisUserBlock = User_Blocks[i];
                thisUserBlock.SetGridVisibility("SearchGrid");
                string status = FriendService.GetFriendStatusUserRelativeUserBlock(thisUserBlock.Owner.UserId, thisUserBlock.Actor.UserId);
                thisUserBlock.SetSearchOrProfileGridVisibility(status);




                if (thisUserBlock.Owner.UserId == thisUserBlock.Actor.UserId)
                {
                    i--;
                    countAfterAdding--;
                    User_Blocks.Remove(thisUserBlock);
                    continue;
                }

                thisUserBlock.Height = 50;
                thisUserBlock.Margin = new Thickness(10, 10, 10, 10);

            }


            for (int i = countBeforeAdding; i < countAfterAdding; i++)
            {
                TheUser_Block_StackPanel.Children.Add(User_Blocks[i]);
            }
        }


        private bool isProcessingPostBlockScrollEvent = false;
        private void User_Block_Scroll_Viewer_In_XAML_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!isProcessingPostBlockScrollEvent && e.VerticalChange > 0 &&
               e.VerticalOffset >= User_Block_Scroll_Viewer_In_XAML.ScrollableHeight)
            {
                isProcessingPostBlockScrollEvent = true;

                LoadUserBlocks();

                isProcessingPostBlockScrollEvent = false;
            }
        }
    }
}
