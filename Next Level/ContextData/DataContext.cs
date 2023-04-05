using Next_Level.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Next_Level.ContextData
{
    internal class DataContext
    {
        private SqlConnection connection;
        internal UserApi Users { get; set; }
        internal AccountApi Accounts { get; set; }
        public DataContext()
        {
            connection = new(NextLevelPath.ConnectionString);
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
            Users = new(connection, this);
            Accounts = new(connection, this);
        }
    }
}
