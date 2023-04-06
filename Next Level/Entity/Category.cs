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
    public class Category
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public DateTime? DeleteDt { get; set; }

        public Category() 
        {
            CategoryId = Guid.NewGuid();
            Name = null;
            DeleteDt = null;
        }
        public Category(SqlDataReader reader)
        {
            CategoryId = reader.GetGuid("CategoryId");
            Name = reader.GetString("Name");
            DeleteDt = reader.GetValue("DeleteDt") == DBNull.Value
              ? null
              : reader.GetDateTime("DeleteDt");
        }

        internal DataContext dataContext;

        public List<Product> ?Products
        {
            get => dataContext?.Products
                .GetProducts()
                .Where(p => p.CategoryId == this.CategoryId&&p.DeleteDt is null)
                .ToList();
        }
    }
}
