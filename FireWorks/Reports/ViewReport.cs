using CrystalDecisions.Shared;
using FireWorksEntity;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FireWorks.Reports
{
    public partial class ViewReport : Form
    {
        public ViewReport(object rptSource, List<ReportParameters> parameters = null)
        {
            InitializeComponent();

            if (parameters != null && parameters.Count > 0)
            {
                ParameterFields paramFields = new ParameterFields();
                foreach (ReportParameters p in parameters)
                {
                    ParameterField paramField = new ParameterField();
                    ParameterDiscreteValue discreteVal = new ParameterDiscreteValue();
                    paramField.ParameterFieldName = p.ParamName;
                    discreteVal.Value = p.ParamValue;
                    paramField.CurrentValues.Add(discreteVal);
                    paramFields.Add(paramField);
                }
                crvReport.ParameterFieldInfo = paramFields;
            }

            crvReport.ReportSource = rptSource;
            //crvReport.RefreshReport();
        }
    }
}
