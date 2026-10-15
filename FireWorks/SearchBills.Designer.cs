namespace FireWorks
{
    partial class SearchBills
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBillId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dgvBills = new System.Windows.Forms.DataGridView();
            this.dtpBill = new System.Windows.Forms.DateTimePicker();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.rbInvoice = new System.Windows.Forms.RadioButton();
            this.rbEstimate = new System.Windows.Forms.RadioButton();
            this.label16 = new System.Windows.Forms.Label();
            this.cbCustomer = new System.Windows.Forms.ComboBox();
            this.txtCustomer = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SlNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustomerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustomerType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BillId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BillNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InvoiceNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BillDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BillAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PhoneNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Estimate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WayBill = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Edit = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Update = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Print = new System.Windows.Forms.DataGridViewButtonColumn();
            this.JSON = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Dispatch = new System.Windows.Forms.DataGridViewButtonColumn();
            this.IsInState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InvoiceStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBills)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 58);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Bill No";
            // 
            // txtBillId
            // 
            this.txtBillId.Location = new System.Drawing.Point(67, 58);
            this.txtBillId.Margin = new System.Windows.Forms.Padding(4);
            this.txtBillId.Name = "txtBillId";
            this.txtBillId.Size = new System.Drawing.Size(160, 22);
            this.txtBillId.TabIndex = 1;
            this.txtBillId.TextChanged += new System.EventHandler(this.txtBillId_TextChanged);
            this.txtBillId.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBillId_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(234, 58);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Bill Date";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(420, 58);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Phone No";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(621, 58);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 16);
            this.label4.TabIndex = 6;
            this.label4.Text = "Customer";
            // 
            // dgvBills
            // 
            this.dgvBills.AllowUserToAddRows = false;
            this.dgvBills.AllowUserToDeleteRows = false;
            this.dgvBills.AllowUserToResizeColumns = false;
            this.dgvBills.AllowUserToResizeRows = false;
            this.dgvBills.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBills.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SlNo,
            this.CustomerName,
            this.CustAddress,
            this.CustomerType,
            this.BillId,
            this.BillNo,
            this.InvoiceNo,
            this.BillDate,
            this.BillAmount,
            this.PhoneNo,
            this.Estimate,
            this.WayBill,
            this.Edit,
            this.Update,
            this.Print,
            this.JSON,
            this.Dispatch,
            this.IsInState,
            this.InvoiceStatus});
            this.dgvBills.Location = new System.Drawing.Point(7, 118);
            this.dgvBills.MultiSelect = false;
            this.dgvBills.Name = "dgvBills";
            this.dgvBills.ReadOnly = true;
            this.dgvBills.RowHeadersVisible = false;
            this.dgvBills.RowHeadersWidth = 51;
            this.dgvBills.RowTemplate.Height = 25;
            this.dgvBills.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBills.Size = new System.Drawing.Size(1400, 400);
            this.dgvBills.TabIndex = 159;
            this.dgvBills.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBills_CellContentClick);
            this.dgvBills.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvBills_CellPainting);
            // 
            // dtpBill
            // 
            this.dtpBill.Checked = false;
            this.dtpBill.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpBill.Location = new System.Drawing.Point(298, 58);
            this.dtpBill.MaxDate = new System.DateTime(2020, 4, 2, 0, 0, 0, 0);
            this.dtpBill.MinDate = new System.DateTime(2020, 3, 1, 0, 0, 0, 0);
            this.dtpBill.Name = "dtpBill";
            this.dtpBill.ShowCheckBox = true;
            this.dtpBill.Size = new System.Drawing.Size(115, 22);
            this.dtpBill.TabIndex = 2;
            this.dtpBill.Value = new System.DateTime(2020, 4, 2, 0, 0, 0, 0);
            this.dtpBill.ValueChanged += new System.EventHandler(this.dtpBill_ValueChanged);
            this.dtpBill.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpBill_KeyDown);
            // 
            // txtPhone
            // 
            this.txtPhone.Location = new System.Drawing.Point(505, 58);
            this.txtPhone.Margin = new System.Windows.Forms.Padding(4);
            this.txtPhone.MaxLength = 10;
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(87, 22);
            this.txtPhone.TabIndex = 3;
            this.txtPhone.TextChanged += new System.EventHandler(this.txtPhone_TextChanged);
            this.txtPhone.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPhone_KeyPress);
            // 
            // rbInvoice
            // 
            this.rbInvoice.AutoSize = true;
            this.rbInvoice.Location = new System.Drawing.Point(914, 62);
            this.rbInvoice.Name = "rbInvoice";
            this.rbInvoice.Size = new System.Drawing.Size(68, 20);
            this.rbInvoice.TabIndex = 167;
            this.rbInvoice.Text = "Invoice";
            this.rbInvoice.UseVisualStyleBackColor = true;
            this.rbInvoice.CheckedChanged += new System.EventHandler(this.rbEstimate_CheckedChanged);
            // 
            // rbEstimate
            // 
            this.rbEstimate.AutoSize = true;
            this.rbEstimate.Location = new System.Drawing.Point(818, 61);
            this.rbEstimate.Name = "rbEstimate";
            this.rbEstimate.Size = new System.Drawing.Size(77, 20);
            this.rbEstimate.TabIndex = 166;
            this.rbEstimate.Text = "Estimate";
            this.rbEstimate.UseVisualStyleBackColor = true;
            this.rbEstimate.CheckedChanged += new System.EventHandler(this.rbEstimate_CheckedChanged);
            // 
            // label16
            // 
            this.label16.BackColor = System.Drawing.Color.SteelBlue;
            this.label16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label16.Dock = System.Windows.Forms.DockStyle.Top;
            this.label16.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label16.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.Color.White;
            this.label16.Location = new System.Drawing.Point(0, 0);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(1284, 38);
            this.label16.TabIndex = 211;
            this.label16.Text = "View Bills";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbCustomer
            // 
            this.cbCustomer.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbCustomer.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbCustomer.DisplayMember = "ItemTypeName";
            this.cbCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCustomer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbCustomer.FormattingEnabled = true;
            this.cbCustomer.Location = new System.Drawing.Point(692, 58);
            this.cbCustomer.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.cbCustomer.Name = "cbCustomer";
            this.cbCustomer.Size = new System.Drawing.Size(105, 24);
            this.cbCustomer.TabIndex = 212;
            this.cbCustomer.ValueMember = "ItemTypeId";
            this.cbCustomer.SelectedIndexChanged += new System.EventHandler(this.cbCustomer_SelectedIndexChanged);
            // 
            // txtCustomer
            // 
            this.txtCustomer.Location = new System.Drawing.Point(67, 84);
            this.txtCustomer.Margin = new System.Windows.Forms.Padding(4);
            this.txtCustomer.Name = "txtCustomer";
            this.txtCustomer.Size = new System.Drawing.Size(160, 22);
            this.txtCustomer.TabIndex = 214;
            this.txtCustomer.TextChanged += new System.EventHandler(this.txtBillId_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 84);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 16);
            this.label5.TabIndex = 213;
            this.label5.Text = "Name";
            // 
            // SlNo
            // 
            this.SlNo.DataPropertyName = "SlNo";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.SlNo.DefaultCellStyle = dataGridViewCellStyle1;
            this.SlNo.HeaderText = "Sl. No.";
            this.SlNo.MinimumWidth = 6;
            this.SlNo.Name = "SlNo";
            this.SlNo.ReadOnly = true;
            this.SlNo.Width = 80;
            // 
            // CustomerName
            // 
            this.CustomerName.DataPropertyName = "CustomerName";
            this.CustomerName.HeaderText = "Customer Name";
            this.CustomerName.MinimumWidth = 6;
            this.CustomerName.Name = "CustomerName";
            this.CustomerName.ReadOnly = true;
            this.CustomerName.Width = 200;
            // 
            // CustAddress
            // 
            this.CustAddress.DataPropertyName = "CustAddress";
            this.CustAddress.HeaderText = "Address";
            this.CustAddress.MinimumWidth = 6;
            this.CustAddress.Name = "CustAddress";
            this.CustAddress.ReadOnly = true;
            this.CustAddress.Width = 125;
            // 
            // CustomerType
            // 
            this.CustomerType.DataPropertyName = "CustomerType";
            this.CustomerType.HeaderText = "Customer Type";
            this.CustomerType.MinimumWidth = 6;
            this.CustomerType.Name = "CustomerType";
            this.CustomerType.ReadOnly = true;
            this.CustomerType.Width = 120;
            // 
            // BillId
            // 
            this.BillId.DataPropertyName = "BillId";
            this.BillId.HeaderText = "Bill Id";
            this.BillId.MinimumWidth = 6;
            this.BillId.Name = "BillId";
            this.BillId.ReadOnly = true;
            this.BillId.Width = 125;
            // 
            // BillNo
            // 
            this.BillNo.DataPropertyName = "BillNo";
            this.BillNo.HeaderText = "Bill No";
            this.BillNo.MinimumWidth = 6;
            this.BillNo.Name = "BillNo";
            this.BillNo.ReadOnly = true;
            this.BillNo.Width = 125;
            // 
            // InvoiceNo
            // 
            this.InvoiceNo.DataPropertyName = "InvoiceNo";
            this.InvoiceNo.HeaderText = "Invoice No";
            this.InvoiceNo.MinimumWidth = 6;
            this.InvoiceNo.Name = "InvoiceNo";
            this.InvoiceNo.ReadOnly = true;
            this.InvoiceNo.Width = 125;
            // 
            // BillDate
            // 
            this.BillDate.DataPropertyName = "BillDate";
            this.BillDate.HeaderText = "Bill Date";
            this.BillDate.MinimumWidth = 6;
            this.BillDate.Name = "BillDate";
            this.BillDate.ReadOnly = true;
            this.BillDate.Width = 125;
            // 
            // BillAmount
            // 
            this.BillAmount.DataPropertyName = "BillAmount";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.BillAmount.DefaultCellStyle = dataGridViewCellStyle2;
            this.BillAmount.HeaderText = "Bill Amount";
            this.BillAmount.MinimumWidth = 6;
            this.BillAmount.Name = "BillAmount";
            this.BillAmount.ReadOnly = true;
            this.BillAmount.Width = 125;
            // 
            // PhoneNo
            // 
            this.PhoneNo.DataPropertyName = "PhoneNo";
            this.PhoneNo.HeaderText = "Phone No";
            this.PhoneNo.MinimumWidth = 6;
            this.PhoneNo.Name = "PhoneNo";
            this.PhoneNo.ReadOnly = true;
            this.PhoneNo.Width = 125;
            // 
            // Estimate
            // 
            this.Estimate.DataPropertyName = "Estimate";
            this.Estimate.HeaderText = "Estimate";
            this.Estimate.MinimumWidth = 6;
            this.Estimate.Name = "Estimate";
            this.Estimate.ReadOnly = true;
            this.Estimate.Width = 80;
            // 
            // WayBill
            // 
            this.WayBill.DataPropertyName = "WayBill";
            this.WayBill.HeaderText = "Way Bill";
            this.WayBill.MinimumWidth = 6;
            this.WayBill.Name = "WayBill";
            this.WayBill.ReadOnly = true;
            this.WayBill.Width = 125;
            // 
            // Edit
            // 
            this.Edit.HeaderText = "Edit";
            this.Edit.MinimumWidth = 6;
            this.Edit.Name = "Edit";
            this.Edit.ReadOnly = true;
            this.Edit.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Edit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Edit.Text = "Edit";
            this.Edit.UseColumnTextForButtonValue = true;
            this.Edit.Width = 125;
            // 
            // Update
            // 
            this.Update.HeaderText = "Way Bill";
            this.Update.MinimumWidth = 6;
            this.Update.Name = "Update";
            this.Update.ReadOnly = true;
            this.Update.Text = "Update";
            this.Update.UseColumnTextForButtonValue = true;
            this.Update.Width = 125;
            // 
            // Print
            // 
            this.Print.HeaderText = "Print";
            this.Print.MinimumWidth = 6;
            this.Print.Name = "Print";
            this.Print.ReadOnly = true;
            this.Print.Text = "Print";
            this.Print.ToolTipText = "Print";
            this.Print.UseColumnTextForButtonValue = true;
            this.Print.Width = 125;
            // 
            // JSON
            // 
            this.JSON.HeaderText = "json";
            this.JSON.MinimumWidth = 6;
            this.JSON.Name = "JSON";
            this.JSON.ReadOnly = true;
            this.JSON.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.JSON.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.JSON.Text = "Get";
            this.JSON.ToolTipText = "Get";
            this.JSON.UseColumnTextForButtonValue = true;
            this.JSON.Width = 125;
            // 
            // Dispatch
            // 
            this.Dispatch.HeaderText = "Dispatch";
            this.Dispatch.Name = "Dispatch";
            this.Dispatch.ReadOnly = true;
            this.Dispatch.Text = "Dispatch";
            this.Dispatch.UseColumnTextForButtonValue = true;
            // 
            // IsInState
            // 
            this.IsInState.DataPropertyName = "IsInState";
            this.IsInState.HeaderText = "IsInState";
            this.IsInState.MinimumWidth = 6;
            this.IsInState.Name = "IsInState";
            this.IsInState.ReadOnly = true;
            this.IsInState.Visible = false;
            this.IsInState.Width = 125;
            // 
            // InvoiceStatus
            // 
            this.InvoiceStatus.DataPropertyName = "InvoiceStatus";
            this.InvoiceStatus.HeaderText = "InvoiceStatus";
            this.InvoiceStatus.MinimumWidth = 6;
            this.InvoiceStatus.Name = "InvoiceStatus";
            this.InvoiceStatus.ReadOnly = true;
            this.InvoiceStatus.Visible = false;
            this.InvoiceStatus.Width = 125;
            // 
            // SearchBills
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 527);
            this.Controls.Add(this.txtCustomer);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cbCustomer);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.rbInvoice);
            this.Controls.Add(this.rbEstimate);
            this.Controls.Add(this.dtpBill);
            this.Controls.Add(this.dgvBills);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtPhone);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtBillId);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SearchBills";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "View Bills";
            ((System.ComponentModel.ISupportInitialize)(this.dgvBills)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBillId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView dgvBills;
        private System.Windows.Forms.DateTimePicker dtpBill;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.RadioButton rbInvoice;
        private System.Windows.Forms.RadioButton rbEstimate;
        internal System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox cbCustomer;
        private System.Windows.Forms.TextBox txtCustomer;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridViewTextBoxColumn SlNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustAddress;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerType;
        private System.Windows.Forms.DataGridViewTextBoxColumn BillId;
        private System.Windows.Forms.DataGridViewTextBoxColumn BillNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn InvoiceNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn BillDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn BillAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn PhoneNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Estimate;
        private System.Windows.Forms.DataGridViewTextBoxColumn WayBill;
        private System.Windows.Forms.DataGridViewButtonColumn Edit;
        private System.Windows.Forms.DataGridViewButtonColumn Update;
        private System.Windows.Forms.DataGridViewButtonColumn Print;
        private System.Windows.Forms.DataGridViewButtonColumn JSON;
        private System.Windows.Forms.DataGridViewButtonColumn Dispatch;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsInState;
        private System.Windows.Forms.DataGridViewTextBoxColumn InvoiceStatus;
    }
}