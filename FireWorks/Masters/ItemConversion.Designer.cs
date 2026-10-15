namespace FireWorks.Masters
{
    partial class ItemConversion
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
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.txtConvesion = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvConvesions = new System.Windows.Forms.DataGridView();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.gbCatId = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblConversionId = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.rbInactive = new System.Windows.Forms.RadioButton();
            this.rbActive = new System.Windows.Forms.RadioButton();
            this.ConversionId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ConversionName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsActive = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConvesions)).BeginInit();
            this.gbCatId.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvItems
            // 
            this.dgvItems.AllowUserToAddRows = false;
            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItems.Location = new System.Drawing.Point(26, 157);
            this.dgvItems.Margin = new System.Windows.Forms.Padding(4);
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.Size = new System.Drawing.Size(461, 372);
            this.dgvItems.TabIndex = 1;
            this.dgvItems.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvItems_CellContentClick);
            // 
            // txtConvesion
            // 
            this.txtConvesion.Location = new System.Drawing.Point(167, 90);
            this.txtConvesion.Margin = new System.Windows.Forms.Padding(4);
            this.txtConvesion.Name = "txtConvesion";
            this.txtConvesion.Size = new System.Drawing.Size(320, 23);
            this.txtConvesion.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 90);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Convesion Name";
            // 
            // dgvConvesions
            // 
            this.dgvConvesions.AllowUserToAddRows = false;
            this.dgvConvesions.AllowUserToDeleteRows = false;
            this.dgvConvesions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvConvesions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ConversionId,
            this.ConversionName,
            this.IsActive});
            this.dgvConvesions.Location = new System.Drawing.Point(512, 157);
            this.dgvConvesions.Margin = new System.Windows.Forms.Padding(4);
            this.dgvConvesions.Name = "dgvConvesions";
            this.dgvConvesions.ReadOnly = true;
            this.dgvConvesions.Size = new System.Drawing.Size(503, 372);
            this.dgvConvesions.TabIndex = 5;
            this.dgvConvesions.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvConvesions_CellContentClick);
            // 
            // btnSave
            // 
            this.btnSave.Image = global::FireWorks.Properties.Resources.save;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(723, 97);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(122, 39);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClear
            // 
            // Image resource removed
            this.btnClear.Image = global::FireWorks.Properties.Resources.Cancel;
            this.btnClear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClear.Location = new System.Drawing.Point(863, 97);
            this.btnClear.Margin = new System.Windows.Forms.Padding(4);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(122, 39);
            this.btnClear.TabIndex = 4;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(583, 97);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(4);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(122, 39);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add Item";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // gbCatId
            // 
            this.gbCatId.Controls.Add(this.label2);
            this.gbCatId.Controls.Add(this.lblConversionId);
            this.gbCatId.Location = new System.Drawing.Point(27, 33);
            this.gbCatId.Margin = new System.Windows.Forms.Padding(4);
            this.gbCatId.Name = "gbCatId";
            this.gbCatId.Padding = new System.Windows.Forms.Padding(4);
            this.gbCatId.Size = new System.Drawing.Size(461, 49);
            this.gbCatId.TabIndex = 7;
            this.gbCatId.TabStop = false;
            this.gbCatId.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 20);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Conversion Id";
            // 
            // lblConversionId
            // 
            this.lblConversionId.AutoSize = true;
            this.lblConversionId.Location = new System.Drawing.Point(136, 20);
            this.lblConversionId.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblConversionId.Name = "lblConversionId";
            this.lblConversionId.Size = new System.Drawing.Size(80, 17);
            this.lblConversionId.TabIndex = 0;
            this.lblConversionId.Text = "Category Id";
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.SteelBlue;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label4.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(1028, 28);
            this.label4.TabIndex = 158;
            this.label4.Text = "Item Conversion";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rbInactive
            // 
            this.rbInactive.AutoSize = true;
            this.rbInactive.Location = new System.Drawing.Point(257, 120);
            this.rbInactive.Name = "rbInactive";
            this.rbInactive.Size = new System.Drawing.Size(74, 21);
            this.rbInactive.TabIndex = 171;
            this.rbInactive.TabStop = true;
            this.rbInactive.Text = "Inactive";
            this.rbInactive.UseVisualStyleBackColor = true;
            // 
            // rbActive
            // 
            this.rbActive.AutoSize = true;
            this.rbActive.Location = new System.Drawing.Point(166, 120);
            this.rbActive.Name = "rbActive";
            this.rbActive.Size = new System.Drawing.Size(64, 21);
            this.rbActive.TabIndex = 170;
            this.rbActive.TabStop = true;
            this.rbActive.Text = "Active";
            this.rbActive.UseVisualStyleBackColor = true;
            // 
            // ConversionId
            // 
            this.ConversionId.DataPropertyName = "ConversionId";
            this.ConversionId.HeaderText = "Conversion Id";
            this.ConversionId.Name = "ConversionId";
            this.ConversionId.ReadOnly = true;
            this.ConversionId.Width = 80;
            // 
            // ConversionName
            // 
            this.ConversionName.DataPropertyName = "ConversionName";
            this.ConversionName.HeaderText = "Conversion Name";
            this.ConversionName.Name = "ConversionName";
            this.ConversionName.ReadOnly = true;
            this.ConversionName.Width = 300;
            // 
            // IsActive
            // 
            this.IsActive.DataPropertyName = "IsActive";
            this.IsActive.HeaderText = "Is Active";
            this.IsActive.Name = "IsActive";
            this.IsActive.ReadOnly = true;
            this.IsActive.Width = 70;
            // 
            // ItemConversion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1028, 582);
            this.Controls.Add(this.rbInactive);
            this.Controls.Add(this.rbActive);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.gbCatId);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvConvesions);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtConvesion);
            this.Controls.Add(this.dgvItems);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ItemConversion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Item Conversion";
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConvesions)).EndInit();
            this.gbCatId.ResumeLayout(false);
            this.gbCatId.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvItems;
        private System.Windows.Forms.TextBox txtConvesion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvConvesions;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.GroupBox gbCatId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblConversionId;
        internal System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton rbInactive;
        private System.Windows.Forms.RadioButton rbActive;
        private System.Windows.Forms.DataGridViewTextBoxColumn ConversionId;
        private System.Windows.Forms.DataGridViewTextBoxColumn ConversionName;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsActive;
    }
}