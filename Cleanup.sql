truncate table FW_Invoice_Items
truncate table FW_Bills_Items
delete from FW_Bills

truncate table FW_Stock
delete from FW_Master_SubCategory--
truncate table FW_ErrorLog
truncate table FW_Invoice
truncate table FW_Master_Items_Conversions
delete from FW_Master_Items--

truncate table FW_Invoice_Removed
truncate table FW_Invoice_Removed_Items
truncate table FW_Master_Bills
delete from FW_Master_Category--
delete from FW_Master_CompanyNames--
truncate table FW_Master_Conversion
truncate table FW_Master_Customers
truncate table FW_Master_HSN
delete from FW_Master_godown

delete from FW_Master_ItemType--
delete from FW_Master_SubCategory--
delete from FW_User_Details where RoleID not in (0,1)