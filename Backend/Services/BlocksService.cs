using EchoVibe.Backend.Classes;
using EchoVibe.View.Control_User;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Xml.Linq;

namespace EchoVibe.Backend.Services
{
    static class BlocksService
    {
        // returns empty list or normal list
        static public List<Post_Block> SelectNewestRowsForPostBlockForProfilePage(int numberOfAlreadyExistentElements, int posterId, User actor)
        {
            string tableName = "_post";
            string creatorIdColumnName = "poster_id";
            List<Post_Block> result = new List<Post_Block>();

            int totalRows = PostService.GetPostRowCount();
            Database.Instance.Connect();

            if (totalRows > numberOfAlreadyExistentElements)
            {
                int numberOfRowsToLoad = 10;

                try
                {
                    List<Post> postsBatch = new List<Post>();
                    string query = $" SELECT * FROM {tableName} WHERE {creatorIdColumnName} = @poster" +
                                   $" ORDER BY 1 DESC LIMIT {numberOfRowsToLoad} OFFSET {numberOfAlreadyExistentElements}";

                    using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
                    {
                        cmd.Parameters.AddWithValue("@poster", posterId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                List<int> listOfUserId = new List<int>();

                                while (reader.Read())
                                {
                                    int post_id = reader.GetInt32(0);
                                    int poster_id = reader.GetInt32(1);
                                    string content = reader.GetString(2);
                                    DateTime DOP = reader.GetDateTime(3);
                                    int like_counter = reader.GetInt32(4);
                                    int comment_counter = reader.GetInt32(5);
                                    postsBatch.Add(new Post(post_id, poster_id, content, DOP, like_counter, comment_counter));
                                    listOfUserId.Add(poster_id);
                                }

                                reader.Close();

                                List<User> usersBatch = UserService.FetchUserBatchInstancesForPosts(listOfUserId);

                                for (int i = 0; i < postsBatch.Count; i++)
                                {
                                    Post_Block obj = new Post_Block(postsBatch[i], actor, usersBatch[i]);
                                    result.Add(obj);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
                finally
                {
                    Database.Instance.Disconnect();
                }
            }

            return result;
        }

        // returns empty list or normal list
        static public List<Post_Block> SelectNewestRowsForPostBlockForHomePage(int numberOfAlreadyExistentElements, User actor)
        {
            string tableName = "_post";
            string creatorIdColumnName = "poster_id";
            string conditionColumnName = "status";
            string friendTable = "_friend";
            string user1IdColumnName = "user1_id";
            string user2IdColumnName = "user2_id";

            List<Post_Block> result = new List<Post_Block>();

            int totalRows = PostService.GetPostRowCount();
            Database.Instance.Connect();

            if (totalRows > numberOfAlreadyExistentElements)
            {
                int numberOfRowsToLoad = 10;


                List<Post> postsBatch = new List<Post>();

                string creatorsIdQuery =
                                      $"(SELECT {user1IdColumnName} FROM {friendTable} " +
                                      $" WHERE {user2IdColumnName} = @actorId AND {conditionColumnName} = 'accepted' " +
                                      $" UNION " +
                                      $" SELECT {user2IdColumnName} FROM {friendTable}" +
                                      $" WHERE {user1IdColumnName} = @actorId AND {conditionColumnName} = 'accepted' " +
                                      $" UNION " +
                                      $" SELECT @actorId)";
                string query = $" SELECT * FROM {tableName} WHERE {creatorIdColumnName} IN {creatorsIdQuery} " +
                               $" ORDER BY 1 DESC LIMIT {numberOfRowsToLoad} OFFSET {numberOfAlreadyExistentElements}";

                

                using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
                {
                    cmd.Parameters.AddWithValue("@actorId", actor.UserId);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    List<int> listOfUserId = new List<int>();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {


                            int post_id = reader.GetInt32(0);
                            int poster_id = reader.GetInt32(1);
                            string content = reader.GetString(2);
                            DateTime DOP = reader.GetDateTime(3);
                            int like_counter = reader.GetInt32(4);
                            int comment_counter = reader.GetInt32(5);
                            postsBatch.Add(new Post(post_id, poster_id, content, DOP, like_counter, comment_counter));
                            listOfUserId.Add(poster_id);

                        }
                        reader.Close();

                        List<User> usersBatch = UserService.FetchUserBatchInstancesForPosts(listOfUserId);


                        for (int i = 0; i < postsBatch.Count; i++)
                        {
                            Post_Block obj = new Post_Block(postsBatch[i], actor, usersBatch[i]);
                            result.Add(obj);
                        }
                    }
                }


            }
            Database.Instance.Disconnect();
            return result;

        }

        // returns empty list or normal list
        static public List<User_Block> SelectNewestRowsForFriendBlock(int numberOfAlreadyExistentElements, User mainUser)
        {
            string tableName = "_friend";
            string user1IdColumnName = "user1_id";
            string user2IdColumnName = "user2_id";
            string statusColumnName = "status";
            string status = "accepted";

            List<User_Block> result = new List<User_Block>();


            int numberOfRowsToLoad = 10;
            try
            {
                List<int> listOfUser1Id = new List<int>();
                List<int> listOfUser2Id = new List<int>();

                try
                {
                    Database.Instance.Connect();

                    string query1 = $"SELECT * FROM {tableName} WHERE" +
                        $" {user2IdColumnName} = @userId  AND {statusColumnName} = @status" +
                        $" ORDER BY 1 DESC LIMIT {numberOfRowsToLoad} OFFSET {numberOfAlreadyExistentElements}";

                    using (MySqlCommand cmd = new MySqlCommand(query1, Database.connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", mainUser.UserId);
                        cmd.Parameters.AddWithValue("@status", status);

                        MySqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {

                            while (reader.Read())
                            {
                                listOfUser1Id.Add(reader.GetInt32(1)); // user1Id is the second column

                            }
                            reader.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");

                }
                Database.Instance.Disconnect();
                try
                {
                    Database.Instance.Connect();

                    string query2 = $"SELECT * FROM {tableName} WHERE" +
                        $" ({user1IdColumnName} = @userId  AND {statusColumnName} = @status)" +
                        $" ORDER BY 1 DESC LIMIT {numberOfRowsToLoad} OFFSET {numberOfAlreadyExistentElements}";

                    using (MySqlCommand cmd = new MySqlCommand(query2, Database.connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", mainUser.UserId);
                        cmd.Parameters.AddWithValue("@status", status);

                        MySqlDataReader reader = cmd.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                listOfUser2Id.Add(reader.GetInt32(2)); // user2Id is the third column

                            }
                            reader.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");

                }
                Database.Instance.Disconnect();


                List<User> usersBatch = UserService.FetchUserBatchInstancesForFriends(listOfUser1Id, listOfUser2Id, mainUser.UserId);

                for (int i = 0; i < usersBatch.Count; i++)
                {
                    User_Block obj = new User_Block(usersBatch[i], mainUser);// mainUser is the future actor
                    result.Add(obj);
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
        static public List<User_Block> SelectNewestRowsForPotentielFriendBlock(int numberOfAlreadyExistentElements, User mainUser)
        {
            string tableName = "_friend";
            string user2IdColumnName = "user2_id";
            string statusColumnName = "status";
            string status = "pending";

            List<User_Block> result = new List<User_Block>();


            int numberOfRowsToLoad = 10;
            try
            {
                List<int> listOfUser1Id = new List<int>();
                List<int> listOfUser2Id = new List<int>();

                try
                {
                    Database.Instance.Connect();

                    string query1 = $"SELECT * FROM {tableName} WHERE" +
                        $" {user2IdColumnName} = @userId  AND {statusColumnName} = @status" +
                        $" ORDER BY 1 DESC LIMIT {numberOfRowsToLoad} OFFSET {numberOfAlreadyExistentElements}";

                    using (MySqlCommand cmd = new MySqlCommand(query1, Database.connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", mainUser.UserId);
                        cmd.Parameters.AddWithValue("@status", status);

                        MySqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {

                            while (reader.Read())
                            {
                                listOfUser1Id.Add(reader.GetInt32(1)); // user1Id is the second column

                            }
                            reader.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");

                }
                Database.Instance.Disconnect();


                List<User> usersBatch = UserService.FetchUserBatchInstancesForPotentielFriends(listOfUser1Id, listOfUser2Id, mainUser.UserId);

                for (int i = 0; i < usersBatch.Count; i++)
                {
                    User_Block obj = new User_Block(usersBatch[i], mainUser);// mainUser is the future actor
                    obj.SetGridVisibility("RequestGrid");
                    result.Add(obj);
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
        static public List<Comment_Block> SelectNewestRowsForCommentBlock(int numberOfAlreadyExistentElements, int postId, User actor)
        {
            string tableName = "_comment";
            string fatherColumnName = "post_id";
            List<Comment_Block> result = new List<Comment_Block>();

            int totalRows = CommentService.GetCommentRowCount();

            Database.Instance.Connect();
            if (totalRows > numberOfAlreadyExistentElements)
            {
                int numberOfRowsToLoad = 10;

                try
                {
                    string query = $"SELECT * FROM {tableName} WHERE {fatherColumnName} = @postId" +
                        $" ORDER BY 1 DESC LIMIT {numberOfRowsToLoad} OFFSET {numberOfAlreadyExistentElements}";

                    using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
                    {
                        cmd.Parameters.AddWithValue("@postId", postId);

                        MySqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            List<Comment> commentsBatch = new List<Comment>();
                            List<int> listOfUserId = new List<int>();

                            while (reader.Read())
                            {
                                int comment_id = reader.GetInt32(0);
                                int post_id = reader.GetInt32(1);
                                int commentator_id = reader.GetInt32(2);
                                string content = reader.GetString(3);
                                DateTime DOC = reader.GetDateTime(4);
                                int like_counter = reader.GetInt32(5);

                                commentsBatch.Add(new Comment(comment_id, post_id, commentator_id, content, DOC, like_counter));
                                listOfUserId.Add(commentator_id);
                            }
                            reader.Close();
                            List<User> usersBatch = UserService.FetchUserBatchInstancesForComments(listOfUserId);

                            for (int i = 0; i < commentsBatch.Count; i++)
                            {
                                Comment_Block obj = new Comment_Block(commentsBatch[i], actor, usersBatch[i]);
                                result.Add(obj);
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
                finally
                {
                    Database.Instance.Disconnect();
                }
            }
            return result;
        }

        // returns empty list or normal list
        static public List<User_Block> SelectNewestRowsForPostLikeBlock(int numberOfAlreadyExistentElements, int postId, User mainUser)
        {
            string tableName = "_post_like";
            string postIdColumnName = "post_id";
            List<User_Block> result = new List<User_Block>();

            int totalRows = PostLikeService.GetLikeRowCount();

            Database.Instance.Connect();
            if (totalRows > numberOfAlreadyExistentElements)
            {
                int numberOfRowsToLoad = 10;
                try
                {
                    string query = $"SELECT * FROM {tableName} WHERE {postIdColumnName} = @postId" +
                        $" ORDER BY 1 DESC LIMIT {numberOfRowsToLoad} OFFSET {numberOfAlreadyExistentElements}";
                    using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
                    {
                        cmd.Parameters.AddWithValue("@postId", postId);

                        MySqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            List<int> listOfUserId = new List<int>();

                            while (reader.Read())
                            {



                                listOfUserId.Add(reader.GetInt32(2));

                            }
                            reader.Close();
                            List<User> usersBatch = UserService.FetchUserBatchInstancesForPostLikes(listOfUserId);


                            for (int i = 0; i < usersBatch.Count; i++)
                            {
                                User_Block obj = new User_Block(usersBatch[i], mainUser);
                                result.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");

                }
            }
            Database.Instance.Disconnect();
            return result;


        }

        // returns empty list or normal list
        static public List<User_Block> SelectNewestRowsForCommentLikeBlock(int numberOfAlreadyExistentElements, int commentId, User mainUser)
        {
            string tableName = "_comment_like";
            string commentIdColumnName = "comment_id";
            List<User_Block> result = new List<User_Block>();

            int totalRows = CommentService.GetCommentNumberOfLikes(commentId);
            Database.Instance.Connect();
            if (totalRows > numberOfAlreadyExistentElements)
            {
                int numberOfRowsToLoad = 10;
                try
                {
                    string query = $"SELECT * FROM {tableName} " +
                        $" WHERE  {commentIdColumnName} = @comment " +
                        $" ORDER BY 1 DESC LIMIT {numberOfRowsToLoad} OFFSET {numberOfAlreadyExistentElements}";

                    using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
                    {
                        cmd.Parameters.AddWithValue("@comment", commentId);

                        MySqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            List<int> listOfUserId = new List<int>();

                            while (reader.Read())
                            {



                                listOfUserId.Add(reader.GetInt32(2));

                            }
                            reader.Close();
                            List<User> usersBatch = UserService.FetchUserBatchInstancesForCommentLikes(listOfUserId);


                            for (int i = 0; i < usersBatch.Count; i++)
                            {
                                User_Block obj = new User_Block(usersBatch[i], mainUser);
                                result.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");

                }
            }
            Database.Instance.Disconnect();
            return result;


        }

        // returns empty list or normal list
        static public List<User_Block> SelectNewestRowsForProfileBlock(int numberOfAlreadyExistentElements, string fullName, User mainUser)
        {
            string tableName = "_user";
            string fullNameColumnName = "full_name";
            List<User_Block> result = new List<User_Block>();

            int totalRows = UserService.GetUserRowCount();
            Database.Instance.Connect();
            if (totalRows > numberOfAlreadyExistentElements)
            {
                int numberOfRowsToLoad = 10;
                try
                {
                    List<User> usersBatch = new List<User>();
                    string query = $"SELECT * FROM {tableName} " +
                        $"  WHERE LOWER({fullNameColumnName}) LIKE LOWER(@fullName)  " +
                        $" ORDER BY 1 DESC LIMIT {numberOfRowsToLoad} OFFSET {numberOfAlreadyExistentElements}";




                    using (MySqlCommand cmd = new MySqlCommand(query, Database.connection))
                    {
                        cmd.Parameters.AddWithValue("@fullName", fullName);

                        MySqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            //List<int> listOfPostId = new List<int>();
                            List<int> listOfUserId = new List<int>();

                            while (reader.Read())
                            {


                                listOfUserId.Add(reader.GetInt32(0));

                            }
                            reader.Close();
                            usersBatch = UserService.FetchUserBatchInstancesForProfiles(listOfUserId);


                            for (int i = 0; i < usersBatch.Count; i++)
                            {
                                User_Block obj = new User_Block(usersBatch[i], mainUser);
                                result.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");

                }
            }
            Database.Instance.Disconnect();
            return result;


        }


    }
}
