namespace Printer
{
    partial class FormBatchInstead
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
            this.labelText1 = new UserControls.LabelText();
            this.labelText2 = new UserControls.LabelText();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelText1
            // 
            this.labelText1.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelText1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelText1.LabelString = "原内容";
            this.labelText1.Location = new System.Drawing.Point(0, 0);
            this.labelText1.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.labelText1.Name = "labelText1";
            this.labelText1.Size = new System.Drawing.Size(389, 37);
            this.labelText1.TabIndex = 0;
            // 
            // labelText2
            // 
            this.labelText2.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelText2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelText2.LabelString = "新内容";
            this.labelText2.Location = new System.Drawing.Point(0, 37);
            this.labelText2.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.labelText2.Name = "labelText2";
            this.labelText2.Size = new System.Drawing.Size(389, 37);
            this.labelText2.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(84, 102);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "替换";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(197, 101);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "取消";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // FormBatchInstead
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 181);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.labelText2);
            this.Controls.Add(this.labelText1);
            this.Name = "FormBatchInstead";
            this.Text = "FormBatchInstead";
            this.ResumeLayout(false);

        }

        #endregion

        private UserControls.LabelText labelText1;
        private UserControls.LabelText labelText2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}