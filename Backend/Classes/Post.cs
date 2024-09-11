using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EchoVibe.Backend.Classes;
using EchoVibe.Backend.Services;
using EchoVibe.View.App_Windows;
using EchoVibe.View.Control_User;

namespace EchoVibe.Backend.Classes
{
    public class Post
    {
        public int PostId { get; set; } // auto_implements the "postId" private field
        public int PosterId { get; set; }
        public string Content { get; set; }
        public DateTime DateOfPost { get; set; }
        public int LikeCounter { get; set; }
        public int CommentCounter { get; set; }

        public Post(int posterId, string content)
        {

            PostService.AddPost(posterId, content);
        }


        // we only use this constructor inside the Fetch function 
        public Post(int postId,int posterId, string content, DateTime dateOfPost,int likeCounter,int commentCounter)
        {
            this.PostId = postId;
            this.PosterId = posterId;
            this.Content = content;
            this.DateOfPost = dateOfPost;
            this.LikeCounter = likeCounter;
            this.CommentCounter = commentCounter;

        }


    }
}