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
    public partial class categoryForm : UserControl
    {
        string connection = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\USER\Documents\restaurantsystem.mdf;Integrated Security=True;Connect Timeout=30";
        public categoryForm()
        {
            InitializeComponent();
            displayCategories();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtCategory.Text == "" || comboStatus.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill all the fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }else
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();

                    var selectCategory = "SELECT * FROM categories WHERE category = @cat";

                    using (SqlCommand checkCat = new SqlCommand(selectCategory, connect))
                    {
                        checkCat.Parameters.AddWithValue("@cat", txtCategory.Text.Trim());
                        SqlDataAdapter adapter = new SqlDataAdapter(checkCat);
                        DataTable table = new DataTable();

                        adapter.Fill(table);

                        if (table.Rows.Count > 0)
                        {
                            MessageBox.Show(txtCategory.Text.Trim() + " is already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            var insertData = "INSERT INTO categories (category, status, date_insert) VALUES (@cat, @status, @date)";

                            using (SqlCommand cmd = new SqlCommand(insertData, connect))
                            {
                                cmd.Parameters.AddWithValue("@cat", txtCategory.Text.Trim());
                                cmd.Parameters.AddWithValue("@status", comboStatus.SelectedItem.ToString());

                                DateTime today = DateTime.Now;
                                cmd.Parameters.AddWithValue("@date", today);

                                cmd.ExecuteNonQuery();

                                MessageBox.Show("Category added successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                clearFields();
                            }
                        }
                    }
                }
            }
            displayCategories();
        }

        void clearFields()
        {
            txtCategory.Clear();
            comboStatus.SelectedIndex = -1;
            getID = 0;
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            clearFields();
        }
        public void displayCategories()
        {
            categoriesList data = new categoriesList();
            List<categoriesList> listData = data.categoriesListData();

            dataGridViewCategory.DataSource = listData;
        }

        private int getID = 0;

        private void dataGridViewCategory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow row = dataGridViewCategory.Rows[e.RowIndex];

                getID = (int)row.Cells[0].Value;
                txtCategory.Text = row.Cells[1].Value.ToString();
                comboStatus.Text = row.Cells[2].Value.ToString();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (getID == 0)
            {
                MessageBox.Show("Select item first", "Error message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (MessageBox.Show($"Are you sure? You want to update this ID : {getID}?", "Confirm Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    using (SqlConnection connect = new SqlConnection(connection))
                    {
                        connect.Open();

                        string updateData = "UPDATE categories SET category = @cat, status = @status WHERE id = @id";

                        using (SqlCommand cmd = new SqlCommand(updateData, connect))
                        {
                            cmd.Parameters.AddWithValue("@id", getID);
                            cmd.Parameters.AddWithValue("@cat", txtCategory.Text.Trim());
                            cmd.Parameters.AddWithValue("@status", comboStatus.SelectedItem.ToString());

                            cmd.ExecuteNonQuery();
                            clearFields();

                            MessageBox.Show("Update Succefully", "Information message", MessageBoxButtons.OK, MessageBoxIcon.Information );
                        }
                        connect.Close();
                    }
                }
            }
            displayCategories();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (getID == 0)
            {
                MessageBox.Show("Select item first", "Error message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (MessageBox.Show($"Are you sure? You want to delete this ID : {getID} ?", "Confirm Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    using (SqlConnection connect = new SqlConnection(connection))
                    {
                        connect.Open();

                        string updateData = "DELETE FROM categories WHERE id = @id";

                        using (SqlCommand cmd = new SqlCommand(updateData, connect))
                        {
                            cmd.Parameters.AddWithValue("@id", getID);

                            cmd.ExecuteNonQuery();
                            clearFields();

                            MessageBox.Show("Delete Succefully", "Information message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        connect.Close();
                    }
                }
            }
            displayCategories();
        }
        private void textSearch_TextChanged(object sender, EventArgs e)
        {
            string searchTerm = textSearch.Text.Trim();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();

                    // SQL query to filter by name, id, or status
                    string searchQuery = "SELECT * FROM categories WHERE category LIKE @search OR id LIKE @search OR status LIKE @search";

                    using (SqlCommand cmd = new SqlCommand(searchQuery, connect))
                    {
                        cmd.Parameters.AddWithValue("@search", "%" + searchTerm + "%");

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        // Bind the filtered data to the DataGridView
                        dataGridViewCategory.DataSource = table;
                    }

                    connect.Close();
                }
            }
            else
            {
                // If no search term is entered, display all categories
                displayCategories();
            }
        }
    }
}
