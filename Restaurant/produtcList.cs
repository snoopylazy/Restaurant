using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Restaurant
{
    internal class produtcList
    {
        string connection = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\USER\Documents\restaurantsystem.mdf;Integrated Security=True;Connect Timeout=30";
        
        public int ID { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public string stock { get; set; }
        public string price { get; set; }
        public string status { get; set; }
        public string image { get; set; }
        public string DateInsert { get; set; }
        public string DateUpdate { get; set; }

        public List<produtcList> productListData()
        {
            List<produtcList> listData = new List<produtcList>();

            using (SqlConnection connect = new SqlConnection(connection))
            {
                connect.Open();

                string selectData = "SELECT * FROM products";

                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        produtcList ProList = new produtcList();

                        ProList.ID = (int)reader["id"];
                        ProList.ProductID = reader["productid"].ToString();
                        ProList.ProductName = reader["productname"].ToString();
                        ProList.Category = reader["category"].ToString();
                        ProList.stock = reader["stock"].ToString();
                        ProList.price = reader["price"].ToString();
                        ProList.status = reader["status"].ToString();
                        ProList.image = reader["image"].ToString();
                        ProList.DateInsert = ((DateTime)reader["date_insert"]).ToString("dd-MM-yyyy");
                        ProList.DateUpdate = reader["date_update"] == DBNull.Value ? null : ((DateTime)reader["date_update"]).ToString("dd-MM-yyyy");

                        listData.Add(ProList);
                    }
                }
            }
            return listData;
        }
    }
}
