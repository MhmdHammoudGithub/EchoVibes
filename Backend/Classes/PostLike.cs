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
    public class PostLike
    {

        public int LikeId { get; set; }// auto_implements the "likeId" private field
        public int PostId { get; set; }
        public int LikerId { get; set; }


        public PostLike(int postId, int likerId )
        {
            
            PostLikeService.AddLike(postId, likerId);
        }

        // we only use this constructor inside the Fetch function in the Database class
        public PostLike(int likeId,int postId, int likerId)
        {
            this.LikeId = likeId;
            this.PostId = postId;
            this.LikerId = likerId;
        }

    }
}
