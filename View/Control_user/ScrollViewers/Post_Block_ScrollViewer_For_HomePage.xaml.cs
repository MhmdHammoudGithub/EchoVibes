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
using EchoVibe.Backend.Classes;
using EchoVibe.Backend.Services;
using EchoVibe.View.Pages;

namespace EchoVibe.View.Control_User.ScrollViewers
{

    public partial class Post_Block_ScrollViewer_For_HomePage : UserControl
    {

        public User TheUser { get; set; }// this will be the actor

        public List<Post_Block> Post_Blocks { get; set; }

        public StackPanel ThePost_Block_StackPanel {  get; set; }
        public ScrollViewer ThePost_Block_ScrollViewer { get; set; }

        public Post_Block_ScrollViewer_For_HomePage(User user)
        {
            InitializeComponent();
            this.TheUser = user;
            this.Post_Blocks = new List<Post_Block>();
            this.Post_Blocks = new List<Post_Block>();
            this.ThePost_Block_StackPanel = Post_Block_StackPanel_In_The_ScrollViewer_In_XAML;
            this.ThePost_Block_ScrollViewer = Post_Block_Scroll_Viewer_In_XAML;
            LoadPostBlocks();
        }

        private void LoadPostBlocks()
        {
            int countBeforeAdding = Post_Blocks.Count();


            List<Post_Block> list2 = BlocksService.SelectNewestRowsForPostBlockForHomePage(countBeforeAdding, this.TheUser);

            Post_Blocks = Post_Blocks.Concat(list2).ToList();

            foreach (Post_Block postBlock in Post_Blocks)
            {
                postBlock.Margin = new Thickness(10, 10, 10, 10);
            }

            int countAfterAdding = Post_Blocks.Count();
            for (int i = countBeforeAdding; i < countAfterAdding; i++)
            {
                ThePost_Block_StackPanel.Children.Add(Post_Blocks[i]);
            }
        }


        private bool isProcessingPostBlockScrollEvent = false;
        private void Post_Block_Scroll_Viewer_In_XAML_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!isProcessingPostBlockScrollEvent && e.VerticalChange > 0 &&
               e.VerticalOffset >= Post_Block_Scroll_Viewer_In_XAML.ScrollableHeight)
            {
                isProcessingPostBlockScrollEvent = true;

                LoadPostBlocks();

                isProcessingPostBlockScrollEvent = false;
            }
        }
    }
}
