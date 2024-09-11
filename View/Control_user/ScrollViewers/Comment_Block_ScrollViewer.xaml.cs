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
    
    public partial class Comment_Block_ScrollViewer : UserControl
    {
        public User TheUser { get; set; }// this will be the actor
        public Post ThePost { get; set; }

        public List<Comment_Block> Comment_Blocks { get; set; }

        public StackPanel Comment_Block_StackPanel { get; set; }
        public ScrollViewer TheComment_Block_ScrollViewer { get; set; }

        


        public Comment_Block_ScrollViewer(User user, Post post)
        {
            InitializeComponent();
            this.TheUser = user;
            this.ThePost = post;
            this.Comment_Blocks = new List<Comment_Block>();
            this.Comment_Block_StackPanel = Comment_Block_StackPanel_In_The_ScrollViewer;
            this.TheComment_Block_ScrollViewer = Comment_Block_Scroll_Viewer_In_XAML;
            LoadCommentBlocks();
        }
        

        private void LoadCommentBlocks()
        {
            int countBeforeAdding = Comment_Blocks.Count();


            List<Comment_Block> list2 = BlocksService.SelectNewestRowsForCommentBlock(countBeforeAdding, this.ThePost.PostId, this.TheUser);

            Comment_Blocks = Comment_Blocks.Concat(list2).ToList();

            if(Comment_Blocks.Count > 0)
            {
                foreach (Comment_Block commentBlock in Comment_Blocks)
                {
                    commentBlock.Height = Comment_Blocks_Grid.Height;
                    commentBlock.Margin = new Thickness(10, 10, 10, 10);
                }

                int countAfterAdding = Comment_Blocks.Count();
                for (int i = countBeforeAdding; i < countAfterAdding; i++)
                {
                    Comment_Block_StackPanel.Children.Add(Comment_Blocks[i]);
                }
            }

            
        }


        private bool isProcessingCommentBlockScrollEvent = false;
        private void Comment_Block_Scroll_Viewer_In_XAML_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!isProcessingCommentBlockScrollEvent && e.VerticalChange > 0 &&
                e.VerticalOffset >= Comment_Block_Scroll_Viewer_In_XAML.ScrollableHeight)
            {
                isProcessingCommentBlockScrollEvent = true;

                LoadCommentBlocks();

                isProcessingCommentBlockScrollEvent = false;
            }
        }
    }
}
