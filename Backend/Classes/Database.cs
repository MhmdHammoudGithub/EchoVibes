using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows;
using System.Collections;
using System.Windows.Navigation;
using System.Windows.Input;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.IO.Packaging;
using System.Xml.Linq;
using System.Numerics;
using EchoVibe.Backend.Classes;
using EchoVibe.Backend.Services;
using EchoVibe.View.App_Windows;
using EchoVibe.View.Control_User;

namespace EchoVibe.Backend.Classes
{

    sealed class Database
    {
        private static Database instance;
        public static MySqlConnection connection = new MySqlConnection("Server=localhost;Database=echovibe;User ID=root;Password=mhmd1234;");

        // Private constructor to ensure only one instance is created
        private Database(){}

        // Public property to access the instance
        public static Database Instance
        {
            get
            {
                if (instance == null)
                    instance = new Database();
                return instance;
            }
        }



        // Function to connect to MySQL database
        public void Connect()
         {

            try
            {
                if (connection != null && connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to MySQL: {ex.Message}");
            }
         }

        // Function to disconnect from MySQL database
        public void Disconnect()
        {
            try
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connectiong to MySQL: {ex.Message}");
            }

        }


    }
}



