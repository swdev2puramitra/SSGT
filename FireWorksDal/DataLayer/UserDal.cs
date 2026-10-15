using FireWorksDal.Constants;
using FireWorksDal.DataObjects;
using FireWorksEntity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FireWorksDal.DataLayer
{
    public class UserDal : CommonMethods
    {
        IDBManager dbManager = null;
        public UserDal()
        {
            if (dbManager == null)
                dbManager = new DBManager();
        }

        public UserEntity UserVerify(string UserName)
        {
            try
            {
                dbManager.Open();
                dbManager.CreateParameters();
                dbManager.AddParameters("UserName", GetDBNullOrValue(UserName), ParameterDirection.Input, DaoConstants.InParamSize);
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_VALIDATE_USER");
                UserEntity user = (from DataRow dr in ds.Tables[0].Rows
                                   select new UserEntity()
                                   {
                                       UserID = dr["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["UserID"]),
                                       UserName = dr["UserName"] == DBNull.Value ? null : dr["UserName"].ToString(),
                                       UserPassword = dr["UserPassword"] == DBNull.Value ? null : dr["UserPassword"].ToString(),
                                       ContactNo = dr["ContactNo"] == DBNull.Value ? null : dr["ContactNo"].ToString(),
                                       RoleId = dr["RoleId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["RoleId"]),
                                       ExpiryDate = dr["PayStatus"] == DBNull.Value ? DateTime.Now.AddDays(-1) : Convert.ToDateTime(dr["PayStatus"], new System.Globalization.CultureInfo("fr-FR", true))
                                   }).FirstOrDefault();
                return user;

            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_VALIDATE_USER", "Class : UserDal, Method : UserVerify");
                return null;
            }
            finally
            {
                dbManager.Dispose();
            }
        }

        public void AllowUser(int IsExtend, DateTime expiryDate, AlertMsgInfo alertMsgInfo)
        {
            try
            {
                dbManager.CreateParameters();
                dbManager.AddParameters("IsExtend", GetDBNullOrValue(IsExtend), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("ExpiryDate", GetDBNullOrValue(expiryDate), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Result", null, ParameterDirection.Output, DaoConstants.OutParamSize);

                dbManager.Open();
                dbManager.BeginTransaction();
                dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_DML_UserAllow");

                if (Convert.ToInt32(dbManager.Parameters[2].Value) > 0)
                {
                    dbManager.CommitTransaction();

                    alertMsgInfo.Message = "Validity extended successfully. Please login again";
                    alertMsgInfo.SaveStatus = true;
                }
                else
                {
                    dbManager.RollBackTransaction();
                    alertMsgInfo.SaveStatus = false;
                    alertMsgInfo.Message = "Error occured. Please try again.";
                }
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_DML_UserAllow", "Class : UserDal, Method : AllowUser");
                alertMsgInfo.SaveStatus = false;
                alertMsgInfo.Message = "Error occured. Please try again.";
            }
            finally
            {
                dbManager.Close();
            }
        }

        public List<UserEntity> GetUsers()
        {
            try
            {
                dbManager.Open();
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_Users");
                if (ds.Tables.Count > 0)
                {
                    List<UserEntity> user = (from DataRow dr in ds.Tables[0].Rows
                                             select new UserEntity()
                                             {
                                                 UserID = dr["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["UserID"]),
                                                 UserName = dr["UserName"] == DBNull.Value ? null : dr["UserName"].ToString(),
                                                 ContactNo = dr["ContactNo"] == DBNull.Value ? null : dr["ContactNo"].ToString(),
                                                 RoleId = dr["RoleID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["RoleID"]),
                                                 RoleName = dr["RoleName"] == DBNull.Value ? null : dr["RoleName"].ToString(),
                                                 IsActive = dr["IsActive"] == DBNull.Value ? false : Convert.ToBoolean(dr["IsActive"])
                                             }).ToList();
                    return user;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_Users", "Class : UserDal, Method : GetUsers");
                return null;
            }
            finally
            {
                dbManager.Dispose();
            }
        }

        public void SaveUsers(UserEntity user, AlertMsgInfo alertMsgInfo)
        {
            try
            {
                dbManager.CreateParameters();
                dbManager.AddParameters("UserID", GetDBNullOrValue(user.UserID), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("UserName", GetDBNullOrValue(user.UserName), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("ContactNo", GetDBNullOrValue(user.ContactNo), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("RoleID", GetDBNullOrValue(user.RoleId), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("IsActive", GetDBNullOrValue(user.IsActive), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("DMLTYPE", GetDBNullOrValue(user.DMLType), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Result", null, ParameterDirection.Output, DaoConstants.OutParamSize);
                dbManager.AddParameters("CreatedBy", GetDBNullOrValue(UserSessionEntity.UserID), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("UserPwd", GetDBNullOrValue(user.UserPassword), ParameterDirection.Input, DaoConstants.InParamSize);

                dbManager.Open();
                dbManager.BeginTransaction();
                dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_DML_User");
                if (Convert.ToInt32(dbManager.Parameters[6].Value) == -1)
                {
                    dbManager.RollBackTransaction();
                    alertMsgInfo.Message = "User already exists";
                    alertMsgInfo.SaveStatus = false;
                }

                else if (Convert.ToInt32(dbManager.Parameters[6].Value) > 0)
                {
                    dbManager.CommitTransaction();
                    if (user.DMLType == 'I')
                        alertMsgInfo.Message = "Saved successfully.";
                    else
                        alertMsgInfo.Message = "Updated successfully.";
                    alertMsgInfo.SaveStatus = true;
                }
                else
                {
                    dbManager.RollBackTransaction();
                    alertMsgInfo.SaveStatus = false;
                    alertMsgInfo.Message = "Error occured. Please try again.";
                }
            }
            catch (Exception ex)
            {
                dbManager.RollBackTransaction();
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_DML_User", "Class : UserDal, Method : SaveUsers");
                alertMsgInfo.SaveStatus = false;
                alertMsgInfo.Message = "Error occured. Please try again.";
            }
            finally
            {
                dbManager.Close();
            }
        }

        public DataTable GetMenus()
        {
            try
            {
                dbManager.Open();
                dbManager.CreateParameters();
                dbManager.AddParameters("UserLevel", GetDBNullOrValue(UserSessionEntity.RoleId), ParameterDirection.Input, DaoConstants.InParamSize);
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_Menudetails");
                if (ds.Tables.Count > 0)
                {
                    return ds.Tables[0];
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_Menudetails", "Class : UserDal, Method : GetMenus");
                return null;
            }
            finally
            {
                dbManager.Dispose();
            }
        }
        public void FillShopDetails()
        {
            try
            {
                dbManager.Open();
                dbManager.CreateParameters();
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_ShotDetails");
                ShopDetails.dtShopDetails = ds.Tables[0];

                var row = ShopDetails.dtShopDetails.AsEnumerable().FirstOrDefault();
                if (row != null)
                {
                    ShopDetails.ShopName = row.Field<string>("ShopName");
                    ShopDetails.Address = row.Field<string>("Address");
                    ShopDetails.GST = row.Field<string>("GST");
                    ShopDetails.PhoneNo = row.Field<string>("PhoneNo");
                    ShopDetails.LicenceNo = row.Field<string>("LicenceNo");
                    ShopDetails.BankName = row.Field<string>("BankName");
                    ShopDetails.BranchName = row.Field<string>("BranchName");
                    ShopDetails.AccountNo = row.Field<string>("AccountNo");
                    ShopDetails.IFSC = row.Field<string>("IFSC");
                }

            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_VALIDATE_USER", "Class : UserDal, Method : UserVerify");
            }
            finally
            {
                dbManager.Dispose();
            }
        }
    }
}
