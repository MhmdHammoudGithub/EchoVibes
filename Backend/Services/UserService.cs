using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EchoVibe.Backend.Classes;
using EchoVibe.Backend.Services;
using EchoVibe.View.App_Windows;
using EchoVibe.View.Control_User;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Data;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Runtime.Remoting.Messaging;
using MySqlX.XDevAPI.Common;

namespace EchoVibe.Backend.Services
{
    static class UserService
    {

        static public int AddUser(string fullName, string password, DateTime dateOfBirth, char gender)
        {
            int id = 0; // Default value
            dateOfBirth = dateOfBirth.Date;

            string tableName = "_user";
            string[] columnNames = { "full_name", "password", "date_of_birth", "gender" };
            string[] columnValues = { $"{fullName}", $"{password}", $"{dateOfBirth.ToString("yyyy-MM-dd")}", $"{gender}" };
            string columns = string.Join(", ", columnNames);
            string values = string.Join(", ", columnNames.Select(c => $"@{c}"));

            Database.Instance.Connect();
            using (MySqlCommand cmd = new MySqlCommand($"INSERT INTO {tableName} ({columns}) VALUES ({values});", Database.connection))
            {
                for (int i = 0; i < columnNames.Length; i++)
                {
                    cmd.Parameters.AddWithValue($"@{columnNames[i]}", columnValues[i]);
                }

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    using (MySqlCommand getLastIdCmd = new MySqlCommand("SELECT LAST_INSERT_ID();", Database.connection))
                    {
                        id = Convert.ToInt32(getLastIdCmd.ExecuteScalar());
                    }

                }

            }
            Database.Instance.Disconnect();
            return id;
        }

        static public void RemoveUser(int primaryKey)
        {
            string userTableName = "_user";
            string primaryKeyColumnName = "user_id";





            Database.Instance.Connect();
            string deleteUserQuery = $"DELETE FROM {userTableName} WHERE {primaryKeyColumnName} = @primaryKey";
            using (MySqlCommand cmd = new MySqlCommand(deleteUserQuery, Database.connection))
            {
                cmd.Parameters.AddWithValue("@primaryKey", primaryKey);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("User and related records deleted successfully.");
                }
                else
                {
                    MessageBox.Show("User not found or an error occurred.");
                }
            }

