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
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace EchoVibe.Backend.Services
{
    static class FriendService
    {
        static public void AddFriend(int user1Id, int user2Id)
        {
            int friendId = 0;// this value does not matter, this is the primary key of the table in the database and it is auto_incremented
            string status = "pending";
            string tableName = "_friend";
            string[] columnNames = { "friend_id", "user1_id", "user2_id", "status" };
            string[] columnValues = { $"{friendId}", $"{user1Id}", $"{user2Id}", $"{status}" };
            string columns = string.Join(", ", columnNames);
            string values = string.Join(", ", columnNames.Select(c => $"@{c}"));

            Database.Instance.Connect();
            using (MySqlCommand cmd = new MySqlCommand($"INSERT INTO {tableName} ({columns}) VALUES ({values})", Database.connection))
            {
                for (int i = 0; i < columnNames.Length; i++)
                {
                    cmd.Parameters.AddWithValue($"@{columnNames[i]}", columnValues[i]);
                }

                int rowsAffected = cmd.ExecuteNonQuery();


            }
            Database.Instance.Disconnect();
        }

        static public int SearchPotentielFriendBasedOnUser2(int user1Id, int user2Id)
        {
            string tableName = "_friend";
            string user1ColumnName = "user2_id";
            string user2ColumnName = "user1_id";
            string statusColumnName = "status";
            string status = "pending";
            int result = 0;

            Database.Instance.Connect();

            string query = $"SELECT * FROM {tableName} WHERE " +
                $" {user1ColumnName} = @user1Id AND {user2ColumnName} = @user2Id " +
                $" AND {statusColumnName} = @status";

            using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
            {
                cmd.Parameters.AddWithValue("@user1Id", user1Id);
                cmd.Parameters.AddWithValue("@user2Id", user2Id);
                cmd.Parameters.AddWithValue("@status", status);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }
            }

            Database.Instance.Disconnect();

            return result;
        }
        static public int SearchPotentielFriendBasedOnUser1(int user1Id, int user2Id)
        {
            string tableName = "_friend";
            string user1ColumnName = "user1_id";
            string user2ColumnName = "user2_id";
            string statusColumnName = "status";
            string status = "pending";
            int result = 0;

            Database.Instance.Connect();

            string query = $"SELECT * FROM {tableName} WHERE " +
                $" {user1ColumnName} = @user1Id AND {user2ColumnName} = @user2Id " +
                $" AND {statusColumnName} = @status";

            using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
            {
                cmd.Parameters.AddWithValue("@user1Id", user1Id);
                cmd.Parameters.AddWithValue("@user2Id", user2Id);
                cmd.Parameters.AddWithValue("@status", status);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }
            }

            Database.Instance.Disconnect();

            return result;
        }

        static public string GetFriendStatusUserRelativeUserBlock(int actorId, int userInUserBlockId)
        {

            int primaryKey;

            primaryKey = SearchFriendBasedOnBothUsers(actorId, userInUserBlockId);
            if (primaryKey != 0)
                return "AlreadyFriend";

            primaryKey = SearchPotentielFriendBasedOnUser1(actorId, userInUserBlockId);
            if (primaryKey != 0)
                return "RequestAnswer";
            primaryKey = SearchPotentielFriendBasedOnUser2(actorId, userInUserBlockId);
            if (primaryKey != 0)
                return "RequestSent";

            return "SendFriendRequest";

        }


        static public void RemoveFriend(int primaryKey)
        {
            string tableName = "_friend";
            string primaryKeyColumnName = "friend_id";
            string deleteQuery = $"DELETE FROM {tableName} WHERE {primaryKeyColumnName} = @primaryKey";

            Database.Instance.Connect();
            using (MySqlCommand cmd = new MySqlCommand(deleteQuery, Database.connection))
            {
                cmd.Parameters.AddWithValue("@primaryKey", primaryKey);


                int rowsAffected = cmd.ExecuteNonQuery();
                MessageBox.Show("Friend Removed");


            }
            Database.Instance.Disconnect();
        }

        // returns 0 or primaryKey
        static public int SearchFriendBasedOnBothUsers(int user1Id, int user2Id)
        {
            string tableName = "_friend";
            string user1ColumnName = "user1_id";
            string user2ColumnName = "user2_id";
            string statusColumnName = "status";
            string status = "accepted";
            int result = 0;

            Database.Instance.Connect();

            string query = $"SELECT * FROM {tableName} WHERE " +
                $" ( ({user1ColumnName} = @user1Id AND {user2ColumnName} = @user2Id ) " +
                $" OR ({user1ColumnName} = @user2Id AND {user2ColumnName} = @user1Id) ) " +
                $" AND {statusColumnName} = @status";

            using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
            {
                cmd.Parameters.AddWithValue("@user1Id", user1Id);
                cmd.Parameters.AddWithValue("@user2Id", user2Id);
                cmd.Parameters.AddWithValue("@status", status);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }
            }

            Database.Instance.Disconnect();

            return result;
        }


        // returns 0 or primaryKey
        static public int SearchPotentielFriendBasedOnBothUsers(int user1Id, int user2Id)
        {
            string tableName = "_friend";
            string user1ColumnName = "user1_id";
            string user2ColumnName = "user2_id";
            string statusColumnName = "status";
            string status = "pending";
            int result = 0;

            Database.Instance.Connect();

            string query = $"SELECT * FROM {tableName} WHERE " +
                $" ( ({user1ColumnName} = @user1Id AND {user2ColumnName} = @user2Id ) " +
                $" OR ({user1ColumnName} = @user2Id AND {user2ColumnName} = @user1Id) ) " +
                $" AND {statusColumnName} = @status";

            using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
            {
                cmd.Parameters.AddWithValue("@user1Id", user1Id);
                cmd.Parameters.AddWithValue("@user2Id", user2Id);
                cmd.Parameters.AddWithValue("@status", status);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }
            }

            Database.Instance.Disconnect();

            return result;
        }

        // returns empty list or normal list
        static public List<int> SearchListOfFriendBasedOnOneUser(int userId)
        {
            List<int> primaryKeys = new List<int>();
            string tableName = "_friend";
            string primaryKeyColumnName = "friend_id";
            string columnName1 = "user1_id";
            string columnName2 = "user2_id";
            string columnName3 = "status";
            string status = "accepted";


            string query = $"SELECT {primaryKeyColumnName} FROM {tableName} WHERE " +
                $"({columnName1} = @userId OR {columnName2} = @userId) AND {columnName3} = @status";

            using (MySqlCommand command = new MySqlCommand(query, Database.connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@status", status);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int primaryKey = reader.GetInt32("PrimaryKeyColumn");
                        primaryKeys.Add(primaryKey);
                    }
                }
            }


            return primaryKeys;
        }

        // returns empty list or normal list
        static public List<int> SearchListOfPotentielFriendBasedOnOneUser(int userId)
        {
            List<int> primaryKeys = new List<int>();
            string tableName = "_friend";
            string primaryKeyColumnName = "friend_id";
            string columnName1 = "user1_id";
            string columnName2 = "user2_id";
            string columnName3 = "status";
            string status = "pending";

            string query = $"SELECT {primaryKeyColumnName} FROM {tableName} WHERE " +
                $" ({columnName1} = @userId OR {columnName2} = @userId) AND {columnName3} = @status";

            using (MySqlCommand command = new MySqlCommand(query, Database.connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@status", status);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int primaryKey = reader.GetInt32(0);

                        primaryKeys.Add(primaryKey);
                    }
                }
            }


            return primaryKeys;
        }

        // returns null or friend instance
        static public Friend FetchFriend(int primaryKey)
        {
            string tableName = "_friend";
            string primaryColumnName = "friend_id";

            Database.Instance.Connect();
            Friend result = null;
            string query = $"SELECT * FROM {tableName} WHERE {primaryColumnName} = @primaryKey";
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
                {

                    cmd.Parameters.AddWithValue("@primaryKey", primaryKey);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int friend_id = reader.GetInt32(0);
                            int user1_id = reader.GetInt32(1);
                            int user2_id = reader.GetInt32(2);
                            string status = reader.GetString(3);
                            result = new Friend(friend_id, user1_id, user2_id, status);

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

        static public void AcceptFriendRequest(int primaryKey)
        {
            string tableName = "_friend";
            string primaryKeyColumnName = "friend_id";
            string statusColumnName = "status";
            string status = "accepted";
            Database.Instance.Connect();
            try
            {
                string updateQuery = $"UPDATE {tableName} SET {statusColumnName} = @status WHERE {primaryKeyColumnName} = @primaryKey ";
                using (MySqlCommand command = new MySqlCommand(updateQuery, Database.connection))
                {

                    command.Parameters.AddWithValue("@status", status);
                    command.Parameters.AddWithValue("@primaryKey", primaryKey);



                    int rowsAffected = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

            Database.Instance.Disconnect();

        }

        // returns 0 or newestFriendId
        static public int NewestFriendRowId()
        {
            string tableName = "_friend";
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
        static public int GetFriendRowCount()
        {
            string tableName = "_friend";
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




    }
}
