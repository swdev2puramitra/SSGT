using System;
using System.Data;
using System.Configuration;
using FireWorksDal.Constants;
using System.Collections.Generic;
using FireWorksEntity;

namespace FireWorksDal.DataObjects
{
    public class DBManager : IDBManager, IDisposable
    {
        /// <summary>
        /// constructor
        /// </summary>

        public DBManager()//default connection
        {
            // get the provicer type from the web.config file
            string typeOfProvider = ConfigurationManager.AppSettings["Sqlprovider"].ToString();
            //  get the provider type passed from the sql server
            SetProviderType(typeOfProvider);
            // get the connection string 
            //this.ConnectionString = ConfigurationManager.ConnectionStrings["FireWorks.Properties.Settings.FireworksConnectionString"].ConnectionString.ToString();
            string connection = ConfigurationManager.ConnectionStrings["Development.FireworksConnectionString"].ConnectionString.ToString();
            if (FireworksAppStatus.IsReleaseMode)
            {
                connection = ConfigurationManager.ConnectionStrings["Production.FireworksConnectionString"].ConnectionString.ToString();
            }
            this.ConnectionString = connection;
        }

        public DBManager(string connectionname)//connection with name
        {
            // get the provicer type from the web.config file
            string typeOfProvider = ConfigurationManager.AppSettings["Sqlprovider"].ToString();
            //  get the provider type passed from the sql server
            SetProviderType(typeOfProvider);
            // get the connection string 
            this.ConnectionString = ConfigurationManager.ConnectionStrings[connectionname].ConnectionString.ToString();
        }

        /// <summary>
        /// constructur with provider type
        /// </summary>
        /// <param name="providerType"></param>
        public DBManager(DataProvider providerType)
        {
            this.providerType = providerType;
            strConnection = ConfigurationManager.ConnectionStrings[DaoConstants.ConfigConnectionString].ConnectionString.ToString();
        }

        /// <summary>
        /// constructur with provider and connection string
        /// </summary>
        /// <param name="providerType"></param>
        /// <param name="connectionString"></param>
        public DBManager(DataProvider providerType, string connectionString)
        {
            this.providerType = providerType;
            this.strConnection = connectionString;
        }

        /// <summary>
        /// method to set the provider type for the application
        /// </summary>
        /// <param name="typeOfProvider"></param>
        void SetProviderType(string typeOfProvider)
        {
            switch (typeOfProvider)
            {
                case "SqlServer":
                    this.providerType = DataProvider.SqlServer;
                    break;
                case "OleDb":
                    this.providerType = DataProvider.OleDb;
                    break;
                case "Odbc":
                    this.providerType = DataProvider.Odbc;
                    break;
                case "MySql":
                    this.providerType = DataProvider.MySql;
                    break;
            }
        }

        #region "Global Decleration"

        private IDbConnection idbConnection;
        private IDataReader idataReader;
        private IDbCommand idbCommand;
        private DataProvider providerType;
        private IDbTransaction idbTransaction = null;
        private List<IDbDataParameter> idbParameters = null;
        private string strConnection;

        #endregion

        #region " Properties for global variables "

        public IDbConnection Connection
        {
            get
            {
                return idbConnection;
            }
        }

        public IDataReader DataReader
        {
            get
            {
                return idataReader;
            }
            set
            {
                idataReader = value;
            }
        }

        public DataProvider ProviderType
        {
            get
            {
                return providerType;
            }
            set
            {
                providerType = value;
            }
        }

        public string ConnectionString
        {
            get
            {
                return strConnection;
            }
            set
            {
                strConnection = value;
            }
        }

        public IDbCommand Command
        {
            get
            {
                return idbCommand;
            }
        }

        public IDbTransaction Transaction
        {
            get
            {
                return idbTransaction;
            }
        }

        public List<IDbDataParameter> Parameters
        {
            get
            {
                return idbParameters;
            }
        }

        #endregion

