namespace WebApp.Models;

public partial class FwMenu
{
    public int MenuId { get; set; }

    public string MenuName { get; set; } = null!;

    public int ParentId { get; set; }

    public byte UserLevel { get; set; }

    public int SortOrder { get; set; }

    public string? FormName { get; set; }

    public string? MenuIcon { get; set; }

    public string? MenuDescription { get; set; }

    public bool? IsActive { get; set; }

    public string? MenuUrl { get; set; }
}
