namespace WebApp.Models
{
    public class SetDispatchRequest
    {
        public long BillId { get; set; }
        public string Status { get; set; } = "D";
    }
}
