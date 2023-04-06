using Next_Level.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace Next_Level.ContextData
{
    internal class DataContext
    {
        private SqlConnection connection;
        internal UserApi Users { get; set; }
        internal AccountApi Accounts { get; set; }
        internal CategoryApi Categories { get; set; }
        internal FeedbackApi Feedbacks { get; set; }
        internal ProductApi Products { get; set; }
        public DataContext()
        {
            string db_path = System.IO.Path.GetFullPath(NextLevelPath.DB);
            string temp = db_path.Substring(db_path.IndexOf("\\bin"));
            db_path = db_path.Replace(temp, String.Empty);
            db_path = Path.Combine(db_path, NextLevelPath.DB);
            db_path = Path.Combine(db_path, "NextLevelDB.mdf");
            String connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|;Integrated Security=True".Replace("|DataDirectory|", db_path);
            connection = new(connectionString);
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                String msg =
                    DateTime.Now + ": " +
                    this.GetType().Name +
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name +
                    " " + ex.Message;
                App.Logger.Log(msg, "SEVERE");
                throw new Exception("Context creation failed. See server logs for details");
            }
            Accounts = new(connection, this);
            Users = new(connection, this);
            Categories = new(connection, this);
            Products = new(connection, this);
            Feedbacks = new(connection, this);
        }
    }
}
