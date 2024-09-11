using EchoVibe.Backend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoVibe.Backend.Classes
{
    public class Comment
    {
        public int CommentId {  get; set; }// auto_implements the "commentId" private field
        public int PostId { get; set; } 
        public int CommentatorId { get; set; }
        public string Content { get; set; }
        public DateTime DateOfComment { get; set; }
        public int LikeCounter { get; set; }
        
        
        public Comment(int postId,int commentatorId, string content)
        {
            CommentService.AddComment(postId, commentatorId, content);
        }


        // we only use this constructor inside the Fetch function in the Service class
        public Comment(int commentId, int postId, int commentatorId,string content, DateTime dateOfComment, int likeCounter)
        {
            this.CommentId = commentId;
            this.PostId = postId;
            this.CommentatorId = commentatorId;
            this.Content = content;
            this.DateOfComment = dateOfComment;
            this.LikeCounter = likeCounter;
        }
    }
}
