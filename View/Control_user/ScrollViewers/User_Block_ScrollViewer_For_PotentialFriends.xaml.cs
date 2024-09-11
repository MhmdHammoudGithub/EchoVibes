using EchoVibe.Backend.Classes;
using EchoVibe.Backend.Services;
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

namespace EchoVibe.View.Control_User.ScrollViewers
{
    public partial class User_Block_ScrollViewer_For_PotentialFriends : UserControl
    {
        public User TheUser { get; set; }// this will be the actor

        public List<User_Block> User_Blocks { get; set; }

        public StackPanel TheUser_Block_StackPanel { get; set; }
        public ScrollViewer TheUser_Block_ScrollViewer { get; set; }

        public User_Block_ScrollViewer_For_PotentialFriends(User user)
        {
            InitializeComponent();
            this.TheUser = user;
            this.User_Blocks = new List<User_Block>();
            this.TheUser_Block_StackPanel = User_Block_StackPanel_In_The_ScrollViewer_In_XAML;
            this.TheUser_Block_ScrollViewer = User_Block_Scroll_Viewer_In_XAML;
            LoadUserBlocks();
        }

        private void LoadUserBlocks()
        {
            int countBeforeAdding = User_Blocks.Count();


            List<User_Block> list2 = BlocksService.SelectNewestRowsForPotentielFriendBlock(countBeforeAdding, this.TheUser);

            User_Blocks = User_Blocks.Concat(list2).ToList();

            foreach (User_Block userBlock in User_Blocks)
            {
                userBlock.Height = 40;
                userBlock.Margin = new Thickness(10, 10, 10, 10);
                userBlock.SetGridVisibility("RequestGrid");
            }

            int countAfterAdding = User_Blocks.Count();
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
