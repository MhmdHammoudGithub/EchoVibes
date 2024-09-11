using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using EchoVibe.Backend.Classes;
using EchoVibe.Backend.Services;
using EchoVibe.View.App_Windows;
using EchoVibe.View.Control_User;

namespace EchoVibe.Backend.Classes
{
    public class User
    {
        public int UserId { get; set; } // auto_implements the "userId" private field
        public string FullName { get; set; } 
        public string Password { get; set; }
        public DateTime DateOfBirth { get; set; }
        public char Gender { get; set; }

        // sign up
        public User(string fullName, string password,DateTime dateofbirth, char gender)
        {

            this.UserId = UserService.AddUser(fullName, password, dateofbirth, gender);
            this.FullName = fullName;
            this.Password = password;
            this.DateOfBirth = dateofbirth;
            this.Gender = gender;
        }
        
        // we only use this constructor inside the Fetch function 
        public User(int userId,string fullName, string password, DateTime dateofbirth, char gender)
        {
            this.UserId = userId; 
            this.FullName = fullName;
            this.Password = password;
            this.DateOfBirth = dateofbirth;
            this.Gender = gender;

        }

      
 
    }
}
