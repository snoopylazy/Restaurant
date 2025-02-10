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
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;


//using static System.Net.Mime.MediaTypeNames;

namespace Restaurant
{
    public partial class opForm : UserControl
    {
        string connection = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\USER\Documents\restaurantsystem.mdf;Integrated Security=True;Connect Timeout=30";
        public opForm()
        {
            InitializeComponent();
            loadProducts();
        }

        public void carditems(int id, string productname, string stock, string price, Image image, string productid, string category, string quantity)
        {
            var card = new cardProducts()
            {
                id = id,
                productName = productname,
                productStock = stock, // Stock is stored as a string
                productPrice = price,
                productImage = image,
                productId = productid,
                category = category,
                productQuantity = quantity,
            };
            flowLayoutPanel2.Controls.Add(card);

            card.selectCard += (q, w) =>
            {
                var selectedCard = (cardProducts)q;
                bool flag = false;

                // Validate quantity input
                if (string.IsNullOrWhiteSpace(selectedCard.productQuantity))
                {
                    MessageBox.Show("Please enter quantity", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Stop execution if quantity is empty
                }

                if (!int.TryParse(selectedCard.productQuantity, out int getQuantity))
                {
                    MessageBox.Show("Please enter a valid number for quantity", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Stop execution if quantity is not a number
                }

                if (!int.TryParse(selectedCard.productStock, out int stockAvailable))
                {
                    MessageBox.Show("Stock data is invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Stop execution if stock is invalid
                }

                // Check if requested quantity exceeds available stock
                if (getQuantity > stockAvailable)
                {
                    MessageBox.Show("Cannot add more than available stock", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Stop execution if quantity exceeds stock
                }

                decimal getPrice = Convert.ToDecimal(selectedCard.productPrice.Replace("$", ""));

                // Check if the product already exists in the DataGridView
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["id"].Value != null && (int)row.Cells["id"].Value == selectedCard.id)
                    {
                        // Product already exists, update the quantity and price
                        int existingQuantity = Convert.ToInt32(row.Cells["QTY"].Value);
                        int newQuantity = existingQuantity + getQuantity; // Increase quantity

                        // Check stock availability before updating quantity
                        if (newQuantity > stockAvailable)
                        {
                            MessageBox.Show("Cannot add more than available stock", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return; // Stop execution if the total quantity exceeds stock
                        }

                        row.Cells["QTY"].Value = newQuantity;
                        row.Cells["Price"].Value = getPrice * newQuantity; // Update total price
                        flag = true;
                        break;
                    }
                }

                // If product is not found, add it as a new row
                if (!flag)
                {
                    dataGridView1.Rows.Add(selectedCard.id, selectedCard.productName, getQuantity, getPrice * getQuantity);
                }

                updateTotalprice(); // Update total price after changes

            };
        }
        private void updateTotalprice()
        {
            decimal totalprice = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["id"].Value != null)
                {
                    decimal price = Convert.ToDecimal(row.Cells["Price"].Value);
                    totalprice += price;
                }
            }

            total.Text = $"${totalprice:F2}";
        }
        public void loadProducts(string searchTerm = "")
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();

                    //string selectData = "SELECT * FROM products WHERE productname LIKE @searchTerm OR productid LIKE @searchTerm OR stock LIKE @searchTerm";
                    string selectData = "SELECT * FROM products WHERE (productname LIKE @searchTerm OR productid LIKE @searchTerm OR stock LIKE @searchTerm) AND status <> 'unavailable'";


                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        cmd.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable table = new DataTable();

                        adapter.Fill(table);

                        flowLayoutPanel2.Controls.Clear();

                        foreach (DataRow row in table.Rows)
                        {
                            int id = row["id"] != DBNull.Value ? (int)row["id"] : 0;
                            string productname = row["productname"] != DBNull.Value ? row["productname"].ToString() : "N/A";
                            string stock = row["stock"] != DBNull.Value ? row["stock"].ToString() : "0";
                            string price = row["price"] != DBNull.Value ? $"${row["price"].ToString()}.00" : "0.00";
                            string productid = row["productid"] != DBNull.Value ? row["productid"].ToString() : "N/A";
                            string category = row["category"] != DBNull.Value ? row["category"].ToString() : "N/A";

                            Image image = null;
                            if (row["image"] != DBNull.Value)
                            {
                                string imagePath = row["image"].ToString();
                                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                                {
                                    try
                                    {
                                        image = Image.FromFile(imagePath);
                                    }
                                    catch (Exception ex)
                                    {
                                        image = null;
                                    }
                                }
                            }

                            carditems(id, productname, stock, price, image, productid, category, "");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex}", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        bool check = false;
        private void btnOrder_Click(object sender, EventArgs e)
        {
            if (!check)
            {
                MessageBox.Show("Invalid: Insufficient Amount", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to process?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    using (SqlConnection connect = new SqlConnection(connection))
                    {
                        connect.Open();

                        string countData = "SELECT COUNT(*) FROM orders";
                        int count = 1;

                        using (SqlCommand cData = new SqlCommand(countData, connect))
                        {
                            count = Convert.ToInt32(cData.ExecuteScalar()) + 1;
                        }

                        List<string> productIds = new List<string>();
                        List<string> quantities = new List<string>();
                        List<string> prices = new List<string>();

                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells["id"].Value != null && row.Cells["QTY"] != null && row.Cells["price"] != null)
                            {
                                productIds.Add(row.Cells["id"].Value.ToString());
                                quantities.Add(row.Cells["QTY"].Value.ToString());
                                prices.Add(row.Cells["price"].Value.ToString());
                            }
                        }

                        string productIdsStr = string.Join(",", productIds);
                        string quantitiesStr = string.Join(",", quantities);
                        string pricesStr = string.Join(",", prices);

                        decimal totalAmount = Convert.ToDecimal(total.Text.Replace("$", ""));

                        // **Modify query to include 'staff' column**
                        string insertData = "INSERT INTO orders (customerId, productids, quantities, prices, total, date_order, staff) VALUES (@cid, @pid, @qty, @price, @total, @date, @staff)";

                        using (SqlCommand cmd = new SqlCommand(insertData, connect))
                        {
                            cmd.Parameters.AddWithValue("@cid", $"CID-{count}");
                            cmd.Parameters.AddWithValue("@pid", productIdsStr);
                            cmd.Parameters.AddWithValue("@qty", quantitiesStr);
                            cmd.Parameters.AddWithValue("@price", pricesStr);
                            cmd.Parameters.AddWithValue("@total", totalAmount);
                            cmd.Parameters.AddWithValue("@date", DateTime.Now);

                            // **Store the logged-in user's name in the 'staff' column**
                            cmd.Parameters.AddWithValue("@staff", UserSession.LoggedInUser);

                            int rowAffected = cmd.ExecuteNonQuery();

                            if (rowAffected > 0)
                            {
                                for (int q = 0; q < productIds.Count; q++)
                                {
                                    string getStockData = "SELECT stock FROM products WHERE id = @id";
                                    int currentStock = 0;

                                    using (SqlCommand getSD = new SqlCommand(getStockData, connect))
                                    {
                                        getSD.Parameters.AddWithValue("@id", productIds[q]);
                                        object result = getSD.ExecuteScalar();

                                        if (result != null)
                                        {
                                            currentStock = Convert.ToInt32(result);
                                        }
                                    }

                                    int newStock = currentStock - Convert.ToInt32(quantities[q]);

                                    if (newStock < 0)
                                    {
                                        MessageBox.Show($"Insufficient Stock for product ID: {productIds[q]}", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }

                                    string updateData = "UPDATE products SET stock = stock - @qty WHERE id = @id";

                                    using (SqlCommand updateCmd = new SqlCommand(updateData, connect))
                                    {
                                        updateCmd.Parameters.AddWithValue("@qty", quantities[q]);
                                        updateCmd.Parameters.AddWithValue("@id", productIds[q]);

                                        updateCmd.ExecuteNonQuery();
                                    }
                                }
                                MessageBox.Show("Order Successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                loadProducts();
                                check = false;
                            }
                            else
                            {
                                MessageBox.Show("Error: Order Failed", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
        }

        private void change_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    decimal getTotal = Convert.ToDecimal(total.Text.ToString().Replace("$", ""));
                    decimal getChange = Convert.ToDecimal(change.Text);

                    if (getTotal > getChange)
                    {
                        MessageBox.Show("Invalid: Insufficient Amount", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        check = true;
                        amount.Text = $"${(getChange - getTotal):0.00}";
                    }
                    e.SuppressKeyPress = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex}", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private int rowIndex = 0;
        private void btnReceipt_Click(object sender, EventArgs e)
        {
            printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(printDocument1_PrintPage);
            printDocument1.BeginPrint += new System.Drawing.Printing.PrintEventHandler(printDocument1_BeginPrint);

            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
            dataGridView1.Rows.Clear();
            total.Text = "$0.00";
            change.Text = "";
            amount.Text = "$0.00";
        }

        private void printDocument1_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            rowIndex = 0;
        }
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // Define page dimensions and margins
            float pageWidth = e.MarginBounds.Width;
            float pageHeight = e.MarginBounds.Height;
            float y = e.MarginBounds.Top;
            int padding = 5;
            int colWidth = 120;
            int colCount = 4;
            int tableWidth = colWidth * colCount; // Total width of table
            float tableStartX = e.MarginBounds.Left + (pageWidth - tableWidth) / 2; // Center table

            // Define fonts and brushes
            Font font = new Font("Arial", 12);
            Font bold = new Font("Arial", 12, FontStyle.Bold);
            Font headerFont = new Font("Arial", 18, FontStyle.Bold);
            Font labelFont = new Font("Arial", 14, FontStyle.Bold);

            Brush headerBrush = Brushes.DarkBlue;
            Brush totalBrush = Brushes.DarkRed;
            Brush textBrush = Brushes.Black;
            Brush backgroundBrush = Brushes.LightGray;

            StringFormat alignCenter = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            // **Print Receipt Header (Title)**
            string headerText = "RECEIPT";
            float headerX = e.MarginBounds.Left + (pageWidth / 2) - (e.Graphics.MeasureString(headerText, headerFont).Width / 2);
            e.Graphics.DrawString(headerText, headerFont, headerBrush, headerX, y);
            y += headerFont.GetHeight(e.Graphics) + 20;

            // **Draw Table Headers with Background**
            string[] headers = { "PID", "ProdName", "QTY", "Price" };
            float x = tableStartX;

            for (int q = 0; q < colCount; q++)
            {
                RectangleF headerRect = new RectangleF(x + (q * colWidth), y, colWidth, bold.GetHeight(e.Graphics) + padding);
                e.Graphics.FillRectangle(backgroundBrush, headerRect);
                e.Graphics.DrawRectangle(Pens.Black, Rectangle.Round(headerRect));
                e.Graphics.DrawString(headers[q], bold, textBrush, headerRect, alignCenter);
            }
            y += bold.GetHeight(e.Graphics) + padding;

            // **Print Rows**
            int rowIndex = 0;
            while (rowIndex < dataGridView1.Rows.Count)
            {
                DataGridViewRow row = dataGridView1.Rows[rowIndex];
                x = tableStartX;

                for (int q = 0; q < colCount; q++)
                {
                    object cellValue = row.Cells[q].Value;
                    string cell = (cellValue != null) ? cellValue.ToString() : string.Empty;

                    RectangleF cellRect = new RectangleF(x + (q * colWidth), y, colWidth, font.GetHeight(e.Graphics) + padding);
                    e.Graphics.DrawRectangle(Pens.Black, Rectangle.Round(cellRect));
                    e.Graphics.DrawString(cell, font, textBrush, cellRect, alignCenter);
                }
                y += font.GetHeight(e.Graphics) + padding;
                rowIndex++;

                if (y + font.GetHeight(e.Graphics) > e.MarginBounds.Bottom - 150)
                {
                    e.HasMorePages = true;
                    return;
                }
            }

            // Space before totals
            y += 40;

            // **Total, Amount, Change, and Cashier Info**
            float labelX = e.MarginBounds.Left + (pageWidth - 200); // Adjusted to make space for the text
            string totalText = $"Total Price: {total.Text.Trim()}\nAmount: {amount.Text.Trim()}\nChange: {change.Text.Trim()}";
            e.Graphics.DrawString(totalText, labelFont, totalBrush, labelX, y);

            // **Print Cashier Name near Total**
            y += labelFont.GetHeight(e.Graphics) + 50;
            string cashierName = "Cashier: " + UserSession.LoggedInUser;
            e.Graphics.DrawString(cashierName, labelFont, textBrush, labelX, y);

            // **Print Date**
            y += labelFont.GetHeight(e.Graphics) + 50;
            string dateText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // Formatting date
            e.Graphics.DrawString(dateText, labelFont, textBrush, labelX, y);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Check if a row is selected in the DataGridView
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Show confirmation message before deleting
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to delete this item?",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    // Get the selected row
                    DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                    // Remove the selected row from DataGridView
                    dataGridView1.Rows.Remove(selectedRow);

                    // Update total price after deletion
                    updateTotalprice();
                }
            }
            else
            {
                MessageBox.Show("Please select an item to delete", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void pictureBoxReload_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show( "Are you sure you want to reload the products?", "Confirm Reload", MessageBoxButtons.YesNo,MessageBoxIcon.Question);
           
            if (result == DialogResult.Yes)
            {
                loadProducts(); // Reload the products if the user confirms
            }
        }
        private void textSearch_TextChanged(object sender, EventArgs e)
        {
            // Get the search term from the text box
            string searchTerm = textSearch.Text.Trim();

            // Reload the products with the search term
            loadProducts(searchTerm);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to clear the order?", "Confirm Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                dataGridView1.Rows.Clear();
                total.Text = "$0.00";
                change.Text = "";
                amount.Text = "$0.00";
            }
        }

        //private void btnDiscount_Click(object sender, EventArgs e)
        //{

        //}
    }
}