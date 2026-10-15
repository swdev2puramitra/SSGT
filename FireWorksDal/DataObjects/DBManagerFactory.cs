using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace FireWorksDal.DataObjects
{
    public sealed class DBManagerFactory
    {
        /// <summary>
        /// constructor
        /// </summary>
        private DBManagerFactory() { }

        /// <summary>
        /// method to 
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public static IDbConnection GetConnection(DataProvider providerType)
        {
            try
            {
                IDbConnection iDbConnection = null;
                switch (providerType)
                {
                    case DataProvider.SqlServer:
                        iDbConnection = new SqlConnection();
                        break;
                    case DataProvider.OleDb:
                        iDbConnection = new OleDbConnection();
                        break;
                    case DataProvider.Odbc:
                        iDbConnection = new OdbcConnection();
                        break;
                    //case DataProvider.MySql:
                    //    iDbConnection = new MySqlConnection();
                    //    break;
                    default:
                        return null;
                }
                return iDbConnection;
            }
            catch (Exception ex)
            { throw new Exception("Could not establish connection.  " + ex.Message); }
        }

        /// <summary>
        /// method to get the command object
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public static IDbCommand GetCommand(DataProvider providerType)
        {
            try
            {
                switch (providerType)
                {
                    case DataProvider.SqlServer:
                        return new SqlCommand();
                    case DataProvider.OleDb:
                        return new OleDbCommand();
                    case DataProvider.Odbc:
                        return new OdbcCommand();
                    //case DataProvider.MySql:
                    //    return new MySqlCommand();
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            { throw new Exception("Error in initilizing the command object. " + ex.Message); }
        }

        /// <summary>
        /// method to get the data adapter
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public static IDbDataAdapter GetDataAdapter(DataProvider providerType)
        {
            try
            {
                switch (providerType)
                {
                    case DataProvider.SqlServer:
                        return new SqlDataAdapter();
                    case DataProvider.OleDb:
                        return new OleDbDataAdapter();
                    case DataProvider.Odbc:
                        return new OdbcDataAdapter();
                    //case DataProvider.MySql:
                    //    return new MySqlDataAdapter();
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            { throw new Exception("Error in getdataadapter. " + ex.Message); }
        }

        /// <summary>
        /// method to get the transaction
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public static IDbTransaction GetTransaction(IDbConnection iDbConnection, DataProvider providerType)
        {
            try
            {
                IDbTransaction iDbTransaction = iDbConnection.BeginTransaction();
                return iDbTransaction;
            }
            catch (Exception ex)
            { throw new Exception("Error in getting parameter. " + ex.Message); }
        }

        /// <summary>
        /// method to get the parameter
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public static IDataParameter GetParameter(DataProvider providerType)
        {
            IDataParameter iDataParameter = null;
            try
            {
                switch (providerType)
                {
                    case DataProvider.SqlServer:
                        iDataParameter = new SqlParameter();
                        break;
                    case DataProvider.OleDb:
                        iDataParameter = new OleDbParameter();
                        break;
                    case DataProvider.Odbc:
                        iDataParameter = new OdbcParameter();
                        break;
                        //case DataProvider.MySql:
                        //    iDataParameter = new MySqlParameter();
                        //    break;

                }
                return iDataParameter;
            }
            catch (Exception ex)
            { throw new Exception("Error in getting parameter. " + ex.Message); }
        }

        /// <summary>
        /// method to get the array of parameters
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public static List<IDbDataParameter> GetParameters(DataProvider providerType, List<IDbDataParameter> idbParams)
        {
            try
            {
                switch (providerType)
                {
                    case DataProvider.SqlServer:
                        for (int i = 0; i < idbParams.Count; ++i)
                        {
                            idbParams[i] = new SqlParameter();
                        }
                        break;
                    case DataProvider.OleDb:
                        for (int i = 0; i < idbParams.Count; ++i)
                        {
                            idbParams[i] = new OleDbParameter();
                        }
                        break;
                    case DataProvider.Odbc:
                        for (int i = 0; i < idbParams.Count; ++i)
                        {
                            idbParams[i] = new OdbcParameter();
                        }
                        break;
                    //case DataProvider.MySql:
                    //    for (int i = 0; i < paramsCount; ++i)
                    //    {
                    //        idbParams[i] = new MySqlParameter();
                    //    }
                    //    break;
                    default:
                        idbParams = null;
                        break;
                }
                return idbParams;
            }
            catch (Exception ex)
            { throw new Exception("Error in getting parameters. " + ex.Message); }
        }


        /// <summary>
        /// method to get the array of parameters
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public static IDbDataParameter GetParametersType(DataProvider providerType, IDbDataParameter idbParams)
        {
            try
            {
                switch (providerType)
                {
                    case DataProvider.SqlServer:
                        idbParams = new SqlParameter();
                        break;
                    case DataProvider.OleDb:

                        idbParams = new OleDbParameter();

                        break;
                    case DataProvider.Odbc:

                        idbParams = new OdbcParameter();

                        break;
                    //case DataProvider.MySql:
                    //    for (int i = 0; i < paramsCount; ++i)
                    //    {
                    //        idbParams[i] = new MySqlParameter();
                    //    }
                    //    break;
                    default:
                        idbParams = null;
                        break;
                }
                return idbParams;
            }
            catch (Exception ex)
            { throw new Exception("Error in getting parameters. " + ex.Message); }
        }
    }
}
