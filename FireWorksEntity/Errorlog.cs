using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace FireWorksEntity
{
    public class Errorlog
    {
        public static void WriteToErrorLog(Exception ex, int? UserId, string PrimaryKeyDetails, string Methodname) 
        {
            try
            {
                if (ex.Message.Trim() == "Thread was being aborted.".Trim())
                    return;
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["FireWorks.Properties.Settings.FireworksConnectionString"].ToString());
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.Parameters.Add("@UserID", SqlDbType.VarChar).Value = UserId;
                cmd.Parameters.Add("@Error", SqlDbType.VarChar).Value = ex.Message;
                cmd.Parameters.Add("@Methodname", SqlDbType.Text).Value = Methodname;
                if (!String.IsNullOrEmpty(PrimaryKeyDetails.Trim()))
                    cmd.Parameters.Add("@PrimaryKeyDetails", SqlDbType.Text).Value = PrimaryKeyDetails;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "USP_ErrorLog_Insert";

                if (con.State == ConnectionState.Closed)
                    con.Open();
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                con.Close();
            }
            catch
            {

            }
        }
    }
}
