namespace WebApp.Models;

public partial class FwErrorLog
{
    public long ErrorId { get; set; }

    public string? UserId { get; set; }

    public string? Methodname { get; set; }

    public string? PrimaryKeyDetails { get; set; }

    public string? Error { get; set; }

    public DateTime? ErrDate { get; set; }
}
