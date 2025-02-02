using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Restaurant
{
    public partial class customerForm : UserControl
    {
        string connection = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\USER\Documents\restaurantsystem.mdf;Integrated Security=True;Connect Timeout=30";
        public customerForm()
        {
            InitializeComponent();
            displayCustomers();
        }

        public void displayCustomers()
        {
            customerList cData = new customerList();
            List<customerList> listData = cData.customerListData();
            dataGridView1.DataSource = listData;
        }
        private void textSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = textSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                displayCustomers(); // Reload all data when search is empty
            }
            else
            {
                searchCustomers(searchText);
            }
        }

        private void searchCustomers(string searchText)
        {
            List<customerList> filteredList = new List<customerList>();

            using (SqlConnection connect = new SqlConnection(connection))
            {
                connect.Open();
                string query = "SELECT * FROM orders WHERE customerId LIKE @searchText OR id LIKE @searchText";

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    cmd.Parameters.AddWithValue("@searchText", "%" + searchText + "%");
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

                        filteredList.Add(cData);
                    }
                }
            }
            dataGridView1.DataSource = filteredList;
        }
    }
}
