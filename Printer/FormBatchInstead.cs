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
    public partial class FormBatchInstead : Form
    {
        public string productName = "";
        SystemConfig current = Common.SystemConfig;
        public FormBatchInstead()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            List<SystemConfigProduct> plist = current.Products.ToList();
            SystemConfigProduct product = plist.Find(x => x.Name == productName);
            plist.Remove(product);

            List<SystemConfigProductData> tempList = product.Datas.ToList();
            for(int i=0;i<tempList.Count;i++)
            {
                if(tempList[i].Text.IndexOf(this.labelText1.textBox.Text)>=0)
                {
                    tempList[i].Text = tempList[i].Text.Replace(this.labelText1.textBox.Text,this.labelText2.textBox.Text);
                }
            }
            product.Datas = tempList.ToArray();
            plist.Add(product);
            current.Products = plist.ToArray();
            Common.SaveConfigToFile(current);
            Common.GetSystemConfigFromXmlFile();
            this.Close();
        }
    }
}
