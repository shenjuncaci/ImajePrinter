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
    public partial class FormAddProduct : Form
    {
        public int isEdit=0;
        public string productName = "";
        SystemConfig current = Common.SystemConfig;
        public FormAddProduct()
        {
            InitializeComponent();
        }

        private void FormAddProduct_Load(object sender, EventArgs e)
        {

            if(isEdit==1)
            {
                //var current = Common.SystemConfig;
                List<SystemConfigProduct> plist = current.Products.ToList();
                SystemConfigProduct product = plist.Find(x => x.Name == productName);

                labelText1.textBox.Text = product.Name;
                labelTextBaseCord.textBox.Text = product.BaseCords.BaseCord1;
                labelText2.textBox.Text = product.BaseCords.BaseCord2;
                labelText3.textBox.Text = product.BaseCords.BaseCord3;

                labelText4.textBox.Text = product.BaseCords.BaseCord4;
                labelText5.textBox.Text = product.BaseCords.BaseCord5;
                labelText6.textBox.Text = product.BaseCords.BaseCord6;
                labelText7.textBox.Text = product.BaseCords.BaseCord7;
                labelText8.textBox.Text = product.BaseCords.BaseCord8;

                List<SystemConfigProductData> printDataList = product.Datas.ToList();

                foreach (var d in printDataList)
                {
                    var rowNum = dataGridView1.Rows.Add();
                    dataGridView1.Rows[rowNum].Cells["No"].Value = d.No;
                    dataGridView1.Rows[rowNum].Cells["Type"].Value = d.Type;
                    dataGridView1.Rows[rowNum].Cells["data"].Value = d.Text;
                    dataGridView1.Rows[rowNum].Cells["PosX"].Value = d.PosX;
                    dataGridView1.Rows[rowNum].Cells["PosY"].Value = d.PosY;
                    dataGridView1.Rows[rowNum].Cells["Angle"].Value = d.Angle;
                    dataGridView1.Rows[rowNum].Cells["TemplateNo"].Value = d.TemplateNo;
                    dataGridView1.Rows[rowNum].Cells["k"].Value = d.k;
                }
            }
            else
            {
                labelTextBaseCord.textBox.Text = "1";
                labelText2.textBox.Text = "1";
                labelText3.textBox.Text = "1";

                labelText4.textBox.Text = "1";
                labelText5.textBox.Text = "1";
                labelText6.textBox.Text = "1";
                labelText7.textBox.Text = "1";
                labelText8.textBox.Text = "1";
            }
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            if (isEdit == 1)
            {
                List<SystemConfigProduct> plist = current.Products.ToList();
                SystemConfigProduct product = plist.Find(x => x.Name == productName);
                plist.Remove(product);

                product.Name = labelText1.textBox.Text;
                SystemConfigProductBaseCords cords = new SystemConfigProductBaseCords();
                cords.BaseCord1 = labelTextBaseCord.textBox.Text;
                cords.BaseCord2 = labelText2.textBox.Text;
                cords.BaseCord3 = labelText3.textBox.Text;
                cords.BaseCord4 = labelText4.textBox.Text;
                cords.BaseCord5 = labelText5.textBox.Text;
                cords.BaseCord6 = labelText6.textBox.Text;
                cords.BaseCord7 = labelText7.textBox.Text;
                cords.BaseCord8 = labelText8.textBox.Text;
                product.BaseCords = cords;
                //product.Datas = new SystemConfigProductData()[];
                List<SystemConfigProductData> tempList = new List<SystemConfigProductData>();
                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    if (item.Cells["No"].Value != null)
                    {
                        SystemConfigProductData dataTemp = new SystemConfigProductData();
                        dataTemp.No = item.Cells["No"].Value.ToString();
                        dataTemp.Type = item.Cells["Type"].Value==null?"":item.Cells["Type"].Value.ToString();
                        dataTemp.Text = item.Cells["data"].Value == null ? "" : item.Cells["data"].Value.ToString().Replace("，",",");   //将全角,替换为半角
                        dataTemp.PosX = item.Cells["PosX"].Value==null?"":item.Cells["PosX"].Value.ToString();
                        dataTemp.PosY = item.Cells["PosY"].Value==null?"":item.Cells["PosY"].Value.ToString();
                        dataTemp.Angle = item.Cells["Angle"].Value == null ? "" : item.Cells["Angle"].Value.ToString();
                        dataTemp.TemplateNo = item.Cells["TemplateNo"].Value == null ? "" : item.Cells["TemplateNo"].Value.ToString();
                        dataTemp.k = item.Cells["k"].Value == null ? "" : item.Cells["k"].Value.ToString();
                        if (Convert.ToInt32(dataTemp.PosX) > 10 || Convert.ToInt32(dataTemp.PosX) < -300)
                        {
                            MessageBox.Show(dataTemp.No + "号X坐标超过临界值，请确认");
                            return;
                        }
                        if (Convert.ToInt32(dataTemp.PosY) < -10 || Convert.ToInt32(dataTemp.PosY) > 350)
                        {
                            MessageBox.Show(dataTemp.No + "号Y坐标超过临界值，请确认");
                            return;
                        }
                        tempList.Add(dataTemp);
                    }
                }
                product.Datas = tempList.ToArray();
                plist.Add(product);
                current.Products = plist.ToArray();
                Common.BackupFile();
                Common.SaveConfigToFile(current);
                Common.GetSystemConfigFromXmlFile();

                this.Close();
            }
            else
            {
               
                List<SystemConfigProduct> plist = current.Products.ToList();
                SystemConfigProduct product = new SystemConfigProduct();
                product.Name = labelText1.textBox.Text;
                SystemConfigProductBaseCords cords = new SystemConfigProductBaseCords();
                cords.BaseCord1 = labelTextBaseCord.textBox.Text;
                cords.BaseCord2 = labelText2.textBox.Text;
                cords.BaseCord3 = labelText3.textBox.Text;
                cords.BaseCord4 = labelText4.textBox.Text;
                cords.BaseCord5 = labelText5.textBox.Text;
                cords.BaseCord6 = labelText6.textBox.Text;
                cords.BaseCord7 = labelText7.textBox.Text;
                cords.BaseCord8 = labelText8.textBox.Text;
                product.BaseCords = cords;
                //product.Datas = new SystemConfigProductData()[];
                List<SystemConfigProductData> tempList = new List<SystemConfigProductData>();
                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    if (item.Cells["No"].Value != null)
                    {
                        SystemConfigProductData dataTemp = new SystemConfigProductData();
                        dataTemp.No = item.Cells["No"].Value.ToString();
                        dataTemp.Type = item.Cells["Type"].Value == null ? "Text" : item.Cells["Type"].Value.ToString();
                        dataTemp.Text = item.Cells["data"].Value == null ? "0" : item.Cells["data"].Value.ToString(); 
                        dataTemp.PosX = item.Cells["PosX"].Value == null ? "0" : item.Cells["PosX"].Value.ToString();
                        dataTemp.PosY = item.Cells["PosY"].Value == null ? "0" : item.Cells["PosY"].Value.ToString();
                        dataTemp.Angle = item.Cells["Angle"].Value == null ? "0" : item.Cells["Angle"].Value.ToString();
                        dataTemp.TemplateNo = item.Cells["TemplateNo"].Value == null ? "1" : item.Cells["TemplateNo"].Value.ToString();
                        dataTemp.k = item.Cells["k"].Value == null ? "" : item.Cells["k"].Value.ToString();
                        if (Convert.ToInt32(dataTemp.PosX) > 10 || Convert.ToInt32(dataTemp.PosX) <-300)
                        {
                            MessageBox.Show(dataTemp.No+"号X坐标超过临界值，请确认");
                            return;
                        }
                        if (Convert.ToInt32(dataTemp.PosY) < -10 || Convert.ToInt32(dataTemp.PosY) >350)
                        {
                            MessageBox.Show(dataTemp.No + "号Y坐标超过临界值，请确认");
                            return;
                        }
                        tempList.Add(dataTemp);
                    }
                }
                product.Datas = tempList.ToArray();
                plist.Add(product);
                current.Products = plist.ToArray();
                Common.BackupFile();
                Common.SaveConfigToFile(current);
                Common.GetSystemConfigFromXmlFile();

                this.Close();
            }
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewColumn column = dataGridView1.Columns[e.ColumnIndex];
                if (column is DataGridViewButtonColumn)
                {

                    this.dataGridView1.Rows.RemoveAt(e.RowIndex);//删除当前行
                }
            }
        }

        private void SplitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void ToolStripButtonCancel_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("确定要删除该记录吗？删除前请注意备份数据文件","删除",MessageBoxButtons.OKCancel)==DialogResult.OK)
            {
                List<SystemConfigProduct> plist = current.Products.ToList();
                SystemConfigProduct product = plist.Find(x => x.Name == productName);
                plist.Remove(product);
                current.Products = plist.ToArray();
                Common.BackupFile();
                Common.SaveConfigToFile(current);
                Common.GetSystemConfigFromXmlFile();

            }
        }

        private void toolStripButtonOrder_Click(object sender, EventArgs e)
        {
            int order = 1;
            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                if (!item.IsNewRow)
                {
                    item.Cells["No"].Value = order.ToString();
                    order++;
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                //dataGridView1.Rows.Add();
                dataGridView1.Rows.Insert(dataGridView1.CurrentCell.RowIndex,1);
                //((DataTable)dataGridView1.DataSource).Rows.InsertAt(((DataTable)dataGridView1.DataSource).NewRow(), dataGridView1.CurrentCell.RowIndex);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

       
    }
}
