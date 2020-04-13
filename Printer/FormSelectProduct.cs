using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Printer
{
    public partial class FormSelectProduct : Form
    {
        public FormSelectProduct()
        {
            InitializeComponent();
        }

        private void FormSelectProduct_Load(object sender, EventArgs e)
        {
            foreach (var product in Common.SystemConfig.Products)
            {
                labelComboxProducts.comboBox.Items.Add(product.Name);
            }

            labelComboxProducts.comboBox.SelectedIndex = 0;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {

        }
    }
}
