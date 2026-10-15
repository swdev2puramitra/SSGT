using System.Collections.Generic;
using System.Data;

namespace FireWorksDal.DataObjects
{
    public enum DataProvider
    {
        SqlServer, OleDb, Odbc, MySql, Oracle
    }
    public interface IDBManager
    {
        DataProvider ProviderType
        {
            get;
            set;
        }

        string ConnectionString
        {
            get;
            set;
        }

        IDbConnection Connection
        {
            get;
        }

        IDbTransaction Transaction
        {
            get;
        }

        IDataReader DataReader
        {
            get;
        }

        IDbCommand Command
        {
            get;
        }

        List<IDbDataParameter> Parameters
        {
            get;
        }





        /// <summary>
        ///  method to open the connection
        /// </summary>
        void Open();

        /// <summary>
        /// method to begin transaction
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// method to commit transaction
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// method to create parameters
        /// </summary>
        /// <param name="idbParameters"></param>
        void CreateParameters();

        /// <summary>
        /// method to roll back the transcation when any error occurs
        /// </summary>
        void RollBackTransaction();

        /// <summary>
        /// method to add parameters
        /// </summary>
        /// <param name="index"></param>
        /// <param name="paramName"></param>
        /// <param name="objValue"></param>
        void AddParameters(string paramName, object objValue, ParameterDirection paramDirection, int size);

        /// <summary>
        /// method to execute reader
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        IDataReader ExecuteReader(CommandType commandType, string commandText);

        /// <summary>
        /// method to execute dataset
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        DataSet ExecuteDataSet(CommandType commandType, string commandText);

        /// <summary>
        /// method to execute scalar
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        object ExecuteScalar(CommandType commandType, string commandText);

        /// <summary>
        /// method to execute nonquery
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        int ExecuteNonQuery(CommandType commandType, string commandText);

        /// <summary>
        /// method to close reader
        /// </summary>
        void CloseReader();

        /// <summary>
        /// method to close connection
        /// </summary>
        void Close();

        /// <summary>
        /// method to dispose the opened objects
        /// </summary>
        void Dispose();
    }
}
