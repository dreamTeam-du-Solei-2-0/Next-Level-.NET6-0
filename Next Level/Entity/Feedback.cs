using Next_Level.Classes;
using Next_Level.ContextData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next_Level.Entity
{
    public class Feedback
    {
        public Guid FeedbackId { get; set; }
        public Guid AccountId { get; set; }
        public Guid ProductId { get; set; }
        public DateTime Date { get; set; }
        public String Text { get; set; }
        public DateTime? DeleteDt { get; set; }

        public Feedback()
        {
            FeedbackId = Guid.NewGuid();
            AccountId = Guid.Empty;
            ProductId = Guid.Empty;
            Date = DateTime.Now;
            Text = null;
            DeleteDt = null;
        }
        public Feedback(SqlDataReader reader)
        {
            FeedbackId = reader.GetGuid("FeedbackId");
            AccountId = reader.GetGuid("AccountId");
            ProductId = reader.GetGuid("ProductId");
            Date = reader.GetDateTime("Date");
            Text = reader.GetString("Text");
            DeleteDt = reader.GetValue("DeleteDt") == DBNull.Value
                ? null
                : reader.GetDateTime("DeleteDt");
        }

        internal DataContext? dataContext;

        public Account? Account
        {
            get => dataContext?
                .Accounts
                .GetAccounts()
                .Find(a => a.AccountId == this.AccountId);
        }
    }
}
