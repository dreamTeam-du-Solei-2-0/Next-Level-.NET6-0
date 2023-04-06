using Next_Level.ContextData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace Next_Level.Entity
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public Guid CategoryId { get; set; }
        public byte[] Photo { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public double Price{ get; set; }
        public int Count { get; set; }
        public DateTime? DeleteDt { get; set; }

        public Product()
        {
            ProductId = Guid.NewGuid();
            CategoryId = Guid.Empty;
            Photo = null;
            Name = null;
            Description = null;
            Price = 0;
            Count = 0;
            DeleteDt = null;
        }

        public Product(SqlDataReader reader)
        {
            ProductId = reader.GetGuid("ProductId");
            CategoryId = reader.GetGuid("CategoryId");
            long photoDataLength = reader.GetBytes(reader.GetOrdinal("Photo"), 0, null, 0, 0);
            if (photoDataLength > 0)
            {
                Photo = new byte[photoDataLength];
                reader.GetBytes(reader.GetOrdinal("Photo"), 0, Photo, 0, (int)photoDataLength);
            }
            Name = reader.GetString("Name");
            Description = reader.GetString("Description");
            Price = reader.GetDouble("Price");
            Count = reader.GetInt32("Count");
            DeleteDt = reader.GetValue("DeleteDt") == DBNull.Value
              ? null
              : reader.GetDateTime("DeleteDt");
        }
        internal DataContext? dataContext;

        public List<Feedback>? Feedbacks
        {
            get => dataContext?
                .Feedbacks
                .GetFeedbacks()
                .Where(f => f.ProductId == this.ProductId)
                .ToList();
        }
    }
}
