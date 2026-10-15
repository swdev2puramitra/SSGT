namespace FireWorksEntity
{
    public  class UserSessionEntity
    {
        public static int? UserID { get; set; }
        public static string UserName { get; set; }
        public static string UserPassword { get; set; }
        public static string ContactNo { get; set; }
        public static string EemailAddress { get; set; }
        public static int? RoleId { get; set; }
        public static int? CreatedBy { get; set; }
    }
    public class UserWebSessionEntity
    {
        public int? UserID { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string ContactNo { get; set; }
        public string EemailAddress { get; set; }
        public int? RoleId { get; set; }
        public int? CreatedBy { get; set; }
    }
}