        /// <summary>
        /// mehod to open  the connection to the data base
        /// </summary>
        public void Open()
        {
            try
            {
                // calling method from DBManagerFactory class to get the provider type connection object
                idbConnection = DBManagerFactory.GetConnection(this.providerType);

                // passing connection string
                idbConnection.ConnectionString = this.ConnectionString;

                if (idbConnection.State == ConnectionState.Open)
                    idbConnection.Close();

                // chek for open connection state
                if (idbConnection.State != ConnectionState.Open)
                    idbConnection.Open();
                // calling method from DBManagerFactory class to get the provider type command object
                this.idbCommand = DBManagerFactory.GetCommand(this.ProviderType);
                this.idbTransaction = DBManagerFactory.GetTransaction(idbConnection, this.ProviderType);
            }
            catch (Exception ex)
            { throw ex; }
        }

        /// <summary>
        /// method to close the connection
        /// </summary>
        public void Close()
        {
            try
            {
                if (idbConnection.State != ConnectionState.Closed)
                    idbConnection.Close();
            }
            catch (Exception ex)
            { throw ex; }
        }

        /// <summary>
        /// method to dispose all the opened objects
        /// </summary>
        public void Dispose()
        {
            try
            {
                GC.SuppressFinalize(this);
                this.Close();
                this.idbCommand = null;
                this.idbTransaction = null;
                this.idbConnection = null;
            }
            catch (Exception ex)
            { throw ex; }
        }

        /// <summary>
        /// method to add the parameters to the specified provider type command object
        /// </summary>
        /// <param name="idbParameters"></param>
        public void CreateParameters()
        {
            try
            {
                // method to get the list of parametes added to the parameter object array
                idbParameters = new List<IDbDataParameter>();
            }
            catch (Exception ex)
            { throw ex; }
        }

        /// <summary>
        /// add the parameters 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="paramName"></param>
        /// <param name="objValue"></param>
        public void AddParameters(string paramName, object objValue, ParameterDirection paramDirection, int size)
        {
            try
            {
                IDbDataParameter parameter = null;
                parameter = DBManagerFactory.GetParametersType(this.ProviderType, parameter);
                parameter.ParameterName = paramName;
                parameter.Value = objValue;
                parameter.Direction = paramDirection;
                if (size > 0)
                    parameter.Size = size;                
                idbParameters.Add(parameter);
            }
            catch (Exception ex)
            { throw ex; }
        }



        /// <summary>
        /// method to begin the transaction
        /// </summary>
        public void BeginTransaction()
        {
            try
            {
                if (this.idbTransaction == null)
                    // calling method from DBManagerFactory to open the transaction of specific provider
                    idbTransaction = DBManagerFactory.GetTransaction(this.idbConnection, this.ProviderType);
                // assign the transaction to the command object
                this.idbCommand.Transaction = idbTransaction;
            }
            catch (Exception ex)
            { throw ex; }
        }

        /// <summary>
        ///  method to commit transaction
        /// </summary>
        public void CommitTransaction()
        {
            try
            {
                if (this.idbTransaction != null)
                    this.idbTransaction.Commit();
                idbTransaction = null;
            }
            catch (Exception ex)
            { throw ex; }
        }

        /// <summary>
        /// method to roll back the transcation when any error occurs
        /// </summary>
        public void RollBackTransaction()
        {
            if (this.idbTransaction != null)
                this.idbTransaction.Rollback();
            idbTransaction = null;
        }

        /// <summary>
        /// method to execute the reader of the specific provider
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(CommandType commandType, string commandText)
        {
            try
            {
                // get the command object from DBManagerFactory by the provider type
                this.idbCommand = DBManagerFactory.GetCommand(this.ProviderType);
                // assingn the connection property to thcommand object
                idbCommand.Connection = this.Connection;

                // prepare the command object with the parameters
                PrepareCommand(idbCommand, this.Connection, this.Transaction,
                 commandType, commandText, this.Parameters);

                // assign the datareader
                this.DataReader = idbCommand.ExecuteReader();
                // clear the paramerts
                idbCommand.Parameters.Clear();
                return this.DataReader;
            }
            catch (Exception ex)
            { throw ex; }
        }

