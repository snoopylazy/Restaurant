using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant
{
    internal class categoriesList
    {
        string connection = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\USER\Documents\restaurantsystem.mdf;Integrated Security=True;Connect Timeout=30";

        public int ID { get; set; }
        public string category { get; set; }
        public string Status { get; set; }
        public string DateInsert  { get; set; }

        public List<categoriesList> categoriesListData()
        {
            List<categoriesList> listData = new List<categoriesList>();

            using (SqlConnection connect = new SqlConnection(connection))
            {
                connect.Open();

                string selectData = "SELECT * FROM categories";

                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        categoriesList data = new categoriesList();

                        data.ID = (int)reader["id"];
                        data.category = reader["category"].ToString();
                        data.Status = reader["status"].ToString();
                        data.DateInsert = ((DateTime)reader["date_insert"]).ToString("dd-MM-yyyy");

                        listData.Add(data);
                    }
                }
            }
            return listData;
        }
    }
}
