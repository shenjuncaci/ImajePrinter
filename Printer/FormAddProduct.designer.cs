namespace Printer
{
    partial class FormAddProduct
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStripAddProduct = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCancel = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonOrder = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.labelText8 = new UserControls.LabelText();
            this.labelText7 = new UserControls.LabelText();
            this.labelText6 = new UserControls.LabelText();
            this.labelText5 = new UserControls.LabelText();
            this.labelText4 = new UserControls.LabelText();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TemplateNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.data = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PosX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PosY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Angle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.delete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.labelText3 = new UserControls.LabelText();
            this.labelText2 = new UserControls.LabelText();
            this.labelTextBaseCord = new UserControls.LabelText();
            this.labelText1 = new UserControls.LabelText();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.toolStripAddProduct.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripAddProduct
            // 
            this.toolStripAddProduct.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonSave,
            this.toolStripButtonCancel,
            this.toolStripButtonOrder,
            this.toolStripButton1});
            this.toolStripAddProduct.Location = new System.Drawing.Point(0, 0);
            this.toolStripAddProduct.Name = "toolStripAddProduct";
            this.toolStripAddProduct.Size = new System.Drawing.Size(1185, 39);
            this.toolStripAddProduct.TabIndex = 0;
            this.toolStripAddProduct.Text = "toolStrip1";
            // 
            // toolStripButtonSave
            // 
            this.toolStripButtonSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStripButtonSave.Image = global::Printer.Properties.Resources.Save;
            this.toolStripButtonSave.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSave.Name = "toolStripButtonSave";
            this.toolStripButtonSave.Size = new System.Drawing.Size(77, 36);
            this.toolStripButtonSave.Text = "保存";
            this.toolStripButtonSave.Click += new System.EventHandler(this.toolStripButtonSave_Click);
            // 
            // toolStripButtonCancel
            // 
            this.toolStripButtonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStripButtonCancel.Image = global::Printer.Properties.Resources.del;
            this.toolStripButtonCancel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCancel.Name = "toolStripButtonCancel";
            this.toolStripButtonCancel.Size = new System.Drawing.Size(109, 36);
            this.toolStripButtonCancel.Text = "删除产品";
            this.toolStripButtonCancel.Click += new System.EventHandler(this.ToolStripButtonCancel_Click);
            // 
            // toolStripButtonOrder
            // 
            this.toolStripButtonOrder.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.toolStripButtonOrder.Image = global::Printer.Properties.Resources.ArrowUp;
            this.toolStripButtonOrder.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonOrder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOrder.Name = "toolStripButtonOrder";
            this.toolStripButtonOrder.Size = new System.Drawing.Size(109, 36);
            this.toolStripButtonOrder.Text = "生成编号";
            this.toolStripButtonOrder.Click += new System.EventHandler(this.toolStripButtonOrder_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.toolStripButton1.Image = global::Printer.Properties.Resources.edit;
            this.toolStripButton1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(93, 36);
            this.toolStripButton1.Text = "插入行";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 39);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.labelText8);
            this.splitContainer1.Panel1.Controls.Add(this.labelText7);
            this.splitContainer1.Panel1.Controls.Add(this.labelText6);
            this.splitContainer1.Panel1.Controls.Add(this.labelText5);
            this.splitContainer1.Panel1.Controls.Add(this.labelText4);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            this.splitContainer1.Panel1.Controls.Add(this.labelText3);
            this.splitContainer1.Panel1.Controls.Add(this.labelText2);
            this.splitContainer1.Panel1.Controls.Add(this.labelTextBaseCord);
            this.splitContainer1.Panel1.Controls.Add(this.labelText1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1185, 622);
            this.splitContainer1.SplitterDistance = 1147;
            this.splitContainer1.TabIndex = 1;
            this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.SplitContainer1_SplitterMoved);
            // 
            // labelText8
            // 
            this.labelText8.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelText8.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelText8.LabelString = "计数8($)";
            this.labelText8.Location = new System.Drawing.Point(0, 296);
            this.labelText8.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.labelText8.Name = "labelText8";
            this.labelText8.Size = new System.Drawing.Size(1147, 37);
            this.labelText8.TabIndex = 11;
            // 
            // labelText7
            // 
            this.labelText7.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelText7.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelText7.LabelString = "计数7(|)";
            this.labelText7.Location = new System.Drawing.Point(0, 259);
            this.labelText7.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.labelText7.Name = "labelText7";
            this.labelText7.Size = new System.Drawing.Size(1147, 37);
            this.labelText7.TabIndex = 10;
            // 
            // labelText6
            // 
            this.labelText6.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelText6.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelText6.LabelString = "计数6(+)";
            this.labelText6.Location = new System.Drawing.Point(0, 222);
            this.labelText6.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.labelText6.Name = "labelText6";
            this.labelText6.Size = new System.Drawing.Size(1147, 37);
            this.labelText6.TabIndex = 9;
            // 
            // labelText5
            // 
            this.labelText5.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelText5.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelText5.LabelString = "计数5(-)";
            this.labelText5.Location = new System.Drawing.Point(0, 185);
            this.labelText5.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.labelText5.Name = "labelText5";
            this.labelText5.Size = new System.Drawing.Size(1147, 37);
            this.labelText5.TabIndex = 8;
            // 
            // labelText4
            // 
            this.labelText4.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelText4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelText4.LabelString = "计数4(!)";
            this.labelText4.Location = new System.Drawing.Point(0, 148);
            this.labelText4.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.labelText4.Name = "labelText4";
            this.labelText4.Size = new System.Drawing.Size(1147, 37);
            this.labelText4.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Info;
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(0, 336);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1060, 286);
            this.panel1.TabIndex = 6;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.No,
            this.TemplateNo,
            this.Type,
            this.data,
            this.PosX,
            this.PosY,
            this.Angle,
            this.delete});
            this.dataGridView1.Location = new System.Drawing.Point(0, 21);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(954, 265);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellContentClick);
            // 
            // No
            // 
            this.No.HeaderText = "编号";
            this.No.Name = "No";
            this.No.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.No.Width = 48;
            // 
            // TemplateNo
            // 
            this.TemplateNo.HeaderText = "模板编号";
            this.TemplateNo.Name = "TemplateNo";
            this.TemplateNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.TemplateNo.Width = 80;
            // 
            // Type
            // 
            this.Type.HeaderText = "类型";
            this.Type.Items.AddRange(new object[] {
            "Text",
            "Barcode"});
            this.Type.Name = "Type";
            this.Type.Width = 48;
            // 
            // data
            // 
            this.data.HeaderText = "数据";
            this.data.Name = "data";
            this.data.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.data.Width = 48;
            // 
            // PosX
            // 
            this.PosX.HeaderText = "X坐标";
            this.PosX.Name = "PosX";
            this.PosX.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.PosX.Width = 58;
            // 
            // PosY
            // 
            this.PosY.HeaderText = "Y坐标";
            this.PosY.Name = "PosY";
            this.PosY.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.PosY.Width = 58;
            // 
            // Angle
            // 
            this.Angle.HeaderText = "角度";
            this.Angle.Name = "Angle";
            this.Angle.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Angle.Width = 48;
            // 
            // delete
            // 
            this.delete.HeaderText = "删除";
            this.delete.Name = "delete";
            this.delete.Width = 48;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "喷码数据表";
            // 
            // labelText3
            // 
            this.labelText3.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelText3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelText3.LabelString = "计数3(%)";
            this.labelText3.Location = new System.Drawing.Point(0, 111);
            this.labelText3.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.labelText3.Name = "labelText3";
            this.labelText3.Size = new System.Drawing.Size(1147, 37);
            this.labelText3.TabIndex = 5;
            // 
            // labelText2
            // 
            this.labelText2.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelText2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelText2.LabelString = "计数2(#)";
            this.labelText2.Location = new System.Drawing.Point(0, 74);
            this.labelText2.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.labelText2.Name = "labelText2";
            this.labelText2.Size = new System.Drawing.Size(1147, 37);
            this.labelText2.TabIndex = 4;
            // 
            // labelTextBaseCord
            // 
            this.labelTextBaseCord.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelTextBaseCord.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelTextBaseCord.LabelString = "计数1(*)";
            this.labelTextBaseCord.Location = new System.Drawing.Point(0, 37);
            this.labelTextBaseCord.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.labelTextBaseCord.Name = "labelTextBaseCord";
            this.labelTextBaseCord.Size = new System.Drawing.Size(1147, 37);
            this.labelTextBaseCord.TabIndex = 2;
            // 
            // labelText1
            // 
            this.labelText1.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelText1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelText1.LabelString = "产品名称";
            this.labelText1.Location = new System.Drawing.Point(0, 0);
            this.labelText1.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.labelText1.Name = "labelText1";
            this.labelText1.Size = new System.Drawing.Size(1147, 37);
            this.labelText1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.button6);
            this.splitContainer2.Panel2.Controls.Add(this.button5);
            this.splitContainer2.Panel2.Controls.Add(this.button4);
            this.splitContainer2.Panel2.Controls.Add(this.button3);
            this.splitContainer2.Panel2.Controls.Add(this.button2);
            this.splitContainer2.Panel2.Controls.Add(this.button1);
            this.splitContainer2.Size = new System.Drawing.Size(34, 622);
            this.splitContainer2.SplitterDistance = 465;
            this.splitContainer2.TabIndex = 0;
            // 
            // button6
            // 
            this.button6.Dock = System.Windows.Forms.DockStyle.Right;
            this.button6.Location = new System.Drawing.Point(-116, 0);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 153);
            this.button6.TabIndex = 5;
            this.button6.Text = "Mark Point2";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Dock = System.Windows.Forms.DockStyle.Right;
            this.button5.Location = new System.Drawing.Point(-41, 0);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 153);
            this.button5.TabIndex = 4;
            this.button5.Text = "Mark Point1";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.BackgroundImage = global::Printer.Properties.Resources.ArrowDown;
            this.button4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button4.Dock = System.Windows.Forms.DockStyle.Left;
            this.button4.Location = new System.Drawing.Point(225, 0);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 153);
            this.button4.TabIndex = 3;
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.BackgroundImage = global::Printer.Properties.Resources.ArrowUp;
            this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button3.Dock = System.Windows.Forms.DockStyle.Left;
            this.button3.Location = new System.Drawing.Point(150, 0);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 153);
            this.button3.TabIndex = 2;
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.BackgroundImage = global::Printer.Properties.Resources.ArrowRight;
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button2.Dock = System.Windows.Forms.DockStyle.Left;
            this.button2.Location = new System.Drawing.Point(75, 0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 153);
            this.button2.TabIndex = 1;
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.BackgroundImage = global::Printer.Properties.Resources.ArrowLeft;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button1.Dock = System.Windows.Forms.DockStyle.Left;
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 153);
            this.button1.TabIndex = 0;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // FormAddProduct
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1185, 661);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStripAddProduct);
            this.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "FormAddProduct";
            this.Text = "添加产品";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormAddProduct_Load);
            this.toolStripAddProduct.ResumeLayout(false);
            this.toolStripAddProduct.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripAddProduct;
        private System.Windows.Forms.ToolStripButton toolStripButtonSave;
        private System.Windows.Forms.ToolStripButton toolStripButtonCancel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private UserControls.LabelText labelText1;
        private UserControls.LabelText labelTextBaseCord;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label1;
        private UserControls.LabelText labelText3;
        private UserControls.LabelText labelText2;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private UserControls.LabelText labelText8;
        private UserControls.LabelText labelText7;
        private UserControls.LabelText labelText6;
        private UserControls.LabelText labelText5;
        private UserControls.LabelText labelText4;
        private System.Windows.Forms.DataGridViewTextBoxColumn No;
        private System.Windows.Forms.DataGridViewTextBoxColumn TemplateNo;
        private System.Windows.Forms.DataGridViewComboBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn data;
        private System.Windows.Forms.DataGridViewTextBoxColumn PosX;
        private System.Windows.Forms.DataGridViewTextBoxColumn PosY;
        private System.Windows.Forms.DataGridViewTextBoxColumn Angle;
        private System.Windows.Forms.DataGridViewButtonColumn delete;
        private System.Windows.Forms.ToolStripButton toolStripButtonOrder;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
    }
}