using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using System.Reflection;

namespace Restaurant
{
    public partial class dashboardForm : UserControl
    {
        string connection = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\USER\Documents\restaurantsystem.mdf;Integrated Security=True;Connect Timeout=30";

        public dashboardForm()
        {
            InitializeComponent();
            displayTotalUsers();
            displayTotalProducts();
            displayTotalToday();
            displayTotalMoney();
            displayTodaySales();
            displayWeeklySales(); // Add the method to display weekly sales chart

            ReloadForm(); // Call this to display data when the form is loaded
        }

        public void ReloadForm()
        {
            displayTotalUsers();
            displayTotalProducts();
            displayTotalToday();
            displayTotalMoney();
            displayTodaySales();
            displayWeeklySales(); // Reload weekly sales chart
        }
        public void displayWeeklySales()
        {
            // Get the sales data for the last 7 days
            List<decimal> weeklySales = GetWeeklySales();

            // Prepare the chart
            chartWeekSales.Series.Clear();
            Series series = new Series("Weekly Sales")
            {
                ChartType = SeriesChartType.Pie // Use a Pie Chart
            };

            // Populate the chart with sales data for each day
            for (int i = 0; i < weeklySales.Count; i++)
            {
                series.Points.AddXY(GetDayName(i), weeklySales[i]);
            }

            // Add the series to the chart
            chartWeekSales.Series.Add(series);

            // Chart settings
            chartWeekSales.ChartAreas[0].Area3DStyle.Enable3D = true; // Enable 3D effect
            chartWeekSales.ChartAreas[0].AxisX.IsMarginVisible = false; // Hide margin for better look

            // Show percentages on pie slices
            series.IsValueShownAsLabel = true;
            series.LabelFormat = "0.00"; // Format the label to show two decimal places
            series["PieLabelStyle"] = "Outside"; // Place labels outside the pie slices
            series["Exploded"] = "True"; // Add explosion effect to highlight slices

            // Customize the color palette
            chartWeekSales.Palette = ChartColorPalette.BrightPastel;
            chartWeekSales.Legends[0].Docking = Docking.Bottom; // Position legend at the bottom
        }
        private List<decimal> GetWeeklySales()
        {
            List<decimal> sales = new List<decimal>();
            using (SqlConnection connect = new SqlConnection(connection))
            {
                connect.Open();

                for (int i = 6; i >= 0; i--) // Start from 6 days ago to today
                {
                    string selectData = "SELECT SUM(CAST(total AS DECIMAL(10,2))) FROM orders WHERE CAST(date_order AS DATE) = @date";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        DateTime targetDate = DateTime.Today.AddDays(-i); // Oldest first
                        cmd.Parameters.Add("@date", SqlDbType.Date).Value = targetDate; // Use Date type

                        object result = cmd.ExecuteScalar();
                        sales.Add(result != DBNull.Value ? Convert.ToDecimal(result) : 0);
                    }
                }
            }
            return sales; // No need to reverse, already ordered correctly
        }
        private string GetDayName(int index)
        {
            DateTime targetDate = DateTime.Today.AddDays(-6 + index); // Align with loop order
            return targetDate.ToString("dddd"); // Returns full day name
        }

        public void displayTodaySales()
        {
            customerList cDate = new customerList();
            List<customerList> listData = cDate.todaySalescustomerListData();
            dataGridView1.DataSource = listData;
        }

        public void displayTotalUsers()
        {
            using (SqlConnection connect = new SqlConnection(connection))
            {
                connect.Open();
                string selectData = "SELECT COUNT(id) FROM users";
                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int count = Convert.ToInt32(reader[0]);
                        totalUsers.Text = count.ToString();
                    }
                }
            }
        }

        public void displayTotalProducts()
        {
            using (SqlConnection connect = new SqlConnection(connection))
            {
                connect.Open();
                string selectData = "SELECT COUNT(id) FROM products";
                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int count = Convert.ToInt32(reader[0]);
                        totalProducts.Text = count.ToString();
                    }
                }
            }
        }

        public void displayTotalToday()
        {
            using (SqlConnection connect = new SqlConnection(connection))
            {
                connect.Open();
                string selectData = "SELECT SUM(CAST(total as DECIMAL(10,2))) FROM orders WHERE date_order = @date";
                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    DateTime today = DateTime.Now;
                    string getToday = today.ToString("yyyy-MM-dd");

                    cmd.Parameters.AddWithValue("@date", getToday);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        if (reader[0] != DBNull.Value)
                        {
                            decimal revenue = Convert.ToDecimal(reader[0]);
                            totalToday.Text = "$" + revenue.ToString("0.00");
                        }
                        else
                        {
                            totalToday.Text = "0.00";
                        }
                    }
                }
            }
        }

        public void displayTotalMoney()
        {
            using (SqlConnection connect = new SqlConnection(connection))
            {
                connect.Open();
                string selectData = "SELECT SUM(CAST(total as DECIMAL(10,2))) FROM orders";
                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        if (reader[0] != DBNull.Value)
                        {
                            decimal revenus = Convert.ToDecimal(reader[0]);
                            totalmoney.Text = "$" + revenus.ToString("0.00");
                        }
                        else
                        {
                            totalmoney.Text = "0.00";
                        }
                    }
                }
            }
        }

        private void pictureBoxReload_Click(object sender, EventArgs e)
        {
            ReloadForm();
        }
    }
}
