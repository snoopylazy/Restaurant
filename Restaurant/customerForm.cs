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
                string query = "SELECT * FROM orders WHERE customerId LIKE @searchText OR id LIKE @searchText OR staff LIKE @searchText";

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    cmd.Parameters.AddWithValue("@searchText", "%" + searchText + "%"); // Supports partial matches
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
                        cData.staff = reader["staff"].ToString(); // Fetch staff name

                        filteredList.Add(cData);
                    }
                }
            }
            // Update DataGridView with filtered results
            dataGridView1.DataSource = filteredList;
        }

        private void dateTimePickerCustomer_ValueChanged(object sender, EventArgs e)
        {

            DateTime selectedDate = dateTimePickerCustomer.Value;
            string selectedDateString = selectedDate.ToString("yyyy-MM-dd");

            // Modify the query based on the selected date (Day, Month, or Year)
            List<customerList> filteredList = new List<customerList>();

            using (SqlConnection connect = new SqlConnection(connection))
            {
                connect.Open();

                // Check whether the user has selected a specific day, month, or year
                string query = string.Empty;

                if (dateTimePickerCustomer.CustomFormat == "yyyy") // Year selected
                {
                    query = "SELECT * FROM orders WHERE YEAR(date_order) = @year";
                }
                else if (dateTimePickerCustomer.CustomFormat == "MM/yyyy") // Month selected
                {
                    query = "SELECT * FROM orders WHERE MONTH(date_order) = @month AND YEAR(date_order) = @year";
                }
                else // Day selected
                {
                    query = "SELECT * FROM orders WHERE date_order = @date";
                }

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    cmd.Parameters.AddWithValue("@date", selectedDateString);
                    cmd.Parameters.AddWithValue("@year", selectedDate.Year);
                    cmd.Parameters.AddWithValue("@month", selectedDate.Month);

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
                        cData.staff = reader["staff"].ToString(); // Fetch staff name

                        filteredList.Add(cData);
                    }
                }
            }

            // Update the DataGridView with filtered results
            dataGridView1.DataSource = filteredList;
        }

        private void pictureBoxReload_Click(object sender, EventArgs e)
        {
            // Reload or refresh the UserControl data
            displayCustomers(); // This method reloads the customer data

            // Optionally, you can reset other UI components as needed (e.g., clear search text, reset filters)
            textSearch.Clear();
            dateTimePickerCustomer.ResetText(); // Reset the DateTimePicker
        }
    }
}