        /// <summary>
        /// method to close the data reader
        /// </summary>
        public void CloseReader()
        {
            try
            {
                if (this.DataReader != null)
                    this.DataReader.Close();
            }
            catch (Exception ex)
            { throw ex; }
        }

        /// <summary>
        /// method to attach the parameters to the command object
        /// </summary>
        /// <param name="command"></param>
        /// <param name="commandParameters"></param>
        private void AttachParameters(IDbCommand command, List<IDbDataParameter> commandParameters)
        {
            try
            {
                foreach (IDbDataParameter idbParameter in commandParameters)
                {
                    if ((idbParameter.Direction == ParameterDirection.InputOutput) && (idbParameter.Value == null))
                    {
                        idbParameter.Value = DBNull.Value;
                    }
                    command.Parameters.Add(idbParameter);
                }
            }
            catch (Exception ex)
            { throw ex; }
        }

        /// <summary>
        /// method to prepare the command object with the parameters passed to it
        /// </summary>
        /// <param name="command"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        private void PrepareCommand(IDbCommand command, IDbConnection connection,
                                    IDbTransaction transaction, CommandType commandType,
                                    string commandText, List<IDbDataParameter> commandParameters)
        {
            try
            {
                // assign properties
                command.Connection = connection;
                command.CommandText = commandText;
                command.CommandType = commandType;

                // check for transaction null
                if (transaction != null)
                {
                    //  assign transaction to command object
                    command.Transaction = transaction;
                }
                // check for parameters null
                if (commandParameters != null)
                {
                    // attach the parameters to the command object
                    AttachParameters(command, commandParameters);
                }
            }
            catch (Exception ex)
            { throw ex; }
        }

        /// <summary>
        /// method to execute the non query
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            try
            {
                // get the command object from DBManagerFactory
                this.idbCommand = DBManagerFactory.GetCommand(this.ProviderType);

                // prepare the command object
                PrepareCommand(idbCommand, this.Connection, this.Transaction, commandType,
                    commandText, this.Parameters);
                // get the executed value
                int returnValue = idbCommand.ExecuteNonQuery();

                // clear the command parameters
                idbCommand.Parameters.Clear();
                return returnValue;
            }
            catch (Exception ex)
            { throw ex; }
        }

        /// <summary>
        /// method to execute thte scalar property of the command object
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public object ExecuteScalar(CommandType commandType, string commandText)
        {
            try
            {
                // get the command object for DBManagerFactory
                this.idbCommand = DBManagerFactory.GetCommand(this.ProviderType);

                // prepare the command object
                PrepareCommand(idbCommand, this.Connection, this.Transaction, commandType,
                  commandText, this.Parameters);

                // get the scalar return value to object
                object returnValue = idbCommand.ExecuteScalar();
                // clear the paerameters
                idbCommand.Parameters.Clear();
                return returnValue;
            }
            catch (Exception ex)
            { throw ex; }
        }

        /// <summary>
        /// method to execute the dataset property of the command object
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(CommandType commandType, string commandText)
        {
            try
            {
                // get the command object from the DBManagerFactory
                this.idbCommand = DBManagerFactory.GetCommand(this.ProviderType);

                // method to prepare the oommand object
                PrepareCommand(idbCommand, this.Connection, this.Transaction, commandType, commandText, this.Parameters);
                // get the data adapter from DBManagerFactory
                IDbDataAdapter dataAdapter = DBManagerFactory.GetDataAdapter(this.ProviderType);
                // assign the selected command property
                dataAdapter.SelectCommand = idbCommand;

                DataSet dataSet = new DataSet();
                // fill the dataset
                dataAdapter.Fill(dataSet);
                // clear the command parameters
                idbCommand.Parameters.Clear();
                return dataSet;
            }
            catch (Exception ex)
            { throw ex; }
        }
    }
}
