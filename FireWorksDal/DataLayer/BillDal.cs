using FireWorksDal.Constants;
using FireWorksDal.DataObjects;
using FireWorksEntity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace FireWorksDal.DataLayer
{
    public class BillDal : CommonMethods //ICommon
    {
        IDBManager dbManager = null;
        public BillDal()
        {
            if (dbManager == null)
                dbManager = new DBManager();
        }
        public void SaveBills(Bill bill, AlertMsgInfo alertMsgInfo, int? User = null)
        {
            try
            {
                if (User == null)
                {
                    User = UserSessionEntity.UserID;
                }
                DataTable dataTable = new DataTable();
                dataTable.Columns.AddRange(new DataColumn[12] {
                    new DataColumn("SlNo", typeof(int)),
                    new DataColumn("ItemId", typeof(int)),
                    new DataColumn("ItemTypeId", typeof(int)),
                    new DataColumn("Quantity", typeof(int)),
                    new DataColumn("ItemCost", typeof(decimal)),
                    new DataColumn("TotalCost", typeof(decimal)),
                    new DataColumn("CGST", typeof(decimal)),
                    new DataColumn("SGST", typeof(decimal)),
                    new DataColumn("IGST", typeof(decimal)),
                    new DataColumn("ChangedPrice", typeof(decimal)),
                    new DataColumn("SellType", typeof(int)),
                    new DataColumn("SellQty", typeof(int))
                });

                foreach (BilledItems billed in bill.billedItems)
                {
                    int? SellType = null, SellQty = null;
                    if (billed.SellTypeId != 0)
                    {
                        SellType = billed.SellTypeId;
                        SellQty = billed.SellQty;
                    }
                    dataTable.Rows.Add(billed.SlNo, billed.ItemId, billed.ItemTypeId, billed.Quantity, billed.IndividualCost,
                        billed.TotalCost, billed.CGST, billed.SGST, billed.IGST, billed.ChangedPrice, SellType, SellQty);
                }

                dbManager.CreateParameters();
                dbManager.AddParameters("BilledItems_TVP", dataTable, ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Name", GetDBNullOrValue(bill.Name), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Address", GetDBNullOrValue(bill.Address), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("MobileNo", GetDBNullOrValue(bill.PhoneNo), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("CustomerType", GetDBNullOrValue(bill.CustomerType), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("IsEstimateInvoice", GetDBNullOrValue(bill.IsEstimateInvoice), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("BillAmount", GetDBNullOrValue(bill.BillAmount), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("DiscountPerItem", GetDBNullOrValue(bill.DiscountPerItem), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("DiscountPercentage", GetDBNullOrValue(bill.DiscountPercentage), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("IsInState", GetDBNullOrValue(bill.IsInState), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("CustGSTIN", GetDBNullOrValue(bill.CustGSTIN), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("TotalBillAmount", GetDBNullOrValue(bill.TotalBillAmount), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("AmountPaid", GetDBNullOrValue(bill.AmountPaid), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("BalanceAmount", GetDBNullOrValue(bill.BalanceAmount), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("CreatedBy", GetDBNullOrValue(User), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Result", null, ParameterDirection.Output, DaoConstants.OutParamSize);
                if (bill.BillId != null)
                {
                    dbManager.AddParameters("BillId", GetDBNullOrValue(bill.BillId.Replace("E", "")), ParameterDirection.Input, DaoConstants.InParamSize);
                    dbManager.AddParameters("BillNo", GetDBNullOrValue(bill.BillNo), ParameterDirection.Input, DaoConstants.InParamSize);
                }
                dbManager.AddParameters("DMLType", GetDBNullOrValue(bill.DMLType), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("AddlDisc", GetDBNullOrValue(bill.AddlDisc), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("FinalAmount", GetDBNullOrValue(bill.TotalFinalAmount), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("AddDesc", GetDBNullOrValue(bill.AddDesc), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Mobile2", GetDBNullOrValue(bill.Mobile2), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Invoice", GetDBNullOrValue(bill.InvoiceNo), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("POS", GetDBNullOrValue(bill.POS), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("StateCode", GetDBNullOrValue(bill.StateCode), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Pincode", GetDBNullOrValue(bill.Pincode), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Location", GetDBNullOrValue(bill.Location), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.Open();
                dbManager.BeginTransaction();
                int resultId = dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_DML_Bills");
                if (Convert.ToInt32(dbManager.Parameters[15].Value) > 0)
                {
                    dbManager.CommitTransaction();
                    alertMsgInfo.Message = "Saved successfully.";
                    alertMsgInfo.SaveStatus = true;
                    alertMsgInfo.Id = Convert.ToInt64(dbManager.Parameters[15].Value);
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
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_DML_Bills", "Class : BillDal, Method : SaveBills");
                alertMsgInfo.SaveStatus = false;
                if (ex.Message.Contains("PK_FW_Bills"))
                    alertMsgInfo.Message = "Bill No already exists";
                else if (ex.Message.Contains("IX_FW_Bills_BillNo"))
                    alertMsgInfo.Message = "Bill No already exists";
                else if (ex.Message.Contains("PriceError"))
                    alertMsgInfo.Message = "Mismatch in Item price";
                else
                    alertMsgInfo.Message = "Error occured. Please try again.";
            }
            finally
            {
                dbManager.Close();
            }
        }

        public DataTable GetSavedBills(Int64? PhoneNo, string cusName, DateTime? CreateDate, Int64? BillId, int? CustomerType, char? IsEstimate)
        {
            try
            {
                dbManager.CreateParameters();
                if (CreateDate != null)
                    dbManager.AddParameters("CreatedDate", GetDBNullOrValue(CreateDate), ParameterDirection.Input, DaoConstants.InParamSize);
                if (cusName != null)
                    dbManager.AddParameters("Name", GetDBNullOrValue(cusName), ParameterDirection.Input, DaoConstants.InParamSize);
                if (BillId != null)
                    dbManager.AddParameters("BillNo", GetDBNullOrValue(BillId), ParameterDirection.Input, DaoConstants.InParamSize);
                if (PhoneNo != null)
                    dbManager.AddParameters("MobileNo", GetDBNullOrValue(PhoneNo), ParameterDirection.Input, DaoConstants.InParamSize);
                if (CustomerType != null)
                    dbManager.AddParameters("CustomerType", GetDBNullOrValue(CustomerType), ParameterDirection.Input, DaoConstants.InParamSize);
                if (IsEstimate != null)
                    dbManager.AddParameters("IsEstimate", GetDBNullOrValue(IsEstimate), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Role", GetDBNullOrValue(UserSessionEntity.RoleId), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.Open();
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_BILLS");
                if (ds == null)
                    return null;
                if (ds.Tables.Count > 0)
                {
                    return ds.Tables[0];
                }
                return null;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_BILLS", "Class : BillDal, Method : GetSavedBills");
                return null;
            }
            finally
            {
                dbManager.Dispose();
            }
        }

        public List<SavedBills> GetWebSavedBills(Int64? PhoneNo, string cusName, DateTime? CreateDate, Int64? BillId, int? CustomerType, char? IsEstimate)
        {
            List<SavedBills> list = new List<SavedBills>();
            try
            {
                dbManager.CreateParameters();
                if (CreateDate != null)
                    dbManager.AddParameters("CreatedDate", GetDBNullOrValue(CreateDate), ParameterDirection.Input, DaoConstants.InParamSize);
                if (cusName != null)
                    dbManager.AddParameters("Name", GetDBNullOrValue(cusName), ParameterDirection.Input, DaoConstants.InParamSize);
                if (BillId != null)
                    dbManager.AddParameters("BillNo", GetDBNullOrValue(BillId), ParameterDirection.Input, DaoConstants.InParamSize);
                if (PhoneNo != null)
                    dbManager.AddParameters("MobileNo", GetDBNullOrValue(PhoneNo), ParameterDirection.Input, DaoConstants.InParamSize);
                if (CustomerType != null)
                    dbManager.AddParameters("CustomerType", GetDBNullOrValue(CustomerType), ParameterDirection.Input, DaoConstants.InParamSize);
                if (IsEstimate != null)
                    dbManager.AddParameters("IsEstimate", GetDBNullOrValue(IsEstimate), ParameterDirection.Input, DaoConstants.InParamSize);

                dbManager.Open();
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_BILLS");
                if (ds == null)
                    return list;
                if (ds.Tables.Count > 0)
                {
                    list = (from DataRow dr in ds.Tables[0].Rows
                            select new SavedBills()
                            {
                                SlNo = dr["SlNo"] == DBNull.Value ? default(int) : Convert.ToInt16(dr["SlNo"]),
                                BillId = dr["BillId"] == DBNull.Value ? default(int) : Convert.ToInt64(dr["BillId"]),
                                BillNo = dr["BillNo"].ToString(),
                                Name = dr["CustomerName"].ToString(),
                                BillAmount = dr["BillAmount"] == DBNull.Value ? default(decimal) : Convert.ToDecimal(dr["BillAmount"]),
                                BillDate = dr["BillDate"].ToString(),
                                Mobile = dr["PhoneNo"] == DBNull.Value ? default(int) : Convert.ToInt64(dr["PhoneNo"]),
                                Estimate = dr["Estimate"].ToString(),
                                CustType = dr["CustomerType"].ToString(),
                                InvoiceNo = dr["InvoiceNo"].ToString()
                            }).ToList();

                    return list;
                }
                return list;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_BILLS", "Class : BillDal, Method : GetSavedBills");
                return list;
            }
            finally
            {
                dbManager.Dispose();
            }
        }

        public Bill GetBillDetailsById(Int64 BillId)
        {
            try
            {
                dbManager.Open();
                dbManager.CreateParameters();
                dbManager.AddParameters("BillId", GetDBNullOrValue(BillId), ParameterDirection.Input, DaoConstants.InParamSize);
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_BillDetailsById");
                Bill bill = (from DataRow dr in ds.Tables[0].Rows
                             select new Bill()
                             {
                                 BillId = dr["BillId"] == DBNull.Value ? null : dr["BillId"].ToString(),
                                 BillNo = dr["BillNo"] == DBNull.Value ? null : dr["BillNo"].ToString(),
                                 CreatedDate = Convert.ToDateTime(dr["CreatedDate"]),
                                 Name = dr["Name"] == DBNull.Value ? null : dr["Name"].ToString(),
                                 Address = dr["CustAddress"] == DBNull.Value ? null : dr["CustAddress"].ToString(),
                                 PhoneNo = dr["MobileNo"] == DBNull.Value ? 0 : Convert.ToInt64(dr["MobileNo"]),
                                 CustomerType = dr["CustomerType"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CustomerType"]),
                                 IsEstimateInvoice = Convert.ToChar(dr["IsEstimateInvoice"]),
                                 BillAmount = dr["BillAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["BillAmount"]),
                                 DiscountPerItem = dr["DiscountPerItem"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["DiscountPerItem"]),
                                 DiscountPercentage = dr["DiscountPercentage"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["DiscountPercentage"]),
                                 IsInState = Convert.ToBoolean(dr["IsInState"]),
                                 CustGSTIN = dr["CustGSTIN"] == DBNull.Value ? null : dr["CustGSTIN"].ToString(),
                                 //HSNCode = dr["HSNCode"] == DBNull.Value ? null : dr["HSNCode"].ToString(),
                                 TotalBillAmount = dr["TotalBillAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TotalBillAmount"]),
                                 AmountPaid = dr["AmountPaid"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["AmountPaid"]),
                                 BalanceAmount = dr["BalanceAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["BalanceAmount"]),
                                 AddlDisc = dr["AddlDisc"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["AddlDisc"]),
                                 TotalFinalAmount = dr["FinalAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["FinalAmount"]),
                                 AddDesc = dr["AddDesc"] == DBNull.Value ? null : dr["AddDesc"].ToString(),
                                 Mobile2 = dr["Mobile2"] == DBNull.Value ? (Int64?)null : Convert.ToInt64(dr["Mobile2"]),
                                 InvoiceNo = dr["InvoiceNo"] == DBNull.Value ? null : dr["InvoiceNo"].ToString(),
                                 POS = dr["POS"] == DBNull.Value ? null : dr["POS"].ToString(),
                                 Pincode = dr["Pincode"] == DBNull.Value ? null : dr["Pincode"].ToString(),
                                 StateCode = dr["StateCode"] == DBNull.Value ? null : dr["StateCode"].ToString(),
                                 Location = dr["Loc"] == DBNull.Value ? null : dr["Loc"].ToString(),
                                 billedItems = (from DataRow dr1 in ds.Tables[1].Rows
                                                select new BilledItems()
                                                {
                                                    SlNo = dr1["SlNo"] == DBNull.Value ? 0 : Convert.ToInt32(dr1["SlNo"]),
                                                    ItemId = dr1["ItemId"] == DBNull.Value ? 0 : Convert.ToInt32(dr1["ItemId"]),
                                                    ItemCode = dr1["ItemCode"] == DBNull.Value ? null : dr1["ItemCode"].ToString(),
                                                    ItemName = dr1["ItemName"] == DBNull.Value ? null : dr1["ItemName"].ToString(),
                                                    ItemTypeId = dr1["ItemTypeId"] == DBNull.Value ? 0 : Convert.ToInt32(dr1["ItemTypeId"]),
                                                    ItemTypeName = dr1["ItemType"] == DBNull.Value ? null : dr1["ItemType"].ToString(),
                                                    Detailed = dr1["Detailed"] == DBNull.Value ? null : dr1["Detailed"].ToString(),
                                                    HSNCode = dr1["HSNCode"] == DBNull.Value ? null : dr1["HSNCode"].ToString(),
                                                    Quantity = dr1["Quantity"] == DBNull.Value ? 0 : Convert.ToInt32(dr1["Quantity"]),
                                                    IndividualCost = dr1["IndividualCost"] == DBNull.Value ? 0 : Convert.ToDecimal(dr1["IndividualCost"]),
                                                    ItemCost = dr1["ItemCost"] == DBNull.Value ? 0 : Convert.ToDecimal(dr1["ItemCost"]),
                                                    TotalCost = dr1["TotalCost"] == DBNull.Value ? 0 : Convert.ToDecimal(dr1["TotalCost"]),
                                                    OriginalCost = dr1["OriginalCost"] == DBNull.Value ? 0 : Convert.ToDecimal(dr1["OriginalCost"]),
                                                    CGST = dr1["CGST"] == DBNull.Value ? 0 : Convert.ToDecimal(dr1["CGST"]),
                                                    SGST = dr1["SGST"] == DBNull.Value ? 0 : Convert.ToDecimal(dr1["SGST"]),
                                                    IGST = dr1["IGST"] == DBNull.Value ? 0 : Convert.ToDecimal(dr1["IGST"]),
                                                    ChangedPrice = dr1["ChangedPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(dr1["ChangedPrice"]),
                                                    SellTypeId = dr1["SellType"] == DBNull.Value ? 0 : Convert.ToInt32(dr1["SellType"]),
                                                    SellQty = dr1["SellQty"] == DBNull.Value ? 0 : Convert.ToInt32(dr1["SellQty"]),
                                                    UnitName = dr1["UnitName"] == DBNull.Value ? null : dr1["UnitName"].ToString()
                                                }).ToList()
                             }).FirstOrDefault();
                return bill;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_BillDetailsById", "Class : BillDal, Method : GetBillDetailsById");
                return null;
            }
            finally
            {
                dbManager.Dispose();
            }
        }

        public DataSet GetBillDetailsByIdRpt(Int64 BillId)
        {
            try
            {
                dbManager.Open();
                dbManager.CreateParameters();
                dbManager.AddParameters("BillId", GetDBNullOrValue(BillId), ParameterDirection.Input, DaoConstants.InParamSize);
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_RPT_GET_BillDetailsById");

                return ds;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_RPT_GET_BillDetailsById", "Class : BillDal, Method : GetBillDetailsByIdRpt");
                return null;
            }
            finally
            {
                dbManager.Dispose();
            }
        }

        public DataSet GetInvoiceBillDetailsById(Int64 BillId)
        {
            try
            {
                dbManager.Open();
                dbManager.CreateParameters();
                dbManager.AddParameters("BillId", GetDBNullOrValue(BillId), ParameterDirection.Input, DaoConstants.InParamSize);
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_RPT_GET_InvoiceBillDetailsById");

                return ds;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_RPT_GET_InvoiceBillDetailsById", "Class : BillDal, Method : GetInvoiceBillDetailsById");
                return null;
            }
            finally
            {
                dbManager.Dispose();
            }
        }

        public void SaveStock(GoodsReceiptEntity goods, AlertMsgInfo alertMsgInfo)
        {
            try
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.AddRange(new DataColumn[3] {
                    new DataColumn("ItemId", typeof(int)),
                    new DataColumn("ItemTypeId", typeof(int)),
                    new DataColumn("Quantity", typeof(Int64))
                });

                foreach (StockItemsList receipt in goods.stockItems)
                {
                    dataTable.Rows.Add(receipt.ItemId, receipt.ItemTypeId, receipt.Quantity);
                }

                dbManager.CreateParameters();
                dbManager.AddParameters("PTVP_Stock", dataTable, ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("CreatedBy", GetDBNullOrValue(UserSessionEntity.UserID), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Result", null, ParameterDirection.Output, DaoConstants.OutParamSize);
                dbManager.AddParameters("StockFrom", GetDBNullOrValue(goods.StockFrom), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("DMLType", GetDBNullOrValue(goods.DMLType), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("ReceiptId", GetDBNullOrValue(goods.StockId), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("InvoiceNo", GetDBNullOrValue(goods.InvoiceNo), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("ArrivalDt", GetDBNullOrValue(goods.ArrivalDate), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Comments", GetDBNullOrValue(goods.Comments), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("GodownId", GetDBNullOrValue(goods.GodownId), ParameterDirection.Input, DaoConstants.InParamSize);

                dbManager.Open();
                dbManager.BeginTransaction();
                int resultId = dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_DML_Stock");

                switch (Convert.ToInt32(dbManager.Parameters[2].Value))
                {
                    case 0:
                        dbManager.RollBackTransaction();
                        alertMsgInfo.SaveStatus = false;
                        alertMsgInfo.Message = "Invoice No. already exists";
                        break;
                    case -1:
                        dbManager.RollBackTransaction();
                        alertMsgInfo.SaveStatus = false;
                        alertMsgInfo.Message = "Error occured. Please try again.";
                        break;
                    default:
                        dbManager.CommitTransaction();
                        alertMsgInfo.Message = "Saved successfully.";
                        alertMsgInfo.SaveStatus = true;
                        break;
                }

            }
            catch (Exception ex)
            {
                dbManager.RollBackTransaction();
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_DML_Stock", "Class : BillDal, Method : SaveStock");
                alertMsgInfo.SaveStatus = false;
                alertMsgInfo.Message = "Error occured. Please try again.";
            }
            finally
            {
                dbManager.Close();
            }
        }

        public DataSet GetStockTrade(DateTime From, DateTime To, int? CustomerType)
        {
            try
            {
                dbManager.Open();
                dbManager.CreateParameters();
                dbManager.AddParameters("FromDate", GetDBNullOrValue(From), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("ToDate", GetDBNullOrValue(To), ParameterDirection.Input, DaoConstants.InParamSize);
                if (CustomerType != null)
                    dbManager.AddParameters("CustomerType", GetDBNullOrValue(CustomerType), ParameterDirection.Input, DaoConstants.InParamSize);
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_StockTrade");

                return ds;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_StockTrade", "Class : BillDal, Method : GetStockTrade");
                return null;
            }
            finally
            {
                dbManager.Dispose();
            }
        }

        public void RemoveInvoice(Int64 BillId, AlertMsgInfo alertMsgInfo)
        {
            try
            {
                dbManager.CreateParameters();
                dbManager.AddParameters("BillId", GetDBNullOrValue(BillId), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Result", null, ParameterDirection.Output, DaoConstants.OutParamSize);

                dbManager.Open();
                dbManager.BeginTransaction();
                dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_DML_RemoveInvoice");
                if (Convert.ToInt32(dbManager.Parameters[1].Value) > 0)
                {
                    dbManager.CommitTransaction();
                    alertMsgInfo.Message = "Invoice removed successfully.";
                    alertMsgInfo.SaveStatus = true;
                }
                else if (Convert.ToInt32(dbManager.Parameters[1].Value) == -100)
                {
                    dbManager.RollBackTransaction();
                    alertMsgInfo.Message = "Please select latest Invoice.";
                    alertMsgInfo.SaveStatus = false;
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
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_DML_RemoveInvoice", "Class : BillDal, Method : RemoveInvoice");
                alertMsgInfo.SaveStatus = false;
                alertMsgInfo.Message = "Error occured. Please try again.";
            }
            finally
            {
                dbManager.Close();
            }
        }

        public DataSet GetHSNRpt(DateTime From, DateTime To)
        {
            try
            {
                dbManager.Open();
                dbManager.CreateParameters();
                dbManager.AddParameters("FromDate", GetDBNullOrValue(From), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("ToDate", GetDBNullOrValue(To), ParameterDirection.Input, DaoConstants.InParamSize);
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "GET_RPT_HSN");

                return ds;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "GET_RPT_HSN", "Class : BillDal, Method : GetHSNRpt");
                return null;
            }
            finally
            {
                dbManager.Dispose();
            }
        }

        public void VeifyBill(Int64 BillId, AlertMsgInfo alertMsgInfo)
        {
            try
            {
                dbManager.CreateParameters();
                dbManager.AddParameters("BillId", GetDBNullOrValue(BillId), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("VerifiedBy", GetDBNullOrValue(UserSessionEntity.UserID), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.Open();
                dbManager.BeginTransaction();
                int resultId = dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_DML_VERIFY_Bill");
                if (resultId > 0)
                {
                    dbManager.CommitTransaction();
                    alertMsgInfo.Message = "Item particulars are verified successfully.";
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
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_DML_VERIFY_Bill", "Class : BillDal, Method : VeifyBill");
                alertMsgInfo.SaveStatus = false;
            }
            finally
            {
                dbManager.Close();
            }
        }

        public void DeleteStock(Int64 StockId, AlertMsgInfo alertMsgInfo)
        {
            try
            {
                dbManager.CreateParameters();
                dbManager.AddParameters("ReceiptId", GetDBNullOrValue(StockId), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Result", null, ParameterDirection.Output, DaoConstants.OutParamSize);

                dbManager.Open();
                dbManager.BeginTransaction();
                int resultId = dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_DML_Stock_Delete");

                if (Convert.ToInt32(dbManager.Parameters[1].Value) == 1)
                {
                    dbManager.CommitTransaction();
                    alertMsgInfo.Message = "Stock deleted successfully.";
                    alertMsgInfo.SaveStatus = true;
                }

            }
            catch (Exception ex)
            {
                dbManager.RollBackTransaction();
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_DML_Stock_Delete", "Class : BillDal, Method : DeleteStock");
                alertMsgInfo.SaveStatus = false;
                alertMsgInfo.Message = "Error occured. Please try again.";
            }
            finally
            {
                dbManager.Close();
            }
        }

        public void UpdateWayBill(Int64 BillId, string WayBill, AlertMsgInfo alertMsgInfo)
        {
            try
            {
                dbManager.CreateParameters();
                dbManager.AddParameters("BillId", GetDBNullOrValue(BillId), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("WayBill", GetDBNullOrValue(WayBill), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Result", null, ParameterDirection.Output, DaoConstants.OutParamSize);

                dbManager.Open();
                dbManager.BeginTransaction();
                dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_DML_WayBill");
                if (Convert.ToInt32(dbManager.Parameters[2].Value) > 0)
                {
                    dbManager.CommitTransaction();
                    alertMsgInfo.Message = "Way Bill updated successfully.";
                    alertMsgInfo.SaveStatus = true;
                }
                else if (Convert.ToInt32(dbManager.Parameters[1].Value) == -100)
                {
                    dbManager.RollBackTransaction();
                    alertMsgInfo.Message = "Please select latest Invoice.";
                    alertMsgInfo.SaveStatus = false;
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
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_DML_WayBill", "Class : BillDal, Method : UpdateWayBill");
                alertMsgInfo.SaveStatus = false;
                alertMsgInfo.Message = "Error occured. Please try again.";
            }
            finally
            {
                dbManager.Close();
            }
        }
        public DataSet GetTaxAmountRpt(DateTime From, DateTime To)
        {
            try
            {
                dbManager.Open();
                dbManager.CreateParameters();
                dbManager.AddParameters("FromDate", GetDBNullOrValue(From), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("ToDate", GetDBNullOrValue(To), ParameterDirection.Input, DaoConstants.InParamSize);
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_TAX_AMOUNT");

                return ds;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_TAX_AMOUNT", "Class : BillDal, Method : GetTaxAmountRpt");
                return null;
            }
            finally
            {
                dbManager.Dispose();
            }
        }
        public DataTable GetEstimateBills(Int64? PhoneNo, string cusName, DateTime? CreateDate, Int64? BillId, int? CustomerType)
        {
            try
            {
                dbManager.CreateParameters();
                if (CreateDate != null)
                    dbManager.AddParameters("CreatedDate", GetDBNullOrValue(CreateDate), ParameterDirection.Input, DaoConstants.InParamSize);
                if (cusName != null)
                    dbManager.AddParameters("Name", GetDBNullOrValue(cusName), ParameterDirection.Input, DaoConstants.InParamSize);
                if (BillId != null)
                    dbManager.AddParameters("BillNo", GetDBNullOrValue(BillId), ParameterDirection.Input, DaoConstants.InParamSize);
                if (PhoneNo != null)
                    dbManager.AddParameters("MobileNo", GetDBNullOrValue(PhoneNo), ParameterDirection.Input, DaoConstants.InParamSize);
                if (CustomerType != null)
                    dbManager.AddParameters("CustomerType", GetDBNullOrValue(CustomerType), ParameterDirection.Input, DaoConstants.InParamSize);

                dbManager.Open();
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GET_ESTIMATES");
                if (ds == null)
                    return null;
                if (ds.Tables.Count > 0)
                {
                    return ds.Tables[0];
                }
                return null;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GET_ESTIMATES", "Class : BillDal, Method : GetSavedBills");
                return null;
            }
            finally
            {
                dbManager.Dispose();
            }
        }

        public void UpdateJSONStatus(Int64 BillId, AlertMsgInfo alertMsgInfo)
        {
            try
            {
                dbManager.CreateParameters();
                dbManager.AddParameters("BillId", GetDBNullOrValue(BillId), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Result", null, ParameterDirection.Output, DaoConstants.OutParamSize);

                dbManager.Open();
                dbManager.BeginTransaction();
                dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_DML_Json_Status");
                if (Convert.ToInt32(dbManager.Parameters[1].Value) > 0)
                {
                    dbManager.CommitTransaction();
                    alertMsgInfo.Message = "JSON generated successfully.";
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
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_DML_Json_Status", "Class : BillDal, Method : UpdateJSONStatus");
                alertMsgInfo.SaveStatus = false;
                alertMsgInfo.Message = "Error occured. Please try again.";
            }
            finally
            {
                dbManager.Close();
            }
        }
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        public void CancelInvoice(Int64 BillId, AlertMsgInfo alertMsgInfo)
        {
            try
            {
                dbManager.CreateParameters();
                dbManager.AddParameters("BillId", GetDBNullOrValue(BillId), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("CreatedBy", GetDBNullOrValue(UserSessionEntity.UserID), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Result", null, ParameterDirection.Output, DaoConstants.OutParamSize);

                dbManager.Open();
                dbManager.BeginTransaction();
                dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_DML_CancelInvoice");
                if (Convert.ToInt32(dbManager.Parameters[2].Value) > 0)
                {
                    dbManager.CommitTransaction();
                    alertMsgInfo.Message = "Invoice cancelled successfully.";
                    alertMsgInfo.SaveStatus = true;
                }
                else if (Convert.ToInt32(dbManager.Parameters[2].Value) == -100)
                {
                    dbManager.RollBackTransaction();
                    alertMsgInfo.Message = "Please select latest Invoice.";
                    alertMsgInfo.SaveStatus = false;
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
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_DML_CancelInvoice", "Class : BillDal, Method : RemoveInvoice");
                alertMsgInfo.SaveStatus = false;
                alertMsgInfo.Message = "Error occured. Please try again.";
            }
            finally
            {
                dbManager.Close();
            }
        }

        public void UpdateDispatch(Int64 BillId, char DispatchStatus, AlertMsgInfo alertMsgInfo)
        {
            try
            {
                dbManager.CreateParameters();
                dbManager.AddParameters("BillId", GetDBNullOrValue(BillId), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Status", GetDBNullOrValue(DispatchStatus), ParameterDirection.Input, DaoConstants.InParamSize);
                dbManager.AddParameters("Result", null, ParameterDirection.Output, DaoConstants.OutParamSize);

                dbManager.Open();
                dbManager.BeginTransaction();
                dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_DML_Dispatch");
                if (Convert.ToInt32(dbManager.Parameters[2].Value) > 0)
                {
                    dbManager.CommitTransaction();
                    alertMsgInfo.Message = "Dispatch status updated successfully.";
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
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_DML_Dispatch", "Class : BillDal, Method : UpdateDispatch");
                alertMsgInfo.SaveStatus = false;
                alertMsgInfo.Message = "Error occured. Please try again.";
            }
            finally
            {
                dbManager.Close();
            }
        }
        public DataSet GetPackingDetailsById(Int64 BillId)
        {
            try
            {
                dbManager.Open();
                dbManager.CreateParameters();
                dbManager.AddParameters("BillId", GetDBNullOrValue(BillId), ParameterDirection.Input, DaoConstants.InParamSize);
                DataSet ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GetBillGodownWiseAllocation");

                return ds;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, "USP_GetBillGodownWiseAllocation", "Class : BillDal, Method : GetPackingDetailsById");
                return null;
            }
            finally
            {
                dbManager.Dispose();
            }
        }
    }
}