using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Next_Level.Entity
{
    public class Account
    {
        public Guid AccountId { get; set; }
        public String Login { get; set; }
        public String Password { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime? DeleteDt { get; set; }

        public Account()
        {
            AccountId = Guid.NewGuid();
            Login = null;
            Password = null;
            IsAdmin = false;
            DeleteDt = null;
        }
        public Account(SqlDataReader reader)
        {
            AccountId = reader.GetGuid("AccountId");
            Login = reader.GetString("Login");
            Password = reader.GetString("Password");
            IsAdmin = reader.GetBoolean("IsAdmin");
            DeleteDt = reader.GetValue("DeleteDt") == DBNull.Value
                ? null
                : reader.GetDateTime("DeleteDt");
        }
    }
}