            Database.Instance.Disconnect();
        }

        // returns 0 or primaryKey 
        static public int SearchUser(string fullname, string password)
        {
            string tableName = "_user";
            string fullNameColumnName = "full_name";
            string passwordColumnName = "password";
            int result = 0;
            Database.Instance.Connect();

            string query = $"SELECT * FROM {tableName} WHERE {fullNameColumnName} = @fullName AND {passwordColumnName} = @password";

            using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
            {
                cmd.Parameters.AddWithValue("@fullName", fullname);
                cmd.Parameters.AddWithValue("@password", password);

                object objResult = cmd.ExecuteScalar();
                if (objResult != null)
                    result = (int)objResult;
            }

            Database.Instance.Disconnect();

            return result;
        }

        // returns empty list or normal list
        static public List<int> SearchUserBasedOnFullName(string fullname, bool isCaseSensitive)
        {
            List<int> result = new List<int>();
            string tableName = "_user";
            string fullNameColumnName = "full_name";

            try
            {
                Database.Instance.Connect();

                string query;
                if (isCaseSensitive)
                {
                    query = $"SELECT * FROM {tableName} WHERE {fullNameColumnName} = @fullName";
                    using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
                    {
                        cmd.Parameters.AddWithValue("@fullName", fullname);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int userId = reader.GetInt32(0);
                                result.Add(userId);
                            }
                        }
                    }
                }
                else
                    query = $"SELECT * FROM {tableName} WHERE LOWER({fullNameColumnName}) LIKE LOWER(@fullName)";

                using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
                {
                    cmd.Parameters.AddWithValue("@fullName", "%" + fullname + "%");

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int userId = reader.GetInt32(0);
                            result.Add(userId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                Database.Instance.Disconnect();
            }

            return result;
        }


        // returns null or user instance
        static public User FetchUser(int primaryKey)
        {
            string tableName = "_user";
            string primaryKeyColumnName = "user_id";

            Database.Instance.Connect();
            User result = null;
            string query = $"SELECT * FROM {tableName} WHERE {primaryKeyColumnName} = @primaryKey";
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
                {

                    cmd.Parameters.AddWithValue("@primaryKey", primaryKey);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int user_id = reader.GetInt32(0);
                            string full_name = reader.GetString(1);
                            string password = reader.GetString(2);
                            DateTime DOB = reader.GetDateTime(3);
                            char gender = reader.GetChar(4);
                            result = new User(user_id, full_name, password, DOB.Date, gender);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }


            Database.Instance.Disconnect();
            return result;


        }

        // returns empty list or normal list
        static public List<User> FetchUserBatchInstancesForPosts(List<int> primaryKeys)
        {
            List<User> result = new List<User>();
            string tableName = "_user";
            string[] columnNames = { "user_id", "full_name", "password", "date_of_birth", "gender" };
            string joinTableName = "_post";
            string joinTableConditionColumnName = "poster_id";
            string joinTablePrimaryKeyColumnName = "post_id";



            Database.Instance.Connect();

            if (primaryKeys.Count != 0)
            {
                string smallQuery = $"SELECT u.{columnNames[0]}, u.{columnNames[1]}, u.{columnNames[2]}, u.{columnNames[3]}, u.{columnNames[4]} " +
                  $" FROM {tableName} u JOIN {joinTableName} j ON u.{columnNames[0]} = j.{joinTableConditionColumnName} ";

                string query = $"{smallQuery} WHERE u.{columnNames[0]} IN ({string.Join(",", primaryKeys)}) " +
                    $"ORDER BY j.{joinTablePrimaryKeyColumnName} DESC"; 

                using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            int user_id = reader.GetInt32(0);
                            string full_name = reader.GetString(1);
                            string password = reader.GetString(2);
                            DateTime DOB = reader.GetDateTime(3);
                            char gender = reader.GetChar(4);
                            result.Add(new User(user_id, full_name, password, DOB.Date, gender));

                        }
                    }
                }
                Database.Instance.Disconnect();
            }
            return result;


        }

        // returns empty list or normal list
        static public List<User> FetchUserBatchInstancesForComments(List<int> primaryKeys)
        {
            List<User> result = new List<User>();
            string tableName = "_user";
            string[] columnNames = { "user_id", "full_name", "password", "date_of_birth", "gender" };
            string joinTableName = "_comment";
            string joinTableConditionColumnName = "commentator_id";
            string joinTablePrimaryKeyColumnName = "comment_id";


            Database.Instance.Connect();
            string smallQuery = $"SELECT u.{columnNames[0]}, u.{columnNames[1]}, u.{columnNames[2]}, u.{columnNames[3]}, u.{columnNames[4]} " +
                  $" FROM {tableName} u JOIN {joinTableName} j ON u.{columnNames[0]} = j.{joinTableConditionColumnName} ";

            string query = $"{smallQuery} WHERE u.{columnNames[0]} IN ({string.Join(",", primaryKeys)}) " + 
                $"ORDER BY j.{joinTablePrimaryKeyColumnName} DESC "; 

            using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
            {

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {

                            int user_id = reader.GetInt32(0);
                            string full_name = reader.GetString(1);
                            string password = reader.GetString(2);
                            DateTime DOB = reader.GetDateTime(3);
                            char gender = reader.GetChar(4);
                            result.Add(new User(user_id, full_name, password, DOB.Date, gender));

                        }
                    }
                }
            }

            Database.Instance.Disconnect();
            return result;
        }

        // returns empty list or normal list
        static public List<User> FetchUserBatchInstancesForFriends(List<int> user1PrimaryKeys, List<int> user2PrimaryKeys, int mainUserId)
        {
            List<User> result = new List<User>();
            string tableName = "_user";
            string[] columnNames = { "user_id", "full_name", "password", "date_of_birth", "gender" };
            string joinTableName = "_friend";
            string joinTablePrimaryKeyColumnName = "friend_id";
            string user1ColumnName = "user1_id";
            string user2ColumnName = "user2_id";
            string statusColumnName = "status";
            string status = "accepted";



            string user1Query = "";
            if (user1PrimaryKeys.Count != 0)
            {
                user1Query =
               $" SELECT u1.{columnNames[0]}, u1.{columnNames[1]}, u1.{columnNames[2]}, u1.{columnNames[3]}, u1.{columnNames[4]}, j1.{joinTablePrimaryKeyColumnName}" +
               $" FROM {tableName} u1    JOIN {joinTableName} j1 ON u1.{columnNames[0]} = j1.{user1ColumnName}" +
               $" WHERE j1.{user1ColumnName} IN ({string.Join(",", user1PrimaryKeys)})  AND j1.{user2ColumnName} = {mainUserId} AND j1.{statusColumnName} = @status";
            }



            string user2Query = "";
            if (user2PrimaryKeys.Count != 0)
            {
                user2Query =
                $" SELECT u2.{columnNames[0]}, u2.{columnNames[1]}, u2.{columnNames[2]}, u2.{columnNames[3]}, u2.{columnNames[4]}, j2.{joinTablePrimaryKeyColumnName}" +
                $" FROM {tableName} u2    JOIN {joinTableName} j2 ON u2.{columnNames[0]} = j2.{user2ColumnName} " +
                $" WHERE j2.{user2ColumnName} IN ({string.Join(",", user2PrimaryKeys)}) AND j2.{user1ColumnName} = {mainUserId} AND j2.{statusColumnName} = @status";

            }


            string fromStatement = null;
            if (user1PrimaryKeys.Count == 0 && user2PrimaryKeys.Count != 0)
            {
                fromStatement = $" FROM ( {user2Query} )" +
                                $"AS table2 ";
            }
            else if (user1PrimaryKeys.Count != 0 && user2PrimaryKeys.Count == 0)
            {
                fromStatement = $" FROM ( {user1Query} ) " +
                                $"AS table1 ";

            }
            else if (user1PrimaryKeys.Count != 0 && user2PrimaryKeys.Count != 0)
            {
                fromStatement = $" FROM ( {user1Query} UNION {user2Query} )" +
                                $" AS combined_tables"; // the alias here is necessary, in mysql, when using a subquery in FROM, an alias is necessary

            }
            if (fromStatement != null)
            {
                string query =
                $" SELECT {columnNames[0]}, {columnNames[1]}, {columnNames[2]}, {columnNames[3]},{columnNames[4]}" +
                fromStatement +
                $" ORDER BY {joinTablePrimaryKeyColumnName} DESC "; 
                Database.Instance.Connect();
                using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
                {
                    cmd.Parameters.AddWithValue("@status", status);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {

                            int user_id = reader.GetInt32(0);
                            string full_name = reader.GetString(1);
                            string password = reader.GetString(2);
                            DateTime DOB = reader.GetDateTime(3);
                            char gender = reader.GetChar(4);
                            result.Add(new User(user_id, full_name, password, DOB.Date, gender));

                        }


                    }
                }
                Database.Instance.Disconnect();
            }


            return result;
        }

        static public List<User> FetchUserBatchInstancesForPotentielFriends(List<int> user1PrimaryKeys, List<int> user2PrimaryKeys, int mainUserId)
        {
            List<User> result = new List<User>();
            string tableName = "_user";
            string[] columnNames = { "user_id", "full_name", "password", "date_of_birth", "gender" };
            string joinTableName = "_friend";
            string joinTablePrimaryKeyColumnName = "friend_id";
            string user1ColumnName = "user1_id";
            string user2ColumnName = "user2_id";
            string statusColumnName = "status";
            string status = "pending";



            string user1Query = "";
            if (user1PrimaryKeys.Count != 0)
            {
                user1Query =
               $" SELECT u1.{columnNames[0]}, u1.{columnNames[1]}, u1.{columnNames[2]}, u1.{columnNames[3]}, u1.{columnNames[4]}, j1.{joinTablePrimaryKeyColumnName}" +
               $" FROM {tableName} u1    JOIN {joinTableName} j1 ON u1.{columnNames[0]} = j1.{user1ColumnName}" +
               $" WHERE j1.{user1ColumnName} IN ({string.Join(",", user1PrimaryKeys)})  AND j1.{user2ColumnName} = {mainUserId} AND j1.{statusColumnName} = @status";
            }



            string user2Query = "";
            if (user2PrimaryKeys.Count != 0)
            {
                user2Query =
                $" SELECT u2.{columnNames[0]}, u2.{columnNames[1]}, u2.{columnNames[2]}, u2.{columnNames[3]}, u2.{columnNames[4]}, j2.{joinTablePrimaryKeyColumnName}" +
                $" FROM {tableName} u2    JOIN {joinTableName} j2 ON u2.{columnNames[0]} = j2.{user2ColumnName} " +
                $" WHERE j2.{user2ColumnName} IN ({string.Join(",", user2PrimaryKeys)}) AND j2.{user1ColumnName} = {mainUserId} AND j2.{statusColumnName} = @status";

            }


            string fromStatement = null;
            if (user1PrimaryKeys.Count == 0 && user2PrimaryKeys.Count != 0)
            {
                fromStatement = $" FROM ( {user2Query} )" +
                                $"AS table2 ";
            }
            else if (user1PrimaryKeys.Count != 0 && user2PrimaryKeys.Count == 0)
            {
                fromStatement = $" FROM ( {user1Query} ) " +
                                $"AS table1 ";

            }
            else if (user1PrimaryKeys.Count != 0 && user2PrimaryKeys.Count != 0)
            {
                fromStatement = $" FROM ( {user1Query} UNION {user2Query} )" +
                                $" AS combined_tables"; // the alias here is necessary, in mysql, when using a subquery in FROM, an alias is necessary


            }
            if (fromStatement != null)
            {
                string query =
                $" SELECT {columnNames[0]}, {columnNames[1]}, {columnNames[2]}, {columnNames[3]},{columnNames[4]}" +
                fromStatement +
                $" ORDER BY {joinTablePrimaryKeyColumnName} DESC "; 
                Database.Instance.Connect();
                using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
                {
                    cmd.Parameters.AddWithValue("@status", status);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {

                            int user_id = reader.GetInt32(0);
                            string full_name = reader.GetString(1);
                            string password = reader.GetString(2);
                            DateTime DOB = reader.GetDateTime(3);
                            char gender = reader.GetChar(4);
                            result.Add(new User(user_id, full_name, password, DOB.Date, gender));

                        }


                    }
                }
                Database.Instance.Disconnect();
            }


            return result;
        }


        // returns empty list or normal list
        static public List<User> FetchUserBatchInstancesForPostLikes(List<int> primaryKeys)
        {
            List<User> result = new List<User>();
            string tableName = "_user";
            string[] columnNames = { "user_id", "full_name", "password", "date_of_birth", "gender" };




            int numberOfElementsToLoad = 10;
            Database.Instance.Connect();

            
            

            string smallQuery = $"SELECT u.{columnNames[0]}, u.{columnNames[1]}, u.{columnNames[2]}, u.{columnNames[3]}, u.{columnNames[4]} " +
                  $" FROM {tableName} u ";

            string query = $"{smallQuery} WHERE u.{columnNames[0]} IN ({string.Join(",", primaryKeys)}) LIMIT {numberOfElementsToLoad}";
            using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        int user_id = reader.GetInt32(0);
                        string full_name = reader.GetString(1);
                        string password = reader.GetString(2);
                        DateTime DOB = reader.GetDateTime(3);
                        char gender = reader.GetChar(4);
                        result.Add(new User(user_id, full_name, password, DOB.Date, gender));

                    }
                }
            }
            Database.Instance.Disconnect();
            return result;
        }

        // returns empty list or normal list
        static public List<User> FetchUserBatchInstancesForCommentLikes(List<int> primaryKeys)
        {
            List<User> result = new List<User>();
            string tableName = "_user";
            string[] columnNames = { "user_id", "full_name", "password", "date_of_birth", "gender" };
            string joinTableName = "_comment_like";
            string joinTableConditionColumnName = "liker_id";
            string joinTablePrimaryKeyColumnName = "like_id";



            int numberOfElementsToLoad = 10;
            Database.Instance.Connect();

            string smallQuery = $"SELECT u.{columnNames[0]}, u.{columnNames[1]}, u.{columnNames[2]}, u.{columnNames[3]}, u.{columnNames[4]} " +
                  $" FROM {tableName} u JOIN {joinTableName} j ON u.{columnNames[0]} = j.{joinTableConditionColumnName} ";

            string query = $"{smallQuery} WHERE u.{columnNames[0]} IN ({string.Join(",", primaryKeys)}) " +
                $" ORDER BY j.{joinTablePrimaryKeyColumnName} DESC LIMIT {numberOfElementsToLoad}";

            using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        int user_id = reader.GetInt32(0);
                        string full_name = reader.GetString(1);
                        string password = reader.GetString(2);
                        DateTime DOB = reader.GetDateTime(3);
                        char gender = reader.GetChar(4);
                        result.Add(new User(user_id, full_name, password, DOB.Date, gender));

                    }
                }
            }

            Database.Instance.Disconnect();
            return result;
        }

        // returns empty list or normal list
        static public List<User> FetchUserBatchInstancesForProfiles(List<int> primaryKeys)
        {
            List<User> result = new List<User>();
            string tableName = "_user";
            string[] columnNames = { "user_id", "full_name", "password", "date_of_birth", "gender" };
            string joinTableName = "_user";
            string joinTableConditionColumnName = "user_id";
            string joinTablePrimaryKeyColumnName = "user_id";

            try
            {
                Database.Instance.Connect();

                string smallQuery = $"SELECT u.{columnNames[0]}, u.{columnNames[1]}, u.{columnNames[2]}, u.{columnNames[3]}, u.{columnNames[4]} " +
                    $" FROM {tableName} u JOIN {joinTableName} j ON u.{columnNames[0]} = j.{joinTableConditionColumnName} ";

                string query = $"{smallQuery} WHERE u.{columnNames[0]} IN ({string.Join(",", primaryKeys)}) " +
                    $" ORDER BY j.{joinTablePrimaryKeyColumnName} DESC ";

                using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int user_id = reader.GetInt32(0);
                            string full_name = reader.GetString(1);
                            string password = reader.GetString(2);
                            DateTime DOB = reader.GetDateTime(3);
                            char gender = reader.GetChar(4);
                            result.Add(new User(user_id, full_name, password, DOB.Date, gender));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FetchUserBatchInstancesForProfiles: {ex.Message}");
            }
            finally
            {
                Database.Instance.Disconnect();
            }

            return result;
        }

        // returns 0 or newestUserId
        static public int NewestUserRowId()
        {
            string tableName = "_user";
            int newestId = 0;

            Database.Instance.Connect();
            string query = $"SELECT * FROM {tableName} ORDER BY 1 DESC LIMIT 1"; // Using column index
            using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        newestId = reader.GetInt32(0); // since the first column is always of integer type
                    }
                }
            }
            Database.Instance.Disconnect();
            return newestId;
        }

        // returns 0 or rowCount
        static public int GetUserRowCount()
        {
            string tableName = "_user";
            int rowCount = 0;

            Database.Instance.Connect();
            string countQuery = $"SELECT COUNT(*) FROM {tableName}";
            using (MySqlCommand cmd = new MySqlCommand(countQuery, Database.connection))
            {
                rowCount = Convert.ToInt32(cmd.ExecuteScalar());
            }
            Database.Instance.Disconnect();
            return rowCount;
        }

        static public void CreatePost(int posterId, string content)
        {
            new Post(posterId, content);
        }
        static public void DeletePost(int postId)
        {

            // this will delete everything related to the deleted post because of the foreign key ON DELETE CASCADE
            PostService.RemovePost(postId);
        }


        static public void CreateComment(int postId, int commentatorId, string content)
        {
            new Comment(postId, commentatorId, content);

        }
        static public void DeleteComment(int commentId)
        {

            // this will delete everything related to the deleted comment because of the foreign key ON DELETE CASCADE
            CommentService.RemoveComment(commentId);
        }


        static public void SendFriendRequest(int userId, int potentielFriendId)
        {
            new Friend(userId, potentielFriendId);

        }
        static public void FriendRequestDecision(int userId, int potentielFriendId, string decision)
        {
            decision = decision.ToLower().Trim();
            int primaryKey = FriendService.SearchPotentielFriendBasedOnBothUsers(potentielFriendId, userId);
            if (primaryKey != 0)
            {
                if (decision == "accept")
                {
                    FriendService.AcceptFriendRequest(primaryKey);
                }
                else
                {
                    FriendService.RemoveFriend(primaryKey);
                }
            }
        }
        static public void RemoveFriend(int user1Id, int user2Id)
        {
            int primaryKey;

            primaryKey = FriendService.SearchFriendBasedOnBothUsers(user1Id, user2Id);
            if (primaryKey != 0)
                FriendService.RemoveFriend(primaryKey);
        }


        static public void DeleteAccount(int userId)
        {

            // this will delete everything related to the deleted user because of the foreign key ON DELETE CASCADE
            RemoveUser(userId);

        }



    }
}
