using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Restaurant
{
    public partial class cardProducts : UserControl
    {
        public cardProducts()
        {
            InitializeComponent();
        }

            public int id { get; set; }
            public string productId { get; set; }
        public string productName
        {
            get
            {
                return productname.Text;
            }
            set
            {
                productname.Text = value;
            }
        }
        public string productStock
        {
            get
            {
                return stock.Text;
            }
            set
            {
                stock.Text = value;
            }
        }
        public string productPrice
        {
            get
            {
                return price.Text;
            }
            set
            {
                price.Text = value;
            }
        }
        public Image productImage
        {
            get
            {
                return pictureBox1.Image;
            }
            set
            {
                pictureBox1.Image = value;
            }
        }
        public string productQuantity
        {
            get
            {
                return quantity.Text;
            }
            set
            {
                quantity.Text = value;
            }
        }
        public string category { get; set; }

        public event EventHandler selectCard = null;
        private void btnAdd_Click(object sender, EventArgs e)
        {
            selectCard?.Invoke(this, EventArgs.Empty);
        }
    }
}
