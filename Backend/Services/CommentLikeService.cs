using EchoVibe.Backend.Classes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EchoVibe.Backend.Services
{
    static class CommentLikeService
    {
        static public void AddLike(int commentId, int likerId)
        {
            int likeId = 0;// this value does not matter, this is the primary key of the table in the database and it is auto_incremented
            string tableName = "_comment_like";
            string[] columnNames = { "like_id", "comment_id", "liker_id" };
            string[] columnValues = { $"{likeId}", $"{commentId}", $"{likerId}" };
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
            string tableName = "_comment_like";
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
        static public int SearchLikeBasedOnCommentAndUserIds(int commentId, int likerId)
        {
            string tableName = "_comment_like";
            string key1ColumnName = "comment_id";
            string key2ColumnName = "liker_id";
            int result = 0;
            Database.Instance.Connect();
            string query = $"SELECT * FROM {tableName} WHERE {key1ColumnName} = @Key1 AND {key2ColumnName} = @Key2";
            using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
            {

                cmd.Parameters.AddWithValue("@Key1", commentId);
                cmd.Parameters.AddWithValue("@Key2", likerId);



                object objResult = cmd.ExecuteScalar();
                if (objResult != null)
                    result = (int)objResult;

               
            }
            Database.Instance.Disconnect();

            return result;
        }

        // returns empty list or normal list
        static public List<int> SearchListOfLikesBasedOnCommentId(int commentId)
        {
            List<int> primaryKeys = new List<int>();
            string tableName = "_comment_like";
            string primaryKeyColumnName = "like_id";
            string postColumnName = "comment_id";

            Database.Instance.Connect();
            string query = $"SELECT {primaryKeyColumnName} FROM {tableName} WHERE  {postColumnName} = @commentId";

            using (MySqlCommand command = new MySqlCommand(query, Database.connection))
            {
                command.Parameters.AddWithValue("@commentId", commentId);

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

        // returns null or commentLike instance
        static public CommentLike FetchLike(int primaryKey)
        {
            string tableName = "_comment_like";
            string primaryKeyColumnNames = "like_id";

            Database.Instance.Connect();
            CommentLike result = null;
            string query = $"SELECT * FROM {tableName} WHERE {primaryKeyColumnNames} = @primaryKey";
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
                            int comment_id = reader.GetInt32(1);
                            int liker_id = reader.GetInt32(2);
                            result = new CommentLike(like_id, comment_id, liker_id);

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

        // returns 0 or newestCommentLikeId
        static public int NewestLikeRowId()
        {
            string tableName = "_comment_like";
            int newestId = 0;
            Database.Instance.Connect();
            string query = $"SELECT * FROM {tableName} ORDER BY 1 DESC LIMIT 1"; 
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
            string tableName = "_comment_like";
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
