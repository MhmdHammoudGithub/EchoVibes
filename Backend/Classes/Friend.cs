using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using EchoVibe.Backend.Classes;
using EchoVibe.Backend.Services;
using EchoVibe.View.App_Windows;
using EchoVibe.View.Control_User;

namespace EchoVibe.Backend.Classes
{
    public class Friend
    {

        public int FriendId {  get; set; } // auto_implements the "friendId" private field
        public int User1Id { get; set; } 
        public int User2Id { get; set; }
        public string Status { get; set; }
        public Friend(int user1Id, int user2Id)
        {

            FriendService.AddFriend(user1Id, user2Id);

        }

        // we only use this constructor inside the Fetch function in the Database class
        public Friend(int friendId,int user1Id, int user2Id, string status)
        {
            this.FriendId = friendId;
            this.User1Id = user1Id;
            this.User2Id = user2Id;
            this.Status = status;
        }

    }
}
