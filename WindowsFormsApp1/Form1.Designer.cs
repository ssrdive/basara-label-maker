namespace WindowsFormsApp1
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.btnLoadItems = new System.Windows.Forms.Button();
            this.itemsList = new System.Windows.Forms.ComboBox();
            this.printQty = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.searchText = new System.Windows.Forms.TextBox();
            this.searchBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 168);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(110, 46);
            this.button1.TabIndex = 0;
            this.button1.Text = "Print";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnLoadItems
            // 
            this.btnLoadItems.Location = new System.Drawing.Point(12, 12);
            this.btnLoadItems.Name = "btnLoadItems";
            this.btnLoadItems.Size = new System.Drawing.Size(173, 48);
            this.btnLoadItems.TabIndex = 1;
            this.btnLoadItems.Text = "Load Item Master";
            this.btnLoadItems.UseVisualStyleBackColor = true;
            this.btnLoadItems.Click += new System.EventHandler(this.button2_Click);
            // 
            // itemsList
            // 
            this.itemsList.FormattingEnabled = true;
            this.itemsList.Location = new System.Drawing.Point(12, 78);
            this.itemsList.Name = "itemsList";
            this.itemsList.Size = new System.Drawing.Size(811, 28);
            this.itemsList.TabIndex = 2;
            this.itemsList.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // printQty
            // 
            this.printQty.Location = new System.Drawing.Point(12, 126);
            this.printQty.Name = "printQty";
            this.printQty.Size = new System.Drawing.Size(110, 26);
            this.printQty.TabIndex = 3;
            this.printQty.Text = "1";
            this.printQty.TextChanged += new System.EventHandler(this.printQty_TextChanged);
            this.printQty.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.printQty_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(301, 314);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(254, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "2022 © FarmGear (Private) Limited";
            // 
            // searchText
            // 
            this.searchText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchText.Location = new System.Drawing.Point(216, 19);
            this.searchText.Name = "searchText";
            this.searchText.Size = new System.Drawing.Size(431, 35);
            this.searchText.TabIndex = 7;
            // 
            // searchBtn
            // 
            this.searchBtn.Location = new System.Drawing.Point(663, 12);
            this.searchBtn.Name = "searchBtn";
            this.searchBtn.Size = new System.Drawing.Size(160, 48);
            this.searchBtn.TabIndex = 8;
            this.searchBtn.Text = "Search";
            this.searchBtn.UseVisualStyleBackColor = true;
            this.searchBtn.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(835, 343);
            this.Controls.Add(this.searchBtn);
            this.Controls.Add(this.searchText);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.printQty);
            this.Controls.Add(this.itemsList);
            this.Controls.Add(this.btnLoadItems);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Item Printer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnLoadItems;
        private System.Windows.Forms.ComboBox itemsList;
        private System.Windows.Forms.TextBox printQty;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox searchText;
        private System.Windows.Forms.Button searchBtn;
    }
}

