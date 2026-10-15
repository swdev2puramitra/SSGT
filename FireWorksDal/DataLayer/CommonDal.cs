using FireWorksDal.Constants;
using FireWorksDal.DataObjects;
using FireWorksEntity;
using FireWorksEntity.Masters;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FireWorksDal.DataLayer
{
    public class CommonDal : CommonMethods //ICommon
    {
        IDBManager dbManager = null;
        public CommonDal()
        {
            if (dbManager == null)
                dbManager = new DBManager();
        }

        // Backward-compatible overload: return all items (no paging)
        public DataTable GetItemsList(bool Status)
        {
            // request with PageNumber=1 and PageSize=0 to indicate no paging (SP should handle PageSize=0 as return all)
            return GetItemsList(Status, 1, 0, true);
        }

        // Holds the total records returned by last GetItemsList call (if stored proc provides it)
        public int LastGetItemsTotalRecords { get; private set; }

        public DataTable GetCategories(bool? IsActive)
        {
            try
            {
                dbManager.CreateParameters();
                dbManager.AddParameters("IsActive", GetDBNullOrValue(IsActive), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.Open();
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_CATEGORY");
                if (ds == null) return null;
                if (ds.Tables.Count > 0)
                {
                    return ds.Tables[0];
                }
                return null;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_CATEGORY", "Class : CommonDal, Method : GetCategories");
                return null;
            }
            finally
            {
                dbManager.Dispose();
                //dbManager = null;
            }
        }

        public void SaveCategories(CategoryEntity category, AlertMsgInfo alertMsgInfo)
        {
            try
            {
                dbManager.CreateParameters();
                if (category.CategoryId != null)
                    dbManager.AddParameters("CatId", GetDBNullOrValue(category.CategoryId), ParameterDirection.Input, DaoConstants.InParamSize);
                else
                    dbManager.AddParameters("CatId", null, ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("CatDescription", GetDBNullOrValue(category.CategoryName), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("DMLTYPE", GetDBNullOrValue(category.DMLType), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Result", null, ParameterDirection.Output, DaoConstants.OutParamSize);
                dbManager.AddParameters("CreatedBy", GetDBNullOrValue(UserSessionEntity.UserID), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("IsActive", GetDBNullOrValue(category.IsActive), ParameterDirection.Input, DaoConstants.InParamSize);

                dbManager.Open();
                dbManager.BeginTransaction();
                dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_DML_CATEGORY");
                if (Convert.ToInt32(dbManager.Parameters[3].Value) == -1)
                {
                    dbManager.RollBackTransaction();
                    alertMsgInfo.Message = "Category already exists";
                    alertMsgInfo.SaveStatus = false;
                }

                else if (Convert.ToInt32(dbManager.Parameters[3].Value) > 0)
                {
                    dbManager.CommitTransaction();
                    if (category.DMLType == 'I')
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
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_DML_CATEGORY", "Class : CommonDal, Method : SaveCategories");
                alertMsgInfo.SaveStatus = false;
                alertMsgInfo.Message = "Error occured. Please try again.";
            }
            finally
            {
                dbManager.Close();
            }
        }

        public DataTable GetSubCategories(bool? IsActive, int? categoryId)
        {
            try
            {
                dbManager.CreateParameters();
                dbManager.AddParameters("IsActive", GetDBNullOrValue(IsActive), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("categoryId", GetDBNullOrValue(categoryId), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.Open();
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_SUBCATEGORY");
                if (ds == null) return null;
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];
                return null;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_SUBCATEGORY", "Class : CommonDal, Method : GetSubCategories");
                return null;
            }
            finally
            {
                dbManager.Dispose();
                //dbManager = null;
            }
        }

        public void SaveSubCategories(SubCategoryEntity category, AlertMsgInfo alertMsgInfo)
        {
            try
            {
                dbManager.CreateParameters();
                if (category.SubCategoryId != null)
                    dbManager.AddParameters("SubCatId", GetDBNullOrValue(category.SubCategoryId), ParameterDirection.Input, DaoConstants.InParamSize);
                else
                    dbManager.AddParameters("SubCatId", null, ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("SubCatDescription", GetDBNullOrValue(category.SubCategoryName), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("DMLTYPE", GetDBNullOrValue(category.DMLType), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("CreatedBy", GetDBNullOrValue(UserSessionEntity.UserID), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Result", null, ParameterDirection.Output, DaoConstants.OutParamSize);
                dbManager.AddParameters("IsActive", GetDBNullOrValue(category.IsActive), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("CategoryId", GetDBNullOrValue(category.CategoryId), ParameterDirection.Input, DaoConstants.InParamSize);

                dbManager.Open();
                dbManager.BeginTransaction();
                int resultId = dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_DML_SUBCATEGORY");
                if (Convert.ToInt32(dbManager.Parameters[4].Value) > 0)
                {
                    dbManager.CommitTransaction();
                    if (category.DMLType == 'I')
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
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_DML_SUBCATEGORY", "Class : CommonDal, Method : GetCategories");
                alertMsgInfo.SaveStatus = false;
                alertMsgInfo.Message = "Error occured. Please try again.";
            }
            finally
            {
                dbManager.Close();
            }
        }

        public DataTable GetItemType(bool? IsActive)
        {
            try
            {
                dbManager.CreateParameters();
                dbManager.AddParameters("IsActive", GetDBNullOrValue(IsActive), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.Open();
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_ITEMTYPE");
                if (ds == null) return null;
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];
                return null;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_ITEMTYPE", "Class : CommonDal, Method : GetItemType");
                return null;
            }
            finally
            {
                dbManager.Dispose();
                //dbManager = null;
            }
        }

        public void SaveItemType(ItemTypeEntity itemType, AlertMsgInfo alertMsgInfo)
        {
            try
            {
                dbManager.CreateParameters();
                if (itemType.ItemTypeId != null)
                    dbManager.AddParameters("TypeId", GetDBNullOrValue(itemType.ItemTypeId), ParameterDirection.Input, DaoConstants.InParamSize);
                else
                    dbManager.AddParameters("TypeId", null, ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("TypeName", GetDBNullOrValue(itemType.ItemTypeName), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("DMLTYPE", GetDBNullOrValue(itemType.DMLType), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("CreatedBy", GetDBNullOrValue(UserSessionEntity.UserID), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Result", null, ParameterDirection.Output, DaoConstants.OutParamSize);
                dbManager.AddParameters("IsActive", GetDBNullOrValue(itemType.IsActive), ParameterDirection.Input, DaoConstants.InParamSize);

                dbManager.Open();
                dbManager.BeginTransaction();
                int resultId = dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_DML_ITEMPTYPE");
                if (Convert.ToInt32(dbManager.Parameters[3].Value) > 0)
                {
                    dbManager.CommitTransaction();
                    if (itemType.DMLType == 'I')
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
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_DML_ITEMPTYPE", "Class : CommonDal, Method : SaveItemType");
                alertMsgInfo.SaveStatus = false;
                alertMsgInfo.Message = "Error occured. Please try again.";
            }
            finally
            {
                dbManager.Close();
            }
        }

        public DataTable GetItemTypeByItem(int ItemId)
        {
            try
            {
                dbManager.CreateParameters();
                dbManager.AddParameters("ItemId", GetDBNullOrValue(ItemId), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.Open();
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_ItemTypeByItem");
                if (ds == null) return null;
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];
                return null;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_ItemTypeByItem", "Class : CommonDal, Method : GetItemTypeByItem");
                return null;
            }
            finally
            {
                dbManager.Dispose();
                //dbManager = null;
            }
        }

        public DataTable GetCompanies(bool? IsActive)
        {
            try
            {
                dbManager.CreateParameters();
                dbManager.AddParameters("IsActive", GetDBNullOrValue(IsActive), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.Open();
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_CompanyNames");
                if (ds == null) return null;
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];
                return null;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_CompanyNames", "Class : CommonDal, Method : GetCompanies");
                return null;
            }
            finally
            {
                dbManager.Dispose();
                //dbManager = null;
            }
        }

        public void SaveCompanies(CompanyEntity company, AlertMsgInfo alertMsgInfo)
        {
            try
            {
                dbManager.CreateParameters();
                if (company.CompanyId != null)
                    dbManager.AddParameters("CompId", GetDBNullOrValue(company.CompanyId), ParameterDirection.Input, DaoConstants.InParamSize);
                else
                    dbManager.AddParameters("CompId", null, ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("CompanyName", GetDBNullOrValue(company.CompanyName), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("DMLTYPE", GetDBNullOrValue(company.DMLType), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("CreatedBy", GetDBNullOrValue(UserSessionEntity.UserID), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Result", null, ParameterDirection.Output, DaoConstants.OutParamSize);
                dbManager.AddParameters("IsActive", GetDBNullOrValue(company.IsActive), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("SemiWhole", GetDBNullOrValue(company.SemiWholeSale), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Retail", GetDBNullOrValue(company.Retail), ParameterDirection.Input, DaoConstants.InParamSize);

                dbManager.Open();
                dbManager.BeginTransaction();
                int resultId = dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_DML_CompanyNames");
                if (Convert.ToInt32(dbManager.Parameters[4].Value) > 0)
                {
                    dbManager.CommitTransaction();
                    if (company.DMLType == 'I')
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
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_DML_CompanyNames", "Class : CommonDal, Method : SaveCompanies");
                alertMsgInfo.SaveStatus = false;
                alertMsgInfo.Message = "Error occured. Please try again.";
            }
            finally
            {
                dbManager.Close();
            }
        }

        public CompanyEntity GetCompanyMargin(int CompanyId)
        {
            try
            {
                dbManager.Open();
                dbManager.CreateParameters();
                dbManager.AddParameters("CompanyId", GetDBNullOrValue(CompanyId), ParameterDirection.Input, DaoConstants.InParamSize);
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_CompanyMargin");
                CompanyEntity company = (from DataRow dr in ds.Tables[0].Rows
                                         select new CompanyEntity()
                                         {
                                             SemiWholeSale = dr["SemiWholeSale"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SemiWholeSale"]),
                                             Retail = dr["Retail"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Retail"]),
                                         }).FirstOrDefault();
                return company;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_CompanyMargin", "Class : CommonDal, Method : GetCompanyMargin");
                return null;
            }
            finally
            {
                dbManager.Dispose();
                //dbManager = null;
            }
        }
        public List<ConversionsEntity> GetConversions(bool? IsActive)
        {
            try
            {
                dbManager.CreateParameters();
                dbManager.AddParameters("IsActive", GetDBNullOrValue(IsActive), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.Open();
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_Conversions");
                List<ConversionsEntity> subcategories = (from DataRow dr in ds.Tables[0].Rows
                                                         select new ConversionsEntity()
                                                         {
                                                             ConversionId = dr["ConvesionId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ConvesionId"]),
                                                             ConversionName = dr["ConversionName"] == DBNull.Value ? null : dr["ConversionName"].ToString(),
                                                             IsActive = dr["IsActive"] == DBNull.Value ? false : Convert.ToBoolean(dr["IsActive"])
                                                         }).ToList();
                return subcategories;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_Conversions", "Class : CommonDal, Method : GetConversions");
                return null;
            }
            finally
            {
                dbManager.Dispose();
                //dbManager = null;
            }
        }

        public List<ItemTypeEntity> GetConversionItems(int ConversionId)
        {
            try
            {
                dbManager.CreateParameters();
                dbManager.AddParameters("ConvesionId", GetDBNullOrValue(ConversionId), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.Open();
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_ConversionItems");
                List<ItemTypeEntity> subcategories = (from DataRow dr in ds.Tables[0].Rows
                                                      select new ItemTypeEntity()
                                                      {
                                                          SlNo = dr["SlNo"] == DBNull.Value ? 0 : Convert.ToInt32(dr["SlNo"]),
                                                          ItemTypeId = dr["ItemTypeId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ItemTypeId"]),
                                                          ItemTypeName = dr["TypeName"] == DBNull.Value ? null : dr["TypeName"].ToString(),
                                                          ItemOrder = dr["ItemOrder"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ItemOrder"])
                                                      }).ToList();
                return subcategories;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_ConversionItems", "Class : CommonDal, Method : GetConversionItems");
                return null;
            }
            finally
            {
                dbManager.Dispose();
                dbManager = null;
            }
        }

        public void SaveConversion(ConversionsEntity conversions, AlertMsgInfo alertMsgInfo)
        {
            try
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.AddRange(new DataColumn[3] {
                    new DataColumn("SlNo", typeof(int)),
                    new DataColumn("ItemTypeId", typeof(int)),
                    new DataColumn("ItemOrder", typeof(int))
                });

                foreach (ItemTypeEntity itemType in conversions.ItemType)
                {
                    dataTable.Rows.Add(itemType.SlNo, itemType.ItemTypeId, itemType.ItemOrder);
                }
                dbManager.CreateParameters();
                if (conversions.ConversionId != null)
                    dbManager.AddParameters("ConvesionId", GetDBNullOrValue(conversions.ConversionId), ParameterDirection.Input, DaoConstants.InParamSize);
                else
                    dbManager.AddParameters("ConvesionId", null, ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("TVP_ItemConversion", dataTable, ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("ConversionName", GetDBNullOrValue(conversions.ConversionName), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("DMLTYPE", GetDBNullOrValue(conversions.DMLType), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("CreatedBy", GetDBNullOrValue(UserSessionEntity.UserID), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Result", null, ParameterDirection.Output, DaoConstants.OutParamSize);
                dbManager.AddParameters("IsActive", GetDBNullOrValue(conversions.IsActive), ParameterDirection.Input, DaoConstants.InParamSize);

                dbManager.Open();
                dbManager.BeginTransaction();
                int resultId = dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_DML_Conversions");
                if (Convert.ToInt32(dbManager.Parameters[4].Value) > 0)
                {
                    dbManager.CommitTransaction();
                    if (conversions.DMLType == 'I')
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
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_DML_Conversions", "Class : CommonDal, Method : SaveCompanies");
                alertMsgInfo.SaveStatus = false;
                alertMsgInfo.Message = "Error occured. Please try again.";
            }
            finally
            {
                dbManager.Close();
            }
        }

        public void SaveItems(ItemsEntity itemsEntity, AlertMsgInfo alertMsgInfo)
        {
            try
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.AddRange(new DataColumn[6] {
                    new DataColumn("ItemTypeId", typeof(int)),
                    new DataColumn("Quantity", typeof(int)),
                    new DataColumn("QtyPerType", typeof(decimal)),
                    new DataColumn("WholeSale", typeof(decimal)),
                    new DataColumn("SemiWholeSale", typeof(decimal)),
                    new DataColumn("Retail", typeof(decimal))
                });

                foreach (ItemConversionQuantity quantity in itemsEntity.quantities)
                {
                    dataTable.Rows.Add(quantity.ItemTypeId, quantity.Quantity, quantity.QuantityPerType,
                        quantity.WholeSale, quantity.SemiWholeSale, quantity.Retail);
                }
                dbManager.CreateParameters();
                if (itemsEntity.ItemId != null)
                    dbManager.AddParameters("ItemId", GetDBNullOrValue(itemsEntity.ItemId), ParameterDirection.Input, DaoConstants.InParamSize);
                else
                    dbManager.AddParameters("ItemId", null, ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("ItemCode", GetDBNullOrValue(itemsEntity.ItemCode), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("ItemName", GetDBNullOrValue(itemsEntity.ItemName), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("ItemType", GetDBNullOrValue(itemsEntity.ItemType), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("CompanyId", GetDBNullOrValue(itemsEntity.CompanyId), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("CategoryId", GetDBNullOrValue(itemsEntity.CategoryId), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("SubCategoryID", GetDBNullOrValue(itemsEntity.SubCategoryId), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("DefaultSaleType", GetDBNullOrValue(itemsEntity.ItemType), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("WholeSalePrice", GetDBNullOrValue(itemsEntity.WholeSalePrice), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("SemiWholeSalePrice", GetDBNullOrValue(itemsEntity.SemiWholeSalePrice), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("RetailPrice", GetDBNullOrValue(itemsEntity.RetailPrice), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("DMLTYPE", GetDBNullOrValue(itemsEntity.DMLType), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Result", null, ParameterDirection.Output, DaoConstants.OutParamSize);
                dbManager.AddParameters("ConversionId", GetDBNullOrValue(itemsEntity.ConversionId), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("CreatedBy", GetDBNullOrValue(UserSessionEntity.UserID), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("TVP_TT_ItemQuantity", dataTable, ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("IsActive", GetDBNullOrValue(itemsEntity.IsActive), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("HSNId", GetDBNullOrValue(itemsEntity.HSNId), ParameterDirection.Input, DaoConstants.InParamSize);

                dbManager.Open();
                dbManager.BeginTransaction();
                int resultId = dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_DML_Items");
                if (Convert.ToInt32(dbManager.Parameters[12].Value) > 0)
                {
                    dbManager.CommitTransaction();
                    if (itemsEntity.DMLType == 'I')
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
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_DML_Items", "Class : CommonDal, Method : SaveItems");
                alertMsgInfo.SaveStatus = false;
                if (ex.Message.Contains("IX_FW_Master_Items_ItemCode"))
                    alertMsgInfo.Message = "Item Code is already exists";
                else if (ex.Message.Contains("IX_FW_Master_Items_ItemName"))
                    alertMsgInfo.Message = "Item Name is already exists";
                else
                    alertMsgInfo.Message = "Error occured. Please try again.";
            }
            finally
            {
                dbManager.Close();
            }
        }

        public ItemsEntity GetItem(int ItemId)
        {
            try
            {
                dbManager.Open();
                dbManager.CreateParameters();
                dbManager.AddParameters("ItemId", GetDBNullOrValue(ItemId), ParameterDirection.Input, DaoConstants.InParamSize);
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_Items");
                ItemsEntity subcategories = (from DataRow dr in ds.Tables[0].Rows
                                             select new ItemsEntity()
                                             {
                                                 ItemCode = dr["ItemCode"] == DBNull.Value ? null : dr["ItemCode"].ToString(),
                                                 ItemName = dr["ItemName"] == DBNull.Value ? null : dr["ItemName"].ToString(),
                                                 ItemType = dr["ItemType"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ItemType"]),
                                                 CompanyId = dr["CompanyId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CompanyId"]),
                                                 CategoryId = dr["CategoryId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CategoryId"]),
                                                 SubCategoryId = dr["SubCategoryId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["SubCategoryId"]),
                                                 DefaultSaleType = dr["DefaultSaleType"] == DBNull.Value ? 0 : Convert.ToInt32(dr["DefaultSaleType"]),
                                                 WholeSalePrice = dr["WholeSalePrice"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["WholeSalePrice"]),
                                                 SemiWholeSalePrice = dr["SemiWholeSalePrice"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SemiWholeSalePrice"]),
                                                 RetailPrice = dr["RetailPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["RetailPrice"]),
                                                 ConversionId = dr["ConversionId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ConversionId"]),
                                                 IsActive = dr["IsActive"] == DBNull.Value ? false : Convert.ToBoolean(dr["IsActive"]),
                                                 SemiWholeSaleMargin = dr["SemiWholeSale"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SemiWholeSale"]),
                                                 RetailMargin = dr["Retail"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Retail"]),
                                                 HSNId = dr["HSNId"] == DBNull.Value ? 0 : Convert.ToInt16(dr["HSNId"]),
                                                 quantities = (from DataRow dr1 in ds.Tables[1].Rows
                                                               select new ItemConversionQuantity()
                                                               {
                                                                   ItemTypeId = dr1["ItemTypeId"] == DBNull.Value ? 0 : Convert.ToInt32(dr1["ItemTypeId"]),
                                                                   ItemTypeName = dr1["TypeName"] == DBNull.Value ? null : dr1["TypeName"].ToString(),
                                                                   Quantity = dr1["Quantity"] == DBNull.Value ? 0 : Convert.ToInt32(dr1["Quantity"]),
                                                                   WholeSale = dr1["WholeSale"] == DBNull.Value ? 0 : Convert.ToDecimal(dr1["WholeSale"]),
                                                                   SemiWholeSale = dr1["SemiWholeSale"] == DBNull.Value ? 0 : Convert.ToDecimal(dr1["SemiWholeSale"]),
                                                                   Retail = dr1["Retail"] == DBNull.Value ? 0 : Convert.ToDecimal(dr1["Retail"])
                                                               }).ToList()
                                             }).FirstOrDefault();
                return subcategories;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_Items", "Class : CommonDal, Method : GetItem");
                return null;
            }
            finally
            {
                dbManager.Dispose();
                //dbManager = null;
            }
        }

        public DataTable GetItemsList(bool Status, int PageNumber, int PageSize, bool AllRows = false)
        {
            return GetItemsList(Status, PageNumber, PageSize, AllRows, null, null, null, null);
        }

        public DataTable GetItemsList(bool Status, int PageNumber, int PageSize, bool AllRows, string itemNameOrCode, int? categoryId, int? subCategoryId, int? companyId)
        {
            try
            {
                dbManager.Open();
                dbManager.CreateParameters();
                dbManager.AddParameters("Status", GetDBNullOrValue(Status), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("PageNumber", GetDBNullOrValue(PageNumber), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("PageSize", GetDBNullOrValue(PageSize), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("AllRows", GetDBNullOrValue(AllRows), ParameterDirection.Input, DaoConstants.InParamSize);

                // Add filter parameters
                dbManager.AddParameters("ItemNameOrCode", GetDBNullOrValue(itemNameOrCode), ParameterDirection.Input, DaoConstants.InParamSize);

                if (categoryId.HasValue && categoryId.Value > 0)
                    dbManager.AddParameters("CategoryId", categoryId.Value, ParameterDirection.Input, DaoConstants.InParamSize);
                else
                    dbManager.AddParameters("CategoryId", DBNull.Value, ParameterDirection.Input, DaoConstants.InParamSize);

                if (subCategoryId.HasValue && subCategoryId.Value > 0)
                    dbManager.AddParameters("SubCategoryId", subCategoryId.Value, ParameterDirection.Input, DaoConstants.InParamSize);
                else
                    dbManager.AddParameters("SubCategoryId", DBNull.Value, ParameterDirection.Input, DaoConstants.InParamSize);

                if (companyId.HasValue && companyId.Value > 0)
                    dbManager.AddParameters("CompanyId", companyId.Value, ParameterDirection.Input, DaoConstants.InParamSize);
                else
                    dbManager.AddParameters("CompanyId", DBNull.Value, ParameterDirection.Input, DaoConstants.InParamSize);

                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_ItemsList");
                // Capture total records if stored procedure returns it in second result set
                try
                {
                    if (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                    {
                        int total = 0;
                        int.TryParse(ds.Tables[1].Rows[0][0].ToString(), out total);
                        this.LastGetItemsTotalRecords = total;
                    }
                    else if (ds != null && ds.Tables.Count > 0)
                    {
                        this.LastGetItemsTotalRecords = ds.Tables[0].Rows.Count;
                    }
                }
                catch
                {
                    this.LastGetItemsTotalRecords = (ds != null && ds.Tables.Count > 0) ? ds.Tables[0].Rows.Count : 0;
                }

                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_ItemsList", "Class : CommonDal, Method : GetItemsList");
                return null;
            }
            finally
            {
                dbManager.Dispose();
                dbManager = null;
            }
        }

        public DataTable GetPriceList(int CustomerType)
        {
            try
            {
                dbManager.Open();
                dbManager.CreateParameters();
                dbManager.AddParameters("CustomerType", GetDBNullOrValue(CustomerType), ParameterDirection.Input, DaoConstants.InParamSize);
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_PriceList");
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_PriceList", "Class : CommonDal, Method : GetPriceList");
                return null;
            }
            finally
            {
                dbManager.Dispose();
                dbManager = null;
            }
        }

        public DataTable GetItemsList(string SearchStr, int CustomerType, bool isPrice)
        {
            try
            {
                dbManager.CreateParameters();
                dbManager.AddParameters("SearchString", GetDBNullOrValue(SearchStr), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("CustomerType", GetDBNullOrValue(CustomerType), ParameterDirection.Input, DaoConstants.InParamSize);
                if (isPrice)
                    dbManager.AddParameters("Price", 0, ParameterDirection.Input, DaoConstants.InParamSize);
                else
                    dbManager.AddParameters("Price", -1, ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.Open();
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_ItemsBySearch");
                //if (ds.Tables[0].Rows.Count > 0)
                //else
                //    return null;
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_ItemsBySearch", "Class : CommonDal, Method : GetItemsList with parameters");
                return null;
            }
            finally
            {
                dbManager.Dispose();
                dbManager = null;
            }
        }

        public void UpdateItemPrice(List<ItemsEntity> items, AlertMsgInfo alertMsgInfo)
        {
            try
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.AddRange(new DataColumn[5] {
                    new DataColumn("RowId", typeof(int)),
                    new DataColumn("ItemId", typeof(int)),
                    new DataColumn("WholeSalePrice", typeof(decimal)),
                    new DataColumn("SemiWholeSalePrice", typeof(decimal)),
                    new DataColumn("RetailPrice", typeof(decimal))
                });
                int i = 1;
                foreach (ItemsEntity itemType in items)
                {
                    dataTable.Rows.Add(i++, itemType.ItemId, itemType.WholeSalePrice, itemType.SemiWholeSalePrice, itemType.RetailPrice);
                }
                dbManager.CreateParameters();
                dbManager.AddParameters("P_TVP_Items_Update", dataTable, ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Result", null, ParameterDirection.Output, DaoConstants.OutParamSize);

                dbManager.Open();
                dbManager.BeginTransaction();
                dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_DML_Items_Price");
                if (Convert.ToInt32(dbManager.Parameters[1].Value) > 0)
                {
                    dbManager.CommitTransaction();
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
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_DML_Items_Price", "Class : CommonDal, Method : UpdateItemPrice");
                alertMsgInfo.SaveStatus = false;
                alertMsgInfo.Message = "Error occured. Please try again.";
            }
            finally
            {
                dbManager.Close();
            }
        }

        public DataTable GetItemPrice(int ItemId, int ItemTypeId, int CustomerType)
        {
            try
            {
                dbManager.CreateParameters();
                dbManager.AddParameters("ItemId", ItemId, ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("ItemTypeId", ItemTypeId, ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("CustomerType", CustomerType, ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.Open();
                DataSet dataSet = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_ItemPriceByCustomer");

                return dataSet.Tables[0];
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_ItemPriceByCustomer", "Class : CommonDal, Method : GetItemPrice");
                return null;
            }
            finally
            {
                dbManager.Dispose();
                //dbManager = null;
            }
        }

        public DataTable GetItemPriceWithLowestLevel(int ItemId, int ItemTypeId, int CustomerType)
        {
            try
            {
                dbManager.CreateParameters();
                dbManager.AddParameters("ItemId", ItemId, ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("ItemTypeId", ItemTypeId, ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("CustomerType", CustomerType, ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.Open();
                DataSet dataSet = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_ItemLowestPrice");

                return dataSet.Tables[0];
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_ItemLowestPrice", "Class : CommonDal, Method : GetItemPriceWithLowestLevel");
                return null;
            }
            finally
            {
                dbManager.Dispose();
                //dbManager = null;
            }
        }

        public string GetItemTypeDesc(int ItemId, int Quantity)
        {
            try
            {
                dbManager.CreateParameters();
                dbManager.AddParameters("ItemId", ItemId, ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Quantity", Quantity, ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.Open();
                return dbManager.ExecuteScalar(CommandType.StoredProcedure, "USP_GET_ItemTypewiseDesc").ToString();
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_ItemTypewiseDesc", "Class : CommonDal, Method : GetItemTypeDesc");
                return null;
            }
            finally
            {
                dbManager.Dispose();
            }
        }

        public CustomerDetails GetCustomerDetails(Int64 MobileNo)
        {
            try
            {
                dbManager.CreateParameters();
                dbManager.AddParameters("Mobile", GetDBNullOrValue(MobileNo), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.Open();
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_CustomerDetails");
                CustomerDetails customer = (from DataRow dr in ds.Tables[0].Rows
                                            select new CustomerDetails()
                                            {
                                                Name = dr["Name"] == DBNull.Value ? null : dr["Name"].ToString(),
                                                Address = dr["CustAddress"] == DBNull.Value ? null : dr["CustAddress"].ToString(),
                                                CustomerType = dr["CustomerType"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CustomerType"])
                                            }).FirstOrDefault();

                return customer;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_CustomerDetails", "Class : CommonDal, Method : GetCustomerDetails");
                return null;
            }
            finally
            {
                dbManager.Close();
            }
        }

        public DataTable GetRole()
        {
            try
            {
                dbManager.Open();
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_Roles");
                if (ds == null) return null;
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];
                return null;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_Roles", "Class : CommonDal, Method : GetRole");
                return null;
            }
            finally
            {
                dbManager.Dispose();
                //dbManager = null;
            }
        }

        public DataTable GetStockList()
        {
            try
            {
                dbManager.Open();
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_StockList");
                if (ds == null) return null;
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];

                return null;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_StockList", "Class : CommonDal, Method : GetStockList");
                return null;
            }
            finally
            {
                dbManager.Dispose();
                //dbManager = null;
            }
        }

        public GoodsReceiptEntity GetStockDetails(Int64 StockId)
        {
            try
            {
                dbManager.Open();
                dbManager.CreateParameters();
                dbManager.AddParameters("StockId", GetDBNullOrValue(StockId), ParameterDirection.Input, DaoConstants.InParamSize);
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_StockIdDetails");
                GoodsReceiptEntity goodsReceipt = (from DataRow dr in ds.Tables[0].Rows
                                                   select new GoodsReceiptEntity()
                                                   {
                                                       StockId = dr["StockId"] == DBNull.Value ? 0 : Convert.ToInt64(dr["StockId"]),
                                                       StockFrom = dr["StockFrom"] == DBNull.Value ? null : dr["StockFrom"].ToString(),
                                                       InvoiceNo = dr["InvoiceNo"] == DBNull.Value ? null : dr["InvoiceNo"].ToString(),
                                                       ArrivalDate = dr["ArrivalDt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["ArrivalDt"]),
                                                       Comments = dr["Comments"] == DBNull.Value ? null : dr["Comments"].ToString(),
                                                       GodownName = dr["GodownDescription"] == DBNull.Value ? null : dr["GodownDescription"].ToString(),
                                                       GodownId = dr["GodownId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["GodownId"]),
                                                       stockItems = (from DataRow dr1 in ds.Tables[0].Rows
                                                                     select new StockItemsList()
                                                                     {
                                                                         ItemId = dr1["ItemId"] == DBNull.Value ? 0 : Convert.ToInt32(dr1["ItemId"]),
                                                                         ItemName = dr1["ItemName"] == DBNull.Value ? null : dr1["ItemName"].ToString(),
                                                                         ItemCode = dr1["ItemCode"] == DBNull.Value ? null : dr1["ItemCode"].ToString(),
                                                                         ItemTypeName = dr1["ItemTypeName"] == DBNull.Value ? null : dr1["ItemTypeName"].ToString(),
                                                                         Quantity = dr1["Quantity"] == DBNull.Value ? 0 : Convert.ToInt32(dr1["Quantity"]),
                                                                         Detailed = dr1["Detailed"] == DBNull.Value ? null : dr1["Detailed"].ToString()
                                                                     }).ToList()
                                                   }).FirstOrDefault();
                return goodsReceipt;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_StockIdDetails", "Class : CommonDal, Method : GetStockDetails");
                return null;
            }
            finally
            {
                dbManager.Dispose();
                //dbManager = null;
            }
        }

        public void SaveHSN(HSNEntity entity, AlertMsgInfo alertMsgInfo)
        {
            try
            {
                dbManager.CreateParameters();
                if (entity.HSNId != null)
                    dbManager.AddParameters("HSNId", GetDBNullOrValue(entity.HSNId), ParameterDirection.Input, DaoConstants.InParamSize);
                else
                    dbManager.AddParameters("HSNId", null, ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("HSNCode", GetDBNullOrValue(entity.HSNCode), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("CGST", GetDBNullOrValue(entity.CGST), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("SGST", GetDBNullOrValue(entity.SGST), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("IGST", GetDBNullOrValue(entity.IGST), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("IsActive", GetDBNullOrValue(entity.IsActive), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("DMLTYPE", GetDBNullOrValue(entity.DMLType), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Result", null, ParameterDirection.Output, DaoConstants.OutParamSize);
                dbManager.AddParameters("CreatedBy", GetDBNullOrValue(UserSessionEntity.UserID), ParameterDirection.Input, DaoConstants.InParamSize);

                dbManager.Open();
                dbManager.BeginTransaction();
                dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_DML_HSN");
                if (Convert.ToInt32(dbManager.Parameters[7].Value) > 0)
                {
                    dbManager.CommitTransaction();
                    if (entity.DMLType == 'I')
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
                alertMsgInfo.SaveStatus = false;
                if (ex.Message.Contains("IX_FW_Master_HSN"))
                    alertMsgInfo.Message = "HSN Code already exits.";
                else
                {
                    Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_DML_HSN", "Class : CommonDal, Method : SaveHSN");
                    alertMsgInfo.Message = "Error occured. Please try again.";
                }
            }
            finally
            {
                dbManager.Close();
            }
        }

        public DataTable GetHSN(bool? IsActive)
        {
            try
            {
                dbManager.CreateParameters();
                dbManager.AddParameters("IsActive", GetDBNullOrValue(IsActive), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.Open();
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_HSN");
                if (ds == null) return null;
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];
                return null;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_HSN", "Class : CommonDal, Method : GetHSN");
                return null;
            }
            finally
            {
                dbManager.Dispose();
                //dbManager = null;
            }
        }

        public int? GetItemTypeTop(int ItemId)
        {
            try
            {
                dbManager.CreateParameters();
                dbManager.AddParameters("ItemId", ItemId, ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.Open();
                return Convert.ToInt32(dbManager.ExecuteScalar(CommandType.StoredProcedure, "USP_GET_ItemTypeTop"));
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_ItemTypeTop", "Class : CommonDal, Method : GetItemTypeTop");
                return null;
            }
            finally
            {
                dbManager.Dispose();
            }
        }

        public DataTable GetCustomerList()
        {
            try
            {
                dbManager.Open();
                dbManager.CreateParameters();
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_Customers");
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_Customers", "Class : CommonDal, Method : GetCustomerList");
                return null;
            }
            finally
            {
                dbManager.Dispose();
                dbManager = null;
            }
        }

        public DataTable GetGodown(bool? IsActive)
        {
            try
            {
                dbManager.CreateParameters();
                dbManager.AddParameters("IsActive", GetDBNullOrValue(IsActive), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.Open();
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_GODOWN");
                if (ds == null) return null;
                if (ds.Tables.Count > 0)
                {
                    return ds.Tables[0];
                }
                return null;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_GODOWN", "Class : CommonDal, Method : GetGodown");
                return null;
            }
            finally
            {
                dbManager.Dispose();
                //dbManager = null;
            }
        }

        public void SaveGodown(GodownEntity godown, AlertMsgInfo alertMsgInfo)
        {
            try
            {
                dbManager.CreateParameters();
                if (godown.GodownId != null)
                    dbManager.AddParameters("GodownId", GetDBNullOrValue(godown.GodownId), ParameterDirection.Input, DaoConstants.InParamSize);
                else
                    dbManager.AddParameters("GodownId", null, ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("GodownDescription", GetDBNullOrValue(godown.GodownName), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("DMLTYPE", GetDBNullOrValue(godown.DMLType), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Result", null, ParameterDirection.Output, DaoConstants.OutParamSize);
                dbManager.AddParameters("CreatedBy", GetDBNullOrValue(UserSessionEntity.UserID), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("IsActive", GetDBNullOrValue(godown.IsActive), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Address", GetDBNullOrValue(godown.GodownAddress), ParameterDirection.Input, DaoConstants.InParamSize);

                dbManager.Open();
                dbManager.BeginTransaction();
                dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_DML_GODOWN");
                if (Convert.ToInt32(dbManager.Parameters[3].Value) == -1)
                {
                    dbManager.RollBackTransaction();
                    alertMsgInfo.Message = "Godown already exists";
                    alertMsgInfo.SaveStatus = false;
                }

                else if (Convert.ToInt32(dbManager.Parameters[3].Value) > 0)
                {
                    dbManager.CommitTransaction();
                    if (godown.DMLType == 'I')
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
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_DML_GODOWN", "Class : CommonDal, Method : SaveGodown");
                alertMsgInfo.SaveStatus = false;
                alertMsgInfo.Message = "Error occured. Please try again.";
            }
            finally
            {
                dbManager.Close();
            }
        }
    }
}
