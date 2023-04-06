using Next_Level.Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next_Level.ContextData
{
    internal class FeedbackApi
    {
        private SqlConnection connection;
        private readonly DataContext dataContext;
        private List<Feedback> feedbacks;
        public FeedbackApi(SqlConnection connection, DataContext dataContext)
        {
            this.connection = connection;
            this.dataContext = dataContext;
            this.feedbacks = null;
        }
        public bool Delete(Entity.Feedback feedback)
        {
            try
            {
                using SqlCommand cmd = new()
                {
                    Connection = connection,
                    CommandText = @"UPDATE Feedbacks
                                  SET DeleteDt = CURRENT_TIMESTAMP
                                  WHERE FeedbackId = @id; "
                };
                cmd.Parameters.AddWithValue("@id", feedback.FeedbackId);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                feedbacks = null;
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
        public bool Add(Entity.Feedback feedback)
        {
            try
            {
                using SqlCommand cmd = new()
                {
                    Connection = connection,
                    CommandText = @"INSERT INTO Feedbacks(FeedbackId, AccountId, ProductId, Text, Date)
                                                        VALUES(@feedbackId, @accountId, @productId, @text, @date);"
                };
                cmd.Parameters.AddWithValue("@feedbackId", feedback.FeedbackId);
                cmd.Parameters.AddWithValue("@accountId", feedback.AccountId);
                cmd.Parameters.AddWithValue("@productId", feedback.ProductId);
                cmd.Parameters.AddWithValue("@text", feedback.Text);
                cmd.Parameters.AddWithValue("@date", feedback.Date);
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
        public List<Feedback> GetFeedbacks()
        {
            if (feedbacks != null)
                return feedbacks;
            feedbacks = new();
            try
            {
                using SqlCommand cmd = new()
                {
                    Connection = connection,
                    CommandText = @"SELECT * FROM Feedbacks WHERE DeleteDt IS NULL"
                };
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                    feedbacks.Add(new(reader) { dataContext = this.dataContext });
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
            return feedbacks;
        }
    }
}
