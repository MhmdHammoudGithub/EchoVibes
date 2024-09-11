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

namespace EchoVibe.Backend.Services
{
    static class PostService
    {

        static public void AddPost(int posterId, string content)
        {
            int postId = 0; // this value does not matter, this is the primary key of the table in the database and it is auto_incremented
            DateTime postDate = DateTime.Now;
            int likeCounter = 0;
            int commentCounter = 0;
            string tableName = "_post";
            string[] columnNames = { "post_id", "poster_id", "content", "date_of_post", "like_counter", "comment_counter" };
            string[] columnValues ={ $"{postId}", $"{posterId}",
                                      $"{content}",  postDate.ToString("yyyy-MM-dd HH:mm:ss"), $"{likeCounter}",
                                      $"{commentCounter}" };
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
        static public void RemovePost(int primaryKey)
        {

            string postTableName = "_post";
            string primaryKeyColumnName = "post_id";




            Database.Instance.Connect();
            string deleteUserQuery = $"DELETE FROM {postTableName} WHERE {primaryKeyColumnName} = @primaryKey";
            using (MySqlCommand cmd = new MySqlCommand(deleteUserQuery, Database.connection))
            {
                cmd.Parameters.AddWithValue("@primaryKey", primaryKey);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Post and related records deleted successfully.");
                }
                else
                {
                    MessageBox.Show("Post not found or an error occurred.");
                }
            }

            Database.Instance.Disconnect();


        }

        // returns empty list or normal list
        static public List<int> SearchListOfPostBasedOnPosterId(int posterId)
        {
            List<int> primaryKeys = new List<int>();
            string tableName = "_post";
            string primaryKeyColumnName = "post_id";
            string creatorColumnName = "poster_id";

            Database.Instance.Connect();
            string query = $"SELECT {primaryKeyColumnName} FROM {tableName} WHERE {creatorColumnName} = @posterId";

            using (MySqlCommand command = new MySqlCommand(query, Database.connection))
            {
                command.Parameters.AddWithValue("@posterId", posterId);

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

        // returns null or Post instance
        static public Post FetchPost(int primaryKey)
        {
            string tableName = "_post";
            string primaryKeyColumnname = "post_id";

            Database.Instance.Connect();
            Post result = null;
            string query = $"SELECT * FROM {tableName} WHERE {primaryKeyColumnname} = @primaryKey";
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
                {

                    cmd.Parameters.AddWithValue("@primaryKey", primaryKey);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int post_id = reader.GetInt32(0);
                            int poster_id = reader.GetInt32(1);
                            string content = reader.GetString(2);
                            DateTime DOP = reader.GetDateTime(3);
                            int like_counter = reader.GetInt32(4);
                            int comment_counter = reader.GetInt32(5);
                            result = new Post(post_id, poster_id, content, DOP, like_counter, comment_counter);

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


        // returns 0 or newestPostId 
        static public int NewestPostRowId()
        {
            string tableName = "_post";
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
        static public int GetPostRowCount()
        {
            string tableName = "_post";
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

        static public void UpdateNumberOfLikesInPostRow(int primaryKey, int newNumberOfLikes)
        {
            string tableName = "_post";
            string primaryKeyColumnName = "post_id";
            string columnName = "like_counter";



            Database.Instance.Connect();
            try
            {
                string updateQuery = $"UPDATE {tableName} SET {columnName} = @newNumberOfLikes WHERE {primaryKeyColumnName} = @primaryKey ";
                using (MySqlCommand command = new MySqlCommand(updateQuery, Database.connection))
                {
                    command.Parameters.AddWithValue("@newNumberOfLikes", newNumberOfLikes);
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

        static public void UpdateNumberOfCommentsInPostRow(int primaryKey, int newNumberOfComments)
        {
            string tableName = "_post";
            string primaryKeyColumnName = "post_id";
            string columnName = "comment_counter";



            Database.Instance.Connect();
            try
            {
                string updateQuery = $"UPDATE {tableName} SET {columnName} = @newNumberOfComemnts WHERE {primaryKeyColumnName} = @primaryKey ";
                using (MySqlCommand command = new MySqlCommand(updateQuery, Database.connection))
                {
                    command.Parameters.AddWithValue("@newNumberOfComemnts", newNumberOfComments);
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

        // returns 0 or numberOfLikes
        static public int GetPostNumberOfLikes(int primaryKey)
        {
            string tableName = "_post";
            string primaryKeyColumnName = "post_id";
            string likeCounterOnComment = "like_counter";
            int result = 0;
            string query = $"SELECT {likeCounterOnComment} FROM {tableName} WHERE {primaryKeyColumnName} = @primaryKey";

            Database.Instance.Connect();
            try
            {
                using (MySqlCommand command = new MySqlCommand(query, Database.connection))
                {
                    command.Parameters.AddWithValue("@primaryKey", primaryKey);

                    object resultObject = command.ExecuteScalar();

                    if (resultObject != null && resultObject != DBNull.Value)
                    {
                        result = Convert.ToInt32(resultObject);
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

        // returns 0 or numberOfComments
        static public int GetPostNumberOfComments(int primaryKey)
        {
            string tableName = "_post";
            string primaryKeyColumnName = "post_id";
            string commentCounterOnComment = "comment_counter";
            int result = 0;
            string query = $"SELECT {commentCounterOnComment} FROM {tableName} WHERE {primaryKeyColumnName} = @primaryKey";

            Database.Instance.Connect();
            try
            {
                using (MySqlCommand command = new MySqlCommand(query, Database.connection))
                {
                    command.Parameters.AddWithValue("@primaryKey", primaryKey);

                    object resultObject = command.ExecuteScalar();

                    if (resultObject != null && resultObject != DBNull.Value)
                    {
                        result = Convert.ToInt32(resultObject);
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

        static public void ToggleLike(int postId, int actorId, int likeCounter)
        {

            int LikeprimaryKey = PostLikeService.SearchLikeBasedOnPostAndUserIds(postId, actorId);

            if (LikeprimaryKey == 0)
            {
                new PostLike(postId, actorId);
                UpdateNumberOfLikesInPostRow(postId, likeCounter + 1);
            }
            else
            {
                PostLikeService.RemoveLike(LikeprimaryKey);
                UpdateNumberOfLikesInPostRow(postId, likeCounter - 1);
            }
        }

        static public void IncrementCommentCounter(int postPrimaryKey, int commentCounter)
        {
            UpdateNumberOfCommentsInPostRow(postPrimaryKey, commentCounter + 1);
        }

        static public void DecrementCommentCounter(int postPrimaryKey, int commentCounter)
        {
            UpdateNumberOfCommentsInPostRow(postPrimaryKey, commentCounter - 1);
        }

    }
}
