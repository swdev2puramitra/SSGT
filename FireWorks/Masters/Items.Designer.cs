namespace FireWorks.Masters
{
    partial class Items
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
            this.txtItemCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtItemName = new System.Windows.Forms.TextBox();
            this.cbItemType = new System.Windows.Forms.ComboBox();
            this.cbCompany = new System.Windows.Forms.ComboBox();
            this.cbCategory = new System.Windows.Forms.ComboBox();
            this.cbSubCategory = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtRetail = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtWholeSale = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtSemi = new System.Windows.Forms.TextBox();
            this.cbConversion = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.dgvConversions = new System.Windows.Forms.DataGridView();
            this.ItemTypeId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemTypeName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Quantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WholeSale = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SemiWholeSale = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RetailPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblItemId = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbInactive = new System.Windows.Forms.RadioButton();
            this.rbActive = new System.Windows.Forms.RadioButton();
            this.lblRetail = new System.Windows.Forms.Label();
            this.lblSemi = new System.Windows.Forms.Label();
            this.cbHSN = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConversions)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtItemCode
            // 
            this.txtItemCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtItemCode.Location = new System.Drawing.Point(106, 49);
            this.txtItemCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtItemCode.Name = "txtItemCode";
            this.txtItemCode.Size = new System.Drawing.Size(160, 26);
            this.txtItemCode.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(18, 53);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Item Code";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(703, 89);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Sub-Category";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(349, 89);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "Category";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(703, 53);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(128, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "Company Name";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(19, 89);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 20);
            this.label5.TabIndex = 9;
            this.label5.Text = "Item Type";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(349, 53);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(90, 20);
            this.label6.TabIndex = 11;
            this.label6.Text = "Item Name";
            // 
            // txtItemName
            // 
            this.txtItemName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtItemName.Location = new System.Drawing.Point(497, 49);
            this.txtItemName.Margin = new System.Windows.Forms.Padding(4);
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.Size = new System.Drawing.Size(160, 26);
            this.txtItemName.TabIndex = 1;
            // 
            // cbItemType
            // 
            this.cbItemType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbItemType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbItemType.DisplayMember = "ItemTypeName";
            this.cbItemType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbItemType.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbItemType.FormattingEnabled = true;
            this.cbItemType.Location = new System.Drawing.Point(106, 89);
            this.cbItemType.Margin = new System.Windows.Forms.Padding(4);
            this.cbItemType.Name = "cbItemType";
            this.cbItemType.Size = new System.Drawing.Size(160, 28);
            this.cbItemType.TabIndex = 3;
            this.cbItemType.ValueMember = "ItemTypeId";
            // 
            // cbCompany
            // 
            this.cbCompany.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbCompany.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbCompany.DisplayMember = "CompanyName";
            this.cbCompany.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCompany.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbCompany.FormattingEnabled = true;
            this.cbCompany.Location = new System.Drawing.Point(851, 49);
            this.cbCompany.Margin = new System.Windows.Forms.Padding(4);
            this.cbCompany.Name = "cbCompany";
            this.cbCompany.Size = new System.Drawing.Size(160, 28);
            this.cbCompany.TabIndex = 2;
            this.cbCompany.ValueMember = "CompanyId";
            this.cbCompany.SelectedIndexChanged += new System.EventHandler(this.cbCompany_SelectedIndexChanged);
            // 
            // cbCategory
            // 
            this.cbCategory.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbCategory.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbCategory.DisplayMember = "CategoryName";
            this.cbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbCategory.FormattingEnabled = true;
            this.cbCategory.Location = new System.Drawing.Point(497, 85);
            this.cbCategory.Margin = new System.Windows.Forms.Padding(4);
            this.cbCategory.Name = "cbCategory";
            this.cbCategory.Size = new System.Drawing.Size(160, 28);
            this.cbCategory.TabIndex = 4;
            this.cbCategory.ValueMember = "CategoryId";
            this.cbCategory.SelectedIndexChanged += new System.EventHandler(this.cbCategory_SelectedIndexChanged);
            // 
            // cbSubCategory
            // 
            this.cbSubCategory.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbSubCategory.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbSubCategory.DisplayMember = "SubCategoryName";
            this.cbSubCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSubCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbSubCategory.FormattingEnabled = true;
            this.cbSubCategory.Location = new System.Drawing.Point(851, 89);
            this.cbSubCategory.Margin = new System.Windows.Forms.Padding(4);
            this.cbSubCategory.Name = "cbSubCategory";
            this.cbSubCategory.Size = new System.Drawing.Size(160, 28);
            this.cbSubCategory.TabIndex = 5;
            this.cbSubCategory.ValueMember = "SubCategoryId";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(39, 102);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 20);
            this.label7.TabIndex = 17;
            this.label7.Text = "Retail";
            // 
            // txtRetail
            // 
            this.txtRetail.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRetail.Location = new System.Drawing.Point(192, 99);
            this.txtRetail.Margin = new System.Windows.Forms.Padding(4);
            this.txtRetail.Name = "txtRetail";
            this.txtRetail.Size = new System.Drawing.Size(160, 26);
            this.txtRetail.TabIndex = 2;
            this.txtRetail.TextChanged += new System.EventHandler(this.txtRetail_TextChanged);
            this.txtRetail.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtRetail_KeyPress);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(39, 36);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(94, 20);
            this.label8.TabIndex = 19;
            this.label8.Text = "Whole Sale";
            // 
            // txtWholeSale
            // 
            this.txtWholeSale.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWholeSale.Location = new System.Drawing.Point(192, 33);
            this.txtWholeSale.Margin = new System.Windows.Forms.Padding(4);
            this.txtWholeSale.Name = "txtWholeSale";
            this.txtWholeSale.Size = new System.Drawing.Size(160, 26);
            this.txtWholeSale.TabIndex = 0;
            this.txtWholeSale.TextChanged += new System.EventHandler(this.txtWholeSale_TextChanged);
            this.txtWholeSale.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtWholeSale_KeyPress);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(39, 69);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(137, 20);
            this.label9.TabIndex = 21;
            this.label9.Text = "Semi Whole Sale";
            // 
            // txtSemi
            // 
            this.txtSemi.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSemi.Location = new System.Drawing.Point(192, 66);
            this.txtSemi.Margin = new System.Windows.Forms.Padding(4);
            this.txtSemi.Name = "txtSemi";
            this.txtSemi.Size = new System.Drawing.Size(160, 26);
            this.txtSemi.TabIndex = 1;
            this.txtSemi.TextChanged += new System.EventHandler(this.txtSemi_TextChanged);
            this.txtSemi.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtSemi_KeyPress);
            // 
            // cbConversion
            // 
            this.cbConversion.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbConversion.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbConversion.DisplayMember = "ConversionName";
            this.cbConversion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbConversion.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbConversion.FormattingEnabled = true;
            this.cbConversion.Location = new System.Drawing.Point(106, 132);
            this.cbConversion.Margin = new System.Windows.Forms.Padding(4);
            this.cbConversion.Name = "cbConversion";
            this.cbConversion.Size = new System.Drawing.Size(221, 28);
            this.cbConversion.TabIndex = 6;
            this.cbConversion.ValueMember = "ConversionId";
            this.cbConversion.SelectedIndexChanged += new System.EventHandler(this.cbConversion_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(19, 132);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(93, 20);
            this.label10.TabIndex = 22;
            this.label10.Text = "Conversion";
            // 
            // dgvConversions
            // 
            this.dgvConversions.AllowUserToAddRows = false;
            this.dgvConversions.AllowUserToDeleteRows = false;
            this.dgvConversions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvConversions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ItemTypeId,
            this.ItemTypeName,
            this.Quantity,
            this.WholeSale,
            this.SemiWholeSale,
            this.RetailPrice});
            this.dgvConversions.Location = new System.Drawing.Point(11, 181);
            this.dgvConversions.Margin = new System.Windows.Forms.Padding(4);
            this.dgvConversions.Name = "dgvConversions";
            this.dgvConversions.RowHeadersWidth = 51;
            this.dgvConversions.Size = new System.Drawing.Size(623, 192);
            this.dgvConversions.TabIndex = 7;
            this.dgvConversions.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvConversions_CellValueChanged);
            this.dgvConversions.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvConversions_EditingControlShowing);
            // 
            // ItemTypeId
            // 
            this.ItemTypeId.DataPropertyName = "ItemTypeId";
            this.ItemTypeId.HeaderText = "Item Type Id";
            this.ItemTypeId.MinimumWidth = 6;
            this.ItemTypeId.Name = "ItemTypeId";
            this.ItemTypeId.ReadOnly = true;
            this.ItemTypeId.Width = 125;
            // 
            // ItemTypeName
            // 
            this.ItemTypeName.DataPropertyName = "ItemTypeName";
            this.ItemTypeName.HeaderText = "Item Type Name";
            this.ItemTypeName.MinimumWidth = 6;
            this.ItemTypeName.Name = "ItemTypeName";
            this.ItemTypeName.ReadOnly = true;
            this.ItemTypeName.Width = 150;
            // 
            // Quantity
            // 
            this.Quantity.HeaderText = "Quantity";
            this.Quantity.MinimumWidth = 6;
            this.Quantity.Name = "Quantity";
            this.Quantity.Width = 90;
            // 
            // WholeSale
            // 
            this.WholeSale.HeaderText = "Whole Sale";
            this.WholeSale.MinimumWidth = 6;
            this.WholeSale.Name = "WholeSale";
            this.WholeSale.ReadOnly = true;
            this.WholeSale.Width = 80;
            // 
            // SemiWholeSale
            // 
            this.SemiWholeSale.HeaderText = "Semi Whole Sale";
            this.SemiWholeSale.MinimumWidth = 6;
            this.SemiWholeSale.Name = "SemiWholeSale";
            this.SemiWholeSale.ReadOnly = true;
            this.SemiWholeSale.Width = 80;
            // 
            // RetailPrice
            // 
            this.RetailPrice.HeaderText = "Retail Price";
            this.RetailPrice.MinimumWidth = 6;
            this.RetailPrice.Name = "RetailPrice";
            this.RetailPrice.ReadOnly = true;
            this.RetailPrice.Width = 80;
            // 
            // lblItemId
            // 
            this.lblItemId.AutoSize = true;
            this.lblItemId.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblItemId.Location = new System.Drawing.Point(963, 356);
            this.lblItemId.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblItemId.Name = "lblItemId";
            this.lblItemId.Size = new System.Drawing.Size(59, 20);
            this.lblItemId.TabIndex = 1;
            this.lblItemId.Text = "Item Id";
            this.lblItemId.Visible = false;
            // 
            // btnSave
            // 
            this.btnSave.Image = global::FireWorks.Properties.Resources.save;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(689, 334);
            this.btnSave.Margin = new System.Windows.Forms.Padding(5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(122, 39);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClear
            // 
            this.btnClear.Image = global::FireWorks.Properties.Resources.Cancel;
            this.btnClear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClear.Location = new System.Drawing.Point(832, 334);
            this.btnClear.Margin = new System.Windows.Forms.Padding(5);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(122, 39);
            this.btnClear.TabIndex = 10;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.Color.SteelBlue;
            this.label12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label12.Dock = System.Windows.Forms.DockStyle.Top;
            this.label12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label12.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.White;
            this.label12.Location = new System.Drawing.Point(0, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(1047, 34);
            this.label12.TabIndex = 156;
            this.label12.Text = "Add Inventory";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtWholeSale);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtSemi);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtRetail);
            this.groupBox1.Location = new System.Drawing.Point(641, 181);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(370, 145);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Item Price";
            // 
            // rbInactive
            // 
            this.rbInactive.AutoSize = true;
            this.rbInactive.Location = new System.Drawing.Point(926, 128);
            this.rbInactive.Name = "rbInactive";
            this.rbInactive.Size = new System.Drawing.Size(87, 24);
            this.rbInactive.TabIndex = 173;
            this.rbInactive.TabStop = true;
            this.rbInactive.Text = "Inactive";
            this.rbInactive.UseVisualStyleBackColor = true;
            // 
            // rbActive
            // 
            this.rbActive.AutoSize = true;
            this.rbActive.Location = new System.Drawing.Point(851, 128);
            this.rbActive.Name = "rbActive";
            this.rbActive.Size = new System.Drawing.Size(76, 24);
            this.rbActive.TabIndex = 172;
            this.rbActive.TabStop = true;
            this.rbActive.Text = "Active";
            this.rbActive.UseVisualStyleBackColor = true;
            // 
            // lblRetail
            // 
            this.lblRetail.AutoSize = true;
            this.lblRetail.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRetail.Location = new System.Drawing.Point(703, 161);
            this.lblRetail.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRetail.Name = "lblRetail";
            this.lblRetail.Size = new System.Drawing.Size(0, 20);
            this.lblRetail.TabIndex = 174;
            this.lblRetail.Visible = false;
            // 
            // lblSemi
            // 
            this.lblSemi.AutoSize = true;
            this.lblSemi.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSemi.Location = new System.Drawing.Point(703, 132);
            this.lblSemi.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSemi.Name = "lblSemi";
            this.lblSemi.Size = new System.Drawing.Size(0, 20);
            this.lblSemi.TabIndex = 175;
            this.lblSemi.Visible = false;
            // 
            // cbHSN
            // 
            this.cbHSN.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbHSN.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbHSN.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbHSN.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbHSN.FormattingEnabled = true;
            this.cbHSN.Location = new System.Drawing.Point(497, 128);
            this.cbHSN.Margin = new System.Windows.Forms.Padding(4);
            this.cbHSN.Name = "cbHSN";
            this.cbHSN.Size = new System.Drawing.Size(160, 28);
            this.cbHSN.TabIndex = 176;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(349, 132);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(89, 20);
            this.label11.TabIndex = 177;
            this.label11.Text = "HSN Code";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(703, 132);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(57, 20);
            this.label13.TabIndex = 178;
            this.label13.Text = "Status";
            // 
            // Items
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1047, 400);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.cbHSN);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.lblSemi);
            this.Controls.Add(this.lblRetail);
            this.Controls.Add(this.rbInactive);
            this.Controls.Add(this.rbActive);
            this.Controls.Add(this.lblItemId);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvConversions);
            this.Controls.Add(this.cbConversion);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.cbSubCategory);
            this.Controls.Add(this.cbCategory);
            this.Controls.Add(this.cbCompany);
            this.Controls.Add(this.cbItemType);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtItemName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtItemCode);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Items";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add Inventory";
            ((System.ComponentModel.ISupportInitialize)(this.dgvConversions)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtItemCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtItemName;
        private System.Windows.Forms.ComboBox cbItemType;
        private System.Windows.Forms.ComboBox cbCompany;
        private System.Windows.Forms.ComboBox cbCategory;
        private System.Windows.Forms.ComboBox cbSubCategory;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtRetail;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtWholeSale;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtSemi;
        private System.Windows.Forms.ComboBox cbConversion;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.DataGridView dgvConversions;
        private System.Windows.Forms.Label lblItemId;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClear;
        internal System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemTypeId;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemTypeName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Quantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn WholeSale;
        private System.Windows.Forms.DataGridViewTextBoxColumn SemiWholeSale;
        private System.Windows.Forms.DataGridViewTextBoxColumn RetailPrice;
        private System.Windows.Forms.RadioButton rbInactive;
        private System.Windows.Forms.RadioButton rbActive;
        private System.Windows.Forms.Label lblRetail;
        private System.Windows.Forms.Label lblSemi;
        private System.Windows.Forms.ComboBox cbHSN;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label13;
    }
}