using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Restaurant
{
    class customerList
    {
        string connection = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\USER\Documents\restaurantsystem.mdf;Integrated Security=True;Connect Timeout=30";
        public int id { get; set; }
        public string customerId { get; set; }
        public string productsIds { get; set; }
        public string quantities { get; set; }
        public string prices { get; set; }
        public string totalPrices { get; set; }
        public string dateOrder { get; set; }
        public string staff { get; set; }

       public List<customerList> customerListData()
        {
            List<customerList> listData = new List<customerList>();

            using (SqlConnection connect = new SqlConnection(connection))
            {
                connect.Open();

                string selectData = "SELECT * FROM orders";

                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read()) 
                    {
                        customerList cData = new customerList();

                        cData.id = (int)reader["id"];
                        cData.customerId = reader["customerId"].ToString();
                        cData.productsIds = reader["productids"].ToString();
                        cData.quantities = reader["quantities"].ToString();
                        cData.prices = reader["prices"].ToString();
                        cData.totalPrices = reader["total"].ToString();
                        cData.dateOrder = ((DateTime)reader["date_order"]).ToString("dd-MM-yyyy");
                        cData.staff = reader["staff"].ToString();

                        listData.Add(cData);
                    }
                }
            }
            return listData;
        }

        public List<customerList> todaySalescustomerListData()
        {
            List<customerList> listData = new List<customerList>();

            using (SqlConnection connect = new SqlConnection(connection))
            {
                connect.Open();

                string selectData = "SELECT * FROM orders WHERE date_order = @date";

                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    DateTime today = DateTime.Now;
                    String getToday = today.ToString("yyyy-MM-dd");

                    cmd.Parameters.AddWithValue("@date", getToday);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        customerList cData = new customerList();

                        cData.id = (int)reader["id"];
                        cData.customerId = reader["customerId"].ToString();
                        cData.productsIds = reader["productids"].ToString();
                        cData.quantities = reader["quantities"].ToString();
                        cData.prices = reader["prices"].ToString();
                        cData.totalPrices = reader["total"].ToString();
                        cData.dateOrder = ((DateTime)reader["date_order"]).ToString("dd-MM-yyyy");
                        cData.staff = reader["staff"].ToString();

                        listData.Add(cData);
                    }
                }
            }
            return listData;
        }
    }
}
