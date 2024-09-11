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
using System.Windows.Input;
using MySqlX.XDevAPI.Common;

namespace EchoVibe.Backend.Services
{
    static class PostLikeService
    {

        static public void AddLike(int postId, int likerId)
        {
            int likeId = 0;// this value does not matter, this is the primary key of the table in the database and it is auto_incremented
            string tableName = "_post_like";
            string[] columnNames = { "like_id", "post_id", "liker_id" };
            string[] columnValues = { $"{likeId}", $"{postId}",$"{likerId}" };
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
        static public void RemoveLike(int primaryKey)
        {
            string tableName = "_post_like";
            string primaryKeyColumnName = "like_id";

            Database.Instance.Connect();
            string deleteQuery = $"DELETE FROM {tableName} WHERE {primaryKeyColumnName} = @primaryKey";
            using (MySqlCommand cmd = new MySqlCommand(deleteQuery, Database.connection))
            {
                cmd.Parameters.AddWithValue("@primaryKey", primaryKey);


                int rowsAffected = cmd.ExecuteNonQuery();


            }
            Database.Instance.Disconnect();
        }

        // returns 0 or primaryKey
        static public int SearchLikeBasedOnPostAndUserIds(int postId,int likerId)
        {
            string tableName = "_post_like";
            string postIdColumnName = "post_id";
            string likerIdColumnName = "liker_id";
            int result = 0;
            Database.Instance.Connect();
            string query = $"SELECT * FROM {tableName} WHERE {postIdColumnName} = @postId AND {likerIdColumnName} = @likerId";
            using (MySqlCommand cmd = new MySqlCommand(query,Database.connection))
            {

                cmd.Parameters.AddWithValue("@postId", postId);
                cmd.Parameters.AddWithValue("@likerId", likerId);



                object objResult = cmd.ExecuteScalar();
                if (objResult != null)
                    result = (int)objResult;


            }
            Database.Instance.Disconnect();

            return result;
        }

        // returns empty list or normal list
        static public List<int> SearchListOfLikesBasedOnPostId(int postId)
        {
            List<int> primaryKeys = new List<int>();
            string tableName = "_post_like";
            string primaryKeyColumnName = "like_id";
            string postColumnName = "post_id";

            string query = $"SELECT {primaryKeyColumnName} FROM {tableName} WHERE  {postColumnName} = @postId";

            using (MySqlCommand command = new MySqlCommand(query, Database.connection))
            {
                command.Parameters.AddWithValue("@postId", postId);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int primaryKey = reader.GetInt32(0);
                        primaryKeys.Add(primaryKey);
                    }
                }
            }
            Database.Instance.Disconnect();

            return primaryKeys;
        }

        // returns null or postLike instance
        static public PostLike FetchLike(int primaryKey)
        {
            string tableName = "_post_like";
            string primaryKeyColumnName = "like_id";

            Database.Instance.Connect();
            PostLike result = null;
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
                            int like_id = reader.GetInt32(0);
                            int post_id = reader.GetInt32(1);
                            int liker_id = reader.GetInt32(2);
                            result = new PostLike(like_id, post_id, liker_id);

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

        // returns 0 or newestPostLikeId
        static public int NewestLikeRowId()
        {
            string tableName = "_post_like";
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
        static public int GetLikeRowCount()
        {
            string tableName = "_post_like";
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
