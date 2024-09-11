using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EchoVibe.Backend.Classes;
using EchoVibe.Backend.Services;
using EchoVibe.View.App_Windows;
using EchoVibe.View.Control_User;


namespace EchoVibe.Backend.Services
{
    static class CommentService
    {

        static public void AddComment(int postId, int commentatorId, string content)
        {
            int commentId = 0;// this value does not matter, this is the primary key of the table in the database and it is auto_incremented
            DateTime dateOfComment = DateTime.Now;
            int likeCounter = 0;


            string tableName = "_comment";
            string[] columnNames = { "comment_id", "post_id", "commentator_id", "content", "date_of_comment", "like_counter" };
            string[] columnValues ={ $"{commentId}", $"{postId}",$"{commentatorId}",
                                     $"{content}", dateOfComment.ToString("yyyy-MM-dd HH:mm:ss"), $"{likeCounter}" };
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

            // now that the comment has been added to the table, it's time to increment the post's commentCounter
            Post thePost = PostService.FetchPost(postId);
            if (thePost.PostId != 0)
                PostService.IncrementCommentCounter(postId, thePost.CommentCounter);

        }

        static public void RemoveComment(int primaryKey)
        {
            // first we get the postId of the comment to decrement its commentCounter later
            Comment theComment = CommentService.FetchComment(primaryKey);
            int postId = theComment.PostId;


            string commentTableName = "_comment";
            string primaryKeyColumnName = "comment_id";


            Database.Instance.Connect();
            // Delete from the main user table
            string deleteUserQuery = $"DELETE FROM {commentTableName} WHERE {primaryKeyColumnName} = @primaryKey";
            using (MySqlCommand cmd = new MySqlCommand(deleteUserQuery, Database.connection))
            {
                cmd.Parameters.AddWithValue("@primaryKey", primaryKey);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Comment and related records deleted successfully.");
                }
                else
                {
                    MessageBox.Show("Comment not found or an error occurred.");
                }
            }

            Database.Instance.Disconnect();

            // now the comment has been removed, it's time to decrement the post's commentCounter
            Post thePost = PostService.FetchPost(postId);
            if (thePost.PostId != 0)
                PostService.DecrementCommentCounter(postId, thePost.CommentCounter);

        }



        
        // returns empty list or normal list
        static public List<int> SearchListOfCommentBasedOnPostId(int postId)
        {
            List<int> primaryKeys = new List<int>();
            string tableName = "_comment";
            string primaryKeyColumnName = "comment_id";
            string fatherColumnName = "post_id";

            Database.Instance.Connect();
            string query = $"SELECT {primaryKeyColumnName} FROM {tableName} WHERE {fatherColumnName} = @columnValue";

            using (MySqlCommand command = new MySqlCommand(query, Database.connection))
            {
                command.Parameters.AddWithValue("@columnValue", postId);

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


        // returns null or comment instance
        static public Comment FetchComment(int primaryKey)
        {
            string tableName = "_comment";
            string primaryKeyColumnName = "comment_id";

            Database.Instance.Connect();
            Comment result = null;
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
                            int comment_id = reader.GetInt32(0);
                            int post_id = reader.GetInt32(1);
                            int commentator_id = reader.GetInt32(2);
                            string content = reader.GetString(3);
                            DateTime DOC = reader.GetDateTime(4);
                            int like_counter = reader.GetInt32(5);

                            result = new Comment(comment_id, post_id, commentator_id, content, DOC, like_counter);

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

        // returns 0 or newestCommentId
        static public int NewestCommentRowId()
        {
            string tableName = "_comment";
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
        static public int GetCommentRowCount()
        {
            string tableName = "_comment";
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

        static public void UpdateNumberOfLikesInCommentRow(int primaryKey, int newNumberOfLikes)
        {
            string tableName = "_comment";
            string primaryKeyColumnName = "comment_id";
            string likeCounterColumnName = "like_counter";
            Database.Instance.Connect();
            try
            {
                string updateQuery = $"UPDATE {tableName} SET {likeCounterColumnName} = @numberOfLikes WHERE {primaryKeyColumnName} = @primaryKey";
                using (MySqlCommand command = new MySqlCommand(updateQuery, Database.connection))
                {

                    command.Parameters.AddWithValue("@numberOfLikes", newNumberOfLikes);
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
        static public int GetCommentNumberOfLikes(int primaryKey)
        {
            string tableName = "_comment";
            string primaryKeyColumnName = "comment_id";
            string likeCounterOnComment = "like_counter";


            int result = 0;
            string query = $"SELECT {likeCounterOnComment} FROM {tableName} WHERE {primaryKeyColumnName} = @commentPrimaryKey";
            Database.Instance.Connect();
            try
            {
                using (MySqlCommand command = new MySqlCommand(query, Database.connection))
                {
                    command.Parameters.AddWithValue("@commentPrimaryKey", primaryKey);
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


        static public void ToggleLike(int commentId, int actorId, int likeCounter)
        {
            int primaryKey = CommentLikeService.SearchLikeBasedOnCommentAndUserIds(commentId, actorId);

            if (primaryKey == 0)
            {
                new CommentLike(commentId, actorId);
                UpdateNumberOfLikesInCommentRow(commentId, likeCounter + 1);
            }
            else
            {
                CommentLikeService.RemoveLike(primaryKey);
                UpdateNumberOfLikesInCommentRow(commentId, likeCounter - 1);
            }
        }



    }
}