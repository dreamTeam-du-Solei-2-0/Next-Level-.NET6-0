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
            this.connection = connection;
            this.users = null;
            this.context = context;
        }
        public List<User> GetUsers()
        {
            if (users != null)
                return users;
            users = new List<User>();
            try
            {
                using SqlCommand cmd = new()
                {
                    Connection = connection,
                    CommandText = @"SELECT * FROM Users WHERE DeleteDt IS NULL"
                };
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                    users.Add(new(reader) { dataContext = this.context });
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
