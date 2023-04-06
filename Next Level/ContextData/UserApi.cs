using Next_Level.Entity;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next_Level.ContextData
{
    internal class UserApi
    {
        private SqlConnection connection;
        private List<User> users;
        private readonly DataContext context;

        public UserApi(SqlConnection connection, DataContext context)
        {
            this.context = context;
            this.connection = connection;
            this.users = null;
        }
        public bool Update(Entity.User user)
        {
            try
            {
                using SqlCommand cmd = new()
                {
                    Connection = connection,
                    CommandText = @"UPDATE Users
                                    SET
                                    Surname = @surname,	
                                    Name = @name,		
                                    Phone	 = @phone	,
                                    Email = @email	,	
                                    BirthDate = @birthdate
                                    WHERE UserId = @userId;"
                };
                cmd.Parameters.AddWithValue("@userId", user.UserId);
                cmd.Parameters.AddWithValue("@surname", user.Surname);
                cmd.Parameters.AddWithValue("@name", user.Name);
                cmd.Parameters.AddWithValue("@phone", user.Phone);
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@birthdate", user.BirthDate);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                String msg =
                    DateTime.Now + ": " +
                    this.GetType().Name +
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name +
                    " " + ex.Message;

                // TODO: LOG
                App.Logger.Log(msg, "SEVERE");
                return false;
            }
            return true;
        }
        public List<User> GetUsers()
        {
            if (users != null)
                return users;
            users = new List<User>();
            try
            {
                using SqlCommand cmd = new SqlCommand()
                {
                    CommandText = "SELECT * FROM Users WHERE DeleteDt IS NULL",
                    Connection = connection
                };
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new(reader) { dataContext = this.context });
                }
                reader.Close();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                String msg =
                    DateTime.Now + ": " +
                    this.GetType().Name +
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name +
                    " " + ex.Message;

                // TODO: LOG
                App.Logger.Log(msg, "SEVERE");
            }
            return users;
        }
        public bool Add(Entity.User user)
        {
            try
            {
                using SqlCommand cmd = new()
                {
                    Connection = connection,
                    CommandText = @"INSERT INTO Users(UserId, AccountId, Surname, Name)
                                    VALUES(@userId, @accountId, @surname, @name);"
                };
                cmd.Parameters.AddWithValue("@userId", user.UserId);
                cmd.Parameters.AddWithValue("@accountId", user.AccountId);
                cmd.Parameters.AddWithValue("@surname", user.Surname);
                cmd.Parameters.AddWithValue("@name", user.Name);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                String msg =
                    DateTime.Now + ": " +
                    this.GetType().Name +
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name +
                    " " + ex.Message;

                // TODO: LOG
                App.Logger.Log(msg, "SEVERE");
                return false;
            }
            return true;
        }
    }
}
