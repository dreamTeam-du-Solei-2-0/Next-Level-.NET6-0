using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next_Level.Entity
{
    public class User
    {
        public Guid UserId { get; set; }
        public Guid AccountId { get; set; }
        public String Surname { get;set; }
        public String Name { get;set; }
        public String Secname { get; set; }
        public String Phone { get; set; }
        public String Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? DeleteDt { get; set; }
        

        public User()
        {
            UserId = Guid.NewGuid();
            Surname = null;
            Name = null;
            Secname = null;
            BirthDate = null;
            Phone = null;
            Email = null;
            DeleteDt = null;
        }

        public User(SqlDataReader reader)
        {
            UserId = reader.GetGuid("UserId");
            Surname = reader.GetString("Surname");
            Name = reader.GetString("Name");
            Secname = reader.GetString("Secname");
            Phone = reader.GetString("Phone");
            Email = reader.GetString("Email");
            BirthDate = reader.GetValue("BirthDate") == DBNull.Value
                ? null
                : reader.GetDateTime("BirthDate");
            DeleteDt = reader.GetValue("DeleteDt") == DBNull.Value
                ? null
                : reader.GetDateTime("DeleteDt");
        }
    }
}
