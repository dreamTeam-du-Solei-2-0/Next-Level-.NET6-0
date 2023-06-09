﻿using Next_Level.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Next_Level.ContextData
{
    internal class AccountApi
    {
        private SqlConnection connection;
        private List<Account> accountsList;
        private readonly DataContext context;
        public AccountApi(SqlConnection _connection, DataContext context)
        {
            this.context = context;
            this.connection = _connection;
            accountsList = null;
        }

        public bool Update(Entity.Account account)
        {
            try
            {
                using SqlCommand cmd = new()
                {
                    Connection = connection,
                    CommandText = @"UPDATE Accounts
                                    SET  
                                    Login = @login, 
                                    Password = @password
                                    WHERE AccountId = @accountId;"
                };
                cmd.Parameters.AddWithValue("@accountId", account.AccountId);
                cmd.Parameters.AddWithValue("@login", account.Login);
                cmd.Parameters.AddWithValue("@password", account.Password);
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

        public bool IsExist(string Login)
        {
            try
            {
                using SqlCommand cmd = new SqlCommand()
                {
                    Connection = connection,
                    CommandText = @"SELECT COUNT(*) 
                                    FROM Accounts
                                    WHERE Login = @login"
                };
                cmd.Parameters.AddWithValue("@login", Login);
                var res = cmd.ExecuteScalar();
                cmd.Dispose();
                int count = Convert.ToInt32(res);
                if (count > 0)
                    return true;
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
            return false;
        }
        public Account GetAccount(String login)
        {
            Account account = new Account();
            try
            {
                using SqlCommand cmd = new()
                {
                    Connection = connection,
                    CommandText = @"
                    SELECT*
                    FROM Accounts
                    WHERE Login = @login"
                };
                cmd.Parameters.AddWithValue("@login", login);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                    account = new(reader) { dataContext = this.context };
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
            return account;
        }
        public bool Add(Entity.Account account)
        {
            try
            {
                using SqlCommand cmd = new()
                {
                    Connection = connection,
                    CommandText = @"INSERT INTO Accounts(AccountId, Login, Password,IsAdmin)
                                    VALUES(@accountId,@login, @password,@isAdmin);"
                };
                cmd.Parameters.AddWithValue("@accountId", account.AccountId);
                cmd.Parameters.AddWithValue("@login", account.Login);
                cmd.Parameters.AddWithValue("@password", account.Password);
                cmd.Parameters.AddWithValue("@isAdmin", account.IsAdmin);
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
        public List<Account> GetAccounts()
        {
            if (accountsList != null)
                return accountsList;
            accountsList = new List<Account>();
            try
            {
                using SqlCommand cmd = new SqlCommand()
                {
                    Connection = connection,
                    CommandText = @"SELECT * FROM Accounts WHERE DeleteDt IS NULL"
                };
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                    accountsList.Add(new(reader) { dataContext = this.context });
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
            return accountsList;
        }
    }
}
