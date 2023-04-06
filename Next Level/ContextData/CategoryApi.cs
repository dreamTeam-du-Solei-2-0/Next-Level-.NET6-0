using Next_Level.Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next_Level.ContextData
{
    internal class CategoryApi
    {
        private SqlConnection connection;
        private List<Category> categoryList;
        private readonly DataContext context;
        public CategoryApi(SqlConnection _connection, DataContext context)
        {
            this.context = context;
            this.connection = _connection;
            categoryList = null;
        }

        public bool Delete(Entity.Category category)
        {
            try
            {
                using SqlCommand cmd = new()
                {
                    Connection = connection,
                    CommandText = @"UPDATE Categories
                                  SET DeleteDt = CURRENT_TIMESTAMP
                                  WHERE CategoryId = @id; "
                };
                cmd.Parameters.AddWithValue("@id", category.CategoryId);
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

        public Category GetCategory(String categoryName)
        {
            Category category = null;
            try
            {
                using SqlCommand cmd = new()
                {
                    Connection = connection,
                    CommandText = @"
                    SELECT*
                    FROM Categories
                    WHERE Name = @category"
                };
                cmd.Parameters.AddWithValue("@category", categoryName);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                    category = new(reader) { dataContext = this.context };
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
                return category;
            }
            return category;
        }

        public List<Category> GetCategories()
        {
            if (categoryList != null)
                return categoryList;
            categoryList = new List<Category>();
            try
            {
                using SqlCommand cmd = new SqlCommand()
                {
                    Connection = connection,
                    CommandText = @"SELECT * FROM Categories WHERE DeleteDt IS NULL"
                };
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                    categoryList.Add(new(reader) { dataContext = this.context });
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
            return categoryList;
        }

        public bool Add(Entity.Category category)
        {
            try
            {
                using SqlCommand cmd = new()
                {
                    Connection = connection,
                    CommandText = @"INSERT INTO Categories(CategoryId, Name)
                                                        VALUES(@categoryId,@name);"
                };
                cmd.Parameters.AddWithValue("@categoryId", category.CategoryId);
                cmd.Parameters.AddWithValue("@name", category.Name);
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
