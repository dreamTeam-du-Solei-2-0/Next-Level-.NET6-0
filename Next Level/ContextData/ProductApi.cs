using Next_Level.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next_Level.ContextData
{
    internal class ProductApi
    {
        private SqlConnection connection;
        private List<Product> productList;
        private readonly DataContext context;
        public ProductApi(SqlConnection _connection, DataContext context)
        {
            this.context = context;
            this.connection = _connection;
            productList = null;
        }

        public bool Delete(Entity.Product product)
        {
            try
            {
                using SqlCommand cmd = new()
                {
                    Connection = connection,
                    CommandText = @"UPDATE Products
                                  SET DeleteDt = CURRENT_TIMESTAMP
                                  WHERE ProductId = @id; "
                };
                cmd.Parameters.AddWithValue("@id", product.ProductId);
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

        public List<Product> GetProducts()
        {
            if (productList != null)
                return productList;
            productList = new List<Product>();
            try
            {
                using SqlCommand cmd = new SqlCommand()
                {
                    Connection = connection,
                    CommandText = @"SELECT * FROM Products WHERE DeleteDt IS NULL"
                };
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                    productList.Add(new(reader) { dataContext = this.context });
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
            return productList;
        }

        public bool Add(Entity.Product product)
        {
            try
            {
                using SqlCommand cmd = new()
                {
                    Connection = connection,
                    CommandText = @"INSERT INTO Products(ProductId, CategoryId, Photo, Name, Description, Price, Count)
                                                        VALUES(@productId, @categoryId, @photo,@name, @description, @price, @count);"
                };
                cmd.Parameters.AddWithValue("@productId", product.ProductId);
                cmd.Parameters.AddWithValue("@categoryId", product.CategoryId);
                SqlParameter parameter = new SqlParameter("@photo", SqlDbType.VarBinary);
                parameter.Value = product.Photo;
                cmd.Parameters.Add(parameter);
                cmd.Parameters.AddWithValue("@name", product.Name);
                cmd.Parameters.AddWithValue("@description", product.Description);
                cmd.Parameters.AddWithValue("@price", product.Price);
                cmd.Parameters.AddWithValue("@count", product.Count);
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
