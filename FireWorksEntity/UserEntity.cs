using System;

namespace FireWorksEntity
{
    public class UserEntity
    {
        public int? UserID { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string ContactNo { get; set; }
        public int? RoleId { get; set; }
        public string RoleName { get; set; }
        public int? CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public char DMLType { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
