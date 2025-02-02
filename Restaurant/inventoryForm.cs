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

namespace Restaurant
{
    public partial class inventoryForm : UserControl
    {
        string connection = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\USER\Documents\restaurantsystem.mdf;Integrated Security=True;Connect Timeout=30";
        public inventoryForm()
        {
            InitializeComponent();
            displayCategories();
            displayProducts();
        }
        private void displayProducts()
        {
            produtcList proList = new produtcList();
            List<produtcList> listData = proList.productListData();
            dataGridViewProducts.DataSource = listData;
        }
        public void displayCategories()
        {
            comboCategory.Items.Clear();

            using (SqlConnection connect = new SqlConnection(connection))
            {
                connect.Open();

                string selectCat = "SELECT * FROM categories WHERE status = 'Active'";

                using (SqlCommand cmd = new SqlCommand(selectCat, connect))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        comboCategory.Items.Add(reader["category"]);
                    }
                }
            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtproID.Text == "" || txtproName.Text == "" || comboCategory.SelectedIndex == -1 || txtstock.Text == "" || txtprice.Text == "" || comboStatus.Text == "" || pictureBox1.Image == null)
            {

                MessageBox.Show("Please fill all fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();

                    string checkProductID = "SELECT * FROM products WHERE productid = @proid";
                    using (SqlCommand checkProID = new SqlCommand(checkProductID, connect))
                    {
                        checkProID.Parameters.AddWithValue("@proid", txtproID.Text.Trim());
                        SqlDataAdapter adapter = new SqlDataAdapter(checkProID);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count != 0)
                        {
                            MessageBox.Show($"{txtproID.Text.Trim()} is already exists.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                            string insertData = "INSERT INTO products (productid, productname, category, stock, price, status, image, date_insert)" + " VALUES(@productid, @productname, @category, @stock, @price, @status, @image, @date)";
                            string relativePath = Path.Combine("products_directory", txtproID.Text.Trim() + ".jpg");
                            string path = Path.Combine(baseDirectory, relativePath);
                            string directoryPath = Path.GetDirectoryName(path);

                            if (!Directory.Exists(directoryPath))
                            {
                                Directory.CreateDirectory(directoryPath);
                            }
                            File.Copy(pictureBox1.ImageLocation, path, true);

                            using (SqlCommand cmd = new SqlCommand(insertData, connect))
                            {
                                cmd.Parameters.AddWithValue("@productid", txtproID.Text.Trim());
                                cmd.Parameters.AddWithValue("@productname", txtproName.Text.Trim());
                                cmd.Parameters.AddWithValue("@category", comboCategory.Text.ToString());
                                cmd.Parameters.AddWithValue("@stock", txtstock.Text.Trim());
                                cmd.Parameters.AddWithValue("@price", txtprice.Text.Trim());
                                cmd.Parameters.AddWithValue("@status", comboStatus.SelectedItem.ToString());
                                cmd.Parameters.AddWithValue("@image", path);

                                DateTime today = DateTime.Now;
                                cmd.Parameters.AddWithValue("@date", today);

                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Product Added Successfully", "Success Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                clearField();
                            }
                        }
                    }
                }
            }
            displayProducts();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image Files (*.jpg, *.png| *.jpg; *.png)";

                string imagePath = "";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    imagePath = dialog.FileName;
                    pictureBox1.ImageLocation = imagePath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {e}", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void clearField()
        {
            txtproID.Clear();
            txtproName.Clear();
            comboCategory.SelectedIndex = -1;
            txtstock.Clear();
            txtprice.Clear();
            comboStatus.SelectedIndex = -1;
            pictureBox1.Image = null;
            getID = 0;
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            clearField();
        }
        private int getID = 0;

        private void dataGridViewProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow row = dataGridViewProducts.Rows[e.RowIndex];

                getID = (int)row.Cells[0].Value;
                txtproID.Text = row.Cells[1].Value.ToString();
                txtproName.Text = row.Cells[2].Value.ToString();
                comboCategory.Text = row.Cells[3].Value.ToString();
                txtstock.Text = row.Cells[4].Value.ToString();
                txtprice.Text = row.Cells[5].Value.ToString();
                comboStatus.Text = row.Cells[6].Value.ToString();
                string imagePath = row.Cells[7].Value.ToString();

                try
                {
                    if (imagePath != null)
                    {
                        pictureBox1.Image = Image.FromFile(imagePath);
                    }
                    else
                    {
                        pictureBox1.Image = null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex}", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"Are you sure you want to update this {getID}?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (getID == 0)
                {
                    MessageBox.Show("Please select a product to update", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    using (SqlConnection connect = new SqlConnection(connection))
                    {
                        connect.Open();

                        string checkProductID = "SELECT * FROM products WHERE productid = @productid";

                        using (SqlCommand checkProd = new SqlCommand(checkProductID, connect))
                        {
                            checkProd.Parameters.AddWithValue("@productid", txtproID.Text.Trim());

                            SqlDataAdapter adapter = new SqlDataAdapter(checkProd);
                            DataTable table = new DataTable();
                            adapter.Fill(table);

                            if (table.Rows.Count >= 2)
                            {
                                MessageBox.Show(txtproID.Text.Trim() + " is already exists.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                string updateData = "UPDATE products SET productid = @productid, productname = @productname, category = @category, stock = @stock, price = @price, status = @status, date_update = @date WHERE id = @id";

                                using (SqlCommand cmd = new SqlCommand(updateData, connect))
                                {
                                    cmd.Parameters.AddWithValue("@productid", txtproID.Text.Trim());
                                    cmd.Parameters.AddWithValue("@productname", txtproName.Text.Trim());
                                    cmd.Parameters.AddWithValue("@category", comboCategory.SelectedItem.ToString());
                                    cmd.Parameters.AddWithValue("@stock", txtstock.Text.Trim());
                                    cmd.Parameters.AddWithValue("@price", txtprice.Text.Trim());
                                    cmd.Parameters.AddWithValue("@status", comboStatus.SelectedItem.ToString());

                                    DateTime today = DateTime.Now;
                                    cmd.Parameters.AddWithValue("@date", today);
                                    cmd.Parameters.AddWithValue("@id", getID);

                                    cmd.ExecuteNonQuery();
                                    MessageBox.Show("Product Updated Successfully", "Success Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    clearField();
                                }
                            }
                        }
                    }
                }
            }
            displayProducts();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"Are you sure you want to Delete this {getID}?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (getID == 0)
                {
                    MessageBox.Show("Please select a product to Delete", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    using (SqlConnection connect = new SqlConnection(connection))
                    {
                        connect.Open();

                        string updateData = "DELETE FROM products WHERE id = @id";

                        using (SqlCommand cmd = new SqlCommand(updateData, connect))
                        {
                            cmd.Parameters.AddWithValue("@id", getID);

                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Product Delete Successfully", "Success Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            clearField();
                        }
                    }
                }
            }
            displayProducts();
        }
        private void pictureBoxReload_Click(object sender, EventArgs e)
        {
            loadProduct();
        }
        private void loadProduct()
        {
            // Fetch the category and display them in the DataGridView
            displayCategories();
        }
        private void textSearch_TextChanged(object sender, EventArgs e)
        {
            using (SqlConnection connect = new SqlConnection(connection))
            {
                connect.Open(); // Ensure connection is opened

                string query = "SELECT * FROM products WHERE productid LIKE @search OR productname LIKE @search";
                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    cmd.Parameters.AddWithValue("@search", "%" + textSearch.Text.Trim() + "%");

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        dataGridViewProducts.DataSource = table;
                    }
                }
            }
        }
    }
}
