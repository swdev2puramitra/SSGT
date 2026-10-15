using CrystalDecisions.Shared;
using FireWorksDal.DataLayer;
using System;
using System.Data;
using System.Windows.Forms;

namespace FireWorks.Reports
{
    public partial class HSNReport : Form
    {
        public HSNReport()
        {
            InitializeComponent();
            dtpFrom.MaxDate = DateTime.Today;
            dtpTo.MaxDate = DateTime.Today;
            dtpFrom.Value = DateTime.Today;
            dtpTo.Value = DateTime.Today;
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            HSN hsn = new HSN();
            BillDal billDal = new BillDal();
            DataSet dataSet = billDal.GetHSNRpt(dtpFrom.Value, dtpTo.Value);
            hsn.Database.Tables["GET_RPT_HSN"].SetDataSource(dataSet.Tables[0]);
            hsn.PrintOptions.PaperOrientation = PaperOrientation.Portrait;
            hsn.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;

            ParameterFields paramFields = new ParameterFields();
            ParameterField paramField = new ParameterField();
            ParameterDiscreteValue discreteVal = new ParameterDiscreteValue();
            paramField.ParameterFieldName = "From";

            // Set the discrete value and pass it to the parameter
            discreteVal.Value = dtpFrom.Value;
            paramField.CurrentValues.Add(discreteVal);

            // Add parameter to the parameter fields collection.
            paramFields.Add(paramField);


            ParameterField paramField1 = new ParameterField();
            ParameterDiscreteValue discreteVal1 = new ParameterDiscreteValue();
            paramField1.ParameterFieldName = "To";

            // Set the discrete value and pass it to the parameter
            discreteVal1.Value = dtpTo.Value;
            paramField1.CurrentValues.Add(discreteVal1);
            paramFields.Add(paramField1);

            crvReport.ParameterFieldInfo = paramFields;
            crvReport.ReportSource = hsn;
            crvReport.RefreshReport();
        }
    }
}
