namespace FireWorks.Masters
{
    partial class EditStock
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
            this.label4 = new System.Windows.Forms.Label();
            this.dgvStockList = new System.Windows.Forms.DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            this.txtInvoice = new System.Windows.Forms.TextBox();
            this.cbStockFrom = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.StockId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StockFrom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GodownDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReceivedDt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InvoiceNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ArrivalDt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GodownId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Edit = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Delete = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStockList)).BeginInit();
            this.SuspendLayout();
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
            this.label4.Size = new System.Drawing.Size(1076, 28);
            this.label4.TabIndex = 156;
            this.label4.Text = "Edit Stock";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dgvStockList
            // 
            this.dgvStockList.AllowUserToAddRows = false;
            this.dgvStockList.AllowUserToDeleteRows = false;
            this.dgvStockList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStockList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.StockId,
            this.StockFrom,
            this.GodownDescription,
            this.ReceivedDt,
            this.InvoiceNo,
            this.ArrivalDt,
            this.Comments,
            this.GodownId,
            this.Edit,
            this.Delete});
            this.dgvStockList.Location = new System.Drawing.Point(14, 71);
            this.dgvStockList.Name = "dgvStockList";
            this.dgvStockList.ReadOnly = true;
            this.dgvStockList.Size = new System.Drawing.Size(1041, 345);
            this.dgvStockList.TabIndex = 157;
            this.dgvStockList.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvStockList_CellDoubleClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 41);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 16);
            this.label3.TabIndex = 169;
            this.label3.Text = "Invoice No. ";
            // 
            // txtInvoice
            // 
            this.txtInvoice.Location = new System.Drawing.Point(110, 38);
            this.txtInvoice.Margin = new System.Windows.Forms.Padding(5);
            this.txtInvoice.Name = "txtInvoice";
            this.txtInvoice.Size = new System.Drawing.Size(221, 22);
            this.txtInvoice.TabIndex = 168;
            this.txtInvoice.TextChanged += new System.EventHandler(this.txtInvoice_TextChanged);
            // 
            // cbStockFrom
            // 
            this.cbStockFrom.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbStockFrom.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbStockFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStockFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbStockFrom.FormattingEnabled = true;
            this.cbStockFrom.Location = new System.Drawing.Point(456, 38);
            this.cbStockFrom.Margin = new System.Windows.Forms.Padding(5);
            this.cbStockFrom.Name = "cbStockFrom";
            this.cbStockFrom.Size = new System.Drawing.Size(159, 24);
            this.cbStockFrom.TabIndex = 173;
            this.cbStockFrom.SelectedIndexChanged += new System.EventHandler(this.cbStockFrom_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(369, 41);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 16);
            this.label2.TabIndex = 172;
            this.label2.Text = "Stock From";
            // 
            // StockId
            // 
            this.StockId.DataPropertyName = "StockId";
            this.StockId.HeaderText = "StockId";
            this.StockId.Name = "StockId";
            this.StockId.ReadOnly = true;
            this.StockId.Width = 50;
            // 
            // StockFrom
            // 
            this.StockFrom.DataPropertyName = "StockFrom";
            this.StockFrom.HeaderText = "Stock From";
            this.StockFrom.Name = "StockFrom";
            this.StockFrom.ReadOnly = true;
            this.StockFrom.Width = 250;
            // 
            // GodownDescription
            // 
            this.GodownDescription.DataPropertyName = "GodownDescription";
            this.GodownDescription.HeaderText = "Godown";
            this.GodownDescription.Name = "GodownDescription";
            this.GodownDescription.ReadOnly = true;
            this.GodownDescription.Width = 80;
            // 
            // ReceivedDt
            // 
            this.ReceivedDt.DataPropertyName = "CreatedDate";
            dataGridViewCellStyle1.Format = "f";
            dataGridViewCellStyle1.NullValue = null;
            this.ReceivedDt.DefaultCellStyle = dataGridViewCellStyle1;
            this.ReceivedDt.HeaderText = "Received Date";
            this.ReceivedDt.Name = "ReceivedDt";
            this.ReceivedDt.ReadOnly = true;
            this.ReceivedDt.Width = 90;
            // 
            // InvoiceNo
            // 
            this.InvoiceNo.DataPropertyName = "InvoiceNo";
            this.InvoiceNo.HeaderText = "Invoice No";
            this.InvoiceNo.Name = "InvoiceNo";
            this.InvoiceNo.ReadOnly = true;
            this.InvoiceNo.Width = 90;
            // 
            // ArrivalDt
            // 
            this.ArrivalDt.DataPropertyName = "ArrivalDt";
            this.ArrivalDt.HeaderText = "Arrival Date";
            this.ArrivalDt.Name = "ArrivalDt";
            this.ArrivalDt.ReadOnly = true;
            this.ArrivalDt.Width = 110;
            // 
            // Comments
            // 
            this.Comments.DataPropertyName = "Comments";
            this.Comments.HeaderText = "Comments";
            this.Comments.Name = "Comments";
            this.Comments.ReadOnly = true;
            this.Comments.Width = 170;
            // 
            // GodownId
            // 
            this.GodownId.DataPropertyName = "GodownId";
            this.GodownId.HeaderText = "Godown Id";
            this.GodownId.Name = "GodownId";
            this.GodownId.ReadOnly = true;
            // 
            // Edit
            // 
            this.Edit.HeaderText = "Edit";
            this.Edit.Name = "Edit";
            this.Edit.ReadOnly = true;
            this.Edit.Text = "Edit";
            this.Edit.UseColumnTextForButtonValue = true;
            this.Edit.Width = 80;
            // 
            // Delete
            // 
            this.Delete.HeaderText = "Delete";
            this.Delete.Name = "Delete";
            this.Delete.ReadOnly = true;
            this.Delete.Text = "Delete";
            this.Delete.UseColumnTextForButtonValue = true;
            this.Delete.Width = 80;
            // 
            // EditStock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1076, 435);
            this.Controls.Add(this.cbStockFrom);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtInvoice);
            this.Controls.Add(this.dgvStockList);
            this.Controls.Add(this.label4);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "EditStock";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Stock";
            ((System.ComponentModel.ISupportInitialize)(this.dgvStockList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView dgvStockList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtInvoice;
        private System.Windows.Forms.ComboBox cbStockFrom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn StockId;
        private System.Windows.Forms.DataGridViewTextBoxColumn StockFrom;
        private System.Windows.Forms.DataGridViewTextBoxColumn GodownDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReceivedDt;
        private System.Windows.Forms.DataGridViewTextBoxColumn InvoiceNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ArrivalDt;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comments;
        private System.Windows.Forms.DataGridViewTextBoxColumn GodownId;
        private System.Windows.Forms.DataGridViewButtonColumn Edit;
        private System.Windows.Forms.DataGridViewButtonColumn Delete;
    }
}