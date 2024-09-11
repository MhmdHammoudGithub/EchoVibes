using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EchoVibe.Backend.Classes;
using EchoVibe.Backend.Services;
using EchoVibe.View.App_Windows;
using EchoVibe.View.Control_User;

namespace EchoVibe.Backend.Classes
{
    public class CommentLike
    {
        public int LikeId { get; set; }// auto_implements the "likeId" private field
        public int CommentId { get; set; }
        public int LikerId { get; set; }

        public CommentLike(int commentId, int likerId)
        {

            CommentLikeService.AddLike(commentId, likerId);
        }

        // we only use this constructor inside the Fetch function in the Database class
        public CommentLike(int likeId, int commentId, int likerId)
        {
            this.LikeId = likeId;
            this.CommentId = commentId;
            this.LikerId = likerId;
        }
    }
}
